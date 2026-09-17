using System;
using System.IO;
using Fmod5Sharp.FmodTypes;
using NAudio.Wave;

namespace Fmod5Sharp.CodecRebuilders;

public static class FmodPcmRebuilder
{
	public static byte[] Rebuild(FmodSample sample, FmodAudioType type)
	{
		int num = type switch
		{
			FmodAudioType.PCM8 => 1, 
			FmodAudioType.PCM16 => 2, 
			FmodAudioType.PCM32 => 4, 
			_ => throw new Exception($"FmodPcmRebuilder does not support encoding of type {type}"), 
		};
		int num2 = ((!sample.Metadata.IsStereo) ? 1 : 2);
		WaveFormat format = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, sample.Metadata.Frequency, num2, sample.Metadata.Frequency * num2 * num, num2 * num, num * 8);
		using MemoryStream memoryStream = new MemoryStream();
		using WaveFileWriter waveFileWriter = new WaveFileWriter(memoryStream, format);
		waveFileWriter.Write(sample.SampleBytes, 0, sample.SampleBytes.Length);
		return memoryStream.GetBuffer();
	}
}
