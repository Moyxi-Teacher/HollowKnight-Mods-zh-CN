using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HollowKnightHaptics;

/// <summary>
/// VibrationMixer 包装器：拦截所有振动事件，尝试匹配 PS5 触觉剪辑。
///
/// 匹配优先顺序（与原始模组 HollowKnightHaptics-0.1.2 完全一致）：
///   1. VibrationData.GetPS5Vibration() 返回的 AudioClip.name —— 该名字与 clips 目录中的
///      WAV 文件名（不含扩展名）完全一致，直接作为 clip 名加载即可。
///   2. 若该 API 在运行环境不可用，则回退到 VibrationData.GamepadVibration.name，
///      经归一化模糊匹配（ResolveClipFile）解析为 WAV 文件名。
///
/// 只要 GetPS5Vibration 可用，绝大多数事件（含下落、受伤、落地等）都能精确命中对应 WAV，
/// 不再依赖模糊猜测。
/// </summary>
public sealed class HapticMixerWrapper : VibrationMixer
{
	// 注：原版 GamepadVibration 的 rumble 曲线（small/large motor）现保持引擎默认，
	// 不再按事件类型覆盖（用户要求施法/受伤 rumble 曲线恢复默认）。

	private static readonly Dictionary<VibrationMixer, HapticMixerWrapper> cache = new Dictionary<VibrationMixer, HapticMixerWrapper>();

	private readonly VibrationMixer inner;

	private readonly List<PS5HapticEmission> emissions = new List<PS5HapticEmission>();

	private readonly object gate = new object();

	/// <summary>
	/// 剪辑名归一化索引：归一化名 → 对应 WAV 文件名（不含 .wav 后缀）。
	/// 启动时从 clips 目录自动构建。
	/// </summary>
	private static readonly Dictionary<string, List<string>> clipNameIndex = new Dictionary<string, List<string>>();

	// 未匹配事件名记录（每个名字仅报一次），用于一轮完整游戏后盘点所有缺失映射。
	private static readonly HashSet<string> loggedMisses = new HashSet<string>();
	private static readonly object missGate = new object();

	// 显式别名映射：游戏事件名(归一化) -> clips 目录中的 WAV 文件名(不含扩展名)。
	// 用于游戏事件名与 WAV 文件名无法通过包含匹配命中的情况。
	// 例：暗影冲刺游戏事件为 "ShadowDash"，对应 clips 中的 hero_shade_dash_2_vibration.wav，
	// 二者归一化后无法匹配，故在此显式桥接，使其走上精细触觉 WAV 路径而非弱 rumble 回退。
	private static readonly Dictionary<string, string> clipAliases = new Dictionary<string, string>
	{
		{ "shadowdash", "hero_shade_dash_2_vibration" },     // 游戏事件名与 WAV 名无包含关系
		{ "dash", "hero_dash_vibration" },                   // 避免误配到 hornet_beast_dash_attack_foley_vibration
		// 荒芜俯冲 / 黑暗降临的"砸地冲击"：游戏事件名为 hero_void_quake_impact，
		// 而 clips 目录内对应素材名为 hero_quake_spell_impact_vibration。
		// 二者归一化后为 herovoidquakeimpact 与 heroquakespellimpactvibration，
		// 词元 void 与 spell 不同、且互不包含，精确/自动/包含匹配全部落空，
		// 导致每次砸地都走 rumble 回退（玩家感知为"第二段落地没有震动"）。
		// 法术前两段（hero_quake_spell_prepare / hero_quake_spell_dash）能自动包含匹配，故仅落地段需桥接。
		{ "herovoidquakeimpact", "hero_quake_spell_impact_vibration" },
	};

	// 自动别名：构建索引时根据 WAV 名自动生成。
	// 1) WAV 归一化名本身（事件名恰为 WAV 名时直接命中）
	// 2) 去掉 hero 前缀后的变体（游戏事件名不带 hero 前缀时也能命中）
	private static readonly Dictionary<string, string> autoAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	internal static IEnumerable<string> GetAliasTargets()
	{
		foreach (var v in clipAliases.Values)
			yield return v;
		foreach (var v in autoAliases.Values)
			yield return v;
	}

	/// <summary>
	/// clips 目录路径（用于构建索引）。
	/// </summary>
	private static string clipsDirectory = string.Empty;

	private static bool indexBuilt = false;

