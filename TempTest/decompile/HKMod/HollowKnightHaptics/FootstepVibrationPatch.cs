using System;
using System.Collections.Generic;
using System.Reflection;
using GlobalEnums;
using HarmonyLib;
using UnityEngine;

namespace HollowKnightHaptics;

/// <summary>
/// 脚步震动补丁：Hook HeroAudioController 的脚步声触发事件，
/// 在玩家走路/跑步时注入对应的 PS5 触觉剪辑震动。
///
/// 原版 VibrationManager 不发出走路震动事件（仅有 WallSlide、SmallImpact 等），
/// 因此需要通过此补丁额外注入脚步骤动（用户确认：保留人工触发）。
///
/// 震动剪辑名映射（环境类型 → WAV 文件，均为完整纹理，非脉冲裁剪版）：
///   envType 0 (Dust)      → hero_run_footsteps_stone_vibration
///   envType 1 (Grass)     → hero_run_footsteps_grass_vibration
///   envType 2 (Bone)      → hero_run_footsteps_bones_vibration
///   envType 3 (Spa/Moss)  → hero_run_footsteps_stone_vibration
///   envType 4 (Metal)     → hero_run_footsteps_metal_vibration
///   envType 6 (Water)     → hero_run_footsteps_water_vibration
///   envType 7 (Grass alt) → hero_run_footsteps_grass_vibration
///   walk (any)            → hero_walk_footsteps_stone_vibration
///
/// 触发策略：动作开始时启动对应路面纹理的循环播放，动作结束时停止播放。避免纹理叠加，确保路面质感跟随移动状态。
/// </summary>
[HarmonyPatch]
public static class FootstepVibrationPatch
{
    /// <summary>
    /// 缓存已创建的 GamepadVibration 实例（key: 归一化 clip 名）。
    /// </summary>
    private static readonly Dictionary<string, GamepadVibration> _vibrationCache = new Dictionary<string, GamepadVibration>();
    /// <summary> 当前正在播放的剪辑名（路面变更时切换，停步后由 Update 空闲检测停止）。</summary>
    private static string _currentClipName;
    /// <summary> 最后一次触发脚步的时间（秒），用于停步后自动停止振动。</summary>
    private static float _lastFootstepTime = float.NegativeInfinity;
    /// <summary> 玩家停步后超过该秒数即自动停止脚步触觉。</summary>
    private const float FootstepIdleTimeout = 0.6f;

