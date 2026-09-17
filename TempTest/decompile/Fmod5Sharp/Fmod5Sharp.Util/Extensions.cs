using System;
using System.IO;
using System.Text;

namespace Fmod5Sharp.Util;

internal static class Extensions
{
	internal static T ReadEndian<T>(this BinaryReader reader) where T : IBinaryReadable, new()
	{
		T result = new T();
		result.Read(reader);
		return result;
	}

	internal static long Position(this BinaryReader reader)
	{
		return reader.BaseStream.Position;
	}

	internal static string ReadString(this BinaryReader reader, int length, Encoding? encoding = null)
	{
		if (encoding == null)
		{
			encoding = Encoding.UTF8;
		}
		byte[] bytes = reader.ReadBytes(length);
		return encoding.GetString(bytes);
	}

	internal static ulong Bits(this uint raw, int lowestBit, int numBits)
	{
		return ((ulong)raw).Bits(lowestBit, numBits);
	}

	internal static ulong Bits(this ulong raw, int lowestBit, int numBits)
	{
		ulong num = 1uL;
		for (int i = 1; i < numBits; i++)
		{
			num = (num << 1) | 1;
		}
		num <<= lowestBit;
		return (raw & num) >> lowestBit;
	}

	internal static string ReadNullTerminatedString(this byte[] bytes, int startOffset)
	{
		int num = MemoryExtensions.AsSpan(bytes, startOffset).IndexOf((byte)0);
		if (num == -1)
		{
			throw new Exception("Could not find null terminator");
		}
		return Encoding.UTF8.GetString(bytes, startOffset, num);
	}
}
