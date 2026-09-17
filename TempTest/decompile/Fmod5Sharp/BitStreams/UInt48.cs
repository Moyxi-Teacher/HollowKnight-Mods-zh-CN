using System;

namespace BitStreams;

[Serializable]
internal struct UInt48
{
	private byte b0;

	private byte b1;

	private byte b2;

	private byte b3;

	private byte b4;

	private byte b5;

	private UInt48(ulong value)
	{
		b0 = (byte)(value & 0xFF);
		b1 = (byte)((value >> 8) & 0xFF);
		b2 = (byte)((value >> 16) & 0xFF);
		b3 = (byte)((value >> 24) & 0xFF);
		b4 = (byte)((value >> 32) & 0xFF);
		b5 = (byte)((value >> 40) & 0xFF);
	}

	public static implicit operator UInt48(ulong value)
	{
		return new UInt48(value);
	}

	public static implicit operator ulong(UInt48 i)
	{
		return i.b0 + ((ulong)i.b1 << 8) + ((ulong)i.b2 << 16) + ((ulong)i.b3 << 24) + ((ulong)i.b4 << 32) + ((ulong)i.b5 << 40);
	}

	public Bit GetBit(int index)
	{
		return (byte)((ulong)this >> index);
	}
}
