using System.Collections.Generic;
using UnityEngine;

namespace HollowKnightHaptics;

public sealed class HapticMixerWrapper : VibrationMixer
{
	private static readonly Dictionary<VibrationMixer, HapticMixerWrapper> cache = new Dictionary<VibrationMixer, HapticMixerWrapper>();

	private readonly VibrationMixer inner;

	private readonly List<PS5HapticEmission> emissions = new List<PS5HapticEmission>();

	private readonly object gate = new object();

	private static int playsLogged;

	private static bool fallbackWarned;

	public override bool IsPaused
	{
		get
		{
			return inner.IsPaused;
		}
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

	public override VibrationEmission PlayEmission(VibrationData vibrationData, VibrationTarget vibrationTarget, bool isLooping, string tag, bool isRealtime)
	{
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		AudioClip pS5Vibration = ((VibrationData)(ref vibrationData)).GetPS5Vibration();
		if ((Object)(object)pS5Vibration != (Object)null && !HapticEngine.Connected && !fallbackWarned)
		{
			fallbackWarned = true;
			HapticsPlugin.Log.LogWarning((object)("PS5 haptic event '" + ((Object)pS5Vibration).name + "' arrived but bridge is not connected - using rumble fallback (is HapticsBridge.exe running?)"));
		}
		if ((Object)(object)pS5Vibration != (Object)null && HapticEngine.Connected && HapticEngine.TryGetPcm(((Object)pS5Vibration).name, out var pcm))
		{
			if (playsLogged < 8)
			{
				playsLogged++;
				HapticsPlugin.Log.LogInfo((object)$"Haptic play: '{((Object)pS5Vibration).name}' (loop={isLooping}, tag='{tag}')");
			}
			PS5HapticEmission pS5HapticEmission = new PS5HapticEmission(pcm, ((VibrationData)(ref vibrationData)).Strength, isLooping, tag, vibrationTarget, isRealtime);
			lock (gate)
			{
				emissions.Add(pS5HapticEmission);
			}
			HapticEngine.Register(pS5HapticEmission);
			if (HapticsPlugin.KeepRumble.Value)
			{
				inner.PlayEmission(vibrationData, vibrationTarget, isLooping, tag, isRealtime);
			}
			return (VibrationEmission)(object)pS5HapticEmission;
		}
		return inner.PlayEmission(vibrationData, vibrationTarget, isLooping, tag, isRealtime);
	}

	public override VibrationEmission PlayEmission(VibrationEmission emission)
	{
		if (emission is PS5HapticEmission pS5HapticEmission)
		{
			((VibrationEmission)pS5HapticEmission).Play();
			lock (gate)
			{
				if (!emissions.Contains(pS5HapticEmission))
				{
					emissions.Add(pS5HapticEmission);
				}
			}
			HapticEngine.Register(pS5HapticEmission);
			return (VibrationEmission)(object)pS5HapticEmission;
		}
		return inner.PlayEmission(emission);
	}

	public override void StopAllEmissions()
	{
		lock (gate)
		{
			foreach (PS5HapticEmission emission in emissions)
			{
				((VibrationEmission)emission).Stop();
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
				if (((VibrationEmission)emission).Tag == tag)
				{
					((VibrationEmission)emission).Stop();
				}
			}
			Prune();
		}
		inner.StopAllEmissionsWithTag(tag);
	}

	private void Prune()
	{
		emissions.RemoveAll((PS5HapticEmission e) => !((VibrationEmission)e).IsPlaying);
	}
}
