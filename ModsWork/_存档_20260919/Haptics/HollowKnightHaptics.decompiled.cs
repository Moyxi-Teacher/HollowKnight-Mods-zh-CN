using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using UnityEngine;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("HollowKnightHaptics")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0+7114572c9fe8aadd82fbafb5070cd1fda4be5d58")]
[assembly: AssemblyProduct("HollowKnightHaptics")]
[assembly: AssemblyTitle("HollowKnightHaptics")]
[assembly: AssemblyVersion("1.0.0.0")]
[module: RefSafetyRules(11)]
namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
	internal sealed class RefSafetyRulesAttribute : Attribute
	{
		public readonly int Version;

		public RefSafetyRulesAttribute(int P_0)
		{
			Version = P_0;
		}
	}
}
namespace HollowKnightHaptics
{
	public sealed class HapticPcm
	{
		public readonly float[] Samples;

		public readonly int FrameCount;

		public HapticPcm(float[] samples)
		{
			Samples = samples;
			FrameCount = samples.Length / 2;
		}
	}
	public static class HapticEngine
	{
		public const int SampleRate = 48000;

		private const int BlockFrames = 480;

		private const int BlockBytes = 3840;

		public static volatile float CachedTimeScale = 1f;

		public static volatile float CachedStrengthMultiplier = 1f;

		public static volatile float CachedMasterGain = 1f;

		public static volatile bool CachedMixerPaused;

		public static volatile bool Connected;

		private static readonly List<PS5HapticEmission> active = new List<PS5HapticEmission>();

		private static readonly object mixLock = new object();

		private static readonly Dictionary<string, HapticPcm> pcmCache = new Dictionary<string, HapticPcm>();

		private static readonly HashSet<string> missingWarned = new HashSet<string>();

		private static string clipsDir;

		private static int port;

		private static Thread mixThread;

		private static volatile bool shutdown;

		private static string lastConnectError;

		public static void Start(string clipsDirectory, int bridgePort)
		{
			clipsDir = clipsDirectory;
			port = bridgePort;
			mixThread = new Thread(MixLoop)
			{
				IsBackground = true,
				Name = "PS5HapticMix",
				Priority = ThreadPriority.AboveNormal
			};
			mixThread.Start();
		}

		public static void Shutdown()
		{
			shutdown = true;
		}

		public static void Register(PS5HapticEmission emission)
		{
			lock (mixLock)
			{
				if (!active.Contains(emission))
				{
					active.Add(emission);
				}
			}
		}

		public static bool TryGetPcm(string clipName, out HapticPcm pcm)
		{
			lock (pcmCache)
			{
				if (pcmCache.TryGetValue(clipName, out pcm))
				{
					return pcm != null;
				}
				string text = Path.Combine(clipsDir, clipName + ".wav");
				if (!File.Exists(text))
				{
					if (missingWarned.Add(clipName))
					{
						HapticsPlugin.Log.LogWarning((object)("No haptic wav (yet) for clip '" + clipName + "' - rumble fallback"));
					}
					return false;
				}
				try
				{
					pcm = LoadWav(text);
					pcmCache[clipName] = pcm;
					return true;
				}
				catch (Exception ex)
				{
					HapticsPlugin.Log.LogError((object)("Failed to load " + text + ": " + ex.Message));
					pcmCache[clipName] = null;
					return false;
				}
			}
		}

