using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using HarmonyLib;
using Language;
using Modding;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;
using Object = UnityEngine.Object;

// 简体中文字体替换模组：运行时从霞鹜臻楷.ttf 动态构建 TMP_FontAsset，
// 并通过 Harmony 补丁替换游戏内简体中文字体。
namespace SimplifiedChineseFont;

public class SimplifiedChineseFontMod : Mod, IMenuMod, ITogglableMod
{
	// 补丁类：在游戏 ChangeFontByLanguage.SetFont 执行后，把 TMP 文本字体替换为中文动态字体
	[HarmonyPatch(typeof(ChangeFontByLanguage), "SetFont")]
	private static class ChangeFontByLanguagePatch
	{
		private static bool _logged;

		public static void Postfix(ChangeFontByLanguage __instance)
		{
			SimplifiedChineseFontMod instance = Instance;
			if (instance == null)
			{
				return;
			}
			if (!_logged)
			{
				_logged = true;
				instance.Log("[补丁触发] ChangeFontByLanguage.SetFont 首次调用：" + __instance.gameObject.name);
			}
			if ((Object)(object)instance._chineseFont == (Object)null || TmproField == null)
			{
				return;
			}
			try
			{
				object value = TmproField.GetValue(__instance);
				TextMeshPro val = (TextMeshPro)((value is TextMeshPro) ? value : null);
				if ((Object)(object)val != (Object)null)
				{
					instance.ApplyChineseFont((TMP_Text)(object)val, "SetFont补丁");
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("[简体中文字体替换] SetFont 补丁异常：" + ex);
			}
		}
	}

	// 补丁类：任何对 TMP_Text.font 的赋值都过一遍；若语言为 ZH 且文本含中文，则改用霞鹜臻楷
	[HarmonyPatch(typeof(TMP_Text), "set_font")]
	private static class TmpTextFontSetterPatch
	{
		private static bool _logged;

		public static void Postfix(TMP_Text __instance)
		{
			SimplifiedChineseFontMod instance = Instance;
			if (instance == null || (Object)(object)__instance == (Object)null)
			{
				return;
			}
			if (!_logged)
			{
				_logged = true;
				instance.Log("[补丁触发] TMP_Text.set_font 首次调用：" + __instance.GetType().Name + "，文本=" + Preview(__instance.text));
			}
			try
			{
				instance.ApplyChineseFont(__instance, "TMP设置字体", requireCjk: true);
			}
			catch (Exception ex)
			{
				Debug.LogError("[简体中文字体替换] set_font 补丁异常：" + ex);
			}
		}
	}

	// 补丁类：旧版 uGUI 文本（UnityEngine.UI.Text）设置字体时改写为霞鹜臻楷
	[HarmonyPatch(typeof(UnityEngine.UI.Text), "set_font")]
	private static class UiTextFontSetterPatch
	{
		private static bool _logged;

		public static void Postfix(UnityEngine.UI.Text __instance)
		{
			SimplifiedChineseFontMod instance = Instance;
			if (instance == null || (Object)(object)__instance == (Object)null)
			{
				return;
			}
			if (!_logged)
			{
				_logged = true;
				instance.Log("[补丁触发] UI.Text.set_font 首次调用：" + __instance.gameObject.name + "，文本=" + Preview(__instance.text));
			}
			try
			{
				instance.ApplyLegacyFont(__instance, "UI.Text设置字体");
			}
			catch (Exception ex)
			{
				Debug.LogError("[简体中文字体替换] UI.Text.set_font 补丁异常：" + ex);
			}
		}
	}

	// 补丁类：旧版 uGUI 文本启用时替换字体
	[HarmonyPatch(typeof(UnityEngine.UI.Text), "OnEnable")]
	private static class UiTextOnEnablePatch
	{
		private static bool _logged;

		public static void Postfix(UnityEngine.UI.Text __instance)
		{
			SimplifiedChineseFontMod instance = Instance;
			if (instance == null || (Object)(object)__instance == (Object)null)
			{
				return;
			}
			if (!_logged)
			{
				_logged = true;
				instance.Log("[补丁触发] UI.Text.OnEnable 首次调用：" + __instance.gameObject.name + "，文本=" + Preview(__instance.text));
			}
			try
			{
				instance.ApplyLegacyFont(__instance, "UI.Text启用");
			}
			catch (Exception ex)
			{
				Debug.LogError("[简体中文字体替换] UI.Text.OnEnable 补丁异常：" + ex);
			}
		}
	}

	// 补丁类：3D 旧版文本（TextMesh）设置字体时改写为霞鹜臻楷
	[HarmonyPatch(typeof(TextMesh), "set_font")]
	private static class TextMeshFontSetterPatch
	{
		private static bool _logged;

		public static void Postfix(TextMesh __instance)
		{
			SimplifiedChineseFontMod instance = Instance;
			if (instance == null || (Object)(object)__instance == (Object)null)
			{
				return;
			}
			if (!_logged)
			{
				_logged = true;
				instance.Log("[补丁触发] TextMesh.set_font 首次调用：" + __instance.gameObject.name + "，文本=" + Preview(__instance.text));
			}
			try
			{
				instance.ApplyTextMeshFont(__instance, "TextMesh设置字体");
			}
			catch (Exception ex)
			{
				Debug.LogError("[简体中文字体替换] TextMesh.set_font 补丁异常：" + ex);
			}
		}
	}

	// 补丁类：任何对 TMP_Text.text 的赋值都过一遍；文本含中文时改用霞鹜臻楷
	[HarmonyPatch(typeof(TMP_Text), "set_text")]
	private static class TmpTextSetTextPatch
	{
		private static bool _logged;

		public static void Postfix(TMP_Text __instance)
		{
			SimplifiedChineseFontMod instance = Instance;
			if (instance == null || (Object)(object)__instance == (Object)null)
			{
				return;
			}
			if (!_logged)
			{
				_logged = true;
				instance.Log("[补丁触发] TMP_Text.set_text 首次调用：" + __instance.GetType().Name + "，文本=" + Preview(__instance.text));
			}
			try
			{
				instance.ApplyChineseFont(__instance, "TMP设置文本", requireCjk: true);
			}
			catch (Exception ex)
			{
				Debug.LogError("[简体中文字体替换] set_text 补丁异常：" + ex);
			}
		}
	}

	// 补丁类：UGUI 文本启用时检查并替换字体
	[HarmonyPatch(typeof(TextMeshProUGUI), "OnEnable")]
	private static class TextMeshProUGUIOnEnablePatch
	{
		private static bool _logged;

		public static void Postfix(TextMeshProUGUI __instance)
		{
			SimplifiedChineseFontMod instance = Instance;
			if (instance == null || (Object)(object)__instance == (Object)null)
			{
				return;
			}
			if (!_logged)
			{
				_logged = true;
				instance.Log("[补丁触发] TextMeshProUGUI.OnEnable 首次调用：" + __instance.gameObject.name + "，文本=" + Preview(__instance.text));
			}
			try
			{
				instance.ApplyChineseFont((TMP_Text)(object)__instance, "OnEnable(UGUI)", requireCjk: true);
			}
			catch (Exception ex)
			{
				Debug.LogError("[简体中文字体替换] OnEnable(UGUI) 补丁异常：" + ex);
			}
		}
	}

	// 补丁类：3D 文本启用时检查并替换字体
	[HarmonyPatch(typeof(TextMeshPro), "OnEnable")]
	private static class TextMeshProOnEnablePatch
	{
		private static bool _logged;

		public static void Postfix(TextMeshPro __instance)
		{
			SimplifiedChineseFontMod instance = Instance;
			if (instance == null || (Object)(object)__instance == (Object)null)
			{
				return;
			}
			if (!_logged)
			{
				_logged = true;
				instance.Log("[补丁触发] TextMeshPro.OnEnable 首次调用：" + __instance.gameObject.name + "，文本=" + Preview(__instance.text));
			}
			try
			{
				instance.ApplyChineseFont((TMP_Text)(object)__instance, "OnEnable(3D)", requireCjk: true);
			}
			catch (Exception ex)
			{
				Debug.LogError("[简体中文字体替换] OnEnable(3D) 补丁异常：" + ex);
			}
		}
	}

	// 补丁类：GameManager.Update 每帧回调，作为兜底扫描的可靠驱动（GameManager 常驻所有场景）
	[HarmonyPatch(typeof(GameManager), "Update")]
	private static class GameManagerUpdatePatch
	{
		private static bool _logged;

		public static void Postfix()
		{
			SimplifiedChineseFontMod instance = Instance;
			if (instance == null)
			{
				return;
			}
			if (!_logged)
			{
				_logged = true;
				instance.Log("[补丁触发] GameManager.Update 首次调用");
			}
			instance.TickSweeper();
		}
	}

	// 兜底扫描器：部分文本不经过 ChangeFontByLanguage，定时扫描含中文的 TMP 文本并替换为霞鹜臻楷
	public class FontSweeper : MonoBehaviour
	{
		private float _timer;

		private bool _loggedFirstRun;

		private void Update()
		{
			if (!_loggedFirstRun)
			{
				_loggedFirstRun = true;
				SimplifiedChineseFontMod instance0 = Instance;
				if (instance0 != null)
				{
					instance0.Log("[兜底扫描器] Update 首次运行");
				}
			}
			_timer += Time.unscaledDeltaTime;
			if (_timer < 3f)
			{
				return;
			}
			_timer = 0f;
			SimplifiedChineseFontMod instance = Instance;
			if (instance != null)
			{
				instance.SweepChineseTexts();
			}
		}

		private void OnDestroy()
		{
			SimplifiedChineseFontMod instance = Instance;
			if (instance != null)
			{
				instance.Log("[兜底扫描器] 被销毁");
			}
		}
	}

	internal static SimplifiedChineseFontMod Instance;

	private TMP_FontAsset _chineseFont;

	// 旧版 uGUI 文本（UnityEngine.UI.Text）使用的动态字体，从 TTF 文件路径直接创建
	private Font _legacyZhFont;

	private int _legacyLogCount;

	private int _legacyMeshLogCount;

	private bool _patchInstalled;

	// 旧版 uGUI 文本用的字号与手工静态字体的图集参数
	private const int LegacyFontSize = 48;

	private const int ManualFontSize = 48;

	private const int ManualAtlasSize = 4096;

	// 安全模式标记：为 true 时完全不替换 TMP 字体
	private bool _tmpDisabled;

	// 字体资产导出（一次性）：把游戏自带中文 TMP 字体资产的字形表与图集导出，供离线重制使用
	private bool _fontDumpDone;

	// 参考字体资产（游戏自带 TMP 字体，优先中文资产）的字形边距：图集必须沿用同一套参数，
	// 否则 SDF 文字会发虚/发细，看起来「不像霞鹜臻楷」
	private static int _refPadding;

	// 参考材质参数（从游戏自带的 SDF 字体材质读出，用于对齐我们的材质）
	private static float _refGradientScale;

	private Harmony _harmony;

	private GameObject _sweeperObject;

	private int _applyLogCount;

	private int _sweepCount;

	private int _sweepLogCount;

	private float _sweepTimer;

	private int _langLogCount;

	private int _textDumpCount;

	private bool _tk2dDumped;

	private int _sweepIndex;

	private int _uiFontDumpCount;