	/// <summary>
	/// 归一化剪辑名：转小写，去除下划线和空格，去除 _vibration 后缀。
	/// 用于模糊匹配游戏 clip 名与 WAV 文件名。
	/// </summary>
	private static string NormalizeName(string name)
	{
		if (string.IsNullOrEmpty(name))
			return string.Empty;
		string s = name.ToLowerInvariant();
		s = s.Replace("_", "").Replace(" ", "");
		if (s.EndsWith("_vibration"))
			s = s.Substring(0, s.Length - "_vibration".Length);
		return s;
	}

	/// <summary>
	/// 从 clips 目录扫描所有 WAV 文件，构建归一化名到文件名的映射索引。
	/// 同时构建自动别名：WAV 名本身、去掉 hero 前缀后的变体，用于反向匹配。
	/// 仅在首次调用时构建一次。
	/// </summary>
	private static void BuildClipNameIndex()
	{
		if (indexBuilt)
			return;
		indexBuilt = true;
		if (string.IsNullOrEmpty(clipsDirectory) || !Directory.Exists(clipsDirectory))
			return;
		autoAliases.Clear();
		foreach (var file in Directory.GetFiles(clipsDirectory, "*.wav"))
		{
			string baseName = Path.GetFileNameWithoutExtension(file);
			string normalized = NormalizeName(baseName);
			if (string.IsNullOrEmpty(normalized))
				continue;
			// 归一化索引
			if (!clipNameIndex.TryGetValue(normalized, out var list))
			{
				list = new List<string>();
				clipNameIndex[normalized] = list;
			}
			list.Add(baseName);

			// 自动别名 1：WAV 归一化名本身
			if (!autoAliases.ContainsKey(normalized))
				autoAliases[normalized] = baseName;

			// 自动别名 2：去掉 hero 前缀后的变体
			if (normalized.StartsWith("hero") && normalized.Length > 4)
			{
				string withoutHero = normalized.Substring(4);
				if (!string.IsNullOrEmpty(withoutHero) && !autoAliases.ContainsKey(withoutHero))
					autoAliases[withoutHero] = baseName;
			}
		}
	}

	/// <summary>
	/// 将游戏请求的剪辑名（如 "CycloneSlash"）解析为 clips 目录中的 WAV 文件名。
	/// 匹配策略（按优先级）：
	///   1. 显式别名（游戏事件名与 WAV 名无法通过包含匹配命中时）
	///   2. 自动别名（WAV 名本身、去掉 hero 前缀的变体）
	///   3. 归一化精确匹配
	///   4. 包含匹配：WAV 名包含事件名（或事件名包含 WAV 名），按长度降序取最长匹配
	/// 不再对 pulse 版做特殊处理，按普通 WAV 正常匹配。
	/// </summary>
	private static string ResolveClipFile(string requestedName)
	{
		if (string.IsNullOrEmpty(requestedName))
			return null;
		string normReq = NormalizeName(requestedName);
		if (string.IsNullOrEmpty(normReq))
			return null;

		// 1. 显式别名
		if (clipAliases.TryGetValue(normReq, out var aliasTarget))
			return aliasTarget;

		BuildClipNameIndex();

		// 2. 自动别名（根据 WAV 名反向匹配游戏事件名）
		if (autoAliases.TryGetValue(normReq, out var autoTarget))
			return autoTarget;

		// 3. 精确匹配
		if (clipNameIndex.TryGetValue(normReq, out var exactMatches))
			return exactMatches[0];

		// 4. 包含匹配：WAV 名包含事件名（或反向），按长度降序取最长匹配。
		//    脚步/走路类事件存在 _pulse 脉冲变体（如 hero_run_footsteps_stone_vibration_pulse_vibration），
		//    其文件名比同名非脉冲版更长，会被"取最长"规则误选中。
		//    故优先选取不含 "pulse" 的候选；仅当所有候选都是脉冲版时，才回退到脉冲版。
		var candidates = clipNameIndex
			.Where(kv => kv.Key.Contains(normReq) || normReq.Contains(kv.Key))
			.ToList();

		// 4a. 优先：不含 pulse 的候选，按长度降序取最长
		var nonPulse = candidates
			.Where(kv => !kv.Key.Contains("pulse"))
			.OrderByDescending(kv => kv.Key.Length)
			.ToList();
		if (nonPulse.Count > 0)
			return nonPulse[0].Value[0];

		// 4b. 回退：所有候选均为脉冲版时，按长度降序取最长
		var pulse = candidates
			.OrderByDescending(kv => kv.Key.Length)
			.ToList();
		if (pulse.Count > 0)
			return pulse[0].Value[0];

		return null;
	}