    /// <summary>
    /// GamepadVibration 的序列化字段（反射路径）。
    /// </summary>
    private static readonly FieldInfo _smallMotorField = typeof(GamepadVibration).GetField("smallMotor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    private static readonly FieldInfo _largeMotorField = typeof(GamepadVibration).GetField("largeMotor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    private static readonly FieldInfo _playbackRateField = typeof(GamepadVibration).GetField("playbackRate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

    /// <summary>
    /// 环境类型 → 跑步震动剪辑名（索引 = environmentType），
    /// 使用完整纹理 _vibration.wav（非 pulse），对应用户提供的 WAV 文件。
    /// </summary>
    private static readonly string[] EnvTypeRunClipNames = new string[]
    {
        "hero_run_footsteps_stone_vibration",      // envType 0: Dust（石质）
        "hero_run_footsteps_grass_vibration",      // envType 1: Grass（草地）
        "hero_run_footsteps_bones_vibration",      // envType 2: Bone（骨地）
        "hero_run_footsteps_stone_vibration",      // envType 3: Spa/Moss（无对应 hero 资源，用石质兜底）
        "hero_run_footsteps_metal_vibration",      // envType 4: Metal（金属）
        null,                                       // envType 5: (未使用)
        "hero_run_footsteps_water_vibration",      // envType 6: Water（水面）
        "hero_run_footsteps_grass_vibration",      // envType 7: Grass alt（草地 alt）
    };

    /// <summary>
    /// 步行震动剪辑名（完整纹理版，非脉冲裁剪版）。
    /// </summary>
    private const string WalkClipName = "hero_walk_footsteps_stone_vibration";
    internal static IEnumerable<string> GetKnownClipNames()
    {
        foreach (var name in EnvTypeRunClipNames)
        {
            if (!string.IsNullOrEmpty(name))
                yield return name;
        }
        yield return WalkClipName;
    }


    /// <summary>
    /// 获取脚步震动剪辑名。
    /// </summary>
    private static string GetFootstepClipName(bool isWalking, int environmentType)
    {
        // 按环境类型返回对应路面的完整纹理剪辑名（走路/跑步用同一套，靠 envType 区分路面）。
        // 用户提供 7 个路面 WAV：stone(0) / grass(1,7) / hard(2) / moss(3) / metal(4) / wet_metal(6)。
        // 游戏内骑士普通移动实际触发 FOOTSTEPS_RUN（isWalking=false），故按 environmentType 判定，
        // 不能只靠 isWalking；走路/跑步均按当前路面取对应文件。
        if (environmentType >= 0 && environmentType < EnvTypeRunClipNames.Length && EnvTypeRunClipNames[environmentType] != null)
            return EnvTypeRunClipNames[environmentType];

        // 兜底（未使用/越界 envType，如 5）→ 石头走路剪辑。
        return WalkClipName;
    }

    /// <summary>
    /// 创建或获取缓存的 GamepadVibration 实例。
    /// 使用反射设置序列化私有字段（smallMotor / largeMotor / playbackRate）。
    /// </summary>
    private static GamepadVibration GetOrCreateVibration(string clipName)
    {
        if (_vibrationCache.TryGetValue(clipName, out var vib))
            return vib;

        vib = ScriptableObject.CreateInstance<GamepadVibration>();
        vib.name = clipName;

        // 用反射设置私有序列化字段
        if (_smallMotorField != null)
            _smallMotorField.SetValue(vib, AnimationCurve.Constant(0f, 1f, 0f));
        if (_largeMotorField != null)
            _largeMotorField.SetValue(vib, AnimationCurve.Constant(0f, 1f, 0f));
        if (_playbackRateField != null)
            _playbackRateField.SetValue(vib, 1f);

        _vibrationCache[clipName] = vib;
        return vib;
    }

    /// <summary>
    /// 获取当前场景中的 HeroController 实例。
    /// </summary>
    private static HeroController GetHeroController()
    {
        try
        {
            var ctrl = HeroController.instance;
            return ctrl != null ? ctrl : HeroController.SilentInstance;
        }
        catch
        {
            return null;
        }
    }

    #region Harmony Patches

    [HarmonyPatch(typeof(HeroAudioController), nameof(HeroAudioController.PlaySound))]
    [HarmonyPostfix]
    private static void OnPlaySoundPostfix(HeroSounds soundEffect)
    {
        try
        {
            // 只处理脚步声音
            bool isFootstepRun = soundEffect == HeroSounds.FOOTSTEPS_RUN;
            bool isFootstepWalk = soundEffect == HeroSounds.FOOTSTEPS_WALK;
            if (!isFootstepRun && !isFootstepWalk)
                return;

            var heroCtrl = GetHeroController();
            if (heroCtrl == null || heroCtrl.playerData == null)
                return;

            bool isWalking = isFootstepWalk;
            int envType = heroCtrl.playerData.GetInt("environmentType");
            string clipName = GetFootstepClipName(isWalking, envType);
            if (string.IsNullOrEmpty(clipName))
                return;

            _lastFootstepTime = Time.time;

            // 同一路面：保持连续循环（2 秒纹理完整播放，保留 WAV 内部段落感），
            // 仅刷新最后一步时间即可，不重新触发以免从头截断。
            if (_currentClipName == clipName)
                return;

            // 路面变更：停止上一个路面循环，再以循环方式启动当前路面纹理。
            VibrationManager.StopAllVibrationsWithTag("hk_haptics_footstep");
            var vibration = GetOrCreateVibration(clipName);
            var data = VibrationData.Create(LowFidelityVibrations.None, null, vibration);
            VibrationManager.PlayVibrationClipOneShot(data, new VibrationTarget(VibrationMotors.All), true, "hk_haptics_footstep");
            _currentClipName = clipName;
        }
        catch (Exception ex)
        {
            // 静默失败，不影响游戏正常运行
            HapticsPlugin.Log.LogDebug($"[HKHaptics-FootstepVibration] OnPlaySound error: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(HeroAudioController), nameof(HeroAudioController.StopSound))]
    [HarmonyPostfix]
    private static void OnStopSoundPostfix(HeroSounds soundEffect)
    {
        // 脚步单次音结束不立即停止循环：保持连续纹理，停步由 Update 空闲检测统一处理。
        // 仅在离开英雄（切场景/暂停）时彻底清场。
        try
        {
            bool isFootstep = soundEffect == HeroSounds.FOOTSTEPS_RUN || soundEffect == HeroSounds.FOOTSTEPS_WALK;
            if (isFootstep)
                _lastFootstepTime = Time.time; // 刷新时间，避免误判为停步
        }
        catch (Exception ex)
        {
            HapticsPlugin.Log.LogDebug($"[HKHaptics-FootstepVibration] OnStopSound error: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(HeroAudioController), nameof(HeroAudioController.StopAllSounds))]
    [HarmonyPostfix]
    private static void OnStopAllSoundsPostfix()
    {
        try
        {
            VibrationManager.StopAllVibrationsWithTag("hk_haptics_footstep");
            _currentClipName = null;
        }
        catch (Exception ex)
        {
            HapticsPlugin.Log.LogDebug($"[HKHaptics-FootstepVibration] OnStopAllSounds error: {ex.Message}");
        }
    }

    /// <summary>
    /// 每帧由 HapticsPlugin.Update 调用：玩家停步或离地时立即停止脚步循环纹理。
    /// 判断依据（任一满足即停）：
    ///   1. 离地（cState.onGround == false，跳跃/摔落/漂浮中）→ 脚步声不应继续。
    ///   2. 左摇杆水平输入 ≈ 0（Mathf.Abs(hero.move_input) < 0.1f，站立静止或松杆）→ 立即停止。
    /// 保持播放的条件：onGround == true 且左摇杆仍有水平输入，无需 0.6s 等待。
    ///
    /// 注意：不使用 current_velocity.x（位置速度）判断，因为贴墙行走时 vx 接近 0
    /// 但游戏仍在播放脚步声，会导致振动被错误停止。改用 move_input（左摇杆水平轴、
    /// 已过游戏死区过滤）判断，确保只有松杆/静止时才停振，贴墙走路仍保持振动。
    /// 每一步 OnPlaySoundPostfix / OnStopSoundPostfix 都会刷新 _lastFootstepTime，
    /// 此处用作兜底安全网：若因游戏版本差异导致状态字段异常，仍在超时后停止。
    /// </summary>
    internal static void TickIdleStop()
    {
        try
        {
            if (_currentClipName == null)
                return;

            var hero = GetHeroController();
            if (hero != null && hero.playerData != null)
            {
                // 优先依据英雄状态直接判断
                bool onGround = hero.cState != null && hero.cState.onGround;
                float horizontalInput = hero.move_input;

                if (!onGround)
                {
                    // 离地：脚步声已停止，立即停止触觉循环
                    VibrationManager.StopAllVibrationsWithTag("hk_haptics_footstep");
                    _currentClipName = null;
                    return;
                }

                if (Mathf.Abs(horizontalInput) < 0.1f)
                {
                    // 左摇杆水平输入 ≈ 0（站立静止/松杆）：立即停止触觉循环
                    VibrationManager.StopAllVibrationsWithTag("hk_haptics_footstep");
                    _currentClipName = null;
                    return;
                }
            }

            // 兜底：若状态判断路径失败，仍用时间戳超时作为安全网
            if (Time.time - _lastFootstepTime > FootstepIdleTimeout)
            {
                VibrationManager.StopAllVibrationsWithTag("hk_haptics_footstep");
                _currentClipName = null;
            }
        }
        catch { }
    }

    #endregion
}
