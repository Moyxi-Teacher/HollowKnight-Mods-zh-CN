using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NAudio.CoreAudioApi;

namespace HapticsBridge;

internal static class Program
{
	public const int SampleRate = 48000;

	[STAThread]
	private static int Main(string[] args)
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		if (args.Contains("--list"))
		{
			using (MMDeviceEnumerator mMDeviceEnumerator = new MMDeviceEnumerator())
			{
				IEnumerable<string> values = from d in mMDeviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
					select $"[{d.AudioClient.MixFormat.Channels}ch @ {d.AudioClient.MixFormat.SampleRate}Hz] {d.FriendlyName}";
				MessageBox.Show(string.Join("\n", values), "Active audio render devices");
			}
			return 0;
		}
		if (args.Contains("--extract-only"))
		{
			string arg = GetArg(args, "--extract-only");
			string text = GameLocator.Locate(interactive: true);
			if (text == null)
			{
				return 2;
			}
			int num = ClipExtractor.ExtractAll(text, arg, delegate
			{
			});
			Engine.Log($"Extract-only: {num} clips -> {arg}");
			return (num <= 0) ? 1 : 0;
		}
		bool createdNew;
		using (new Mutex(initiallyOwned: true, "SilksongHapticsBridge", out createdNew))
		{
			if (!createdNew)
			{
				return 0;
			}
			Application.Run(new TrayContext(args));
			return 0;
		}
	}

	public static string GetArg(string[] args, string name)
	{
		int num = Array.IndexOf(args, name);
		if (num < 0 || num + 1 >= args.Length)
		{
			return null;
		}
		return args[num + 1];
	}
}
