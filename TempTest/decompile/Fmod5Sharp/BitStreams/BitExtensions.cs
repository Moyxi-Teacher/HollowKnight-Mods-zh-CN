namespace BitStreams;

internal static class BitExtensions
{
	public static Bit GetBit(this byte n, int index)
	{
		return n >> index;
	}

	public static Bit GetBit(this sbyte n, int index)
	{
		return n >> index;
	}

	public static Bit GetBit(this short n, int index)
	{
		return n >> index;
	}

	public static Bit GetBit(this ushort n, int index)
	{
		return n >> index;
	}

	public static Bit GetBit(this int n, int index)
	{
		return n >> index;
	}

	public static Bit GetBit(this uint n, int index)
	{
		return (byte)(n >> index);
	}

	public static Bit GetBit(this long n, int index)
	{
		return (byte)(n >> index);
	}

	public static Bit GetBit(this ulong n, int index)
	{
		return (byte)(n >> index);
	}

	public static byte CircularShift(this byte n, int bits, bool leftShift)
	{
		n = ((!leftShift) ? ((byte)((n >> bits) | (n << 8 - bits))) : ((byte)((n << bits) | (n >> 8 - bits))));
		return n;
	}

	public static sbyte CircularShift(this sbyte n, int bits, bool leftShift)
	{
		n = ((!leftShift) ? ((sbyte)((n >> bits) | (n << 8 - bits))) : ((sbyte)((n << bits) | (n >> 8 - bits))));
		return n;
	}

	public static short CircularShift(this short n, int bits, bool leftShift)
	{
		n = ((!leftShift) ? ((short)((n >> bits) | (n << 16 - bits))) : ((short)((n << bits) | (n >> 16 - bits))));
		return n;
	}

	public static ushort CircularShift(this ushort n, int bits, bool leftShift)
	{
		n = ((!leftShift) ? ((ushort)((n >> bits) | (n << 16 - bits))) : ((ushort)((n << bits) | (n >> 16 - bits))));
		return n;
	}

	public static int CircularShift(this int n, int bits, bool leftShift)
	{
		n = ((!leftShift) ? ((n >> bits) | (n << 32 - bits)) : ((n << bits) | (n >> 32 - bits)));
		return n;
	}

	public static uint CircularShift(this uint n, int bits, bool leftShift)
	{
		n = ((!leftShift) ? ((n >> bits) | (n << 32 - bits)) : ((n << bits) | (n >> 32 - bits)));
		return n;
	}

	public static long CircularShift(this long n, int bits, bool leftShift)
	{
		n = ((!leftShift) ? ((n >> bits) | (n << 64 - bits)) : ((n << bits) | (n >> 64 - bits)));
		return n;
	}

	public static ulong CircularShift(this ulong n, int bits, bool leftShift)
	{
		n = ((!leftShift) ? ((n >> bits) | (n << 64 - bits)) : ((n << bits) | (n >> 64 - bits)));
		return n;
	}

	public static byte ReverseBits(this byte b)
	{
		return (byte)(((b & 1) << 7) + (((b >> 1) & 1) << 6) + (((b >> 2) & 1) << 5) + (((b >> 3) & 1) << 4) + (((b >> 4) & 1) << 3) + (((b >> 5) & 1) << 2) + (((b >> 6) & 1) << 1) + ((b >> 7) & 1));
	}
}
