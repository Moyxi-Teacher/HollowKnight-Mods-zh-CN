using System;
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;
using System.Globalization;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace HollowKnightHaptics;

[BepInPlugin("com.will.hollowknight.haptics", "Hollow Knight Haptics", "0.1.2")]
public class HapticsPlugin : BaseUnityPlugin
{
	internal static ManualLogSource Log;

	internal static ConfigEntry<bool> KeepRumble;

	internal static ConfigEntry<float> MasterGain;

	// 每个震动剪辑一个独立倍率：启动时扫描 clips 目录 / 别名表 / 脚步表自动注册。
	// key = WAV 文件名去掉 .wav 和 _vibration 后的剪辑名，value = 对应倍率配置（默认 1.0 = 基准版 0.1.2）。
	internal static readonly Dictionary<string, ConfigEntry<float>> ClipGains = new Dictionary<string, ConfigEntry<float>>(StringComparer.OrdinalIgnoreCase);
				private static readonly Dictionary<string, string> clipDisplayNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "acid_ambient_loop", "酸液氛围循环" },
		{ "automaton_generic_damage_1", "傀儡受击-1" },
		{ "automaton_generic_damage_2", "傀儡受击-2" },
		{ "automaton_generic_damage_3", "傀儡受击-3" },
		{ "bell_beast_bellway_emerge_and_spin", "钟兽现身旋转" },
		{ "bell_beast_jump", "钟兽跳" },
		{ "bell_bench_appear", "钟亭出现" },
		{ "bell_bench_bell_large", "钟亭大钟" },
		{ "bell_bench_bell_small_left", "钟亭小钟左" },
		{ "bell_bench_bell_small_right", "钟亭小钟右" },
		{ "bell_hit", "钟声撞击" },
		{ "bell_impact_new_bell_1", "钟声冲击-1" },
		{ "bell_impact_new_bell_2", "钟声冲击-2" },
		{ "bell_impact_new_bell_3", "钟声冲击-3" },
		{ "bell_impact_new_bell_4", "钟声冲击-4" },
		{ "bell_shrine_arrive", "钟祠到达" },
		{ "bell_shrine_lower_finish", "钟祠下降结束" },
		{ "bell_shrine_lower_loop", "钟祠下降循环" },
		{ "bell_shrine_rise_up", "钟祠升起" },
		{ "bellbeast_purr_1", "钟兽呼噜-1" },
		{ "bellbeast_purr_2", "钟兽呼噜-2" },
		{ "bellbeast_purr_3", "钟兽呼噜-3" },
		{ "bellbeast_snore_loop", "钟兽打鼾循环" },
		{ "bench_rest", "长凳休息" },
		{ "bench_rest_burst", "长凳休息爆发" },
		{ "big_bone_hit", "大骨撞击" },
		{ "blasted_storm_effect", "风暴效果" },
		{ "block_hit", "格挡撞击" },
		{ "block_hit_dull", "格挡闷击" },
		{ "block_hit_window", "格挡窗口" },
		{ "bone_big_gate_open_sequence", "骨大门开启" },
		{ "bone_bridge_dust_move_3", "骨桥移动-3" },
		{ "bone_lever_activate", "骨杠杆激活" },
		{ "city_travel_tube_glass_shut", "城隧玻璃关闭" },
		{ "city_travel_tube_hornet_land_in", "城隧入口落" },
		{ "dock_plat_move", "码头平台移动" },
		{ "dummy_hanging_hit_1", "假人悬吊撞1" },
		{ "dummy_hanging_hit_2", "假人悬吊撞2" },
		{ "dummy_hanging_hit_3", "假人悬吊撞3" },
		{ "dummy_hanging_hit_4", "假人悬吊撞4" },
		{ "edge_grab_single", "单抓边缘" },
		{ "enemy_bone_crack_1", "敌人骨裂-1" },
		{ "enemy_bone_crack_2", "敌人骨裂-2" },
		{ "enemy_damage", "敌人受击" },
		{ "enemy_damage_cloakless_hit", "敌人裸体受击" },
		{ "enemy_damage_old", "敌人受击旧" },
		{ "enemy_kill_shake_new", "敌人击杀震颤" },
		{ "explode_wall_charge_up", "爆破墙蓄力" },
		{ "generic_hanging_plat_metal_land_2", "悬挂金属台落地2" },
		{ "greymoor_flip_bridge_activate_pt2", "灰月翻桥激活2" },
		{ "greymoor_flip_bridge_return", "灰月翻桥返回" },
		{ "greymoor_propellor_active_loop", "灰月螺旋桨循环" },
		{ "greymoor_propellor_start_burst", "灰月螺旋桨启动爆发" },
		{ "greymoor_propellor_startup_level_1", "灰月螺旋桨启动1" },
		{ "greymoor_propellor_startup_level_2", "灰月螺旋桨启动2" },
		{ "greymoor_propellor_startup_level_3", "灰月螺旋桨启动3" },
		{ "hero_air_dash", "空中冲刺" },
		{ "hero_damage_less_harsh", "骑士轻受击" },
		{ "hero_dash", "普通冲刺" },
		{ "hero_double_damage", "强化冲刺劈砍" },
		{ "hero_dream_nail_slash", "梦境钉" },
		{ "hero_evade", "闪避" },
		{ "hero_falling", "坠落" },
		{ "hero_land_hard", "硬落地" },
		{ "hero_land_shallow_splash", "浅水落地" },
		{ "hero_land_soft", "软落地" },
		{ "hero_nail_art_charge_complete", "战技充能完成" },
		{ "hero_nail_art_charge_initiate", "战技充能启动" },
		{ "hero_nail_art_charge_loop", "战技充能循环" },
		{ "hero_nail_art_charged_burst", "战技释放爆发" },
		{ "hero_nail_art_cyclone_slash_long", "龙卷斩-长" },
		{ "hero_nail_art_cyclone_slash_short", "龙卷斩-短" },
		{ "hero_nail_art_great_slash", "大斩" },
		{ "hero_quake_spell_dash", "地震术飞行" },
		{ "hero_quake_spell_impact", "地震术落地" },
		{ "hero_quake_spell_prepare", "地震术蓄力(施法)" },
		{ "hero_run_footsteps_bones", "骑士跑步-骨地" },
		{ "hero_run_footsteps_grass", "骑士跑步-草地" },
		{ "hero_run_footsteps_metal", "骑士跑步-金属" },
		{ "hero_run_footsteps_stone", "骑士跑步-石质" },
		{ "hero_run_footsteps_water", "骑士跑步-水面" },
		{ "hero_shade_dash_2", "暗影冲刺" },
		{ "hero_super_dash_air_brake", "超级冲刺-空中刹车" },
		{ "hero_super_dash_burst", "超级冲刺-迸发" },
		{ "hero_super_dash_charge", "超级冲刺-蓄力" },
		{ "hero_super_dash_impact_wall", "超级冲刺-撞墙" },
		{ "hero_super_dash_loop", "超级冲刺-循环" },
		{ "hero_super_dash_ready", "超级冲刺-就绪" },
		{ "hero_walk_footsteps_stone", "骑士走路-石质" },
		{ "hero_wall_jump", "墙跳" },
		{ "hero_wall_slide", "墙滑" },
		{ "hero_wall_slide-01", "墙滑旧版" },
		{ "hero_wings", "翅膀振颤" },
		{ "hollow_shade_startle", "空壳阴影惊跳" },
		{ "lavafall_hit_lava_04", "熔岩瀑布撞击" },
		{ "library_lift_large_activate", "图书馆大升降机激活" },
		{ "library_lift_large_activate_and_move_start", "图书馆大升降机启动" },
		{ "library_lift_large_arrive", "图书馆大升降机到达" },
		{ "library_lift_large_depress_under_weight", "图书馆大升降机负重" },
		{ "library_lift_large_move_loop", "图书馆大升降机循环" },
		{ "library_sliding_plat_arrive", "图书馆滑台到达" },
		{ "library_sliding_plat_hit", "图书馆滑台撞击" },
		{ "library_sliding_plat_slide_loopv2", "图书馆滑台循环2" },
		{ "low_hit_nail_impact", "轻钉撞击" },
		{ "low_hit_nail_impact_stronger", "强钉撞击" },
		{ "low_impact_faster", "轻撞击快" },
		{ "metronome_in", "节拍器入" },
		{ "mid_hit_enemy_death", "中击敌死" },
		{ "moss_walker_footstep_loop", "苔行脚步循环" },
		{ "nail_clash_effect", "钉碰撞" },
		{ "peak_creature_crooning_loop", "山峰生物低吟" },
		{ "peak_creature_singing_option_1", "山峰生物歌唱1" },
		{ "platform_dirt_fall_2", "土台坠落2" },
		{ "rain_drop_single", "雨滴" },
		{ "rain_drop_single_v2", "雨滴2" },
		{ "ruins_rain_outdoor_loop", "废墟室外雨循环" },
		{ "shake_loop", "震颤循环" },
		{ "shellwood_gate_activate", "贝壳门激活" },
		{ "shellwood_gate_button_cancel_and_release", "贝壳门按钮取消" },
		{ "shellwood_gate_button_press", "贝壳门按钮按下" },
		{ "shellwood_lift_activate_and_shut", "贝壳升降机激活关闭" },
		{ "shellwood_lift_arrive", "贝壳升降机到达" },
		{ "shellwood_lift_button_1_touch", "贝壳升降机按钮1" },
		{ "shellwood_lift_button_2_activate", "贝壳升降机按钮2" },
		{ "shellwood_lift_button_2_activate_without_the_detail", "贝壳升降机按钮2简" },
		{ "shellwood_lift_button_rise_if_needed", "贝壳升降机升起" },
		{ "shellwood_lift_doors_open", "贝壳升降机门开" },
		{ "shellwood_lift_moving_loop", "贝壳升降机移动循环" },
		{ "shellwood_twig_wall_break", "贝壳树枝墙碎" },
		{ "shellwood_twig_wall_hit_1", "贝壳树枝墙撞1" },
		{ "shellwood_twig_wall_hit_2", "贝壳树枝墙撞2" },
		{ "shellwood_twig_wall_hit_3", "贝壳树枝墙撞3" },
		{ "silk_lift_door_close", "丝缎门关闭" },
		{ "silk_lift_door_open", "丝缎门开" },
		{ "slab_03_hornet_cage_lean_1", "骨片3牢笼倚1" },
		{ "slab_03_hornet_cage_lean_2", "骨片3牢笼倚2" },
		{ "slab_03_hornet_cage_lean_3", "骨片3牢笼倚3" },
		{ "slab_03_hornet_cage_lean_4", "骨片3牢笼倚4" },
		{ "slab_03_hornet_cage_loop", "骨片3牢笼循环" },
		{ "song_gate_hit", "歌声门撞击" },
		{ "swim_loop_new", "游泳循环新" },
		{ "sword_1", "剑击-1" },
		{ "sword_2", "剑击-2" },
		{ "sword_3", "剑击-3" },
		{ "sword_4", "剑击-4" },
		{ "sword_5", "剑击-5" },
		{ "sword_hit_reject", "剑击拒挡" },
		{ "sword_hit_window_1", "剑击窗口1" },
		{ "toll_booth_insert_coins", "收费站投币" },
		{ "tool_metal_deposit_break", "工具金属破碎" },
		{ "tool_metal_deposit_impact_1", "工具金属撞击1" },
		{ "tool_metal_deposit_impact_2", "工具金属撞击2" },
		{ "tool_throw", "投掷工具" },
		{ "toolmaster_down_slash_charged", "铁匠下劈蓄力" },
		{ "trap_spike_plate_button", "陷阱尖板按钮" },
		{ "understore_toll_bench_activate", "地下收费亭激活" },
		{ "updraft_hornet_takeoff", "上升气流起飞" },
		{ "vent_pass_through_door_frame_arrive", "通风管门框到达" },
		{ "vent_pass_through_door_frame_move_loop", "通风管门框循环" },
		{ "vent_pass_through_door_opened", "通风管门开" },
		{ "vent_pass_through_door_rise_loop", "通风管上升循环" },
		{ "vent_pass_through_wall_front_move_in", "通风管前壁移入" },
		{ "weathervane_hit", "风向标撞击" },
		{ "wraith_damage_2", "幽灵受击2" },
		{ "Average Impact", "Average·Impact" },
		{ "Average Rumble", "Average·Rumble" },
		{ "bell_beast_underground_emerge", "钟·兽·地下·现身" },
		{ "Bellbeast_Travel_Children_V3", "Bellbeast·Travel·Children·V3" },
		{ "Bellbeast_Travel_Normal_V3", "Bellbeast·Travel·Normal·V3" },
		{ "charm_place", "charm·place" },
		{ "charm_place_fail", "charm·place·fail" },
		{ "charm_place_overcharm", "charm·place·overcharm" },
		{ "craft", "制作" },
		{ "focus_health_heal", "聚焦·health·回复" },
		{ "footstep", "脚步" },
		{ "hero_damage", "骑士·受击" },
		{ "Lace_Battle", "Lace·Battle" },
		{ "Large Impact", "Large·Impact" },
		{ "Large Rumble", "Large·Rumble" },
		{ "Medium Impact", "Medium·Impact" },
		{ "Medium Rumble", "Medium·Rumble" },
		{ "overcharm_unlock_final_hit", "overcharm·unlock·final·击中" },
		{ "overcharm_unlock_hit", "overcharm·unlock·击中" },
		{ "single_impact", "单·撞击" },
		{ "Small Impact", "Small·Impact" },
		{ "Small Rumble", "Small·Rumble" },
		{ "Super Tiny Impact", "Super·Tiny·Impact" },
		{ "Super Tiny Rumble", "Super·Tiny·Rumble" },
		{ "Tiny Impact", "Tiny·Impact" },
		{ "Tiny Rumble", "Tiny·Rumble" },
		{ "UI Submit", "UI·Submit" },
	};

	internal static ConfigEntry<int> BridgePort;

	internal static ConfigEntry<string> ClipsPath;

	internal static ConfigEntry<bool> AutoStartBridge;

	internal static ConfigEntry<int> BridgeBufferMs;

	internal static ConfigEntry<int> BridgeLatencyMs;

	internal static ConfigEntry<bool> BridgeEventSync;

	private Process bridgeProcess;

	private void Awake()
	{
		Log = Logger;
		KeepRumble = Config.Bind<bool>("General", "KeepRumble", false, "Also send the normal dual-motor rumble while a haptic clip plays (may feel doubled through DSX).");
		MasterGain = Config.Bind<float>("General", "MasterGain", 1f, "Overall gain applied to the haptic waveform (0.0 - 2.0).");

		// 分类倍率由 RegisterClipGains() 在确定 clips 目录后自动注册。

		BridgePort = Config.Bind<int>("General", "BridgePort", 48111, "Localhost TCP port the HapticsBridge app listens on.");
		ClipsPath = Config.Bind<string>("General", "ClipsPath", "", "Folder containing the extracted haptic WAVs. Empty = <plugin folder>\\clips.");
		AutoStartBridge = Config.Bind<bool>("General", "AutoStartBridge", true, "Launch HapticsBridge.exe (system tray) automatically when the game starts.");
		BridgeBufferMs = Config.Bind<int>("Latency", "BridgeBufferMs", 60, "Jitter buffer in ms (lower = less latency, but more prone to crackle). Try 5 for minimum latency on a stable wired connection.");
		BridgeLatencyMs = Config.Bind<int>("Latency", "BridgeLatencyMs", 100, "WASAPI output latency in ms (lower = snappier). Very low values (e.g. 1) work great wired but can wedge on Bluetooth.");
		BridgeEventSync = Config.Bind<bool>("Latency", "BridgeEventSync", false, "Use low-latency event-driven WASAPI. Wired endpoints only; can stall on Bluetooth.");
		string text = ClipsPath.Value;
		if (string.IsNullOrEmpty(text))
		{
			text = Path.Combine(Path.GetDirectoryName(Info.Location), "clips");
		}
		RegisterClipGains(Config, text);
		if (AutoStartBridge.Value)
		{
			TryStartBridge();
		}
		HapticEngine.Start(text, BridgePort.Value);
		var harmony = new Harmony("com.will.hollowknight.haptics");
		harmony.PatchAll(typeof(MixerPatch));
		harmony.PatchAll(typeof(FootstepVibrationPatch));
		gameObject.AddComponent<HapticsMenu>();
		Log.LogInfo((object)("Hollow Knight haptics plugin loaded. Clips: " + text + " | footstep vibration: ON"));
	}

	private void Update()
	{
		try
		{
			HapticEngine.CachedTimeScale = Time.timeScale;
			// 当前游戏版本 VibrationManager 无 StrengthMultiplier 静态属性；
			// CachedStrengthMultiplier 保持默认值 1.0f。
			HapticEngine.CachedMasterGain = MasterGain.Value;
			// 脚步触觉：停步后自动停止连续循环纹理
			FootstepVibrationPatch.TickIdleStop();
		}
		catch
		{
		}
	}

	private void OnApplicationQuit()
	{
		StopBridge();
	}

	private void OnDestroy()
	{
		HapticEngine.Shutdown();
		StopBridge();
	}

	private void StopBridge()
	{
		try
		{
			if (bridgeProcess != null && !bridgeProcess.HasExited)
			{
				try { bridgeProcess.Kill(); } catch { }
			}
		}
		catch { }

		// 兜底：确保任何残留的 HapticsBridge.exe 都随游戏关闭而结束，
		// 防止单例互斥导致旧实例残留、右下角托盘图标不关闭。
		try
		{
			foreach (Process p in Process.GetProcessesByName("HapticsBridge"))
			{
				try
				{
					if (!p.HasExited)
					{
						p.Kill();
					}
				}
				catch { }
			}
		}
		catch { }

		bridgeProcess = null;
	}

	private void TryStartBridge()
	{
		try
		{
			string text = Path.Combine(Path.GetDirectoryName(Info.Location), "HapticsBridge", "HapticsBridge.exe");
			if (!File.Exists(text))
			{
				text = Path.Combine(Paths.GameRootPath, "HapticsBridge", "HapticsBridge.exe");
			}
			if (File.Exists(text))
			{
				// 修复：追加 --no-hid，禁止 Bridge 写入 DualSense HID 模式选择报告（0x02）。
				// 该报告会将手柄切入 PS4 兼容 HID 模式，导致手柄按键（A键聚集/施法）失效，
				// 且音频触觉在 USB 场景下实际并未生效（日志为假阳性）。禁用后不损失功能。
				// 追加 --exit-on-disconnect：游戏关闭、最后一个客户端断开时，Bridge 自动退出，
				// 右下角托盘灯随之消失（不再残留）。
				string text2 = string.Format(CultureInfo.InvariantCulture, "--no-extract --no-hid --exit-on-disconnect --port {0} --buffer-ms {1} --latency-ms {2}", BridgePort.Value, BridgeBufferMs.Value, BridgeLatencyMs.Value);
				if (BridgeEventSync.Value)
				{
					text2 += " --event-sync";
				}
				bridgeProcess = Process.Start(new ProcessStartInfo(text, text2)
				{
					UseShellExecute = true,
					WorkingDirectory = Path.GetDirectoryName(text)
				});
				Log.LogInfo((object)("Launched bridge: " + text + " " + text2));
			}
			else
			{
				Log.LogWarning((object)"HapticsBridge.exe not found; start it manually or set AutoStartBridge=false");
			}
		}
		catch (Exception ex)
		{
			Log.LogWarning((object)("Could not auto-start bridge: " + ex.Message));
		}
	}

	private static void RegisterClipGains(ConfigFile config, string clipsDir)
	{
		ClipGains.Clear();
		HashSet<string> clipFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (string alias in HapticMixerWrapper.GetAliasTargets())
		{
			if (string.IsNullOrEmpty(alias))
				continue;
			// 脉冲版（*_pulse）剪辑仅供归档，播放与增益一律使用非脉冲版，不在菜单注册。
			if (alias.IndexOf("pulse", StringComparison.OrdinalIgnoreCase) >= 0)
				continue;
			clipFiles.Add(alias);
		}
		foreach (string footstep in FootstepVibrationPatch.GetKnownClipNames())
		{
			if (!string.IsNullOrEmpty(footstep))
				clipFiles.Add(footstep);
		}
		if (!string.IsNullOrEmpty(clipsDir) && Directory.Exists(clipsDir))
		{
			foreach (string file in Directory.GetFiles(clipsDir, "*.wav"))
			{
				string baseName = Path.GetFileNameWithoutExtension(file);
				if (!string.IsNullOrEmpty(baseName) && baseName.IndexOf("pulse", StringComparison.OrdinalIgnoreCase) < 0)
					clipFiles.Add(baseName);
			}
		}
		foreach (string clipFile in clipFiles)
		{
			string key = GetClipKey(clipFile);
			if (string.IsNullOrEmpty(key) || ClipGains.ContainsKey(key))
				continue;
			string display = GetClipDisplayName(key);
			ClipGains[key] = config.Bind<float>("Haptics", "Gain_" + key, 1f, display + " 精细触觉倍率，1.0=基准。范围 0~3。");
		}
	}

	internal static float GetClipGain(string clipFile)
	{
		string key = GetClipKey(clipFile);
		if (ClipGains.TryGetValue(key, out ConfigEntry<float> entry) && entry != null)
			return entry.Value;
		return 1f;
	}

	internal static string GetClipKey(string clipFile)
	{
		string key = clipFile;
		if (string.IsNullOrEmpty(key))
			return string.Empty;
		if (key.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
			key = key.Substring(0, key.Length - 4);
		if (key.EndsWith("_vibration", StringComparison.OrdinalIgnoreCase))
			key = key.Substring(0, key.Length - 10);
		return key;
	}

	internal static IEnumerable<KeyValuePair<string, ConfigEntry<float>>> GetSortedClipGains()
	{
		return ClipGains.OrderBy(kv => ClipSortGroup(kv.Key)).ThenBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase);
	}

	private static int ClipSortGroup(string key)
	{
		string lower = key.ToLowerInvariant();
		if (lower.Contains("footsteps")) return 0;
		if (lower.Contains("wall") || lower.Contains("dash") || lower.Contains("jump")) return 1;
		if (lower.Contains("spell") || lower.Contains("dream") || lower.Contains("nail") || lower.Contains("fireball") || lower.Contains("scream")) return 2;
		if (lower.Contains("hurt") || lower.Contains("damage") || lower.Contains("hit") || lower.Contains("impact") || lower.Contains("slash") || lower.Contains("attack")) return 3;
		return 4;
	}

	internal static string GetClipDisplayName(string clipFile)
	{
		string key = GetClipKey(clipFile);
		if (string.IsNullOrEmpty(key))
			return "未知震动";
		if (clipDisplayNames.TryGetValue(key, out string label))
			return label;
		// 未命中映射时返回纯中文占位，避免出现英文基名
		return "未命名震动";
	}

}