using System;

namespace BitStreams;

[Serializable]
internal struct Int24
{
	private byte b0;

	private byte b1;

	private byte b2;

	private Bit sign;

	private Int24(int value)
	{
		b0 = (byte)(value & 0xFF);
		b1 = (byte)((value >> 8) & 0xFF);
		b2 = (byte)((value >> 16) & 0x7F);
		sign = (byte)((value >> 23) & 1);
	}

	public static implicit operator Int24(int value)
	{
		return new Int24(value);
	}

	public static implicit operator int(Int24 i)
	{
		int num = i.b0 | (i.b1 << 8) | (i.b2 << 16);
		return -((int)i.sign << 23) + num;
	}

	public Bit GetBit(int index)
	{
		return (int)this >> index;
	}
}
