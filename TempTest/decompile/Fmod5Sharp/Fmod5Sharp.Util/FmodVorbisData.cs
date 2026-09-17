using System;
using System.Linq;
using System.Text.Json.Serialization;
using BitStreams;

namespace Fmod5Sharp.Util;

internal class FmodVorbisData
{
	private bool _initialized;

	[JsonPropertyName("headerBytes")]
	public byte[] HeaderBytes { get; set; }

	[JsonPropertyName("seekBit")]
	public int SeekBit { get; set; }

	[JsonIgnore]
	private byte[] BlockFlags { get; set; } = Array.Empty<byte>();

	[JsonConstructor]
	public FmodVorbisData(byte[] headerBytes, int seekBit)
	{
		HeaderBytes = headerBytes;
		SeekBit = seekBit;
	}

	internal void InitBlockFlags()
	{
		if (_initialized)
		{
			return;
		}
		_initialized = true;
		BitStream bitStream = new BitStream(HeaderBytes);
		if (bitStream.ReadByte() == 5 && !(bitStream.ReadString(6) != "vorbis"))
		{
			bitStream.Seek(SeekBit / 8, SeekBit % 8);
			int count = bitStream.ReadByte(6) + 1;
			BlockFlags = Enumerable.Range(0, count).Select((Func<int, byte>)delegate
			{
				byte result = bitStream.ReadBit();
				bitStream.ReadBits(16);
				bitStream.ReadBits(16);
				bitStream.ReadBits(8);
				return result;
			}).ToArray();
		}
	}

	public int GetPacketBlockSize(byte[] packetBytes)
	{
		BitStream bitStream = new BitStream(packetBytes);
		if ((bool)bitStream.ReadBit())
		{
			return 0;
		}
		int num = 0;
		if (BlockFlags.Length > 1)
		{
			num = bitStream.ReadByte(BlockFlags.Length - 1);
		}
		if (BlockFlags[num] == 1)
		{
			return 2048;
		}
		return 256;
	}
}
