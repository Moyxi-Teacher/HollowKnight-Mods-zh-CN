using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fmod5Sharp.ChunkData;
using Fmod5Sharp.FmodTypes;
using Fmod5Sharp.Util;
using NAudio.Wave;

namespace Fmod5Sharp.CodecRebuilders;

public static class FmodGcadPcmRebuilder
{
	private const int BytesPerFrame = 8;

	private const int SamplesPerFrame = 14;

	private const int NibblesPerFrame = 16;

	private static short[] GetPcmData(FmodSample sample)
	{
		int num = ByteCountToSampleCount(sample.SampleBytes.Length);
		double num2 = Math.Ceiling((double)num / 14.0);
		short[] array = new short[num];
		byte[] sampleBytes = sample.SampleBytes;
		List<short> list = ((DspCoefficientsBlockData)sample.Metadata.Chunks.First((FmodSampleChunk c) => c.ChunkType == FmodSampleChunkType.DSPCOEFF).ChunkData).ChannelData[0];
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		short num6 = 0;
		short num7 = 0;
		for (int num8 = 0; (double)num8 < num2; num8++)
		{
			byte b = sampleBytes[num5++];
			int num9 = 1 << (b & 0xF);
			int num10 = b >> 4;
			short num11 = list[num10 * 2];
			short num12 = list[num10 * 2 + 1];
			int num13 = Math.Min(14, num - num3);
			for (int num14 = 0; num14 < num13; num14++)
			{
				short num15 = Clamp16((((num14 % 2 == 0) ? Utils.GetHighNibbleSigned(sampleBytes[num5]) : Utils.GetLowNibbleSigned(sampleBytes[num5++])) * num9 << 11) + 1024 + num11 * num6 + num12 * num7 >> 11);
				num7 = num6;
				num6 = num15;
				array[num4++] = num15;
				num3++;
			}
		}
		return array;
	}

	public static byte[] Rebuild(FmodSample sample)
	{
		int num = ((!sample.Metadata.IsStereo) ? 1 : 2);
		WaveFormat format = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, sample.Metadata.Frequency, num, sample.Metadata.Frequency * num * 2, num * 2, 16);
		using MemoryStream memoryStream = new MemoryStream();
		using WaveFileWriter waveFileWriter = new WaveFileWriter(memoryStream, format);
		short[] pcmData = GetPcmData(sample);
		waveFileWriter.WriteSamples(pcmData, 0, pcmData.Length);
		return memoryStream.ToArray();
	}

	private static int NibbleCountToSampleCount(int nibbleCount)
	{
		int num = nibbleCount / 16;
		int num2 = nibbleCount % 16;
		int num3 = ((num2 >= 2) ? (num2 - 2) : 0);
		return 14 * num + num3;
	}

	private static int ByteCountToSampleCount(int byteCount)
	{
		return NibbleCountToSampleCount(byteCount * 2);
	}

	private static short Clamp16(int value)
	{
		if (value > 32767)
		{
			return short.MaxValue;
		}
		if (value < -32768)
		{
			return short.MinValue;
		}
		return (short)value;
	}
}