	// ChangeFontByLanguage 中私有字段 tmpro（TextMeshPro 组件），通过反射获取
	private static readonly FieldInfo TmproField = typeof(ChangeFontByLanguage).GetField("tmpro", BindingFlags.Instance | BindingFlags.NonPublic);

	// FontEngine.ResetAtlasTexture：native 要求图集纹理先 Reset 才能接受字形打包
	private static readonly MethodInfo ResetAtlasMethod = typeof(FontEngine).GetMethod("ResetAtlasTexture", BindingFlags.Static | BindingFlags.NonPublic);

	// FontEngine.TryAddGlyphsToTexture 私有静态方法：把一批字形渲染编码到图集纹理
	private static readonly MethodInfo TryAddGlyphsMethod = typeof(FontEngine).GetMethod("TryAddGlyphsToTexture", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[8]
	{
		typeof(List<uint>),
		typeof(int),
		typeof(GlyphPackingMode),
		typeof(List<GlyphRect>),
		typeof(List<GlyphRect>),
		typeof(GlyphRenderMode),
		typeof(Texture2D),
		typeof(Glyph[]).MakeByRefType()
	}, null);

	public bool ToggleButtonInsideMenu => true;

	public SimplifiedChineseFontMod()
		: base("简体中文字体替换")
	{
		Instance = this;
	}

	public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
	{
		if (!_patchInstalled)
		{
			_tmpDisabled = IsTmpReplacementDisabled();
			if (_tmpDisabled)
			{
				Log("已检测到「安全模式-不替换TMP字体.txt」：本次只替换旧版界面文本（UI.Text），不改动 TMP 文本字体");
			}
			LoadFontIfNeeded();
			if ((Object)(object)_chineseFont != (Object)null || (Object)(object)_legacyZhFont != (Object)null)
			{
				InstallPatch();
				StartSweeper();
			}
		}
	}

	// 启动兜底扫描器（常驻 GameObject，每 3 秒扫描一次）
	private void StartSweeper()
	{
		if ((Object)(object)_sweeperObject != (Object)null)
		{
			return;
		}
		try
		{
			_sweeperObject = new GameObject("简体中文字体替换-扫描器");
			_sweeperObject.AddComponent<FontSweeper>();
			Object.DontDestroyOnLoad(_sweeperObject);
			string langNow = global::Language.Language.CurrentLanguage().ToString();
			Log("中文文本兜底扫描器已启动，当前语言=" + langNow);
		}
		catch (Exception ex)
		{
			Log("启动兜底扫描器失败：" + ex.Message);
		}
	}

	// 把指定文本的字体替换为霞鹜臻楷。语言非 ZH、或字体已是目标字体时跳过。
	internal void ApplyChineseFont(TMP_Text text, string source, bool requireCjk = false)
	{
		if ((Object)(object)text == (Object)null || (Object)(object)_chineseFont == (Object)null)
		{
			return;
		}
		if (!IsChineseLanguage())
		{
			if (_langLogCount < 3)
			{
				_langLogCount++;
				string langBad = global::Language.Language.CurrentLanguage().ToString();
				Log("[替换] 语言不是 ZH（当前=" + langBad + "），跳过 " + source);
			}
			return;
		}
		if (requireCjk)
		{
			string content = text.text;
			if (string.IsNullOrEmpty(content) || !ContainsCjk(content))
			{
				return;
			}
		}
		TMP_FontAsset current = text.font;
		if ((Object)(object)current == (Object)(object)_chineseFont)
		{
			return;
		}
		AddFallback(current);
		text.font = _chineseFont;
		if (_applyLogCount < 8)
		{
			_applyLogCount++;
			string sample = string.IsNullOrEmpty(text.text) ? "空" : text.text.Substring(0, Math.Min(20, text.text.Length));
			Log($"[替换] {source} → {text.gameObject.name}，原字体={(current == null ? "空" : current.name)}，文本前20字={sample}");
		}
	}

	// 把被替换掉的原字体加入 fallback 链，保证霞鹜臻楷缺字时回退到原版字体
	private void AddFallback(TMP_FontAsset source)
	{
		if ((Object)(object)source == (Object)null || (Object)(object)_chineseFont == (Object)null || (Object)(object)source == (Object)(object)_chineseFont)
		{
			return;
		}
		if (_chineseFont.fallbackFontAssets == null)
		{
			_chineseFont.fallbackFontAssets = new List<TMP_FontAsset>();
		}
		if (!_chineseFont.fallbackFontAssets.Contains(source))
		{
			_chineseFont.fallbackFontAssets.Add(source);
		}
	}

	// 用霞鹜臻楷动态字体替换旧版 uGUI 文本（UnityEngine.UI.Text）的字体。
	// 游戏中文界面实际就是这套系统，因此这里是真正生效的替换点。
	internal void ApplyLegacyFont(UnityEngine.UI.Text text, string source)
	{
		if ((Object)(object)text == (Object)null || (Object)(object)_legacyZhFont == (Object)null)
		{
			return;
		}
		if (!IsChineseLanguage())
		{
			return;
		}
		// 只替换「含中文」的文本：纯英文/数字文本保持原字体，避免改变原版观感
		if (!ContainsCjk(text.text))
		{
			return;
		}
		if ((Object)(object)text.font == (Object)(object)_legacyZhFont)
		{
			return;
		}
		Font oldFont = text.font;
		text.font = _legacyZhFont;
		if (_legacyLogCount < 10)
		{
			_legacyLogCount++;
			Log($"[替换-UI.Text] {source} → {text.gameObject.name}，原字体={(oldFont == null ? "空" : oldFont.name)}，文本={Preview(text.text)}");
		}
	}

	// 用霞鹜臻楷动态字体替换 3D 旧版文本（TextMesh）的字体
	internal void ApplyTextMeshFont(TextMesh mesh, string source)
	{
		if ((Object)(object)mesh == (Object)null || (Object)(object)_legacyZhFont == (Object)null)
		{
			return;
		}
		if (!IsChineseLanguage())
		{
			return;
		}
		// 只替换「含中文」的文本：纯英文/数字文本保持原字体，避免改变原版观感
		if (!ContainsCjk(mesh.text))
		{
			return;
		}
		if ((Object)(object)mesh.font == (Object)(object)_legacyZhFont)
		{
			return;
		}
		Font oldFont = mesh.font;
		mesh.font = _legacyZhFont;
		if (_legacyMeshLogCount < 10)
		{
			_legacyMeshLogCount++;
			Log($"[替换-TextMesh] {source} → {mesh.gameObject.name}，原字体={(oldFont == null ? "空" : oldFont.name)}，文本={Preview(mesh.text)}");
		}
	}