		private static HapticPcm LoadWav(string path)
		{
			using BinaryReader binaryReader = new BinaryReader(File.OpenRead(path));
			if (new string(binaryReader.ReadChars(4)) != "RIFF")
			{
				throw new InvalidDataException("not RIFF");
			}
			binaryReader.ReadInt32();
			if (new string(binaryReader.ReadChars(4)) != "WAVE")
			{
				throw new InvalidDataException("not WAVE");
			}
			short num = 0;
			short num2 = 0;
			int num3 = 0;
			byte[] array = null;
			while (binaryReader.BaseStream.Position + 8 <= binaryReader.BaseStream.Length)
			{
				string text = new string(binaryReader.ReadChars(4));
				int num4 = binaryReader.ReadInt32();
				if (text == "fmt ")
				{
					short num5 = binaryReader.ReadInt16();
					num = binaryReader.ReadInt16();
					num3 = binaryReader.ReadInt32();
					binaryReader.ReadInt32();
					binaryReader.ReadInt16();
					num2 = binaryReader.ReadInt16();
					binaryReader.BaseStream.Seek(num4 - 16, SeekOrigin.Current);
					if (num5 != 1)
					{
						throw new InvalidDataException($"unsupported wav format {num5}");
					}
				}
				else if (text == "data")
				{
					array = binaryReader.ReadBytes(num4);
				}
				else
				{
					binaryReader.BaseStream.Seek(num4 + (num4 & 1), SeekOrigin.Current);
				}
			}
			if (array == null || num == 0)
			{
				throw new InvalidDataException("missing chunks");
			}
			if (num2 != 16)
			{
				throw new InvalidDataException($"expected PCM16, got {num2}-bit");
			}
			if (num3 != 48000)
			{
				HapticsPlugin.Log.LogWarning((object)$"{Path.GetFileName(path)}: {num3} Hz (expected {48000}) - playing as-is");
			}
			int num6 = array.Length / 2 / num;
			float[] array2 = new float[num6 * 2];
			for (int i = 0; i < num6; i++)
			{
				float num7 = (float)BitConverter.ToInt16(array, i * num * 2) / 32768f;
				float num8 = ((num >= 2) ? ((float)BitConverter.ToInt16(array, (i * num + 1) * 2) / 32768f) : num7);
				array2[i * 2] = num7;
				array2[i * 2 + 1] = num8;
			}
			return new HapticPcm(array2);
		}

		private static void MixLoop()
		{
			float[] array = new float[960];
			byte[] array2 = new byte[3840];
			List<PS5HapticEmission> toRemove = new List<PS5HapticEmission>();
			while (!shutdown)
			{
				TcpClient tcpClient = null;
				try
				{
					tcpClient = new TcpClient();
					tcpClient.Connect("127.0.0.1", port);
					tcpClient.NoDelay = true;
					NetworkStream stream = tcpClient.GetStream();
					Connected = true;
					lastConnectError = null;
					HapticsPlugin.Log.LogInfo((object)$"Connected to HapticsBridge on port {port}");
					Stopwatch stopwatch = Stopwatch.StartNew();
					long num = 0L;
					while (!shutdown)
					{
						MixBlock(array, toRemove);
						Buffer.BlockCopy(array, 0, array2, 0, 3840);
						stream.Write(array2, 0, 3840);
						num++;
						long num2 = num * 10 - stopwatch.ElapsedMilliseconds;
						if (num2 > 2)
						{
							Thread.Sleep((int)num2 - 1);
						}
					}
				}
				catch (Exception ex)
				{
					if (Connected)
					{
						HapticsPlugin.Log.LogWarning((object)("Bridge disconnected (" + ex.GetType().Name + ": " + ex.Message + "); falling back to rumble"));
					}
					else
					{
						string text = ex.GetType().Name + ": " + ex.Message;
						if (text != lastConnectError)
						{
							lastConnectError = text;
							HapticsPlugin.Log.LogWarning((object)$"Cannot reach HapticsBridge on 127.0.0.1:{port} ({text}) - using rumble fallback, retrying every 3s");
						}
					}
				}
				finally
				{
					Connected = false;
					tcpClient?.Close();
				}
				if (!shutdown)
				{
					Thread.Sleep(3000);
				}
			}
		}

