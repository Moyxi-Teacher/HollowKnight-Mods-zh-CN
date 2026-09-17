using System;
using System.IO;
using Fmod5Sharp.ChunkData;
using Fmod5Sharp.Util;

namespace Fmod5Sharp.FmodTypes;

internal class FmodSampleChunk : IBinaryReadable
{
	internal static FmodSampleMetadata? CurrentSample;

	public FmodSampleChunkType ChunkType;

	public uint ChunkSize;

	public bool MoreChunks;

	internal IChunkData ChunkData;

	void IBinaryReadable.Read(BinaryReader reader)
	{
		uint raw = reader.ReadUInt32();
		MoreChunks = raw.Bits(0, 1) == 1;
		ChunkSize = (uint)raw.Bits(1, 24);
		ChunkType = (FmodSampleChunkType)raw.Bits(25, 7);
		ChunkData = ChunkType switch
		{
			FmodSampleChunkType.VORBISDATA => new VorbisChunkData(), 
			FmodSampleChunkType.FREQUENCY => new FrequencyChunkData(), 
			FmodSampleChunkType.CHANNELS => new ChannelChunkData(), 
			FmodSampleChunkType.LOOP => new LoopChunkData(), 
			FmodSampleChunkType.DSPCOEFF => new DspCoefficientsBlockData(CurrentSample), 
			_ => new UnknownChunkData(), 
		};
		long num = reader.Position();
		ChunkData.Read(reader, ChunkSize);
		long num2 = reader.Position() - num;
		if (num2 != ChunkSize)
		{
			throw new Exception($"Expected fmod sample chunk to read {ChunkSize} bytes, but it only read {num2}");
		}
	}
}
