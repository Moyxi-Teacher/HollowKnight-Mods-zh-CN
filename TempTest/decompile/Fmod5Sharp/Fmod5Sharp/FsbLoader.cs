using System;
using System.Collections.Generic;
using System.IO;
using Fmod5Sharp.FmodTypes;
using Fmod5Sharp.Util;

namespace Fmod5Sharp;

public static class FsbLoader
{
	internal static readonly Dictionary<uint, int> Frequencies = new Dictionary<uint, int>
	{
		{ 1u, 8000 },
		{ 2u, 11000 },
		{ 3u, 11025 },
		{ 4u, 16000 },
		{ 5u, 22050 },
		{ 6u, 24000 },
		{ 7u, 32000 },
		{ 8u, 44100 },
		{ 9u, 48000 },
		{ 10u, 96000 }
	};

	private static FmodSoundBank? LoadInternal(byte[] bankBytes, bool throwIfError)
	{
		using MemoryStream input = new MemoryStream(bankBytes);
		using BinaryReader binaryReader = new BinaryReader(input);
		FmodAudioHeader fmodAudioHeader = new FmodAudioHeader(binaryReader);
		if (!fmodAudioHeader.IsValid)
		{
			if (throwIfError)
			{
				throw new Exception("File is probably not an FSB file (magic number mismatch)");
			}
			return null;
		}
		List<FmodSample> list = new List<FmodSample>();
		Span<byte> span = MemoryExtensions.AsSpan(bankBytes, (int)(fmodAudioHeader.SizeOfThisHeader + fmodAudioHeader.SizeOfNameTable + fmodAudioHeader.SizeOfSampleHeaders));
		for (int i = 0; i < fmodAudioHeader.Samples.Count; i++)
		{
			FmodSampleMetadata fmodSampleMetadata = fmodAudioHeader.Samples[i];
			int num = (int)fmodSampleMetadata.DataOffset;
			int num2 = (int)fmodAudioHeader.SizeOfData;
			if (i < fmodAudioHeader.Samples.Count - 1)
			{
				num2 = (int)fmodAudioHeader.Samples[i + 1].DataOffset;
			}
			Span<byte> span2 = span;
			int num3 = num;
			FmodSample fmodSample = new FmodSample(fmodSampleMetadata, span2.Slice(num3, num2 - num3).ToArray());
			if (fmodAudioHeader.SizeOfNameTable != 0)
			{
				long position = fmodAudioHeader.SizeOfThisHeader + fmodAudioHeader.SizeOfSampleHeaders + 4 * i;
				binaryReader.BaseStream.Position = position;
				uint num4 = binaryReader.ReadUInt32();
				num4 += fmodAudioHeader.SizeOfThisHeader + fmodAudioHeader.SizeOfSampleHeaders;
				fmodSample.Name = bankBytes.ReadNullTerminatedString((int)num4);
			}
			list.Add(fmodSample);
		}
		return new FmodSoundBank(fmodAudioHeader, list);
	}

	public static bool TryLoadFsbFromByteArray(byte[] bankBytes, out FmodSoundBank? bank)
	{
		bank = LoadInternal(bankBytes, throwIfError: false);
		return bank != null;
	}

	public static FmodSoundBank LoadFsbFromByteArray(byte[] bankBytes)
	{
		return LoadInternal(bankBytes, throwIfError: true);
	}
}
