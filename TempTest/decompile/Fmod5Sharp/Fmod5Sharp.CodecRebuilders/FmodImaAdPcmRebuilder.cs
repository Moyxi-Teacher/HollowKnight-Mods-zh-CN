using System.IO;
using Fmod5Sharp.FmodTypes;
using Fmod5Sharp.Util;
using NAudio.Wave;

namespace Fmod5Sharp.CodecRebuilders;

public static class FmodImaAdPcmRebuilder
{
	public const int SamplesPerFramePerChannel = 64;

	private static readonly int[] ADPCMTable = new int[89]
	{
		7, 8, 9, 10, 11, 12, 13, 14, 16, 17,
		19, 21, 23, 25, 28, 31, 34, 37, 41, 45,
		50, 55, 60, 66, 73, 80, 88, 97, 107, 118,
		130, 143, 157, 173, 190, 209, 230, 253, 279, 307,
		337, 371, 408, 449, 494, 544, 598, 658, 724, 796,
		876, 963, 1060, 1166, 1282, 1411, 1552, 1707, 1878, 2066,
		2272, 2499, 2749, 3024, 3327, 3660, 4026, 4428, 4871, 5358,
		5894, 6484, 7132, 7845, 8630, 9493, 10442, 11487, 12635, 13899,
		15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794, 32767
	};

	private static readonly int[] IMA_IndexTable = new int[16]
	{
		-1, -1, -1, -1, 2, 4, 6, 8, -1, -1,
		-1, -1, 2, 4, 6, 8
	};

	private static void ExpandNibble(MemoryStream stream, long byteOffset, int nibbleShift, ref int hist, ref int stepIndex)
	{
		stream.Seek(byteOffset, SeekOrigin.Begin);
		int num = (stream.ReadByte() >> nibbleShift) & 0xF;
		int num2 = hist;
		int num3 = ADPCMTable[stepIndex];
		int num4 = num3 >> 3;
		if ((num & 1) != 0)
		{
			num4 += num3 >> 2;
		}
		if ((num & 2) != 0)
		{
			num4 += num3 >> 1;
		}
		if ((num & 4) != 0)
		{
			num4 += num3;
		}
		if ((num & 8) != 0)
		{
			num4 = -num4;
		}
		num2 += num4;
		hist = Utils.Clamp((short)num2, short.MinValue, short.MaxValue);
		stepIndex += IMA_IndexTable[num];
		stepIndex = Utils.Clamp((short)stepIndex, 0, 88);
	}

	private static short[] DecodeSamplesFsbIma(FmodSample sample)
	{
		int channels = (int)sample.Metadata.Channels;
		using MemoryStream memoryStream = new MemoryStream(sample.SampleBytes);
		using BinaryReader binaryReader = new BinaryReader(memoryStream);
		short[] array = new short[sample.Metadata.SampleCount * 2];
		int num = (int)sample.Metadata.SampleCount / 64;
		for (int i = 0; i < channels; i++)
		{
			int num2 = i;
			for (int j = 0; j < num; j++)
			{
				int num3 = 36 * channels * j;
				int num4 = num3 + 4 * i;
				memoryStream.Seek(num4, SeekOrigin.Begin);
				int hist = binaryReader.ReadInt16();
				memoryStream.Seek(num4 + 2, SeekOrigin.Begin);
				int num5 = binaryReader.ReadByte();
				num5 = Utils.Clamp((short)num5, 0, 88);
				array[num2] = (short)hist;
				num2 += channels;
				for (int k = 1; k <= 64; k++)
				{
					int num6 = num3 + 8 + 4 * (i % 2) + 8 * ((k - 1) / 8) + (k - 1) % 8 / 2;
					if (channels == 0)
					{
						num6 = num3 + 4 + (k - 1) / 2;
					}
					int nibbleShift = ((((k - 1) & 1) != 0) ? 4 : 0);
					if (k < 64)
					{
						ExpandNibble(memoryStream, num6, nibbleShift, ref hist, ref num5);
						array[num2] = (short)hist;
						num2 += channels;
					}
				}
			}
		}
		return array;
	}

	private static short[] DecodeSamplesXboxIma(FmodSample sample)
	{
		using MemoryStream memoryStream = new MemoryStream(sample.SampleBytes);
		using BinaryReader binaryReader = new BinaryReader(memoryStream);
		int num = (int)sample.Metadata.SampleCount / 64;
		short[] array = new short[sample.Metadata.SampleCount];
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			int num3 = 36 * i;
			memoryStream.Seek(num3, SeekOrigin.Begin);
			int hist = binaryReader.ReadInt16();
			memoryStream.Seek(num3 + 2, SeekOrigin.Begin);
			int num4 = binaryReader.ReadByte();
			num4 = Utils.Clamp((short)num4, 0, 88);
			array[num2] = (short)hist;
			num2++;
			for (int j = 1; j <= 64; j++)
			{
				int num5 = num3 + 4 + (j - 1) / 2;
				int nibbleShift = ((((j - 1) & 1) != 0) ? 4 : 0);
				if (j < 64)
				{
					ExpandNibble(memoryStream, num5, nibbleShift, ref hist, ref num4);
					array[num2] = (short)hist;
					num2++;
				}
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
		short[] array = ((num == 1) ? DecodeSamplesXboxIma(sample) : DecodeSamplesFsbIma(sample));
		waveFileWriter.WriteSamples(array, 0, array.Length);
		return memoryStream.ToArray();
	}
}
