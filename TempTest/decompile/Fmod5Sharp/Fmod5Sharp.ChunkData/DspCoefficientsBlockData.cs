using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fmod5Sharp.FmodTypes;

namespace Fmod5Sharp.ChunkData;

public class DspCoefficientsBlockData : IChunkData
{
	public List<short>[] ChannelData;

	private readonly FmodSampleMetadata _sampleMetadata;

	public DspCoefficientsBlockData(FmodSampleMetadata sampleMetadata)
	{
		_sampleMetadata = sampleMetadata;
		ChannelData = new List<short>[_sampleMetadata.Channels];
		for (int i = 0; i < _sampleMetadata.Channels; i++)
		{
			ChannelData[i] = new List<short>();
		}
	}

	public void Read(BinaryReader reader, uint expectedSize)
	{
		for (int i = 0; i < _sampleMetadata.Channels; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				ChannelData[i].Add(BitConverter.ToInt16(Enumerable.Reverse(reader.ReadBytes(2)).ToArray(), 0));
			}
			reader.ReadInt64();
			reader.ReadInt32();
			reader.ReadInt16();
		}
	}
}
