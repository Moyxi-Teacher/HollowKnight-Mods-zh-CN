using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace HapticsBridge;

internal sealed class Engine
{
	public enum State
	{
		NoDevice,
		Extracting,
		Listening,
		GameConnected
	}

	public struct LatencyPreset(string key, string label, string blurb, int latencyMs, int bufferMs, bool eventSync)
	{
		public readonly string Key = key;

		public readonly string Label = label;

		public readonly string Blurb = blurb;

		public readonly int LatencyMs = latencyMs;

		public readonly int BufferMs = bufferMs;

		public readonly bool EventSync = eventSync;
	}

	private sealed class Lane
	{
		public TcpClient Client;

		public BufferedWaveProvider Buffer;

		public ISampleProvider Input;
	}

	public static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HapticsBridge.log");

	private volatile State state;

	private volatile string stateDetail = "";

	private readonly string deviceMatch;

	private readonly int port;

	private volatile int bufferMs;

	private volatile int latencyMs;

	private volatile bool eventSync;

	private readonly bool hidAssert;

	private readonly bool noExtract;

	private readonly string map;

	private readonly bool chimeOnStart;

	private readonly bool exitOnDisconnect;

	private volatile bool stop;

	private volatile bool sessionDead;

	private volatile bool fastReopen;

	private volatile bool everConnected;

	public bool StopRequested => stop;

	public static readonly LatencyPreset[] Presets = new LatencyPreset[3]
	{
		new LatencyPreset("reliable", "Reliable", "survives Bluetooth & busy PCs (default)", 100, 60, eventSync: false),
		new LatencyPreset("snappy", "Snappy", "lower latency, best on wired USB", 40, 30, eventSync: false),
		new LatencyPreset("minimal", "Minimal", "lowest latency, wired only (may glitch)", 20, 20, eventSync: true)
	};

