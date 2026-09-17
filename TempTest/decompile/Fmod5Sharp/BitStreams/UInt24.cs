using System;

namespace BitStreams;

[Serializable]
internal struct UInt24
{
	private byte b0;

	private byte b1;

	private byte b2;

	private UInt24(uint value)
	{
		b0 = (byte)(value & 0xFF);
		b1 = (byte)((value >> 8) & 0xFF);
		b2 = (byte)((value >> 16) & 0xFF);
	}

	public static implicit operator UInt24(uint value)
	{
		return new UInt24(value);
	}

	public static implicit operator uint(UInt24 i)
	{
		return (uint)(i.b0 | (i.b1 << 8) | (i.b2 << 16));
	}

	public Bit GetBit(int index)
	{
		return (byte)((uint)this >> index);
	}
}
