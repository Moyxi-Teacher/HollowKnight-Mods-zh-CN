using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BitStreams;

internal class BitStream
{
	private Stream stream;

	private Encoding encoding;

	private long offset { get; set; }

	private int bit { get; set; }

	private bool MSB { get; set; }

	public bool AutoIncreaseStream { get; set; }

	public long Length => stream.Length;

	public long BitPosition => bit;

	private bool ValidPosition => offset < Length;

	public bool this[long offset, int bit]
	{
		get
		{
			Seek(offset, bit);
			return ValidPosition;
		}
		private set
		{
		}
	}

	public BitStream(Stream stream, bool MSB = false)
	{
		this.stream = new MemoryStream();
		stream.CopyTo(this.stream);
		this.MSB = MSB;
		offset = 0L;
		bit = 0;
		encoding = Encoding.UTF8;
		AutoIncreaseStream = false;
	}

	public BitStream(Stream stream, Encoding encoding, bool MSB = false)
	{
		this.stream = new MemoryStream();
		stream.CopyTo(this.stream);
		this.MSB = MSB;
		offset = 0L;
		bit = 0;
		this.encoding = encoding;
		AutoIncreaseStream = false;
	}

	public BitStream(byte[] buffer, bool MSB = false)
	{
		stream = new MemoryStream();
		new MemoryStream(buffer).CopyTo(stream);
		this.MSB = MSB;
		offset = 0L;
		bit = 0;
		encoding = Encoding.UTF8;
		AutoIncreaseStream = false;
	}

	public BitStream(byte[] buffer, Encoding encoding, bool MSB = false)
	{
		stream = new MemoryStream();
		new MemoryStream(buffer).CopyTo(stream);
		this.MSB = MSB;
		offset = 0L;
		bit = 0;
		this.encoding = encoding;
		AutoIncreaseStream = false;
	}

	public static BitStream Create(byte[] buffer, bool MSB = false)
	{
		return new BitStream(buffer, MSB);
	}

	public static BitStream Create(byte[] buffer, Encoding encoding, bool MSB = false)
	{
		return new BitStream(buffer, encoding, MSB);
	}

	public static BitStream CreateFromFile(string path, Encoding? encoding = null)
	{
		if (!File.Exists(path))
		{
			throw new IOException("File doesn't exists!");
		}
		if (File.GetAttributes(path) == FileAttributes.Directory)
		{
			throw new IOException("Path is a directory!");
		}
		if (encoding == null)
		{
			encoding = Encoding.UTF8;
		}
		return new BitStream(File.ReadAllBytes(path), encoding);
	}

	public void Seek(long offset, int bit)
	{
		if (offset > Length)
		{
			this.offset = Length;
		}
		else if (offset >= 0)
		{
			this.offset = offset;
		}
		else
		{
			offset = 0L;
		}
		if (bit >= 8)
		{
			int num = bit / 8;
			this.offset += num;
			this.bit = bit % 8;
		}
		else
		{
			this.bit = bit;
		}
		stream.Seek(offset, SeekOrigin.Begin);
	}

	public void AdvanceBit()
	{
		bit = (bit + 1) % 8;
		if (bit == 0)
		{
			offset++;
		}
	}

	public void ReturnBit()
	{
		bit = ((bit - 1 == -1) ? 7 : (bit - 1));
		if (bit == 7)
		{
			offset--;
		}
		if (offset < 0)
		{
			offset = 0L;
		}
	}

	public Stream GetStream()
	{
		return stream;
	}

	public byte[] GetStreamData()
	{
		stream.Seek(0L, SeekOrigin.Begin);
		MemoryStream memoryStream = new MemoryStream();
		stream.CopyTo(memoryStream);
		Seek(offset, bit);
		return memoryStream.ToArray();
	}

	public Encoding GetEncoding()
	{
		return encoding;
	}

	public void SetEncoding(Encoding encoding)
	{
		this.encoding = encoding;
	}

	public bool ChangeLength(long length)
	{
		if (stream.CanSeek && stream.CanWrite)
		{
			stream.SetLength(length);
			return true;
		}
		return false;
	}

	public void CutStream(long offset, long length)
	{
		byte[] streamData = GetStreamData();
		byte[] array = new byte[length];
		Array.Copy(streamData, offset, array, 0L, length);
		stream = new MemoryStream();
		MemoryStream memoryStream = new MemoryStream(array);
		stream = new MemoryStream();
		memoryStream.CopyTo(stream);
		this.offset = 0L;
		bit = 0;
	}