	private static readonly string SettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "latency.cfg");

	private Thread thread;

	private TcpListener listener;

	private volatile BufferedWaveProvider chimeBuffer;

	private string openedDeviceId;

	private readonly List<Lane> lanes = new List<Lane>();

	private int clientCount;

	private static readonly object logLock = new object();

	public State CurrentState => state;

	public string StateDetail => stateDetail;

	public string CurrentPresetKey
	{
		get
		{
			LatencyPreset[] presets = Presets;
			for (int i = 0; i < presets.Length; i++)
			{
				LatencyPreset latencyPreset = presets[i];
				if (latencyPreset.LatencyMs == latencyMs && latencyPreset.BufferMs == bufferMs && latencyPreset.EventSync == eventSync)
				{
					return latencyPreset.Key;
				}
			}
			return null;
		}
	}

	public Engine(string[] args)
	{
		deviceMatch = Program.GetArg(args, "--device") ?? "DualSense";
		port = (int.TryParse(Program.GetArg(args, "--port"), out var result) ? result : 48111);
		bufferMs = (int.TryParse(Program.GetArg(args, "--buffer-ms"), out var result2) ? result2 : 60);
		latencyMs = (int.TryParse(Program.GetArg(args, "--latency-ms"), out var result3) ? result3 : 100);
		eventSync = args.Contains("--event-sync");
		hidAssert = !args.Contains("--no-hid");
		noExtract = args.Contains("--no-extract");
		map = Program.GetArg(args, "--map") ?? "auto";
		chimeOnStart = !args.Contains("--no-chime");
		exitOnDisconnect = args.Contains("--exit-on-disconnect");
		try
		{
			File.WriteAllText(LogPath, "");
		}
		catch
		{
		}
		LoadPreset();
	}

	private void LoadPreset()
	{
		try
		{
			if (!File.Exists(SettingsPath))
			{
				return;
			}
			string text = File.ReadAllText(SettingsPath).Trim();
			LatencyPreset[] presets = Presets;
			for (int i = 0; i < presets.Length; i++)
			{
				LatencyPreset latencyPreset = presets[i];
				if (latencyPreset.Key == text)
				{
					latencyMs = latencyPreset.LatencyMs;
					bufferMs = latencyPreset.BufferMs;
					eventSync = latencyPreset.EventSync;
					break;
				}
			}
		}
		catch
		{
		}
	}

	public void ApplyPreset(string key)
	{
		LatencyPreset[] presets = Presets;
		for (int i = 0; i < presets.Length; i++)
		{
			LatencyPreset latencyPreset = presets[i];
			if (latencyPreset.Key == key)
			{
				latencyMs = latencyPreset.LatencyMs;
				bufferMs = latencyPreset.BufferMs;
				eventSync = latencyPreset.EventSync;
				try
				{
					File.WriteAllText(SettingsPath, key);
				}
				catch
				{
				}
				Log(string.Format("Latency preset: {0} ({1}-driven, {2} ms latency, {3} ms buffer)", latencyPreset.Label, eventSync ? "event" : "timer", latencyMs, bufferMs));
				fastReopen = true;
				KillSession("latency preset changed");
				break;
			}
		}
	}

	public void Start()
	{
		thread = new Thread(Run)
		{
			IsBackground = true,
			Name = "HapticsEngine"
		};
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
	}

	public void Stop()
	{
		stop = true;
		try
		{
			if (listener != null)
			{
				listener.Stop();
			}
		}
		catch
		{
		}
	}

	public void RequestChime()
	{
		BufferedWaveProvider cb = chimeBuffer;
		if (cb == null)
		{
			return;
		}
		Thread obj = new Thread((ThreadStart)delegate
		{
			try
			{
				Chime.Play(cb);
			}
			catch
			{
			}
		});
		obj.IsBackground = true;
		obj.Start();
	}

	private void Run()
	{
		if (!noExtract)
		{
			EnsureClips();
		}
		bool flag = false;
		int num = 2000;
		int tickCount = Environment.TickCount;
		while (!stop)
		{
			MMDevice mMDevice = FindDevice();
			if (mMDevice == null)
			{
				state = State.NoDevice;
				SleepInterruptible(4000);
				continue;
			}
			try
			{
				WaveFormat mixFormat = mMDevice.AudioClient.MixFormat;
				Log($"Device: {mMDevice.FriendlyName} ({mixFormat.SampleRate} Hz, {mixFormat.Channels} ch, {mixFormat.BitsPerSample}-bit)");
				if (mixFormat.BitsPerSample != 32)
				{
					Log("Unsupported mix format (not 32-bit float); retrying in 10s");
					state = State.NoDevice;
					SleepInterruptible(10000);
					continue;
				}
				int num2;
				int num3;
				if (map == "12" || (map == "auto" && mixFormat.Channels < 4))
				{
					num2 = 0;
					num3 = Math.Min(1, mixFormat.Channels - 1);
				}
				else
				{
					num2 = 2;
					num3 = 3;
				}
				Log($"Routing: haptic L -> ch{num2 + 1}, haptic R -> ch{num3 + 1}");
				WaveFormat waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(48000, 2);
				BufferedWaveProvider bufferedWaveProvider = new BufferedWaveProvider(waveFormat)
				{
					BufferDuration = TimeSpan.FromMilliseconds(500.0),
					DiscardOnBufferOverflow = true
				};
				MixingSampleProvider mixer = new MixingSampleProvider(new ISampleProvider[1] { bufferedWaveProvider.ToSampleProvider() })
				{
					ReadFully = true
				};
				ISampleProvider sampleProvider = mixer;
				if (mixFormat.SampleRate != 48000)
				{
					sampleProvider = new WdlResamplingSampleProvider(sampleProvider, mixFormat.SampleRate);
				}
				ChannelMapWaveProvider waveProvider = new ChannelMapWaveProvider(sampleProvider, mixFormat, num2, num3);
				WasapiOut wasapiOut = null;
				sessionDead = false;
				tickCount = Environment.TickCount;
				openedDeviceId = mMDevice.ID;
				chimeBuffer = bufferedWaveProvider;
				Log(string.Format("Output: {0}-driven, {1} ms latency", eventSync ? "event" : "timer", latencyMs));
				wasapiOut = new WasapiOut(mMDevice, AudioClientShareMode.Shared, eventSync, latencyMs);
				try
				{
					wasapiOut.PlaybackStopped += delegate(object s, StoppedEventArgs e)
					{
						if (e.Exception != null)
						{
							Log("Playback stopped: " + e.Exception.Message);
						}
						// 关键修复：音频设备临时不可用时仅暂停输出，不断开会话和 TCP 连接。
						// 旧行为调用 KillSession() 关闭监听器，导致游戏客户端被迫断开重连。
						// 新的 WatchdogLoop 会持续检测设备恢复，设备就绪后自动重新播放。
						// 这样 DualSense 端点被游戏音频占用期间，Bridge 保持监听状态，游戏按键不受影响。
						Log("Playback paused; watchdog will resume when device recovers");
						try { wasapiOut?.Pause(); } catch { }
					};
					wasapiOut.Init(waveProvider);
					wasapiOut.Play();
					if (hidAssert)
					{
						DualSenseHid.AssertAudioHaptics();
					}
					Thread obj = new Thread((ThreadStart)delegate
					{
						WatchdogLoop(wasapiOut);
					});
					obj.IsBackground = true;
					obj.Name = "HapticsWatchdog";
					obj.Start();
					if (chimeOnStart && !flag)
					{
						flag = true;
						Chime.Play(bufferedWaveProvider);
						Log("Startup chime played");
					}
					listener = new TcpListener(IPAddress.Loopback, port);
					listener.Start();
					Log("Listening on 127.0.0.1:" + port);
					state = State.Listening;
					while (!stop && !sessionDead)
					{
						while (!stop && !sessionDead && !listener.Pending())
						{
							Thread.Sleep(100);
						}
						if (!stop && !sessionDead)
						{
							TcpClient tcpClient = listener.AcceptTcpClient();
							tcpClient.NoDelay = true;
							Lane lane = new Lane
							{
								Client = tcpClient,
								Buffer = new BufferedWaveProvider(waveFormat)
								{
									BufferDuration = TimeSpan.FromMilliseconds(Math.Max(bufferMs * 4, 250)),
									DiscardOnBufferOverflow = true
								}
							};
							lane.Input = lane.Buffer.ToSampleProvider();
							lock (lanes)
							{
								lanes.Add(lane);
							}
							mixer.AddMixerInput(lane.Input);
							int num4 = Interlocked.Increment(ref clientCount);
							Log("Game connected (" + num4 + " instance" + ((num4 > 1) ? "s" : "") + ")");
							everConnected = true;
							state = State.GameConnected;
							Thread obj2 = new Thread((ThreadStart)delegate
							{
								ClientLoop(lane, mixer);
							});
							obj2.IsBackground = true;
							obj2.Name = "HapticsClient";
							obj2.Start();
							continue;
						}
						break;
					}
				}
				finally
				{
					if (wasapiOut != null)
					{
						((IDisposable)wasapiOut).Dispose();
					}
				}
			}
			catch (SocketException)
			{
			}
			catch (Exception ex2)
			{
				Log($"Engine error: {ex2.GetType().Name}: {ex2.Message}");
			}
			finally
			{
				sessionDead = true;
				chimeBuffer = null;
				lock (lanes)
				{
					foreach (Lane lane2 in lanes)
					{
						try
						{
							lane2.Client.Close();
						}
						catch
						{
						}
					}
					lanes.Clear();
				}
				try
				{
					if (listener != null)
					{
						listener.Stop();
					}
				}
				catch
				{
				}
				listener = null;
			}
			if (!stop)
			{
				state = State.NoDevice;
				if (fastReopen)
				{
					fastReopen = false;
					num = 2000;
					SleepInterruptible(250);
				}
				else
				{
					num = ((Environment.TickCount - tickCount < 60000) ? Math.Min(num * 2, 5000) : 2000);
					SleepInterruptible(num);
				}
			}
		}
	}

	private void ClientLoop(Lane lane, MixingSampleProvider mixer)
	{
		byte[] array = new byte[3840];
		try
		{
			using NetworkStream s = lane.Client.GetStream();
			int count;
			while (!stop && !sessionDead && (count = ReadBlock(s, array)) > 0)
			{
				if (lane.Buffer.BufferedDuration.TotalMilliseconds > (double)(bufferMs * 2))
				{
					lane.Buffer.ClearBuffer();
				}
				lane.Buffer.AddSamples(array, 0, count);
			}
		}
		catch (IOException)
		{
		}
		catch (ObjectDisposedException)
		{
		}
		finally
		{
			try
			{
				mixer.RemoveMixerInput(lane.Input);
			}
			catch
			{
			}
			lock (lanes)
			{
				lanes.Remove(lane);
			}
			try
			{
				lane.Client.Close();
			}
			catch
			{
			}
			int num = Interlocked.Decrement(ref clientCount);
			Log("Game disconnected (" + num + " remaining)");
			if (!sessionDead)
			{
				state = ((num > 0) ? State.GameConnected : State.Listening);
			}
			if (exitOnDisconnect && num <= 0 && everConnected)
			{
				Log("Last game client left and --exit-on-disconnect set: shutting down engine");
				stop = true;
			}
		}
	}

	private void KillSession(string why)
	{
		if (sessionDead)
		{
			return;
		}
		sessionDead = true;
		Log("Reopening audio session: " + why);
		lock (lanes)
		{
			foreach (Lane lane in lanes)
			{
				try
				{
					lane.Client.Close();
				}
				catch
				{
				}
			}
		}
		try
		{
			if (listener != null)
			{
				listener.Stop();
			}
		}
		catch
		{
		}
	}

	private void WatchdogLoop(WasapiOut output)
	{
		long num = -1L;
		int num2 = 0;
		while (!stop && !sessionDead)
		{
			Thread.Sleep(1500);
			if (stop || sessionDead)
			{
				break;
			}
			try
			{
				if (hidAssert)
				{
					DualSenseHid.AssertAudioHaptics();
				}
				using (MMDeviceEnumerator mMDeviceEnumerator = new MMDeviceEnumerator())
				{
					MMDevice mMDevice = mMDeviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).FirstOrDefault((MMDevice d) => d.FriendlyName.IndexOf(deviceMatch, StringComparison.OrdinalIgnoreCase) >= 0);
					if (mMDevice == null)
					{
						// 设备暂时不可用，不立即终止会话，让 Run 循环稍后重试
						Log("Watchdog: audio device temporarily unavailable, retrying");
						break;
					}
					if (mMDevice.ID != openedDeviceId)
					{
						// 设备实例 ID 变化通常只是 Windows 重新枚举设备，
						// 不一定代表设备真的被移除，不要立即终止会话
						Log("Watchdog: audio device endpoint changed, monitoring");
						openedDeviceId = mMDevice.ID;
					}
				}
				long position = output.GetPosition();
				if (position == num)
				{
					if (++num2 >= 2)
					{
						KillSession("render clock frozen (endpoint stopped consuming audio)");
						break;
					}
				}
				else
				{
					num2 = 0;
				}
				num = position;
			}
			catch (Exception ex)
			{
				// WASAPI 设备失效（HRESULT 0x88890004）时不要立即终止会话，
				// 让 Run 循环稍后重新打开音频设备，避免游戏频繁掉线回退 rumble
				Log("Watchdog: " + ex.GetType().Name + ": " + ex.Message + ", retrying");
				break;
			}
		}
	}

	private void EnsureClips()
	{
		try
		{
			string text = GameLocator.Locate(interactive: false);
			string text2 = null;
			if (text != null)
			{
				text2 = Path.Combine(text, "BepInEx", "plugins", "SilksongPS5Haptics", "clips");
			}
			if (text2 != null && Directory.Exists(text2) && Directory.GetFiles(text2, "*.wav").Length >= 300)
			{
				return;
			}
			if (text == null)
			{
				text = GameLocator.Locate(interactive: true);
				if (text == null)
				{
					Log("Game location not provided; clips unavailable (game will use normal rumble)");
					return;
				}
				text2 = Path.Combine(text, "BepInEx", "plugins", "SilksongPS5Haptics", "clips");
			}
			state = State.Extracting;
			Log("Extracting PS5 haptic clips from game files (one-time)...");
			int num = ClipExtractor.ExtractAll(text, text2, delegate(int done, int total)
			{
				stateDetail = $"{done}/{total}";
			});
			Log($"Extracted {num} haptic clips -> {text2}");
		}
		catch (Exception ex)
		{
			Log("Extraction failed: " + ex);
		}
	}

	private MMDevice FindDevice()
	{
		try
		{
			using MMDeviceEnumerator mMDeviceEnumerator = new MMDeviceEnumerator();
			return mMDeviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).FirstOrDefault((MMDevice d) => d.FriendlyName.IndexOf(deviceMatch, StringComparison.OrdinalIgnoreCase) >= 0);
		}
		catch (Exception ex)
		{
			Log("Device enumeration failed: " + ex.Message);
			return null;
		}
	}

	private void SleepInterruptible(int ms)
	{
		for (int i = 0; i < ms / 100; i++)
		{
			if (stop)
			{
				break;
			}
			Thread.Sleep(100);
		}
	}

	private static int ReadBlock(Stream s, byte[] block)
	{
		int i;
		int num;
		for (i = 0; i < block.Length; i += num)
		{
			num = s.Read(block, i, block.Length - i);
			if (num == 0)
			{
				return i;
			}
		}
		return i;
	}

	public static void Log(string message)
	{
		try
		{
			lock (logLock)
			{
				File.AppendAllText(LogPath, $"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
			}
		}
		catch
		{
		}
	}
}