	public override bool IsPaused
	{
		get { return inner.IsPaused; }
		set
		{
			inner.IsPaused = value;
			HapticEngine.CachedMixerPaused = value;
		}
	}

	public override int PlayingEmissionCount
	{
		get
		{
			lock (gate)
			{
				Prune();
				return emissions.Count + inner.PlayingEmissionCount;
			}
		}
	}

	private HapticMixerWrapper(VibrationMixer inner)
	{
		this.inner = inner;
	}

	public static HapticMixerWrapper GetOrCreate(VibrationMixer inner)
	{
		lock (cache)
		{
			if (!cache.TryGetValue(inner, out var value))
			{
				value = new HapticMixerWrapper(inner);
				cache.Add(inner, value);
			}
			return value;
		}
	}

	public override VibrationEmission GetPlayingEmission(int index)
	{
		lock (gate)
		{
			Prune();
			if (index < emissions.Count)
			{
				return (VibrationEmission)(object)emissions[index];
			}
			return inner.GetPlayingEmission(index - emissions.Count);
		}
	}

	/// <summary>
	/// 设置 clips 目录路径（由 HapticEngine.Start() 调用），用于构建剪辑名映射索引。
	/// </summary>
	public static void SetClipsDirectory(string path)
	{
		clipsDirectory = path;
		indexBuilt = false;
		clipNameIndex.Clear();
	}

	/// <summary>
	/// 从 VibrationData 中提取剪辑名（与原始模组 HollowKnightHaptics-0.1.2 一致：直接用游戏内 PS5 剪辑名作为 WAV 文件名）。
	/// 当前游戏版本的 VibrationData 是结构体，PS5 触觉数据存放于 HighFidelityVibration（TextAsset），
	/// 其 .name 即为精确剪辑名（与 clips 目录 WAV 文件名完全一致，含空格与大小写）。
	/// 解析优先级：
	///   1. HighFidelityVibration.name —— 当前游戏版本的精确 PS5 剪辑名
	///   2. GetPS5Vibration() 反射（兼容原版路径，返回 AudioClip.name）
	///   3. GamepadVibration.name —— 仅作最后回退，经模糊匹配解析
	/// </summary>
	private static string ResolveClipName(VibrationData vibrationData)
	{
		// 方式 1（首选）：当前游戏版本的精确 PS5 剪辑名
		try
		{
			var hf = vibrationData.HighFidelityVibration;
			if (hf != null && !string.IsNullOrEmpty(hf.name))
				return hf.name;
		}
		catch { }

		// 方式 2：原版路径 —— VibrationData.GetPS5Vibration().name
		try
		{
			var method = typeof(VibrationData).GetMethod(
				"GetPS5Vibration",
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				var audioClip = method.Invoke(vibrationData, null) as UnityEngine.AudioClip;
				if (audioClip != null && !string.IsNullOrEmpty(audioClip.name))
					return audioClip.name;
			}
		}
		catch { }

		// 方式 3：回退到 GamepadVibration.name + 模糊匹配
		var gamepadVib = vibrationData.GamepadVibration;
		if (gamepadVib != null && !string.IsNullOrEmpty(gamepadVib.name))
			return ResolveClipFile(gamepadVib.name);

		return null;
	}