	public void CopyStreamTo(Stream stream)
	{
		Seek(0L, 0);
		stream.SetLength(this.stream.Length);
		this.stream.CopyTo(stream);
	}

	public void CopyStreamTo(BitStream stream)
	{
		Seek(0L, 0);
		stream.ChangeLength(this.stream.Length);
		this.stream.CopyTo(stream.stream);
		stream.Seek(0L, 0);
	}

	public void SaveStreamAsFile(string filename)
	{
		File.WriteAllBytes(filename, GetStreamData());
	}

	public MemoryStream CloneAsMemoryStream()
	{
		return new MemoryStream(GetStreamData());
	}

	public BufferedStream CloneAsBufferedStream()
	{
		BufferedStream bufferedStream = new BufferedStream(stream);
		new StreamWriter(bufferedStream).Write(GetStreamData());
		bufferedStream.Seek(0L, SeekOrigin.Begin);
		return bufferedStream;
	}

	private bool ValidPositionWhen(int bits)
	{
		long num = offset;
		if ((bit + 1) % 8 == 0)
		{
			num++;
		}
		return num < Length;
	}

	public Bit ReadBit()
	{
		if (!ValidPosition)
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		stream.Seek(offset, SeekOrigin.Begin);
		byte b = (MSB ? ((byte)((stream.ReadByte() >> 7 - bit) & 1)) : ((byte)((stream.ReadByte() >> bit) & 1)));
		AdvanceBit();
		stream.Seek(offset, SeekOrigin.Begin);
		return b;
	}

	public Bit[] ReadBits(int length)
	{
		Bit[] array = new Bit[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = ReadBit();
		}
		return array;
	}

	public void WriteBit(Bit data)
	{
		stream.Seek(offset, SeekOrigin.Begin);
		byte b = (byte)stream.ReadByte();
		stream.Seek(offset, SeekOrigin.Begin);
		if (!MSB)
		{
			b &= (byte)(~(1 << bit));
			b |= (byte)((int)data << bit);
		}
		else
		{
			b &= (byte)(~(1 << 7 - bit));
			b |= (byte)((int)data << 7 - bit);
		}
		if (ValidPosition)
		{
			stream.WriteByte(b);
		}
		else
		{
			if (!AutoIncreaseStream)
			{
				throw new IOException("Cannot write in an offset bigger than the length of the stream");
			}
			if (!ChangeLength(Length + (offset - Length) + 1))
			{
				throw new IOException("Cannot write in an offset bigger than the length of the stream");
			}
			stream.WriteByte(b);
		}
		AdvanceBit();
		stream.Seek(offset, SeekOrigin.Begin);
	}

	public void WriteBits(ICollection<Bit> bits)
	{
		foreach (Bit bit in bits)
		{
			WriteBit(bit);
		}
	}

	public void WriteBits(ICollection<Bit> bits, int length)
	{
		Bit[] array = new Bit[bits.Count];
		bits.CopyTo(array, 0);
		for (int i = 0; i < length; i++)
		{
			WriteBit(array[i]);
		}
	}

	public void WriteBits(Bit[] bits, int offset, int length)
	{
		for (int i = offset; i < length; i++)
		{
			WriteBit(bits[i]);
		}
	}

	public byte[] ReadBytes(long length, bool isBytes = false)
	{
		if (isBytes)
		{
			length *= 8;
		}
		List<byte> list = new List<byte>();
		long num = 0L;
		while (num < length)
		{
			byte b = 0;
			for (int i = 0; i < 8; i++)
			{
				if (num >= length)
				{
					break;
				}
				b = (MSB ? ((byte)(b | (byte)((int)ReadBit() << 7 - i))) : ((byte)(b | (byte)((int)ReadBit() << i))));
				num++;
			}
			list.Add(b);
		}
		return list.ToArray();
	}

	public byte ReadByte()
	{
		return ReadBytes(8L)[0];
	}

	public byte ReadByte(int bits)
	{
		if (bits < 0)
		{
			bits = 0;
		}
		if (bits > 8)
		{
			bits = 8;
		}
		return ReadBytes(bits)[0];
	}

	public sbyte ReadSByte()
	{
		return (sbyte)ReadBytes(8L)[0];
	}

	public sbyte ReadSByte(int bits)
	{
		if (bits < 0)
		{
			bits = 0;
		}
		if (bits > 8)
		{
			bits = 8;
		}
		return (sbyte)ReadBytes(bits)[0];
	}

