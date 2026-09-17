using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fmod5Sharp.ChunkData;
using Fmod5Sharp.Util;

namespace Fmod5Sharp.FmodTypes;

public class FmodAudioHeader
{
	private static readonly object ChunkReadingLock = new object();

	internal readonly bool IsValid;

	public readonly FmodAudioType AudioType;

	public readonly uint Version;

	public readonly uint NumSamples;

	internal readonly uint SizeOfThisHeader;

	internal readonly uint SizeOfSampleHeaders;

	internal readonly uint SizeOfNameTable;

	internal readonly uint SizeOfData;

	internal readonly List<FmodSampleMetadata> Samples = new List<FmodSampleMetadata>();

	public FmodAudioHeader(BinaryReader reader)
	{
		if (reader.ReadString(4) != "FSB5")
		{
			IsValid = false;
			return;
		}
		Version = reader.ReadUInt32();
		NumSamples = reader.ReadUInt32();
		SizeOfSampleHeaders = reader.ReadUInt32();
		SizeOfNameTable = reader.ReadUInt32();
		SizeOfData = reader.ReadUInt32();
		AudioType = (FmodAudioType)reader.ReadUInt32();
		reader.ReadUInt32();
		if (Version == 0)
		{
			SizeOfThisHeader = 64u;
			reader.ReadUInt32();
		}
		else
		{
			SizeOfThisHeader = 60u;
		}
		reader.ReadUInt32();
		reader.ReadUInt64();
		reader.ReadUInt64();
		reader.ReadUInt64();
		reader.Position();
		for (int i = 0; i < NumSamples; i++)
		{
			FmodSampleMetadata fmodSampleMetadata = reader.ReadEndian<FmodSampleMetadata>();
			if (!fmodSampleMetadata.HasAnyChunks)
			{
				Samples.Add(fmodSampleMetadata);
				continue;
			}
			lock (ChunkReadingLock)
			{
				List<FmodSampleChunk> list = new List<FmodSampleChunk>();
				FmodSampleChunk.CurrentSample = fmodSampleMetadata;
				FmodSampleChunk fmodSampleChunk;
				do
				{
					fmodSampleChunk = reader.ReadEndian<FmodSampleChunk>();
					list.Add(fmodSampleChunk);
				}
				while (fmodSampleChunk.MoreChunks);
				FmodSampleChunk.CurrentSample = null;
				FmodSampleChunk fmodSampleChunk2 = list.FirstOrDefault((FmodSampleChunk c) => c.ChunkType == FmodSampleChunkType.FREQUENCY);
				if (fmodSampleChunk2 != null && fmodSampleChunk2.ChunkData is FrequencyChunkData frequencyChunkData)
				{
					fmodSampleMetadata.FrequencyId = frequencyChunkData.ActualFrequencyId;
				}
				fmodSampleMetadata.Chunks = list;
				Samples.Add(fmodSampleMetadata);
			}
		}
		IsValid = true;
	}
}
