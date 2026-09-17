using HarmonyLib;

namespace HollowKnightHaptics;

[HarmonyPatch(typeof(VibrationManager), "GetMixer")]
internal static class MixerPatch
{
	private static void Postfix(ref VibrationMixer __result)
	{
		if (__result != null && !(__result is HapticMixerWrapper))
		{
			__result = (VibrationMixer)(object)HapticMixerWrapper.GetOrCreate(__result);
		}
	}
}
