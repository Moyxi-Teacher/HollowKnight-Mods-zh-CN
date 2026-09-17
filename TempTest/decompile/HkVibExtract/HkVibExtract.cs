using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Fmod5Sharp;
using Fmod5Sharp.FmodTypes;
using NVorbis;

internal class HkVibExtract
{
	private static int Main(string[] args)
	{
		string path = ((args.Length > 0) ? args[0] : "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Hollow Knight\\hollow_knight_Data");
		string text = ((args.Length > 1) ? args[1] : "C:\\Users\\uptoh\\AppData\\Local\\Temp\\claude\\hk_vib_wavs");
		Directory.CreateDirectory(text);
		string path2 = Path.Combine(path, "resources.assets");
		string path3 = Path.Combine(path, "resources.resource");
		byte[] array = File.ReadAllBytes(path3);
		AssetsManager assetsManager = new AssetsManager();
		AssetsFileInstance assetsFileInstance = assetsManager.LoadAssetsFile(path2);
		AssetsFile file = assetsFileInstance.file;
		AssetsFileReader reader = file.Reader;
		long dataOffset = file.Header.DataOffset;
		int num = 0;
		int num2 = 0;
		foreach (AssetFileInfo item in file.GetAssetsOfType(AssetClassID.AudioClip))
		{
			long position = dataOffset + item.ByteOffset;
			reader.Position = position;
			byte[] array2 = reader.ReadBytes((int)item.ByteSize);
			int num3 = 0;
			int num4 = BitConverter.ToInt32(array2, num3);
			num3 += 4;
			if (num4 <= 0 || num4 > 256 || num3 + num4 > array2.Length)
			{
				continue;
			}
			string text2 = Encoding.ASCII.GetString(array2, num3, num4);
			if (text2.IndexOf("vibrat", StringComparison.OrdinalIgnoreCase) < 0)
			{
				continue;
			}
			num2++;
			try
			{
				string text3 = Encoding.ASCII.GetString(array2);
				int num5 = text3.IndexOf(".resource", StringComparison.Ordinal);
				if (num5 < 0)
				{
					Console.WriteLine(text2 + ": no stream ref");
					continue;
				}
				int num6 = num5;
				while (num6 > 0 && array2[num6 - 1] >= 32 && array2[num6 - 1] < 127)
				{
					num6--;
				}
				int num7 = num5 + 9 - num6;
				int num8 = num6 + num7;
				int num9 = (num8 + 3) & -4;
				ulong num10 = BitConverter.ToUInt64(array2, num9);
				ulong num11 = BitConverter.ToUInt64(array2, num9 + 8);
				if (num11 == 0 || num10 + num11 > (ulong)array.Length)
				{
					Console.WriteLine(text2 + ": bad range off=" + num10 + " size=" + num11);
					continue;
				}
				byte[] array3 = new byte[num11];
				Array.Copy(array, (long)num10, array3, 0L, (long)num11);
				if (array3[0] != 70 || array3[1] != 83 || array3[2] != 66 || array3[3] != 53)
				{
					Console.WriteLine(text2 + ": not FSB5 (starts " + (char)array3[0] + (char)array3[1] + (char)array3[2] + ")");
					continue;
				}
				FmodSoundBank fmodSoundBank = FsbLoader.LoadFsbFromByteArray(array3);
				FmodSample fmodSample = fmodSoundBank.Samples[0];
				if (!fmodSample.RebuildAsStandardFileFormat(out byte[] data, out string fileExtension))
				{
					Console.WriteLine(text2 + ": rebuild failed");
					continue;
				}
				string text4 = Path.Combine(text, Sanitize(text2) + ".wav");
				int frequency = fmodSample.Metadata.Frequency;
				int channels = (int)fmodSample.Metadata.Channels;
				int num12 = (int)fmodSample.Metadata.SampleCount;
				if (fileExtension == "ogg")
				{
					DecodeOggToWav(data, num12, text4);
					goto IL_03cf;
				}
				if (fileExtension == "wav")
				{
					File.WriteAllBytes(text4, data);
					goto IL_03cf;
				}
				Console.WriteLine(text2 + ": unexpected " + fileExtension);
				goto end_IL_013f;
				IL_03cf:
				double num13 = ((frequency > 0) ? ((double)num12 / (double)frequency) : 0.0);
				Console.WriteLine($"{text2,-46} {channels}ch {frequency}Hz {num13:0.00}s  ({fileExtension}, {num11}B fsb)");
				num++;
				end_IL_013f:;
			}
			catch (Exception ex)
			{
				Console.WriteLine(text2 + ": " + ex.GetType().Name + " " + ex.Message);
			}
		}
		assetsManager.UnloadAll();
		Console.WriteLine("\nExtracted " + num + "/" + num2 + " -> " + text);
		return 0;
	}

	private static string Sanitize(string n)
	{
		char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
		foreach (char oldChar in invalidFileNameChars)
		{
			n = n.Replace(oldChar, '_');
		}
		return n.Replace(' ', '_');
	}

	private static void DecodeOggToWav(byte[] ogg, int declaredFrames, string outPath)
	{
		using VorbisReader vorbisReader = new VorbisReader(new MemoryStream(ogg));
		int channels = vorbisReader.Channels;
		int sampleRate = vorbisReader.SampleRate;
		List<float> list = new List<float>((declaredFrames > 0) ? (declaredFrames * channels) : 4096);
		float[] array = new float[4800 * channels];
		int num;
		while ((num = vorbisReader.ReadSamples(array, 0, array.Length)) > 0)
		{
			for (int i = 0; i < num; i++)
			{
				list.Add(array[i]);
			}
		}
		int num2 = list.Count / channels;
		if (declaredFrames > 0 && declaredFrames < num2)
		{
			num2 = declaredFrames;
		}
		WriteWavPcm16(outPath, list, num2, channels, sampleRate);
	}

	private static void WriteWavPcm16(string path, List<float> interleaved, int frames, int channels, int rate)
	{
		using BinaryWriter binaryWriter = new BinaryWriter(File.Create(path));
		int num = frames * channels * 2;
		binaryWriter.Write(Encoding.ASCII.GetBytes("RIFF"));
		binaryWriter.Write(36 + num);
		binaryWriter.Write(Encoding.ASCII.GetBytes("WAVE"));
		binaryWriter.Write(Encoding.ASCII.GetBytes("fmt "));
		binaryWriter.Write(16);
		binaryWriter.Write((short)1);
		binaryWriter.Write((short)channels);
		binaryWriter.Write(rate);
		binaryWriter.Write(rate * channels * 2);
		binaryWriter.Write((short)(channels * 2));
		binaryWriter.Write((short)16);
		binaryWriter.Write(Encoding.ASCII.GetBytes("data"));
		binaryWriter.Write(num);
		int num2 = frames * channels;
		for (int i = 0; i < num2; i++)
		{
			float num3 = interleaved[i];
			num3 = ((num3 < -1f) ? (-1f) : ((num3 > 1f) ? 1f : num3));
			binaryWriter.Write((short)Math.Round(num3 * 32767f));
		}
	}
}