	/// <summary>
	/// 拦截所有振动事件（与原始模组 HollowKnightHaptics-0.1.2 完全一致）：
	/// 1. 通过 VibrationData.GetPS5Vibration().name 获取剪辑名 —— 该名字与 clips 目录中的 WAV 文件名（不含扩展名）完全一致；
	///    若当前环境无此 API，则回退到 GamepadVibration.name 并经模糊匹配。
	/// 2. 若连接了 HapticsBridge 且存在对应 PCM，则注册 PS5HapticEmission 走精细触觉路径。
	/// 3. 否则回退到内层 mixer（普通双马达 rumble）。
	/// </summary>
	public override VibrationEmission PlayEmission(VibrationData vibrationData, VibrationTarget vibrationTarget, bool isLooping, string tag)
	{
		// 故障安全：本方法被游戏所有振动事件调用。任何异常都不得外溢到游戏代码，
		// 必须原样回退到原版 inner mixer（双马达 rumble），保证不破坏任何原版交互。
		try
		{
			// 优先使用原版 GetPS5Vibration().name（与 WAV 文件名完全一致），若不可用则回退到模糊匹配
			string clipName = ResolveClipName(vibrationData);
			string displayInput = clipName ?? "<no PS5 clip>";

			if (emissions.Count <= 20)
			{
				HapticsPlugin.Log.LogInfo((object)("Haptic input: '" + displayInput + "' (bridge=" + HapticEngine.Connected + ", looping=" + isLooping + ", tag='" + tag + "')"));
			}

			if (HapticEngine.Connected && !string.IsNullOrEmpty(clipName))
			{
				// 方式 A：用精确剪辑名直接加载（HighFidelityVibration.name / GetPS5Vibration().name）
				string resolvedClip = clipName;
				if (!HapticEngine.TryGetPcm(resolvedClip, out var pcm))
				{
					// 方式 B：精确名未命中时，按 WAV 名包含事件名的规则模糊匹配（与原始模组目录一致）
					resolvedClip = ResolveClipFile(clipName);
					if (resolvedClip != null)
						HapticEngine.TryGetPcm(resolvedClip, out pcm);
				}
				if (pcm != null)
				{
					// 注册 PS5 触觉发射到引擎混合循环
					// 震动倍率：基础值取游戏原生 VibrationData.Strength（与基准版 0.1.2 一致），
					// 再乘以分类倍率（HapticsPlugin 中的 ConfigEntry，默认 1.0 = 基准，可在游戏内/配置文件中自行调节）。
					float strength = 1f;
					try
					{
						var strengthProp = typeof(VibrationData).GetProperty("Strength", BindingFlags.Instance | BindingFlags.Public);
						if (strengthProp != null)
						{
							var val = strengthProp.GetValue(vibrationData);
							if (val != null)
								strength = Convert.ToSingle(val);
						}
					}
					catch { }

					// 每个震动独立倍率：按 WAV 基名查表，未注册则 1.0。
					float categoryGain = HapticsPlugin.GetClipGain(resolvedClip);
					strength *= categoryGain;
					var emission = new PS5HapticEmission(pcm, strength, isLooping, tag, vibrationTarget);
					// 不设时长上限：FootstepVibrationPatch 每次脚步都会先 StopAllVibrationsWithTag
					// 停止上一段，因而每步 = 一段完整 WAV 脉冲、下一步再停 → 既强又"一段一段"。
					// （此前设 220ms 上限会把 2 秒循环纹理 WAV 截到静音区，导致几乎无震感。）
					HapticEngine.Register(emission);
					emissions.Add(emission);
					// 前 20 次记录：确认 clip 匹配和 PCM 加载成功
					if (emissions.Count <= 20)
						HapticsPlugin.Log.LogInfo((object)("Haptic play: '" + displayInput + "' (loop=" + isLooping + ", tag='" + tag + "')"));
					return emission;
				}
			}

			// 记录未匹配事件名（每个名字仅报一次），便于盘点哪些游戏事件没走上精细 WAV 通路。
			if (!string.IsNullOrEmpty(displayInput) && displayInput != "<no PS5 clip>")
			{
				lock (missGate)
				{
					if (!loggedMisses.Contains(displayInput))
					{
						loggedMisses.Add(displayInput);
						HapticsPlugin.Log.LogWarning((object)("Haptic MISS: '" + displayInput + "' (tag='" + tag + "') — 无匹配 WAV，回退 rumble"));
					}
				}
			}

			// 无有效剪辑或 Bridge 未连接：回退到内层 mixer（普通双马达 rumble）。
			return inner.PlayEmission(vibrationData, vibrationTarget, isLooping, tag);
		}
		catch (Exception ex)
		{
			// 兜底：触觉增强失败绝不影响游戏，原样走原版 rumble。
			HapticsPlugin.Log.LogDebug((object)("[HKHaptics-Wrapper] PlayEmission fallback (haptic skipped): " + ex.Message));
			return inner.PlayEmission(vibrationData, vibrationTarget, isLooping, tag);
		}
	}

	public override void StopAllEmissions()
	{
		lock (gate)
		{
			foreach (PS5HapticEmission emission in emissions)
			{
				emission.Stop();
			}
			emissions.Clear();
		}
		inner.StopAllEmissions();
	}

	public override void StopAllEmissionsWithTag(string tag)
	{
		lock (gate)
		{
			foreach (PS5HapticEmission emission in emissions)
			{
				if (emission.Tag == tag)
				{
					emission.Stop();
				}
			}
			Prune();
		}
		inner.StopAllEmissionsWithTag(tag);
	}

	private void Prune()
	{
		emissions.RemoveAll((PS5HapticEmission e) => !e.IsPlaying);
	}
}
