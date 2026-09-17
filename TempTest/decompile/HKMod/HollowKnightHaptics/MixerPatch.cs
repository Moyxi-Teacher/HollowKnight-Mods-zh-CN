using System;
using HarmonyLib;

namespace HollowKnightHaptics;

[HarmonyPatch(typeof(VibrationManager), "GetMixer")]
internal static class MixerPatch
{
	private static void Postfix(ref VibrationMixer __result)
	{
		// 故障安全：本补丁绝不可因抛异常而破坏游戏原版逻辑。
		// 任何异常都原样保留 __result（原始 mixer），让游戏照常工作，
		// 仅牺牲触觉增强，不影响任何原版交互（墙跳/吉欧/存档等）。
		try
		{
			if (__result != null && !(__result is HapticMixerWrapper))
			{
				__result = (VibrationMixer)(object)HapticMixerWrapper.GetOrCreate(__result);
			}
		}
		catch (Exception ex)
		{
			HapticsPlugin.Log.LogDebug((object)("[HKHaptics-MixerPatch] GetMixer wrap skipped: " + ex.Message));
		}
	}
}
