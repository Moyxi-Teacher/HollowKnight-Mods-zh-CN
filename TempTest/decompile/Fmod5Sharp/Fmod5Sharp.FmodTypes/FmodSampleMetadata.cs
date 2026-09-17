using System;
using System.Collections.Generic;
using System.IO;
using Fmod5Sharp.Util;

namespace Fmod5Sharp.FmodTypes;

public class FmodSampleMetadata : IBinaryReadable
{
	internal bool HasAnyChunks;

	internal uint FrequencyId;

	internal ulong DataOffset;

	internal List<FmodSampleChunk> Chunks = new List<FmodSampleChunk>();

	internal int NumChannels;

	public bool IsStereo;

	public ulong SampleCount;

	public int Frequency
	{
		get
		{
			if (!FsbLoader.Frequencies.TryGetValue(FrequencyId, out var value))
			{
				return (int)FrequencyId;
			}
			return value;
		}
	}

	public uint Channels => (uint)NumChannels;

	void IBinaryReadable.Read(BinaryReader reader)
	{
		ulong num = reader.ReadUInt64();
		HasAnyChunks = (num & 1) == 1;
		FrequencyId = (uint)num.Bits(1, 4);
		int num2 = (int)num.Bits(5, 2);
		NumChannels = 1 << num2;
		if (NumChannels > 2)
		{
			throw new Exception("> 2 channels not supported");
		}
		IsStereo = NumChannels == 2;
		DataOffset = num.Bits(7, 27) * 32;
		SampleCount = num.Bits(34, 30);
	}
}