	public bool ReadBool()
	{
		if (ReadBytes(8L)[0] != 0)
		{
			return true;
		}
		return false;
	}

	public char ReadChar()
	{
		return encoding.GetChars(ReadBytes(encoding.GetMaxByteCount(1) * 8))[0];
	}

	public string ReadString(int length)
	{
		int num = encoding.GetByteCount(" ") * 8;
		return encoding.GetString(ReadBytes(num * length));
	}

	public short ReadInt16()
	{
		return BitConverter.ToInt16(ReadBytes(16L), 0);
	}

	public Int24 ReadInt24()
	{
		byte[] array = ReadBytes(24L);
		Array.Resize(ref array, 4);
		return BitConverter.ToInt32(array, 0);
	}

	public int ReadInt32()
	{
		return BitConverter.ToInt32(ReadBytes(32L), 0);
	}

	public Int48 ReadInt48()
	{
		byte[] array = ReadBytes(48L);
		Array.Resize(ref array, 8);
		return BitConverter.ToInt64(array, 0);
	}

	public long ReadInt64()
	{
		return BitConverter.ToInt64(ReadBytes(64L), 0);
	}

	public ushort ReadUInt16()
	{
		return BitConverter.ToUInt16(ReadBytes(16L), 0);
	}

	public UInt24 ReadUInt24()
	{
		byte[] array = ReadBytes(24L);
		Array.Resize(ref array, 4);
		return BitConverter.ToUInt32(array, 0);
	}

	public uint ReadUInt32()
	{
		return BitConverter.ToUInt32(ReadBytes(32L), 0);
	}

	public UInt48 ReadUInt48()
	{
		byte[] array = ReadBytes(48L);
		Array.Resize(ref array, 8);
		return BitConverter.ToUInt64(array, 0);
	}

	public ulong ReadUInt64()
	{
		return BitConverter.ToUInt64(ReadBytes(64L), 0);
	}

	public void WriteBytes(byte[] data, long length, bool isBytes = false)
	{
		if (isBytes)
		{
			length *= 8;
		}
		int num = 0;
		long num2 = 0L;
		while (num2 < length)
		{
			byte b = 0;
			for (int i = 0; i < 8; i++)
			{
				if (num2 >= length)
				{
					break;
				}
				b = (MSB ? ((byte)((data[num] >> 7 - i) & 1)) : ((byte)((data[num] >> i) & 1)));
				WriteBit(b);
				num2++;
			}
			num++;
		}
	}

	public void WriteByte(byte value)
	{
		WriteBytes(new byte[1] { value }, 8L);
	}

	public void WriteByte(byte value, int bits)
	{
		if (bits < 0)
		{
			bits = 0;
		}
		if (bits > 8)
		{
			bits = 8;
		}
		WriteBytes(new byte[1] { value }, bits);
	}

	public void WriteSByte(sbyte value)
	{
		WriteBytes(new byte[1] { (byte)value }, 8L);
	}

	public void WriteSByte(sbyte value, int bits)
	{
		if (bits < 0)
		{
			bits = 0;
		}
		if (bits > 8)
		{
			bits = 8;
		}
		WriteBytes(new byte[1] { (byte)value }, bits);
	}

	public void WriteBool(bool value)
	{
		WriteBytes(new byte[1] { (byte)(value ? 1 : 0) }, 8L);
	}

	public void WriteChar(char value)
	{
		byte[] bytes = encoding.GetBytes(new char[1] { value }, 0, 1);
		WriteBytes(bytes, bytes.Length * 8);
	}

	public void WriteString(string value)
	{
		byte[] bytes = encoding.GetBytes(value);
		WriteBytes(bytes, bytes.Length * 8);
	}

	public void WriteInt16(short value)
	{
		WriteBytes(BitConverter.GetBytes(value), 16L);
	}

	public void WriteInt24(Int24 value)
	{
		WriteBytes(BitConverter.GetBytes(value), 24L);
	}

	public void WriteInt32(int value)
	{
		WriteBytes(BitConverter.GetBytes(value), 32L);
	}

	public void WriteInt48(Int48 value)
	{
		WriteBytes(BitConverter.GetBytes(value), 48L);
	}

	public void WriteInt64(long value)
	{
		WriteBytes(BitConverter.GetBytes(value), 64L);
	}

	public void WriteUInt16(ushort value)
	{
		WriteBytes(BitConverter.GetBytes(value), 16L);
	}

