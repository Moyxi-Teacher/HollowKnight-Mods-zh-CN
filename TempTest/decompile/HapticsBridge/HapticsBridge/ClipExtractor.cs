using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Fmod5Sharp;
using Fmod5Sharp.FmodTypes;
using NVorbis;

namespace HapticsBridge;

internal static class ClipExtractor
{
	private const string BundleRelPath = "Hollow Knight Silksong_Data\\StreamingAssets\\aa\\StandaloneWindows64\\vibrationstatic_assets_vibrationdataps5.bundle";

	public static int ExtractAll(string gameRoot, string outDir, Action<int, int> progress)
	{
		string text = Path.Combine(gameRoot, "Hollow Knight Silksong_Data\\StreamingAssets\\aa\\StandaloneWindows64\\vibrationstatic_assets_vibrationdataps5.bundle");
		if (!File.Exists(text))
		{
			throw new FileNotFoundException("PS5 vibration bundle not found", text);
		}
		Directory.CreateDirectory(outDir);
		AssetsManager assetsManager = new AssetsManager();
		try
		{
			BundleFileInstance bundle = assetsManager.LoadBundleFile(text);
			List<AssetBundleDirectoryInfo> dirInfos = bundle.file.BlockAndDirInfo.DirectoryInfos;
			Dictionary<string, byte[]> resCache = new Dictionary<string, byte[]>();
			Func<string, byte[]> func = delegate(string name)
			{
				if (resCache.TryGetValue(name, out var value))
				{
					return value;
				}
				for (int i = 0; i < dirInfos.Count; i++)
				{
					if (dirInfos[i].Name == name)
					{
						AssetsFileReader dataReader = bundle.file.DataReader;
						dataReader.Position = dirInfos[i].Offset;
						byte[] array3 = dataReader.ReadBytes((int)dirInfos[i].DecompressedSize);
						resCache[name] = array3;
						return array3;
					}
				}
				return (byte[])null;
			};
			AssetsFileInstance assetsFileInstance = assetsManager.LoadAssetsFileFromBundle(bundle, 0);
			List<AssetFileInfo> assetsOfType = assetsFileInstance.file.GetAssetsOfType(AssetClassID.AudioClip);
			int num = 0;
			int num2 = 0;
			foreach (AssetFileInfo item in assetsOfType)
			{
				num++;
				progress?.Invoke(num, assetsOfType.Count);
				try
				{
					AssetTypeValueField baseField = assetsManager.GetBaseField(assetsFileInstance, item);
					string asString = baseField["m_Name"].AsString;
					AssetTypeValueField assetTypeValueField = baseField["m_Resource"];
					string asString2 = assetTypeValueField["m_Source"].AsString;
					long asLong = assetTypeValueField["m_Offset"].AsLong;
					long asLong2 = assetTypeValueField["m_Size"].AsLong;
					string arg = asString2.Substring(asString2.LastIndexOf('/') + 1);
					byte[] array = func(arg);
					if (array == null)
					{
						Engine.Log(asString + ": resource file missing");
						continue;
					}
					byte[] array2 = new byte[asLong2];
					Array.Copy(array, asLong, array2, 0L, asLong2);
					FmodSample fmodSample = FsbLoader.LoadFsbFromByteArray(array2).Samples[0];
					if (!fmodSample.RebuildAsStandardFileFormat(out byte[] data, out string fileExtension))
					{
						Engine.Log(asString + ": FSB rebuild failed");
						continue;
					}
					string text2 = Path.Combine(outDir, asString + ".wav");
					if (fileExtension == "ogg")
					{
						DecodeOggToWav(data, (int)fmodSample.Metadata.SampleCount, text2);
						goto IL_0216;
					}
					if (fileExtension == "wav")
					{
						File.WriteAllBytes(text2, data);
						goto IL_0216;
					}
					Engine.Log(asString + ": unexpected format " + fileExtension);
					goto end_IL_00ca;
					IL_0216:
					num2++;
					end_IL_00ca:;
				}
				catch (Exception ex)
				{
					Engine.Log("clip " + num + ": " + ex.GetType().Name + ": " + ex.Message);
				}
			}
			return num2;
		}
		finally
		{
			assetsManager.UnloadAll();
		}
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
			float num4 = ((num3 < -1f) ? (-1f) : ((num3 > 1f) ? 1f : num3));
			binaryWriter.Write((short)Math.Round(num4 * 32767f));
		}
	}
}