	// 把字体注册进当前会话的字体表（仅本次进程有效，不写入系统、不需要管理员权限）
	[DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern int AddFontResourceW(string lpFileName);

	// 创建旧版 uGUI 文本用的霞鹜臻楷动态字体，并做字形自检；自检不通过则不替换（避免文字消失）
	// gdiFontPath：用于向 GDI 注册的路径（优先 ASCII 临时副本；副本写入失败时直接用原路径，GDI 支持中文路径）
	private void CreateLegacyFontIfNeeded(string gdiFontPath, string sourceFontPath)
	{
		try
		{
			if (string.IsNullOrEmpty(gdiFontPath) || !File.Exists(gdiFontPath))
			{
				gdiFontPath = sourceFontPath;
			}
			if (string.IsNullOrEmpty(gdiFontPath) || !File.Exists(gdiFontPath))
			{
				gdiFontPath = null;
			}
			Log("[字体注册] GDI 注册路径=" + ((gdiFontPath == null) ? "(无，仅做枚举诊断)" : gdiFontPath));
			// 方式一：注册进会话字体表后按字体名创建（玩家构建下最可靠的动态字体来源）
			try
			{
				int added = ((gdiFontPath == null) ? 0 : AddFontResourceW(gdiFontPath));
				Log("AddFontResource 返回值=" + added + "（0 表示已注册或失败，非致命）");
				string[] osFonts = Font.GetOSInstalledFontNames();
				List<string> candidates = new List<string>();
				foreach (string n in osFonts)
				{
					if (string.IsNullOrEmpty(n))
					{
						continue;
					}
					if (n.IndexOf("LXGW", StringComparison.OrdinalIgnoreCase) >= 0 || n.Contains("霞鹜") || n.Contains("臻楷") || n.IndexOf("ZhenKai", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						candidates.Add(n);
						Log("系统字体表中发现候选字体：" + n);
					}
				}
				Log($"系统字体总数={osFonts.Length}，霞鹜臻楷候选数={candidates.Count}");
				// 打印系统字体清单（截断），用于确认 Unity 是否枚举到「用户级安装」的字体
				string fontList = "";
				int shown = 0;
				foreach (string n in osFonts)
				{
					if (shown >= 80)
					{
						break;
					}
					shown++;
					fontList += n + " | ";
				}
				Log("[系统字体清单] " + fontList);
				bool perUserVisible = false;
				foreach (string n in osFonts)
				{
					if (!string.IsNullOrEmpty(n) && (n.Contains("优设") || n.IndexOf("HONOR", StringComparison.OrdinalIgnoreCase) >= 0))
					{
						perUserVisible = true;
						Log("[系统字体清单] 发现用户级安装字体：" + n);
					}
				}
				Log("[系统字体清单] 是否枚举到用户级安装字体=" + perUserVisible);
				if (candidates.Count == 0)
				{
					Log("系统字体表中未直接匹配到霞鹜臻楷，仍将按字面名逐一尝试创建");
				}
				// 依次尝试：枚举到的候选名 + 字面名（万一枚举名与字体内部名写法不同也能命中）
				List<string> tryNames = new List<string>();
				foreach (string c in candidates)
				{
					if (!tryNames.Contains(c))
					{
						tryNames.Add(c);
					}
				}
				string[] literalNames = new string[3] { "LXGW ZhenKai", "霞鹜臻楷", "LXGW ZhenKai Regular" };
				foreach (string s in literalNames)
				{
					if (!tryNames.Contains(s))
					{
						tryNames.Add(s);
					}
				}
				foreach (string name in tryNames)
				{
					try
					{
						Font byName = Font.CreateDynamicFontFromOSFont(new string[1] { name }, LegacyFontSize);
						if (!SelfTestFont(byName, "系统字体名:" + name))
						{
							continue;
						}
						// 关键核对：系统找不到该字体时会静默回退到别的字体（例如 Arial），
						// 只看字形自检是分辨不出来的，必须核对系统实际解析出的主字体名。
						string primary = ((byName.fontNames != null && byName.fontNames.Length > 0) ? byName.fontNames[0] : null);
						if (!LooksLikeTargetFont(primary))
						{
							Log("[字体名核对] 请求「" + name + "」，但系统解析到的主字体是「" + (primary ?? "(空)") + "」，判定不是霞鹜臻楷，放弃该候选");
							continue;
						}
						_legacyZhFont = byName;
						Log("旧版文本动态字体创建成功（系统字体名方式）：" + name + " → 实际字体 " + primary);
						return;
					}
					catch (Exception exOne)
					{
						Log("创建动态字体异常（" + name + "）：" + exOne.Message);
					}
				}
			}
			catch (Exception exName)
			{
				Log("系统字体名方式异常：" + exName.Message);
			}
			// 说明：不再回退到「new Font(文件路径)」方式——实测该方式在玩家构建下取不到字形，会导致文字全部消失。
			_legacyZhFont = null;
			Log("旧版文本字体创建未成功：将由「手工静态字体」兜底方案接管");
		}
		catch (Exception ex)
		{
			_legacyZhFont = null;
			Log("创建旧版文本动态字体异常：" + ex);
		}
	}

	// 判断某个字体名是否就是我们要的霞鹜臻楷（系统里的写法可能是 "LXGW ZhenKai" 或中文名）
	private static bool LooksLikeTargetFont(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		return name.IndexOf("LXGW", StringComparison.OrdinalIgnoreCase) >= 0 || name.Contains("霞鹜") || name.Contains("臻楷") || name.IndexOf("ZhenKai", StringComparison.OrdinalIgnoreCase) >= 0;
	}

	// 字体自检：要求有材质且有贴图、能取到「永」字字形且步进大于 0
	private bool SelfTestFont(Font font, string tag)
	{
		try
		{
			if ((Object)(object)font == (Object)null)
			{
				Log($"[自检-{tag}] 字体为空");
				return false;
			}
			bool hasMat = (Object)(object)font.material != (Object)null;
			bool hasTex = hasMat && (Object)(object)font.material.mainTexture != (Object)null;
			string names = (font.fontNames == null) ? "(null)" : string.Join(",", font.fontNames);
			bool hasChar = font.HasCharacter('永');
			// 非动态字体调用 RequestCharactersInTexture 无实际作用，异常也要忽略（不能因此否掉可用字体）
			try
			{
				font.RequestCharactersInTexture("永", 64, FontStyle.Normal);
			}
			catch (Exception exReq)
			{
				Log($"[自检-{tag}] RequestCharactersInTexture 异常（忽略）：" + exReq.Message);
			}
			CharacterInfo info;
			bool got = font.GetCharacterInfo('永', out info, 64, FontStyle.Normal);
			bool ok = hasMat && hasTex && hasChar && got && info.advance > 0;
			Log($"[自检-{tag}] dynamic={font.dynamic}, material={(hasMat ? "有" : "无")}, 贴图={(hasTex ? "有" : "无")}, fontNames={names}, HasCharacter('永')={hasChar}, GetCharacterInfo={got}, advance={(got ? info.advance : 0)}, 结论={(ok ? "可用" : "不可用")}");
			return ok;
		}
		catch (Exception ex)
		{
			Log($"[自检-{tag}] 异常：" + ex.Message);
			return false;
		}
	}

	// 方式三（兜底）：完全不依赖系统字体表。
	// Unity 的 Font 是 native 对象：非动态字体（dynamic=false）渲染时只依赖两样数据 ——
	//   ① characterInfo：每个字符的字形图集坐标(uv)与像素度量(vert/advance)；
	//   ② material：指向字形图集纹理。
	// 这两样我们都能自己造，所以系统字体表里没有霞鹜臻楷时也能让 uGUI 用上它。
	private Font TryBuildManualLegacyFont(byte[] fontData, string asciiFontPath)
	{
		try
		{
			if (TryAddGlyphsMethod == null)
			{
				Log("[静态字体] FontEngine.TryAddGlyphsToTexture 不可访问，放弃该方案");
				return null;
			}
			List<uint> codepoints = new List<uint>();
			List<uint> glyphIndexes = new List<uint>();
			CollectCodepoints(0x20u, 0x7Eu, codepoints, glyphIndexes);
			CollectCodepoints(0x2000u, 0x206Fu, codepoints, glyphIndexes);
			CollectCodepoints(0x3000u, 0x303Fu, codepoints, glyphIndexes);
			CollectCodepoints(0x4E00u, 0x9FFFu, codepoints, glyphIndexes);
			CollectCodepoints(0xFF00u, 0xFFEFu, codepoints, glyphIndexes);
			if (codepoints.Count == 0)
			{
				Log("[静态字体] 未收集到任何码位，放弃");
				return null;
			}
			Log($"[静态字体] 共收集 {codepoints.Count} 个码位，开始构建光栅图集（字号 {ManualFontSize}px）");
			FontEngine.InitializeFontEngine();
			bool loaded = false;
			if (!string.IsNullOrEmpty(asciiFontPath) && File.Exists(asciiFontPath))
			{
				loaded = (int)FontEngine.LoadFontFace(asciiFontPath, ManualFontSize) <= 0;
			}
			if (!loaded)
			{
				loaded = (int)FontEngine.LoadFontFace(fontData, ManualFontSize) <= 0;
			}
			if (!loaded)
			{
				Log("[静态字体] 字体面加载失败");
				return null;
			}
			try
			{
				FontEngine.SetFaceSize(ManualFontSize);
			}
			catch (Exception exSize)
			{
				Log("[静态字体] SetFaceSize 异常（忽略）：" + exSize.Message);
			}
			// 必须用光栅（非 SDF）渲染：SDF 字形要配 TMP 专用着色器，而旧版 uGUI 用的是普通文本着色器
			int[] renderModes = new int[3] { 4117, 4121, 4118 };
			int[] formats = new int[2] { 1, 4 };
			Texture2D atlas = null;
			Glyph[] packed = null;
			int usedMode = 0;
			int usedFormat = 0;
			foreach (int rm in renderModes)
			{
				foreach (int fmt in formats)
				{
					Texture2D tex = new Texture2D(ManualAtlasSize, ManualAtlasSize, (TextureFormat)fmt, false);
					((Texture)tex).filterMode = (FilterMode)1;
					((Texture)tex).wrapMode = (TextureWrapMode)1;
					try
					{
						if (ResetAtlasMethod != null)
						{
							ResetAtlasMethod.Invoke(null, new object[1] { tex });
						}
					}
					catch (Exception exR)
					{
						Log("[静态字体] ResetAtlasTexture 异常：" + exR.Message);
					}
					tex.Apply(updateMipmaps: false, makeNoLongerReadable: false);
					List<GlyphRect> freeRects = new List<GlyphRect>();
					freeRects.Add(new GlyphRect(0, 0, ManualAtlasSize - 1, ManualAtlasSize - 1));
					List<GlyphRect> usedRects = new List<GlyphRect>();
					object[] args = new object[8]
					{
						glyphIndexes,
						2,
						(object)(GlyphPackingMode)0,
						freeRects,
						usedRects,
						(object)(GlyphRenderMode)rm,
						tex,
						null
					};
					bool ok = false;
					try
					{
						ok = (bool)TryAddGlyphsMethod.Invoke(null, args);
					}
					catch (Exception exInv)
					{
						Log($"[静态字体] 渲染模式={rm}, 格式={fmt} 调用异常：" + exInv.Message);
					}
					Glyph[] outGlyphs = (Glyph[])args[7];
					int count = 0;
					if (outGlyphs != null)
					{
						while (count < outGlyphs.Length && outGlyphs[count] != null)
						{
							count++;
						}
					}
					Log($"[静态字体] 渲染模式={rm}, 格式={fmt}: flag={ok}, 字形数={count}/{glyphIndexes.Count}");
					if (count > 0)
					{
						atlas = tex;
						packed = outGlyphs;
						usedMode = rm;
						usedFormat = fmt;
						break;
					}
					if ((Object)(object)tex != (Object)null)
					{
						Object.Destroy(tex);
					}
				}
				if ((Object)(object)atlas != (Object)null)
				{
					break;
				}
			}
			if ((Object)(object)atlas == (Object)null || packed == null)
			{
				Log("[静态字体] 所有渲染组合均失败，放弃该方案");
				return null;
			}
			atlas.Apply(updateMipmaps: false, makeNoLongerReadable: true);
			int glyphCount = 0;
			while (glyphCount < packed.Length && packed[glyphCount] != null)
			{
				glyphCount++;
			}
			CharacterInfo[] infos = new CharacterInfo[glyphCount];
			float inv = 1f / (float)ManualAtlasSize;
			for (int i = 0; i < glyphCount; i++)
			{
				Glyph val = packed[i];
				CharacterInfo ci = default(CharacterInfo);
				ci.index = (int)codepoints[i];
				ci.size = ManualFontSize;
				ci.style = FontStyle.Normal;
				GlyphRect r = val.glyphRect;
				ci.uv = new Rect((float)r.x * inv, (float)r.y * inv, (float)r.width * inv, (float)r.height * inv);
				GlyphMetrics m = val.metrics;
				int bx = Mathf.RoundToInt(m.horizontalBearingX);
				int by = Mathf.RoundToInt(m.horizontalBearingY);
				int gw = Mathf.RoundToInt(m.width);
				int gh = Mathf.RoundToInt(m.height);
				ci.vert = new Rect((float)bx, (float)by, (float)gw, (float)(-gh));
				ci.advance = Mathf.RoundToInt(m.horizontalAdvance);
				infos[i] = ci;
			}
			Material template = FindTextMaterialTemplate();
			if ((Object)(object)template == (Object)null)
			{
				Log("[静态字体] 未找到可用的文本材质模板，放弃该方案");
				return null;
			}
			Material mat = new Material(template);
			mat.mainTexture = atlas;
			// Alpha8 贴图采样得到 (0,0,0,a)，要靠 _TextureSampleAdd 补成白色；RGBA32 贴图则必须置零
			if (mat.HasProperty("_TextureSampleAdd"))
			{
				mat.SetColor("_TextureSampleAdd", (usedFormat == 1) ? new Color(1f, 1f, 1f, 0f) : new Color(0f, 0f, 0f, 0f));
			}
			Font manual = new Font();
			manual.material = mat;
			manual.characterInfo = infos;
			Log($"[静态字体] 构造完成：字形数={glyphCount}，渲染模式={usedMode}，纹理格式={usedFormat}，dynamic={manual.dynamic}");
			// 缩放探针：判断 native 是否会按请求字号缩放字形（这决定最终视觉大小是否正确）
			try
			{
				CharacterInfo ciA;
				CharacterInfo ciB;
				bool ga = manual.GetCharacterInfo('永', out ciA, 48, FontStyle.Normal);
				bool gb = manual.GetCharacterInfo('永', out ciB, 96, FontStyle.Normal);
				Log($"[静态字体] 缩放探针：请求48→minX={ciA.minX},maxX={ciA.maxX},advance={ciA.advance}(取到={ga})；请求96→minX={ciB.minX},maxX={ciB.maxX},advance={ciB.advance}(取到={gb})");
			}
			catch (Exception exProbe)
			{
				Log("[静态字体] 缩放探针异常：" + exProbe.Message);
			}
			return manual;
		}
		catch (Exception ex)
		{
			Log("[静态字体] 构建异常：" + ex);
			return null;
		}
	}

	// 找一个现成的文本材质当模板（复用游戏自带的着色器，避免 Shader.Find 在玩家构建里找不到）
	private Material FindTextMaterialTemplate()
	{
		try
		{
			Font[] fonts = Resources.FindObjectsOfTypeAll<Font>();
			Material dynamicMat = null;
			if (fonts != null)
			{
				foreach (Font f in fonts)
				{
					if ((Object)(object)f == (Object)null)
					{
						continue;
					}
					Material m = f.material;
					if ((Object)(object)m == (Object)null || (Object)(object)m.shader == (Object)null)
					{
						continue;
					}
					Log($"[静态字体] 材质模板候选：字体={f.name}，着色器={m.shader.name}，dynamic={f.dynamic}");
					if (!f.dynamic)
					{
						return m;
					}
					if ((Object)(object)dynamicMat == (Object)null)
					{
						dynamicMat = m;
					}
				}
			}
			if ((Object)(object)dynamicMat != (Object)null)
			{
				return dynamicMat;
			}
			Shader shader = Shader.Find("GUI/Text Shader");
			if ((Object)(object)shader != (Object)null)
			{
				Log("[静态字体] 未找到现成材质，改用 Shader.Find(\"GUI/Text Shader\")");
				return new Material(shader);
			}
		}
		catch (Exception ex)
		{
			Log("[静态字体] 查找材质模板异常：" + ex.Message);
		}
		return null;
	}

	// 由 GameManager.Update 每帧驱动的扫描节流器（每 3 秒一次）
	internal void TickSweeper()
	{
		_sweepTimer += Time.unscaledDeltaTime;
		if (_sweepTimer < 6f)
		{
			return;
		}
		_sweepTimer = 0f;
		SweepChineseTexts();
	}

	// 读取游戏自带 TMP 字体资产（优先中文资产 chinese_body）的 SDF 参数，
	// 我们的中文字体图集必须沿用同一套「字形边距 + 渐变缩放」，否则 SDF 文字会发虚、发细。
	private void LoadReferenceFontParameters()
	{
		try
		{
			TMP_FontAsset[] assets = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
			if (assets == null || assets.Length == 0)
			{
				Log("[参考字体资产] 未找到任何 TMP 字体资产");
				return;
			}
			TMP_FontAsset chosen = null;
			foreach (TMP_FontAsset a in assets)
			{
				if ((Object)(object)a == (Object)null)
				{
					continue;
				}
				if (a.name != null && a.name.IndexOf("chinese", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					chosen = a;
					break;
				}
			}
			if ((Object)(object)chosen == (Object)null && (Object)(object)TMP_FontAsset.defaultFontAsset != (Object)null)
			{
				chosen = TMP_FontAsset.defaultFontAsset;
			}
			if ((Object)(object)chosen == (Object)null)
			{
				chosen = assets[0];
			}
			if ((Object)(object)chosen == (Object)null)
			{
				return;
			}
			_refPadding = (int)chosen.fontInfo.Padding;
			Material m = ((TMP_Asset)chosen).material;
			float tw = 0f;
			float th = 0f;
			string shaderName = "(无材质)";
			if ((Object)(object)m != (Object)null)
			{
				if ((Object)(object)m.shader != (Object)null)
				{
					shaderName = m.shader.name;
				}
				if (m.HasProperty("_GradientScale"))
				{
					_refGradientScale = m.GetFloat("_GradientScale");
				}
				if (m.HasProperty("_TextureWidth"))
				{
					tw = m.GetFloat("_TextureWidth");
				}
				if (m.HasProperty("_TextureHeight"))
				{
					th = m.GetFloat("_TextureHeight");
				}
			}
			string atlasSize = "?";
			if ((Object)(object)chosen.atlas != (Object)null)
			{
				atlasSize = ((Texture)chosen.atlas).width + "x" + ((Texture)chosen.atlas).height;
			}
			Log($"[参考字体资产] {chosen.name}：图集={atlasSize}，字形边距={_refPadding}，_GradientScale={_refGradientScale}，_TextureWidth={tw}，_TextureHeight={th}，着色器={shaderName}");
		}
		catch (Exception ex)
		{
			Log("[参考字体资产] 读取异常：" + ex.Message);
		}
	}

	// 诊断：列出主菜单按钮等「自动本地化界面文本」（AutoLocalizeTextUI）实际持有的文本组件与字体
	private void LogAutoLocalizeTextUI()
	{
		try
		{
			AutoLocalizeTextUI[] list = Object.FindObjectsOfType<AutoLocalizeTextUI>();
			Log("[界面文本核对] AutoLocalizeTextUI 数量=" + ((list == null) ? 0 : list.Length));
			if (list == null)
			{
				return;
			}
			foreach (AutoLocalizeTextUI a in list)
			{
				if ((Object)(object)a == (Object)null)
				{
					continue;
				}
				UnityEngine.UI.Text f = a.textField;
				if ((Object)(object)f == (Object)null)
				{
					Log("[界面文本核对] " + a.gameObject.name + "：textField 为空");
					continue;
				}
				string fontName = ((Object)(object)f.font == (Object)null) ? "(空)" : f.font.name;
				bool ours = (Object)(object)f.font == (Object)(object)_legacyZhFont;
				string txt = Preview(f.text);
				bool hasCjk = ContainsCjk(f.text);
				Log($"[界面文本核对] {a.gameObject.name}：文本={txt}，含中文={hasCjk}，字体={fontName}，是我们的字体={ours}");
			}
		}
		catch (Exception ex)
		{
			Log("[界面文本核对] 异常：" + ex.Message);
		}
	}

	// 诊断：核对我们的动态字体当前实际光栅化了多少字形（若长时间只有 1 个，说明界面文本并没有真的用它渲染）
	private void LogOurFontStatus()
	{
		try
		{
			Font f = _legacyZhFont;
			if ((Object)(object)f == (Object)null)
			{
				Log("[字体核对] 动态字体为空");
				return;
			}
			CharacterInfo[] infos = f.characterInfo;
			int n = ((infos == null) ? 0 : infos.Length);
			Texture tex = ((Object)(object)f.material != (Object)null) ? f.material.mainTexture : null;
			string size = ((Object)(object)tex == (Object)null) ? "(无贴图)" : (((Texture)tex).width + "x" + ((Texture)tex).height);
			Log($"[字体核对] 动态字体={f.name}，已光栅化字形数={n}，贴图={size}");
			if ((Object)(object)_chineseFont != (Object)null)
			{
				int glyphs = ((_chineseFont.characterDictionary == null) ? 0 : _chineseFont.characterDictionary.Count);
				string atlasSize = ((Object)(object)_chineseFont.atlas == (Object)null) ? "(无)" : (((Texture)_chineseFont.atlas).width + "x" + ((Texture)_chineseFont.atlas).height);
				Material mat = ((TMP_Asset)_chineseFont).material;
				string shaderName = (((Object)(object)mat != (Object)null && (Object)(object)mat.shader != (Object)null) ? mat.shader.name : "(无)");
				Log($"[字体核对] 中文 TMP 资产：字典字形数={glyphs}，图集={atlasSize}，着色器={shaderName}");
			}
		}
		catch (Exception ex)
		{
			Log("[字体核对] 异常：" + ex.Message);
		}
	}

	// 一次性导出游戏自带中文 TMP 字体资产：字形表（精确矩形）+ 图集 PNG + 材质参数
	private void DumpFontAssetData()
	{
		try
		{
			string dir = @"D:\KDQSHH\字体资产工作\导出";
			Directory.CreateDirectory(dir);
			TMP_FontAsset[] assets = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
			int dumped = 0;
			foreach (TMP_FontAsset a in assets)
			{
				if ((Object)(object)a == (Object)null || a.name == null)
				{
					continue;
				}
				if (a.name.IndexOf("chinese", StringComparison.OrdinalIgnoreCase) < 0)
				{
					continue;
				}
				StringBuilder sb = new StringBuilder();
				TMPro.FaceInfo fi = a.fontInfo;
				sb.AppendLine("name	" + a.name);
				sb.AppendLine("PointSize	" + fi.PointSize + "	Scale	" + fi.Scale + "	Padding	" + fi.Padding + "	AtlasWidth	" + fi.AtlasWidth + "	AtlasHeight	" + fi.AtlasHeight);
				sb.AppendLine("LineHeight	" + fi.LineHeight + "	Baseline	" + fi.Baseline + "	Ascender	" + fi.Ascender + "	Descender	" + fi.Descender + "	CapHeight	" + fi.CapHeight + "	CenterLine	" + fi.CenterLine);
				sb.AppendLine("atlas	" + (((Object)(object)a.atlas != (Object)null) ? (((Texture)a.atlas).width + "x" + ((Texture)a.atlas).height) : "无"));
				Material mat = ((TMP_Asset)a).material;
				if ((Object)(object)mat != (Object)null)
				{
					string shaderName = (((Object)(object)mat.shader != (Object)null) ? mat.shader.name : "(无)");
					sb.AppendLine("material	" + shaderName);
					string[] props = new string[8] { "_GradientScale", "_TextureWidth", "_TextureHeight", "_FaceDilate", "_OutlineWidth", "_WeightNormal", "_WeightBold", "_Sharpness" };
					string propLine = "props";
					foreach (string pr in props)
					{
						if (mat.HasProperty(pr))
						{
							propLine += "	" + pr + "=" + mat.GetFloat(pr);
						}
					}
					sb.AppendLine(propLine);
				}
				Dictionary<int, TMP_Glyph> dict = a.characterDictionary;
				sb.AppendLine("glyphCount	" + ((dict == null) ? 0 : dict.Count));
				sb.AppendLine("id	x	y	width	height	xOffset	yOffset	xAdvance	scale");
				if (dict != null)
				{
					foreach (KeyValuePair<int, TMP_Glyph> kv in dict)
					{
						TMP_Glyph g = kv.Value;
						sb.AppendLine(kv.Key + "	" + g.x + "	" + g.y + "	" + g.width + "	" + g.height + "	" + g.xOffset + "	" + g.yOffset + "	" + g.xAdvance + "	" + g.scale);
					}
				}
				File.WriteAllText(Path.Combine(dir, "字形表_" + a.name + ".txt"), sb.ToString());
				if ((Object)(object)a.atlas != (Object)null)
				{
					SaveTexturePng(a.atlas, Path.Combine(dir, "图集_" + a.name + ".png"));
				}
				dumped++;
				Log("已导出字体资产：" + a.name + "，字形数=" + ((dict == null) ? 0 : dict.Count));
			}
			Log("字体资产导出完成，共 " + dumped + " 个（目录 " + dir + "）");
		}
		catch (Exception ex)
		{
			Log("导出字体资产异常：" + ex);
		}
	}

	// 把（可能不可读的）纹理拷进可读副本再存成 PNG
	private void SaveTexturePng(Texture2D tex, string path)
	{
		try
		{
			RenderTexture rt = RenderTexture.GetTemporary(((Texture)tex).width, ((Texture)tex).height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			Graphics.Blit((Texture)(object)tex, rt);
			RenderTexture prev = RenderTexture.active;
			RenderTexture.active = rt;
			Texture2D copy = new Texture2D(((Texture)tex).width, ((Texture)tex).height, TextureFormat.RGBA32, false);
			copy.ReadPixels(new Rect(0f, 0f, (float)((Texture)tex).width, (float)((Texture)tex).height), 0, 0);
			copy.Apply();
			RenderTexture.active = prev;
			RenderTexture.ReleaseTemporary(rt);
			File.WriteAllBytes(path, copy.EncodeToPNG());
			Object.Destroy(copy);
		}
		catch (Exception ex)
		{
			Log("导出图集 PNG 失败（" + path + "）：" + ex.Message);
		}
	}

	// 安全模式：模组目录下存在该文件时，完全不替换 TMP 字体（只替换旧版界面文本），用于排查故障
	private bool IsTmpReplacementDisabled()
	{
		try
		{
			string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (File.Exists(Path.Combine(dir, "安全模式-不替换TMP字体.txt")))
			{
				return true;
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	// 诊断：一次性列出场景里所有「名字带 Text/Label/Font」的活动组件，
	// 用于确认游戏界面文字到底由哪个文本系统渲染（先前只查了 TMP / UI.Text / TextMesh，均未命中主菜单标签）
	private void DumpTextLikeComponents()
	{
		try
		{
			Component[] comps = Object.FindObjectsOfType<Component>();
			HashSet<string> seen = new HashSet<string>();
			int shown = 0;
			foreach (Component c in comps)
			{
				if ((Object)(object)c == (Object)null)
				{
					continue;
				}
				string tn = c.GetType().Name;
				if (tn.IndexOf("Text", StringComparison.OrdinalIgnoreCase) < 0 && tn.IndexOf("Label", StringComparison.OrdinalIgnoreCase) < 0 && tn.IndexOf("Font", StringComparison.OrdinalIgnoreCase) < 0)
				{
					continue;
				}
				string key = tn + " ← " + c.gameObject.name;
				if (seen.Add(key) && shown < 60)
				{
					shown++;
					Log("[文本类组件] " + key);
				}
			}
			Log("[文本类组件] 共列出 " + shown + " 条（组件总数 " + comps.Length + "）");
		}
		catch (Exception ex)
		{
			Log("[文本类组件] 诊断异常：" + ex.Message);
		}
	}

	// 场景切换过程中不替换 TMP：切换期间是显存/内存峰值，此处动 TMP 极易触发 D3D11 分配失败而闪退
	private bool IsSceneBusy()
	{
		try
		{
			GameManager gm = GameManager.instance;
			if ((Object)(object)gm != (Object)null && gm.IsInSceneTransition)
			{
				return true;
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	// 扫描场景中所有文本（TMP / uGUI / TextMesh），把中文字体替换为霞鹜臻楷
	internal void SweepChineseTexts()
	{
		if ((Object)(object)_chineseFont == (Object)null && (Object)(object)_legacyZhFont == (Object)null)
		{
			return;
		}
		string lang = global::Language.Language.CurrentLanguage().ToString();
		try
		{
			// 同时用「活动对象枚举」与「全对象枚举」，取并集，避免漏掉场景内文本
			TMP_Text[] activeTexts = Object.FindObjectsOfType<TMP_Text>();
			TMP_Text[] allTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
			HashSet<int> seen = new HashSet<int>();
			List<TMP_Text> merged = new List<TMP_Text>();
			foreach (TMP_Text t0 in activeTexts)
			{
				if ((Object)(object)t0 != (Object)null && seen.Add(t0.GetInstanceID()))
				{
					merged.Add(t0);
				}
			}
			foreach (TMP_Text t1 in allTexts)
			{
				if ((Object)(object)t1 != (Object)null && seen.Add(t1.GetInstanceID()))
				{
					merged.Add(t1);
				}
			}
			int activeCount = 0;
			int cjkCount = 0;
			int replaced = 0;
			Dictionary<string, int> fontNames = new Dictionary<string, int>();
			// TMP 替换的三个前置条件：未开启安全模式、不在场景切换中、已过首次扫描（避开加载峰值）
			bool tmpAllowed = !_tmpDisabled && !IsSceneBusy() && _sweepIndex >= 1;
			foreach (TMP_Text t in merged)
			{
				string fontName = (t.font == null) ? "(空)" : t.font.name;
				bool isActive = (Object)(object)t.gameObject != (Object)null && t.gameObject.activeInHierarchy;
				if (isActive)
				{
					activeCount++;
				}
				int existing;
				fontNames.TryGetValue(fontName, out existing);
				fontNames[fontName] = existing + 1;
				string s = t.text;
				// 诊断取样：优先取样「活动且可见」的文本，用于确认游戏实际用哪个文本系统渲染界面
				if (_textDumpCount < 25 && (Object)(object)t.gameObject != (Object)null && (isActive || ContainsCjk(s)))
				{
					_textDumpCount++;
					Log($"[文本样本] {t.GetType().Name} 对象={t.gameObject.name} 活动={isActive} 字体={fontName} 文本={Preview(s)}");
				}
				if (string.IsNullOrEmpty(s) || !ContainsCjk(s))
				{
					continue;
				}
				cjkCount++;
				if (!tmpAllowed || (Object)(object)t.font == (Object)(object)_chineseFont)
				{
					continue;
				}
				ApplyChineseFont(t, "兜底扫描");
				replaced++;
			}
			if (_sweepLogCount < 6 || replaced > 0)
			{
				_sweepLogCount++;
				string fontSummary = "";
				int shown = 0;
				foreach (KeyValuePair<string, int> kv in fontNames)
				{
					if (shown >= 5)
					{
						break;
					}
					shown++;
					fontSummary += $"「{kv.Key}」×{kv.Value} ";
				}
				Log($"[兜底扫描] 语言={lang}，Active枚举={activeTexts.Length}，All枚举={allTexts.Length}，去重后={merged.Count}（活动 {activeCount}），含中文={cjkCount}，本次替换={replaced}，TMP替换可用={tmpAllowed}，字体分布：{fontSummary}");
				if (_sweepIndex == 0)
				{
					long allocMb = global::UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1048576;
					long reservedMb = global::UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1048576;
					long gfxMb = global::UnityEngine.Profiling.Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576;
					Log("[内存] 托管已分配=" + allocMb + "MB，引擎保留=" + reservedMb + "MB，显存估算=" + gfxMb + "MB");
				}
			}
			// uGUI 旧版文本（游戏中文界面实际使用的系统）
			int legacyReplaced = 0;
			int legacyTotal = 0;
			UnityEngine.UI.Text[] uiTexts = Object.FindObjectsOfType<UnityEngine.UI.Text>();
			foreach (UnityEngine.UI.Text ut in uiTexts)
			{
				if ((Object)(object)ut == (Object)null)
				{
					continue;
				}
				legacyTotal++;
				if ((Object)(object)ut.font == (Object)(object)_legacyZhFont)
				{
					continue;
				}
				ApplyLegacyFont(ut, "兜底扫描");
				legacyReplaced++;
			}
			// 3D 旧版文本
			int meshReplaced = 0;
			TextMesh[] textMeshes = Object.FindObjectsOfType<TextMesh>();
			foreach (TextMesh tm in textMeshes)
			{
				if ((Object)(object)tm == (Object)null || (Object)(object)tm.font == (Object)(object)_legacyZhFont)
				{
					continue;
				}
				ApplyTextMeshFont(tm, "兜底扫描");
				meshReplaced++;
			}
			Log($"[兜底扫描] uGUI文本={legacyTotal}，本次替换={legacyReplaced}；TextMesh 替换={meshReplaced}");
			_sweepCount += replaced;
			if (!_tk2dDumped)
			{
				_tk2dDumped = true;
				DumpTk2dDiagnostics();
			}
			if (_sweepIndex == 2)
			{
				DumpTextLikeComponents();
			}
			if (_sweepIndex == 1 && !_fontDumpDone)
			{
				_fontDumpDone = true;
				DumpFontAssetData();
			}
			if (_sweepIndex == 4 || _sweepIndex == 8)
			{
				LogAutoLocalizeTextUI();
				LogOurFontStatus();
			}
			_sweepIndex++;
			if (_sweepIndex <= 15 || _sweepIndex % 10 == 0)
			{
				LogSystemCounts();
			}
			if (_sweepIndex == 2 || _sweepIndex == 5)
			{
				DumpComponentHistogram();
			}
		}
		catch (Exception ex)
		{
			Log("[兜底扫描] 异常：" + ex.Message);
		}
	}

	// 诊断：每次扫描打印各文本系统的活动数量与中文数量，确定游戏 UI 到底用哪套文本系统
	internal void LogSystemCounts()
	{
		try
		{
			int tmpActive = 0;
			int tmpCjk = 0;
			TMP_Text[] tmps = Object.FindObjectsOfType<TMP_Text>();
			foreach (TMP_Text t in tmps)
			{
				if ((Object)(object)t == (Object)null)
				{
					continue;
				}
				tmpActive++;
				string s = t.text;
				if (!string.IsNullOrEmpty(s) && ContainsCjk(s))
				{
					tmpCjk++;
				}
			}
			int tkActive = 0;
			int tkCjk = 0;
			Type tkType = Type.GetType("tk2dTextMesh, Assembly-CSharp");
			if (tkType != null)
			{
				Object[] objs = Object.FindObjectsOfType(tkType);
				foreach (Object o in objs)
				{
					if ((Object)(object)o == (Object)null)
					{
						continue;
					}
					tkActive++;
					string s = GetTk2dText(o, tkType);
					if (!string.IsNullOrEmpty(s) && ContainsCjk(s))
					{
						tkCjk++;
					}
				}
			}
			int uiActive = 0;
			int uiCjk = 0;
			UnityEngine.UI.Text[] uis = Object.FindObjectsOfType<UnityEngine.UI.Text>();
			foreach (UnityEngine.UI.Text t in uis)
			{
				if ((Object)(object)t == (Object)null)
				{
					continue;
				}
				uiActive++;
				string s = t.text;
				if (!string.IsNullOrEmpty(s) && ContainsCjk(s))
				{
					uiCjk++;
				}
			}
			Log($"[文本系统统计] TMP 活动={tmpActive}/含中文={tmpCjk}；tk2dTextMesh 活动={tkActive}/含中文={tkCjk}；UI.Text 活动={uiActive}/含中文={uiCjk}");
			// 打印当前 UI 实际使用的字体身份，确认是否真的是霞鹜臻楷（还是被 Unity 回退成系统字体）
			if (uiActive > 0 && _uiFontDumpCount < 2)
			{
				_uiFontDumpCount++;
				HashSet<int> seenFonts = new HashSet<int>();
				foreach (UnityEngine.UI.Text t in uis)
				{
					if ((Object)(object)t == (Object)null || (Object)(object)t.font == (Object)null)
					{
						continue;
					}
					Font f = t.font;
					if (!seenFonts.Add(f.GetInstanceID()))
					{
						continue;
					}
					int ciLen = 0;
					try
					{
						CharacterInfo[] cis = f.characterInfo;
						ciLen = (cis == null) ? 0 : cis.Length;
					}
					catch
					{
					}
					string texInfo = "无";
					try
					{
						if ((Object)(object)f.material != (Object)null && (Object)(object)f.material.mainTexture != (Object)null)
						{
							texInfo = f.material.mainTexture.width + "x" + f.material.mainTexture.height;
						}
					}
					catch
					{
					}
					string names = (f.fontNames == null) ? "(null)" : string.Join(",", f.fontNames);
					Log($"[字体明细] 「{f.name}」：dynamic={f.dynamic}，fontNames={names}，characterInfo={ciLen}，字体贴图={texInfo}，是否本模组字体={((Object)(object)f == (Object)(object)_legacyZhFont)}");
				}
			}
		}
		catch (Exception ex)
		{
			Log("[文本系统统计] 异常：" + ex.Message);
		}
	}

	// 读取 tk2dTextMesh 的 text 字段（属性或字段）
	private static string GetTk2dText(Object o, Type tkType)
	{
		try
		{
			PropertyInfo pi = tkType.GetProperty("text");
			if (pi != null)
			{
				string s1 = pi.GetValue(o, null) as string;
				if (s1 != null)
				{
					return s1;
				}
			}
			FieldInfo fi = tkType.GetField("text");
			if (fi != null)
			{
				return fi.GetValue(o) as string;
			}
		}
		catch
		{
		}
		return null;
	}

	// 诊断：统计当前活动场景中所有组件类型出现次数，用于判断 UI 使用哪套系统
	internal void DumpComponentHistogram()
	{
		try
		{
			UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			GameObject[] roots = scene.GetRootGameObjects();
			Dictionary<string, int> counts = new Dictionary<string, int>();
			foreach (GameObject root in roots)
			{
				Component[] comps = root.GetComponentsInChildren<Component>(true);
				foreach (Component c in comps)
				{
					if ((Object)(object)c == (Object)null)
					{
						continue;
					}
					string key = c.GetType().Name;
					int existing;
					counts.TryGetValue(key, out existing);
					counts[key] = existing + 1;
				}
			}
			List<KeyValuePair<string, int>> sorted = new List<KeyValuePair<string, int>>(counts);
			sorted.Sort((KeyValuePair<string, int> a, KeyValuePair<string, int> b) => b.Value.CompareTo(a.Value));
			string summary = "";
			int shown = 0;
			foreach (KeyValuePair<string, int> kv in sorted)
			{
				if (shown >= 25)
				{
					break;
				}
				shown++;
				summary += $"{kv.Key}×{kv.Value} ";
			}
			Log($"[组件统计] 场景={scene.name}，根对象={roots.Length}，组件类型分布：{summary}");
		}
		catch (Exception ex)
		{
			Log("[组件统计] 异常：" + ex.Message);
		}
	}

	// 诊断：检查游戏 UI 文本是否使用 2D Toolkit（tk2dTextMesh）渲染，并列出已加载的 tk2d 字体资产
	internal void DumpTk2dDiagnostics()
	{
		try
		{
			Type tk2dTextMeshType = Type.GetType("tk2dTextMesh, Assembly-CSharp");
			if (tk2dTextMeshType == null)
			{
				Log("[tk2d诊断] 未找到 tk2dTextMesh 类型（说明游戏不是 tk2d 文本）");
				return;
			}
			Object[] meshes = Resources.FindObjectsOfTypeAll(tk2dTextMeshType);
			int active = 0;
			int cjk = 0;
			int dumped = 0;
			foreach (Object o in meshes)
			{
				Component c = o as Component;
				if ((Object)(object)c == (Object)null)
				{
					continue;
				}
				bool act = (Object)(object)c.gameObject != (Object)null && c.gameObject.activeInHierarchy;
				if (act)
				{
					active++;
				}
				string text = null;
				try
				{
					PropertyInfo pi = tk2dTextMeshType.GetProperty("text");
					if (pi != null)
					{
						text = pi.GetValue(o, null) as string;
					}
				}
				catch
				{
				}
				if (string.IsNullOrEmpty(text))
				{
					try
					{
						FieldInfo fi = tk2dTextMeshType.GetField("text");
						if (fi != null)
						{
							text = fi.GetValue(o) as string;
						}
					}
					catch
					{
					}
				}
				if (!string.IsNullOrEmpty(text) && ContainsCjk(text))
				{
					cjk++;
				}
				if (dumped < 10 && act)
				{
					dumped++;
					Log($"[tk2d样本] 对象={c.gameObject.name} 文本={Preview(text)}");
				}
			}
			Log($"[tk2d诊断] tk2dTextMesh 总数={meshes.Length}，活动={active}，含中文={cjk}");
			Type tk2dFontDataType = Type.GetType("tk2dFontData, Assembly-CSharp");
			if (tk2dFontDataType != null)
			{
				Object[] fontAssets = Resources.FindObjectsOfTypeAll(tk2dFontDataType);
				Log($"[tk2d诊断] 已加载 tk2dFontData 资产 {fontAssets.Length} 个");
				int shown = 0;
				foreach (Object f in fontAssets)
				{
					if (shown >= 20)
					{
						break;
					}
					shown++;
					string texName = "?";
					try
					{
						FieldInfo matField = tk2dFontDataType.GetField("material");
						Material mat = (matField != null) ? (matField.GetValue(f) as Material) : null;
						if ((Object)(object)mat != (Object)null && (Object)(object)mat.mainTexture != (Object)null)
						{
							texName = mat.mainTexture.name;
						}
					}
					catch
					{
					}
					Log($"[tk2d字体] {f.name}，贴图={texName}");
				}
			}
		}
		catch (Exception ex)
		{
			Log("[tk2d诊断] 异常：" + ex.Message);
		}
	}

	// 缩略预览文本，便于日志定位
	internal static string Preview(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return "(空)";
		}
		string t = s.Replace("\r", " ").Replace("\n", " ");
		return t.Length <= 20 ? t : t.Substring(0, 20);
	}

	// 判断当前语言是否为中文（兼容 ZH / ZH_CN / ZH_SG 等写法）
	private static bool IsChineseLanguage()
	{
		try
		{
			string lang = global::Language.Language.CurrentLanguage().ToString();
			return lang == "ZH" || lang.StartsWith("ZH_", StringComparison.Ordinal);
		}
		catch
		{
			return false;
		}
	}

	// 判断字符串是否含中日韩表意文字（用于只替换中文文本）；0x2E80–0x9FFF 覆盖 CJK 部首与统一表意文字
	private static bool ContainsCjk(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return false;
		}
		foreach (char c in s)
		{
			if ((int)c >= 0x2E80 && (int)c <= 0x9FFF)
			{
				return true;
			}
		}
		return false;
	}

	public void Unload()
	{
		UninstallPatch();
	}

	public override string GetVersion()
	{
		return "1.0.0";
	}

	// 模组菜单项：开关按钮 + 说明条目
	public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
	{
		List<IMenuMod.MenuEntry> list = new List<IMenuMod.MenuEntry>();
		if (toggleButtonEntry.HasValue)
		{
			list.Add(toggleButtonEntry.Value);
		}
		// 说明条目：MenuEntry 是主干构造函数结构体，需用完整构造函数
		// Saver/Loader 不为空，避免菜单系统 NRE
		list.Add(new IMenuMod.MenuEntry(
			name: "简体中文字体替换",
			values: Array.Empty<string>(),
			description: "使用霞鹜臻楷替换简体中文渲染字体",
			saver: _ => { },
			loader: () => 0
		));
		return list;
	}

	// 把字体复制到纯 ASCII 临时路径（native 字体引擎对含中文的路径支持差）。
	// 关键修复：GDI 注册过的字体文件会被系统占用，所以这里用「轮换文件名」，并且复制失败也不影响后续流程。
	private string TryCopyFontToAsciiPath(string fontPath)
	{
		string[] tempDirs = new string[2] { @"D:\KDQSHH\TempTest", @"D:\重要文件\Downloads\temp" };
		string[] fileNames = new string[4] { "hk_zh_font_a.ttf", "hk_zh_font_b.ttf", "hk_zh_font_c.ttf", "hk_zh_font.ttf" };
		string lastError = null;
		foreach (string dir in tempDirs)
		{
			try
			{
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
			}
			catch (Exception exDir)
			{
				lastError = exDir.Message;
				continue;
			}
			foreach (string name in fileNames)
			{
				try
				{
					string candidate = Path.Combine(dir, name);
					File.Copy(fontPath, candidate, overwrite: true);
					if (File.Exists(candidate))
					{
						Log("字体已复制到 ASCII 临时路径：" + candidate);
						return candidate;
					}
				}
				catch (Exception exCopy)
				{
					lastError = exCopy.Message;
				}
			}
		}
		Log("复制字体到 ASCII 临时路径失败（不影响「GDI 注册 + 按字体名创建」主流程）：" + lastError);
		return null;
	}

	private void LoadFontIfNeeded()
	{
		if ((Object)(object)_chineseFont != (Object)null)
		{
			return;
		}
		try
		{
			string fontPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "霞鹜臻楷.ttf");
			if (!File.Exists(fontPath))
			{
				Log("字体文件不存在：" + fontPath);
				return;
			}
			// 读取为字节数组再加载——FontEngine.LoadFontFace(string) 在含中文路径时返回 Invalid_File_Path
			byte[] fontData = File.ReadAllBytes(fontPath);
			if (fontData == null || fontData.Length == 0)
			{
				Log("字体文件读取失败：" + fontPath);
				return;
			}
			Log($"正在构建简体中文字体资产，字体文件 {fontData.Length} 字节，路径：{fontPath}");
			// native 字体引擎对含中文路径支持差：复制到 ASCII 临时路径后用路径方式加载，
			// 保证 FreeType 以文件句柄方式访问字体（byte[] 内存加载的面在图集渲染时可能不完整）
			string asciiFontPath = TryCopyFontToAsciiPath(fontPath);
			// 关键：游戏中文界面使用旧版 uGUI 文本（UnityEngine.UI.Text）+ 动态字体。
			// 方式一：把字体注册进当前会话字体表，再按系统字体名创建动态字体；自检不通过绝不替换（避免文字消失）。
			CreateLegacyFontIfNeeded(asciiFontPath, fontPath);
		LoadReferenceFontParameters();
			(TMP_FontAsset, int) tuple = BuildChineseFontAsset(fontData, asciiFontPath);
			(_chineseFont, _) = tuple;
			if ((Object)(object)_chineseFont == (Object)null)
			{
				Log("简体中文字体资产构建失败");
			}
			else
			{
				Log($"简体中文字体资产构建成功，包含 {_chineseFont.fontInfo.CharacterCount} 个字形（渲染了 {tuple.Item2} 个候选字形）");
			}
			// 方式三兜底：系统字体名方式没成功时，直接用字形数据手工构造静态字体交给 uGUI。
			// 放在字体资产构建之后执行，避免重复加载字体面影响 TMP 图集。
			if ((Object)(object)_legacyZhFont == (Object)null)
			{
				Font manual = TryBuildManualLegacyFont(fontData, asciiFontPath);
				if (SelfTestFont(manual, "手工静态字体"))
				{
					_legacyZhFont = manual;
					Log("旧版文本字体创建成功（手工静态字体方式）");
				}
				else
				{
					_legacyZhFont = null;
					Log("警告：手工静态字体自检未通过，为免文字消失，本次不改动游戏字体");
				}
			}
		}
		catch (Exception ex)
		{
			Log("构建字体资产时发生异常：" + ex);
		}
	}

	// 主构建流程：初始化字体引擎 → 收集码位 → 分批渲染图集 → 串联 fallback 链
	private (TMP_FontAsset fontAsset, int renderedCount) BuildChineseFontAsset(byte[] fontData, string asciiFontPath)
	{
		FontEngineError val = FontEngine.InitializeFontEngine();
		if ((int)val > 0)
		{
			Log("FontEngine 初始化失败：" + val.ToString());
			return (fontAsset: null, renderedCount: 0);
		}
		// 校准诊断：用系统 Arial 走同一套 API，确认「路径加载」与「FaceInfo.scale 正常取值范围」，
		// 便于对比判断霞鹜臻楷的 scale=1 是否异常。
		try
		{
			string sysFont = @"C:\Windows\Fonts\arial.ttf";
			if (File.Exists(sysFont))
			{
				FontEngineError ce = FontEngine.LoadFontFace(sysFont, 36);
				if ((int)ce <= 0)
				{
					global::UnityEngine.TextCore.FaceInfo cfi = FontEngine.GetFaceInfo();
					Log($"[校准] Arial 36pt: family={cfi.familyName}, pointSize={cfi.pointSize}, scale={cfi.scale}, lineHeight={cfi.lineHeight}");
				}
				else
				{
					Log("[校准] Arial 路径加载失败：" + ce);
				}
			}
			else
			{
				Log("[校准] 未找到 C:\\Windows\\Fonts\\arial.ttf");
			}
		}
		catch (Exception exCal)
		{
			Log("[校准] 异常：" + exCal.Message);
		}
		// 优先用 ASCII 路径加载字体面（FreeType 文件句柄方式），失败再退回内存加载。
		// 注：校准日志显示 Arial 36pt 的 FaceInfo.scale 同样为 1，故 scale=1 是本版 Unity 的正常值，
		// 不能作为判定依据；真正的问题是图集纹理未初始化 + 未预置空闲矩形。
		bool loaded = false;
		if (!string.IsNullOrEmpty(asciiFontPath))
		{
			FontEngineError r1 = FontEngine.LoadFontFace(asciiFontPath, 36);
			loaded = (int)r1 <= 0;
			Log($"路径方式加载字体：{(loaded ? "成功" : ("失败 " + r1))}（{asciiFontPath}）");
		}
		if (!loaded)
		{
			FontEngineError r3 = FontEngine.LoadFontFace(fontData, 36);
			loaded = (int)r3 <= 0;
			Log($"内存方式加载字体：{(loaded ? "成功" : ("失败 " + r3))}");
		}
		if (!loaded)
		{
			Log("字体面加载失败：路径与内存两种方式均失败");
			return (fontAsset: null, renderedCount: 0);
		}
		LogFaceInfo("加载后");
		List<uint> list = new List<uint>();
		List<uint> list2 = new List<uint>();
		// CJK 统一表意文字、CJK 标点、全角符号、ASCII 基本区
		CollectCodepoints(19968u, 40959u, list, list2);
		CollectCodepoints(12288u, 12351u, list, list2);
		CollectCodepoints(65280u, 65519u, list, list2);
		CollectCodepoints(32u, 127u, list, list2);
		// 常用中英文标点与特殊符号
		uint[] array = new uint[15]
		{
			8212u, 8211u, 8230u, 183u, 8220u, 8221u, 8216u, 8217u, 169u, 174u,
			182u, 161u, 191u, 215u, 247u
		};
		uint[] array2 = array;
		foreach (uint num in array2)
		{
			uint item = 0u;
			if (FontEngine.TryGetGlyphIndex(num, out item))
			{
				list.Add(num);
				list2.Add(item);
			}
		}
		Log($"共找到 {list.Count} 个可用字形码位");
		if (list.Count == 0)
		{
			return (fontAsset: null, renderedCount: 0);
		}
		// 诊断：打印样例字形度量。若图集矩形在 1000px 量级，说明字形按字体单位取尺寸，
		// 需要数百张图集才装得下，必须先把 FaceInfo.scale 修正到 0.036 量级。
		try
		{
			Glyph sampleGlyph;
			if (FontEngine.TryGetGlyphWithUnicodeValue(0x6C38u, (GlyphLoadFlags)0, out sampleGlyph))
			{
				Log($"样例字形 '永'(U+6C38): 图集矩形={sampleGlyph.glyphRect.width}x{sampleGlyph.glyphRect.height}, 水平推进={sampleGlyph.metrics.horizontalAdvance}, 左轴承={sampleGlyph.metrics.horizontalBearingX}, 上轴承={sampleGlyph.metrics.horizontalBearingY}, scale={sampleGlyph.scale}");
			}
			else
			{
				Log("样例字形 '永'(U+6C38) 加载失败");
			}
		}
		catch (Exception ex)
		{
			Log("样例字形诊断异常：" + ex.Message);
		}
		// 每批最多 256 个字形、最多 200 张 4096x4096 图集，首张为主字体资产
		List<TMP_FontAsset> list3 = new List<TMP_FontAsset>();
		int num2 = 0;
		int j = 0;
		int num3 = 0;
		(TMP_FontAsset, int) tuple;
		for (; j < list2.Count; j += tuple.Item2)
		{
			if (num3 >= 200)
			{
				break;
			}
			num3++;
			int val3 = list2.Count - j;
			// 一次性把所有剩余字形交给 native 打包：能装多少装多少（返回实际字形数），
			// 装不下的在下一轮继续。4096x4096 图集足以容纳约 6000 个 40px 字形，通常只需 1~2 张图集。
			int count = val3;
			List<uint> range = list.GetRange(j, count);
			List<uint> range2 = list2.GetRange(j, count);
			tuple = BuildSingleAtlasAuto(range, range2, 36, 4096);
			if ((Object)(object)tuple.Item1 == (Object)null || tuple.Item2 == 0)
			{
				break;
			}
			list3.Add(tuple.Item1);
			num2 += tuple.Item2;
		}
		if (list3.Count == 0)
		{
			return (fontAsset: null, renderedCount: 0);
		}
		// 串联 fallback：第 k 张图集放不下的字符回退到第 k+1 张
		for (int k = 0; k < list3.Count - 1; k++)
		{
			if (list3[k].fallbackFontAssets == null)
			{
				list3[k].fallbackFontAssets = new List<TMP_FontAsset>();
			}
			list3[k].fallbackFontAssets.Add(list3[k + 1]);
		}
		return (fontAsset: list3[0], renderedCount: num2);
	}

	// 遍历码位区间，只保留字体中真实存在（有字形索引）的码位
	private void CollectCodepoints(uint start, uint end, List<uint> codepoints, List<uint> glyphIndexes)
	{
		for (uint num = start; num <= end; num++)
		{
			uint item = 0u;
			if (FontEngine.TryGetGlyphIndex(num, out item))
			{
				codepoints.Add(num);
				glyphIndexes.Add(item);
			}
		}
	}

	// 诊断：打印字体面全部关键度量。
	private void LogFaceInfo(string tag)
	{
		try
		{
			global::UnityEngine.TextCore.FaceInfo fi = FontEngine.GetFaceInfo();
			Log($"[{tag}] FaceInfo: family={fi.familyName}, pointSize={fi.pointSize}, scale={fi.scale}, lineHeight={fi.lineHeight}, baseline={fi.baseline}, ascentLine={fi.ascentLine}, descentLine={fi.descentLine}, capLine={fi.capLine}, meanLine={fi.meanLine}, tabWidth={fi.tabWidth}");
		}
		catch (Exception ex)
		{
			Log($"[{tag}] FaceInfo 获取异常：" + ex.Message);
		}
	}

	// 自动尝试多种纹理格式/边距/打包模式组合，返回第一个能整批渲染成功的图集
	private (TMP_FontAsset asset, int renderedCount) BuildSingleAtlasAuto(List<uint> codepoints, List<uint> glyphIndexes, int pointSize, int atlasSize)
	{
		// Alpha8 是 TMP 官方生成 SDF 图集时使用的格式，优先尝试；RGBA32 次之。
		int[] formats = new int[3] { 1, 4, 63 };
		// 优先沿用游戏自带字体资产的边距（SDF 渐变宽度一致），并限制在 2~6 之间（再大 4096 图集就装不下 5933 个字形）
		List<int> padCandidates = new List<int>();
		int refPad = Mathf.Clamp(_refPadding, 2, 6);
		padCandidates.Add(refPad);
		int[] extraPads = new int[2] { 5, 2 };
		foreach (int ep in extraPads)
		{
			if (!padCandidates.Contains(ep))
			{
				padCandidates.Add(ep);
			}
		}
		int[] paddings = padCandidates.ToArray();
		int[] packingModes = new int[2] { 0, 2 };
		int[] renderModes = new int[2] { 4134, 4165 };
		foreach (int rm in renderModes)
		{
			foreach (int fmt in formats)
			{
				foreach (int pad in paddings)
				{
					foreach (int pm in packingModes)
					{
						(TMP_FontAsset, int) result = BuildSingleAtlas(codepoints, glyphIndexes, pointSize, atlasSize, pad, fmt, pm, rm);
						if ((Object)(object)result.Item1 != (Object)null && result.Item2 > 0)
						{
							Log($"[图集配置] 采用 渲染模式={rm}, 纹理格式={fmt}, 边距={pad}, 打包模式={pm}");
							return result;
						}
						Log($"[图集配置] 失败组合：渲染模式={rm}, 纹理格式={fmt}, 边距={pad}, 打包模式={pm}");
					}
				}
			}
		}
		return (asset: null, renderedCount: 0);
	}

	// 渲染一批字形到单张图集，并构建对应的 TMP_FontAsset
	private (TMP_FontAsset asset, int renderedCount) BuildSingleAtlas(List<uint> codepoints, List<uint> glyphIndexes, int pointSize, int atlasSize, int padding, int textureFormat, int packingMode, int renderMode)
	{
		MethodInfo tryAddGlyphsMethod = TryAddGlyphsMethod;
		if (tryAddGlyphsMethod == null)
		{
			Log("FontEngine.TryAddGlyphsToTexture 不可访问，无法生成图集");
			return (asset: null, renderedCount: 0);
		}
		Texture2D val = new Texture2D(atlasSize, atlasSize, (TextureFormat)textureFormat, false);
		((Texture)val).filterMode = (FilterMode)1;
		((Texture)val).wrapMode = (TextureWrapMode)1;
		// 关键：native 打包前必须先 ResetAtlasTexture 初始化图集纹理，否则直接返回 false（0 字形）。
		// 参考 UnityEngine.TextCore.FontAsset 的用法：Resize → ResetAtlasTexture → Apply。
		try
		{
			if (ResetAtlasMethod != null)
			{
				ResetAtlasMethod.Invoke(null, new object[1] { val });
			}
			else
			{
				Log("FontEngine.ResetAtlasTexture 不可访问，跳过图集复位");
			}
		}
		catch (Exception exR)
		{
			Log("ResetAtlasTexture 异常：" + exR.Message);
		}
		val.Apply(updateMipmaps: false, makeNoLongerReadable: false);
		Log($"新建图集纹理: {atlasSize}x{atlasSize}, 格式={textureFormat}, 字形数量: {glyphIndexes.Count}");
		LogFaceInfo("渲染前");
		// 关键：空闲矩形必须预置“整张图集”的区域，native 才能据此放置字形。
		// TextCore 的 FontAsset 就是这么做的：freeGlyphRects = { (0,0,w-1,h-1) }。
		List<GlyphRect> list = new List<GlyphRect>();
		list.Add(new GlyphRect(0, 0, atlasSize - 1, atlasSize - 1));
		List<GlyphRect> list2 = new List<GlyphRect>();
		object[] array = new object[8]
		{
			glyphIndexes,
			padding,
			(object)(GlyphPackingMode)packingMode,
			list,
			list2,
			(object)(GlyphRenderMode)renderMode,
			val,
			null
		};
		bool flag = (bool)tryAddGlyphsMethod.Invoke(null, array);
		Glyph[] array2 = (Glyph[])array[7];
		int actualCount = 0;
		if (array2 != null)
		{
			while (actualCount < array2.Length && array2[actualCount] != null)
			{
				actualCount++;
			}
		}
		Log($"TryAddGlyphsToTexture 返回值: flag={flag}, 实际字形数={actualCount}, 输入字形数={glyphIndexes.Count}");
		// 诊断：打印已占用矩形尺寸，判断是否因字形按字体单位（1000px级别）取尺寸导致图集只能塞进十几个字
		Log($"渲染后矩形统计: 空闲矩形={list.Count}, 已用矩形={list2.Count}");
		for (int d = 0; d < list2.Count && d < 3; d++)
		{
			GlyphRect r = list2[d];
			Log($"已用矩形[{d}]: x={r.x}, y={r.y}, 宽={r.width}, 高={r.height}");
		}
		for (int d = 0; d < list.Count && d < 3; d++)
		{
			GlyphRect r = list[d];
			Log($"空闲矩形[{d}]: x={r.x}, y={r.y}, 宽={r.width}, 高={r.height}");
		}
		if (actualCount == 0)
		{
			Log("图集渲染失败或无可用字形");
			if ((Object)(object)val != (Object)null)
			{
				Object.Destroy(val);
			}
			return (asset: null, renderedCount: 0);
		}
		if (!flag)
		{
			Log($"警告：本批未完全渲染（{actualCount}/{glyphIndexes.Count}），将继续下一批");
		}
		Log($"图集渲染完成：{actualCount} / {glyphIndexes.Count} 个字形已编码到 {atlasSize}x{atlasSize} 图集");
		// 上传 native 写入的图集数据到纹理；上传后立即释放 CPU 端副本（省 16MB 内存，缓解显存/内存压力）
		val.Apply(updateMipmaps: false, makeNoLongerReadable: true);
		// 把渲染结果逐字形转换为 TMP_Glyph（图集坐标 + 度量信息）
		TMP_Glyph[] array3 = new TMP_Glyph[actualCount];
		for (int i = 0; i < actualCount; i++)
		{
			Glyph val2 = array2[i];
			TMP_Glyph val3 = new TMP_Glyph();
			((TMP_TextElement)val3).id = (int)codepoints[i];
			GlyphRect glyphRect = val2.glyphRect;
			((TMP_TextElement)val3).x = glyphRect.x;
			glyphRect = val2.glyphRect;
			((TMP_TextElement)val3).y = glyphRect.y;
			glyphRect = val2.glyphRect;
			((TMP_TextElement)val3).width = glyphRect.width;
			glyphRect = val2.glyphRect;
			((TMP_TextElement)val3).height = glyphRect.height;
			GlyphMetrics metrics = val2.metrics;
			((TMP_TextElement)val3).xOffset = metrics.horizontalBearingX;
			metrics = val2.metrics;
			((TMP_TextElement)val3).yOffset = metrics.horizontalBearingY;
			metrics = val2.metrics;
			((TMP_TextElement)val3).xAdvance = metrics.horizontalAdvance;
			((TMP_TextElement)val3).scale = 1f;
			array3[i] = val3;
		}
		// 用字体引擎的 FaceInfo 组装字体资产元数据
		global::UnityEngine.TextCore.FaceInfo faceInfo = FontEngine.GetFaceInfo();
		TMPro.FaceInfo val4 = new TMPro.FaceInfo();
		val4.Name = faceInfo.familyName;
		val4.PointSize = pointSize;
		val4.Scale = faceInfo.scale;
		val4.CharacterCount = array3.Length;
		val4.LineHeight = faceInfo.lineHeight;
		val4.Baseline = faceInfo.baseline;
		val4.Ascender = faceInfo.ascentLine;
		val4.CapHeight = faceInfo.capLine;
		val4.Descender = faceInfo.descentLine;
		val4.CenterLine = faceInfo.meanLine;
		val4.SuperscriptOffset = faceInfo.superscriptOffset;
		val4.SubscriptOffset = faceInfo.subscriptOffset;
		val4.SubSize = faceInfo.superscriptSize;
		val4.UnderlineThickness = 0f;
		val4.TabWidth = 10f * (float)pointSize;
		val4.Padding = padding;
		val4.AtlasWidth = atlasSize;
		val4.AtlasHeight = atlasSize;
		TMP_FontAsset val5 = ScriptableObject.CreateInstance<TMP_FontAsset>();
		val5.AddFaceInfo(val4);
		val5.AddGlyphInfo(array3);
		val5.AddKerningInfo(new KerningTable());
		val5.atlas = val;
		val5.fontAssetType = TMP_FontAsset.FontAssetTypes.SDF;
		val5.fallbackFontAssets = new List<TMP_FontAsset>();
		// 材质优先克隆游戏默认字体材质（保证 SDF 着色器参数一致），否则退回内置着色器
		TMP_FontAsset defaultFontAsset = TMP_FontAsset.defaultFontAsset;
		Material val6;
		if ((Object)(object)defaultFontAsset != (Object)null && (Object)(object)((TMP_Asset)defaultFontAsset).material != (Object)null)
		{
			val6 = new Material(((TMP_Asset)defaultFontAsset).material);
		}
		else
		{
			Shader val7 = Shader.Find("TextMeshPro/Distance Field");
			if ((Object)(object)val7 == (Object)null)
			{
				val7 = Shader.Find("Standard");
			}
			val6 = new Material(val7);
		}
		val6.mainTexture = val;
		// 关键：SDF 着色器依赖材质上的图集尺寸与渐变缩放参数，必须与我们的图集对齐，
		// 否则文字会发虚、发细，看起来「不像霞鹜臻楷」。
		try
		{
			if (val6.HasProperty("_TextureWidth"))
			{
				val6.SetFloat("_TextureWidth", atlasSize);
			}
			if (val6.HasProperty("_TextureHeight"))
			{
				val6.SetFloat("_TextureHeight", atlasSize);
			}
			if (val6.HasProperty("_GradientScale"))
			{
				val6.SetFloat("_GradientScale", (float)(padding * 2 + 1));
			}
			string shaderName = (((Object)(object)val6.shader != (Object)null) ? val6.shader.name : "(无着色器)");
			Log($"[材质对齐] 着色器={shaderName}，图集={atlasSize}，边距={padding}，_GradientScale={padding * 2 + 1}（游戏参考值 {_refGradientScale}）");
		}
		catch (Exception exMat)
		{
			Log("[材质对齐] 异常：" + exMat.Message);
		}
		((Object)val6).name = "简体中文字体替换-材质";
		((TMP_Asset)val5).material = val6;
		return (asset: val5, renderedCount: actualCount);
	}

	// 诊断：当默认组合渲染失败时，遍历不同纹理格式/边距/打包模式各渲染 8 个字形，
	// 定位到底哪种组合可用（日志会给出每个组合的 flag 与实际字形数）。
	private void DiagnoseRenderCombinations(List<uint> glyphIndexes)
	{
		if (TryAddGlyphsMethod == null || glyphIndexes == null || glyphIndexes.Count == 0)
		{
			return;
		}
		int take = Math.Min(8, glyphIndexes.Count);
		int[] formats = new int[3] { 4, 1, 63 };
		int[] paddings = new int[3] { 1, 2, 4 };
		int[] packingModes = new int[2] { 0, 2 };
		foreach (int fmt in formats)
		{
			foreach (int pad in paddings)
			{
				foreach (int pm in packingModes)
				{
					Texture2D tex = null;
					try
					{
						tex = new Texture2D(4096, 4096, (TextureFormat)fmt, false);
						List<uint> sample = glyphIndexes.GetRange(0, take);
						List<GlyphRect> free = new List<GlyphRect>();
						List<GlyphRect> used = new List<GlyphRect>();
						object[] args = new object[8]
						{
							sample,
							pad,
							(GlyphPackingMode)pm,
							free,
							used,
							(GlyphRenderMode)4134,
							tex,
							null
						};
						bool ok = (bool)TryAddGlyphsMethod.Invoke(null, args);
						Glyph[] gs = (Glyph[])args[7];
						int n = 0;
						if (gs != null)
						{
							while (n < gs.Length && gs[n] != null)
							{
								n++;
							}
						}
						string rectInfo = "";
						if (used.Count > 0)
						{
							GlyphRect r0 = used[0];
							rectInfo = $", 首矩形={r0.width}x{r0.height}";
						}
						Log($"[诊断] 纹理格式={fmt} 边距={pad} 打包={pm} => flag={ok}, 实际字形={n}, 已用矩形={used.Count}{rectInfo}");
					}
					catch (Exception ex)
					{
						Log($"[诊断] 纹理格式={fmt} 边距={pad} 打包={pm} 异常：{ex.Message}");
					}
					finally
					{
						if ((Object)(object)tex != (Object)null)
						{
							Object.Destroy(tex);
						}
					}
				}
			}
		}
	}

	// 安装一个 Postfix 补丁并记录结果
	private void PatchMember(Type target, string methodName, Type patchClass, string label)
	{
		try
		{
			MethodInfo targetMethod = target.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if (targetMethod == null)
			{
				Log($"[补丁] 找不到 {target.Name}.{methodName}，跳过 {label}");
				return;
			}
			MethodInfo postfix = patchClass.GetMethod("Postfix", BindingFlags.Static | BindingFlags.Public);
			if (postfix == null)
			{
				Log($"[补丁] {patchClass.Name} 缺少 Postfix，跳过 {label}");
				return;
			}
			_harmony.Patch((MethodBase)targetMethod, (HarmonyMethod)null, new HarmonyMethod(postfix), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
			Log($"[补丁] {label} 安装成功");
		}
		catch (Exception ex)
		{
			Log($"[补丁] {label} 安装异常：{ex.Message}");
		}
	}

	// 安装 Harmony 补丁：Postfix 挂到 ChangeFontByLanguage.SetFont 之后
	private void InstallPatch()
	{
		if (!_patchInstalled)
		{
			_harmony = new Harmony("com.modding.simplifiedchinese.font");
			// 重要（防闪退）：TMP 字体不再用实时补丁替换。
			// 原因：TMP 实时替换会在场景加载的显存/内存峰值期触发 TMP 重新排版与字体图集处理，
			// 实测加载存档时会因 D3D11 创建纹理失败（E_OUTOFMEMORY）而闪退。
			// 现在 TMP 一律交给「兜底扫描器」在场景稳定后统一替换，且只替换含中文的文本。
			// 旧版 uGUI 文本用的是 Unity 原生动态字体（开销极小），保留实时补丁。
			PatchMember(typeof(UnityEngine.UI.Text), "set_font", typeof(UiTextFontSetterPatch), "UI.Text.set_font");
			MethodInfo uiTextOnEnable = typeof(UnityEngine.UI.Text).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if (uiTextOnEnable != null)
			{
				PatchMember(typeof(UnityEngine.UI.Text), "OnEnable", typeof(UiTextOnEnablePatch), "UI.Text.OnEnable");
			}
			else
			{
				PatchMember(typeof(UnityEngine.UI.MaskableGraphic), "OnEnable", typeof(UiTextOnEnablePatch), "MaskableGraphic.OnEnable");
			}
			PatchMember(typeof(TextMesh), "set_font", typeof(TextMeshFontSetterPatch), "TextMesh.set_font");
			// GameManager.Update 驱动兜底扫描（TMP 替换唯一入口）
			PatchMember(typeof(GameManager), "Update", typeof(GameManagerUpdatePatch), "GameManager.Update");
			_patchInstalled = true;
			Log("字体替换补丁已安装（TMP 实时补丁已按防闪退要求移除，改为延迟扫描替换）");
		}
	}

	private void UninstallPatch()
	{
		if (_patchInstalled && _harmony != null)
		{
			_harmony.UnpatchSelf();
			_patchInstalled = false;
		}
	}
}
