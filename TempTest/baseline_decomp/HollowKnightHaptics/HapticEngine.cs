using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace HollowKnightHaptics;

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