		private static void MixBlock(float[] block, List<PS5HapticEmission> toRemove)
		{
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			Array.Clear(block, 0, block.Length);
			float cachedTimeScale = CachedTimeScale;
			float num = CachedStrengthMultiplier * CachedMasterGain;
			bool cachedMixerPaused = CachedMixerPaused;
			lock (mixLock)
			{
				toRemove.Clear();
				foreach (PS5HapticEmission item in active)
				{
					if (!((VibrationEmission)item).IsPlaying)
					{
						toRemove.Add(item);
					}
					else
					{
						if (cachedMixerPaused || (cachedTimeScale <= float.Epsilon && !((VibrationEmission)item).IsRealtime))
						{
							continue;
						}
						VibrationTarget target = ((VibrationEmission)item).Target;
						VibrationMotors motors = ((VibrationTarget)(ref target)).Motors;
						if ((int)motors == 0)
						{
							item.Advance(480f * ((VibrationEmission)item).Speed);
							continue;
						}
						float num2 = (((motors & 1) != 0) ? 1f : 0f);
						float num3 = (((motors & 2) != 0) ? 1f : 0f);
						float num4 = ((VibrationEmission)item).Strength * num;
						double num5 = item.PositionFrames;
						double num6 = ((VibrationEmission)item).Speed;
						HapticPcm pcm = item.Pcm;
						for (int i = 0; i < 480; i++)
						{
							int num7 = (int)num5;
							if (num7 >= pcm.FrameCount)
							{
								if (!((VibrationEmission)item).IsLooping)
								{
									break;
								}
								num5 -= (double)pcm.FrameCount;
								num7 = (int)num5;
							}
							block[i * 2] += pcm.Samples[num7 * 2] * num4 * num2;
							block[i * 2 + 1] += pcm.Samples[num7 * 2 + 1] * num4 * num3;
							num5 += num6;
						}
						item.Advance(480.0 * num6);
					}
				}
				foreach (PS5HapticEmission item2 in toRemove)
				{
					active.Remove(item2);
				}
			}
			for (int j = 0; j < block.Length; j++)
			{
				if (block[j] > 1f)
				{
					block[j] = 1f;
				}
				else if (block[j] < -1f)
				{
					block[j] = -1f;
				}
			}
		}
	}
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
	public sealed class PS5HapticEmission : VibrationEmission
	{
		internal readonly HapticPcm Pcm;

		internal double PositionFrames;

		private volatile bool isPlaying;

		private bool isLooping;

		private string tag;

		private VibrationTarget target;

		private readonly bool isRealtime;

		public override VibrationTarget Target
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return target;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				target = value;
			}
		}

		public override bool IsLooping
		{
			get
			{
				return isLooping;
			}
			set
			{
				isLooping = value;
			}
		}

		public override string Tag
		{
			get
			{
				return tag;
			}
			set
			{
				tag = value;
			}
		}

		public override bool IsRealtime => isRealtime;

		public override bool IsPlaying => isPlaying;

		public override float Time
		{
			get
			{
				return (float)(PositionFrames / 48000.0);
			}
			set
			{
				PositionFrames = value * 48000f;
			}
		}

		public PS5HapticEmission(HapticPcm pcm, float baseStrength, bool isLooping, string tag, VibrationTarget target, bool isRealtime)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			((VibrationEmission)this)..ctor();
			Pcm = pcm;
			this.isLooping = isLooping;
			this.tag = tag;
			this.target = target;
			this.isRealtime = isRealtime;
			((VibrationEmission)this).BaseStrength = baseStrength;
			isPlaying = true;
		}

		public override void Play()
		{
			isPlaying = true;
		}

		public override void Stop()
		{
			isPlaying = false;
		}

		internal void Advance(double frames)
		{
			PositionFrames += frames;
			if (PositionFrames >= (double)Pcm.FrameCount)
			{
				if (isLooping)
				{
					PositionFrames %= Pcm.FrameCount;
				}
				else
				{
					isPlaying = false;
				}
			}
		}
	}
}
