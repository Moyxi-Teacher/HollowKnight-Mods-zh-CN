using System;

namespace BitStreams;

[Serializable]
internal struct Int48
{
	private byte b0;

	private byte b1;

	private byte b2;

	private byte b3;

	private byte b4;

	private byte b5;

	private Bit sign;

	private Int48(long value)
	{
		b0 = (byte)(value & 0xFF);
		b1 = (byte)((value >> 8) & 0xFF);
		b2 = (byte)((value >> 16) & 0xFF);
		b3 = (byte)((value >> 24) & 0xFF);
		b4 = (byte)((value >> 32) & 0xFF);
		b5 = (byte)((value >> 40) & 0x7F);
		sign = (byte)((value >> 47) & 1);
	}

	public static implicit operator Int48(long value)
	{
		return new Int48(value);
	}

	public static implicit operator long(Int48 i)
	{
		long num = (long)(i.b0 + (i.b1 << 8) + (i.b2 << 16)) + (long)((ulong)i.b3 << 24) + (long)((ulong)i.b4 << 32) + (long)((ulong)i.b5 << 40);
		return -((long)(int)i.sign << 47) + num;
	}

	public Bit GetBit(int index)
	{
		return (byte)((long)this >> index);
	}
}
