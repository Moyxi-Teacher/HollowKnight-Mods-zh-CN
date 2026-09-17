using System;
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

	internal static ConfigEntry<int> BridgePort;

	internal static ConfigEntry<string> ClipsPath;

	internal static ConfigEntry<bool> AutoStartBridge;

	internal static ConfigEntry<int> BridgeBufferMs;

	internal static ConfigEntry<int> BridgeLatencyMs;

	internal static ConfigEntry<bool> BridgeEventSync;

	private Process bridgeProcess;

	private void Awake()
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		Log = ((BaseUnityPlugin)this).Logger;
		KeepRumble = ((BaseUnityPlugin)this).Config.Bind<bool>("General", "KeepRumble", false, "Also send the normal dual-motor rumble while a haptic clip plays (may feel doubled through DSX).");
		MasterGain = ((BaseUnityPlugin)this).Config.Bind<float>("General", "MasterGain", 1f, "Overall gain applied to the haptic waveform (0.0 - 2.0).");
		BridgePort = ((BaseUnityPlugin)this).Config.Bind<int>("General", "BridgePort", 48111, "Localhost TCP port the HapticsBridge app listens on.");
		ClipsPath = ((BaseUnityPlugin)this).Config.Bind<string>("General", "ClipsPath", "", "Folder containing the extracted haptic WAVs. Empty = <plugin folder>\\clips.");
		AutoStartBridge = ((BaseUnityPlugin)this).Config.Bind<bool>("General", "AutoStartBridge", true, "Launch HapticsBridge.exe (system tray) automatically when the game starts.");
		BridgeBufferMs = ((BaseUnityPlugin)this).Config.Bind<int>("Latency", "BridgeBufferMs", 60, "Jitter buffer in ms (lower = less latency, but more prone to crackle). Try 5 for minimum latency on a stable wired connection.");
		BridgeLatencyMs = ((BaseUnityPlugin)this).Config.Bind<int>("Latency", "BridgeLatencyMs", 100, "WASAPI output latency in ms (lower = snappier). Very low values (e.g. 1) work great wired but can wedge on Bluetooth.");
		BridgeEventSync = ((BaseUnityPlugin)this).Config.Bind<bool>("Latency", "BridgeEventSync", false, "Use low-latency event-driven WASAPI. Wired endpoints only; can stall on Bluetooth.");
		string text = ClipsPath.Value;
		if (string.IsNullOrEmpty(text))
		{
			text = Path.Combine(Path.GetDirectoryName(((BaseUnityPlugin)this).Info.Location), "clips");
		}
		if (AutoStartBridge.Value)
		{
			TryStartBridge();
		}
		HapticEngine.Start(text, BridgePort.Value);
		new Harmony("com.will.hollowknight.haptics").PatchAll(typeof(MixerPatch));
		Log.LogInfo((object)("Hollow Knight haptics plugin loaded. Clips: " + text));
	}

	private void Update()
	{
		try
		{
			HapticEngine.CachedTimeScale = Time.timeScale;
			HapticEngine.CachedStrengthMultiplier = VibrationManager.StrengthMultiplier;
			HapticEngine.CachedMasterGain = MasterGain.Value;
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
				bridgeProcess.Kill();
			}
		}
		catch
		{
		}
	}

	private void TryStartBridge()
	{
		try
		{
			string text = Path.Combine(Path.GetDirectoryName(((BaseUnityPlugin)this).Info.Location), "HapticsBridge", "HapticsBridge.exe");
			if (!File.Exists(text))
			{
				text = Path.Combine(Paths.GameRootPath, "HapticsBridge", "HapticsBridge.exe");
			}
			if (File.Exists(text))
			{
				string text2 = string.Format(CultureInfo.InvariantCulture, "--no-extract --port {0} --buffer-ms {1} --latency-ms {2}", BridgePort.Value, BridgeBufferMs.Value, BridgeLatencyMs.Value);
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
}
