using System;
using NAudio.Wave;

namespace HapticsBridge;

internal sealed class ChannelMapWaveProvider : IWaveProvider
{
	private readonly ISampleProvider source;

	private readonly int leftCh;

	private readonly int rightCh;

	private readonly WaveFormat format;

	private float[] srcBuf = new float[0];

	public WaveFormat WaveFormat => format;

	public ChannelMapWaveProvider(ISampleProvider stereoSource, WaveFormat deviceMixFormat, int leftCh, int rightCh)
	{
		source = stereoSource;
		format = deviceMixFormat;
		this.leftCh = leftCh;
		this.rightCh = rightCh;
	}

	public int Read(byte[] buffer, int offset, int count)
	{
		int num = count / format.BlockAlign;
		int num2 = num * 2;
		if (srcBuf.Length < num2)
		{
			srcBuf = new float[num2];
		}
		int num3 = source.Read(srcBuf, 0, num2) / 2;
		Array.Clear(buffer, offset, num * format.BlockAlign);
		for (int i = 0; i < num3; i++)
		{
			int num4 = offset + i * format.BlockAlign;
			WriteFloat(buffer, num4 + leftCh * 4, srcBuf[i * 2]);
			WriteFloat(buffer, num4 + rightCh * 4, srcBuf[i * 2 + 1]);
		}
		return num * format.BlockAlign;
	}

	private static void WriteFloat(byte[] buffer, int index, float value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		buffer[index] = bytes[0];
		buffer[index + 1] = bytes[1];
		buffer[index + 2] = bytes[2];
		buffer[index + 3] = bytes[3];
	}
}
