using System.IO;

namespace Fmod5Sharp.ChunkData;

internal interface IChunkData
{
	void Read(BinaryReader reader, uint expectedSize);
}