	public void WriteUInt24(UInt24 value)
	{
		WriteBytes(BitConverter.GetBytes(value), 24L);
	}

	public void WriteUInt32(uint value)
	{
		WriteBytes(BitConverter.GetBytes(value), 32L);
	}

	public void WriteUInt48(UInt48 value)
	{
		WriteBytes(BitConverter.GetBytes(value), 48L);
	}

	public void WriteUInt64(ulong value)
	{
		WriteBytes(BitConverter.GetBytes(value), 64L);
	}

	public void bitwiseShift(int bits, bool leftShift)
	{
		if (!ValidPositionWhen(8))
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		Seek(offset, 0);
		if (bits != 0 && bits <= 7)
		{
			byte b = (byte)stream.ReadByte();
			b = ((!leftShift) ? ((byte)(b >> bits)) : ((byte)(b << bits)));
			Seek(offset, 0);
			stream.WriteByte(b);
		}
		bit = 0;
		offset++;
	}

	public void bitwiseShiftOnBit(int bits, bool leftShift)
	{
		if (!ValidPositionWhen(8))
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		Seek(offset, bit);
		if (bits != 0 && bits <= 7)
		{
			byte b = ReadByte();
			b = ((!leftShift) ? ((byte)(b >> bits)) : ((byte)(b << bits)));
			offset--;
			Seek(offset, bit);
			WriteByte(b);
		}
		offset++;
	}

	public void circularShift(int bits, bool leftShift)
	{
		if (!ValidPositionWhen(8))
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		Seek(offset, 0);
		if (bits != 0 && bits <= 7)
		{
			byte b = (byte)stream.ReadByte();
			b = ((!leftShift) ? ((byte)((b >> bits) | (b << 8 - bits))) : ((byte)((b << bits) | (b >> 8 - bits))));
			Seek(offset, 0);
			stream.WriteByte(b);
		}
		bit = 0;
		offset++;
	}

	public void circularShiftOnBit(int bits, bool leftShift)
	{
		if (!ValidPositionWhen(8))
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		Seek(offset, bit);
		if (bits != 0 && bits <= 7)
		{
			byte b = ReadByte();
			b = ((!leftShift) ? ((byte)((b >> bits) | (b << 8 - bits))) : ((byte)((b << bits) | (b >> 8 - bits))));
			offset--;
			Seek(offset, bit);
			WriteByte(b);
		}
		offset++;
	}

	public void And(byte x)
	{
		if (!ValidPositionWhen(8))
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		Seek(offset, bit);
		byte b = ReadByte();
		offset--;
		Seek(offset, bit);
		WriteByte((byte)(b & x));
	}

	public void Or(byte x)
	{
		if (!ValidPositionWhen(8))
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		Seek(offset, bit);
		byte b = ReadByte();
		offset--;
		Seek(offset, bit);
		WriteByte((byte)(b | x));
	}

	public void Xor(byte x)
	{
		if (!ValidPositionWhen(8))
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		Seek(offset, bit);
		byte b = ReadByte();
		offset--;
		Seek(offset, bit);
		WriteByte((byte)(b ^ x));
	}

	public void Not()
	{
		if (!ValidPositionWhen(8))
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		Seek(offset, bit);
		byte b = ReadByte();
		offset--;
		Seek(offset, bit);
		WriteByte((byte)(~b));
	}

	public void BitAnd(Bit x)
	{
		if (!ValidPosition)
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		Seek(offset, this.bit);
		Bit bit = ReadBit();
		ReturnBit();
		WriteBit(x & bit);
	}

	public void BitOr(Bit x)
	{
		if (!ValidPosition)
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		Seek(offset, this.bit);
		Bit bit = ReadBit();
		ReturnBit();
		WriteBit(x | bit);
	}

	public void BitXor(Bit x)
	{
		if (!ValidPosition)
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		Seek(offset, this.bit);
		Bit bit = ReadBit();
		ReturnBit();
		WriteBit(x ^ bit);
	}

	public void BitNot()
	{
		if (!ValidPosition)
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		Seek(offset, this.bit);
		Bit bit = ReadBit();
		ReturnBit();
		WriteBit(~bit);
	}

	public void ReverseBits()
	{
		if (!ValidPosition)
		{
			throw new IOException("Cannot read in an offset bigger than the length of the stream");
		}
		Seek(offset, 0);
		byte b = ReadByte();
		offset--;
		Seek(offset, 0);
		WriteByte(b.ReverseBits());
	}
}
