using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Fmod5Sharp.ChunkData;
using Fmod5Sharp.FmodTypes;
using Fmod5Sharp.Util;
using OggVorbisEncoder;

namespace Fmod5Sharp.CodecRebuilders;

public static class FmodVorbisRebuilder
{
	private static Dictionary<uint, FmodVorbisData>? headers;

	private static void LoadVorbisHeaders()
	{
		using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Fmod5Sharp.Util.vorbis_headers_converted.json") ?? throw new Exception("Embedded resources for vorbis header data not found, has the assembly been tampered with?");
		using StreamReader streamReader = new StreamReader(stream);
		headers = JsonSerializer.Deserialize(streamReader.ReadToEnd(), Fmod5SharpJsonContext.Default.DictionaryUInt32FmodVorbisData);
	}

	public static byte[] RebuildOggFile(FmodSample sample)
	{
		uint crc = ((VorbisChunkData)(sample.Metadata.Chunks.FirstOrDefault((FmodSampleChunk f) => f.ChunkType == FmodSampleChunkType.VORBISDATA) ?? throw new Exception("Rebuilding Vorbis data requires a VORBISDATA chunk, which wasn't found")).ChunkData).Crc32;
		if (headers == null)
		{
			LoadVorbisHeaders();
		}
		FmodVorbisData fmodVorbisData = headers[crc];
		fmodVorbisData.InitBlockFlags();
		OggPacket packet = BuildInfoPacket((byte)sample.Metadata.Channels, sample.Metadata.Frequency);
		OggPacket packet2 = BuildCommentPacket("Fmod5Sharp (Samboy063)");
		OggPacket packet3 = new OggPacket(fmodVorbisData.HeaderBytes, endOfStream: false, 0, 2);
		OggStream oggStream = new OggStream(1);
		using MemoryStream memoryStream = new MemoryStream();
		oggStream.PacketIn(packet);
		oggStream.PacketIn(packet2);
		oggStream.PacketIn(packet3);
		oggStream.FlushAndCopyTo(memoryStream, force: true);
		CopySampleData(fmodVorbisData, sample.SampleBytes, oggStream, memoryStream);
		return memoryStream.ToArray();
	}

	private static void FlushAndCopyTo(this OggStream stream, Stream other, bool force = false)
	{
		OggPage page;
		while (stream.PageOut(out page, force))
		{
			other.Write(page.Header, 0, page.Header.Length);
			other.Write(page.Body, 0, page.Body.Length);
		}
	}

	private static OggPacket BuildInfoPacket(byte channels, int frequency)
	{
		using MemoryStream memoryStream = new MemoryStream(30);
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write((byte)1);
		binaryWriter.Write(System.Text.Encoding.UTF8.GetBytes("vorbis"));
		binaryWriter.Write(0);
		binaryWriter.Write(channels);
		binaryWriter.Write(frequency);
		binaryWriter.Write(0);
		binaryWriter.Write(0);
		binaryWriter.Write(0);
		binaryWriter.Write((byte)184);
		binaryWriter.Write((byte)1);
		return new OggPacket(memoryStream.ToArray(), endOfStream: false, 0, 0);
	}

	private static OggPacket BuildCommentPacket(string vendor)
	{
		using MemoryStream input = new MemoryStream();
		using (new BinaryReader(input))
		{
			using MemoryStream memoryStream = new MemoryStream();
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Seek(0, SeekOrigin.Begin);
			binaryWriter.Write((byte)3);
			binaryWriter.Write(System.Text.Encoding.UTF8.GetBytes("vorbis"));
			binaryWriter.Write(vendor.Length);
			binaryWriter.Write(System.Text.Encoding.UTF8.GetBytes(vendor));
			binaryWriter.Write(0);
			binaryWriter.Write((byte)1);
			return new OggPacket(memoryStream.ToArray(), endOfStream: false, 0, 1);
		}
	}

	private static void CopySampleData(FmodVorbisData vorbisData, byte[] sampleBytes, OggStream oggStream, Stream outputStream)
	{
		using MemoryStream input = new MemoryStream(sampleBytes);
		using BinaryReader inputReader = new BinaryReader(input);
		ReadSamplePackets(inputReader, out List<int> packetLengths, out List<byte[]> packets);
		int num = 1;
		int num2 = 0;
		int num3 = 0;
		int num4 = packetLengths.Count - 1;
		for (int i = 0; i < packets.Count; i++)
		{
			bool flag = i == num4;
			byte[] array = packets[i];
			num++;
			int num5 = ((packetLengths[i] != 0) ? vorbisData.GetPacketBlockSize(array) : 0);
			num2 = ((num3 != 0) ? (num2 + (num5 + num3) / 4) : 0);
			num3 = num5;
			oggStream.PacketIn(new OggPacket(array, flag, num2, num));
			oggStream.FlushAndCopyTo(outputStream, flag);
		}
	}

	private static void ReadSamplePackets(BinaryReader inputReader, out List<int> packetLengths, out List<byte[]> packets)
	{
		packetLengths = new List<int>();
		packets = new List<byte[]>();
		while (inputReader.BaseStream.Position + 2 < inputReader.BaseStream.Length)
		{
			ushort num = inputReader.ReadUInt16();
			if (num != 0)
			{
				packetLengths.Add(num);
				packets.Add(inputReader.ReadBytes(num));
				continue;
			}
			break;
		}
	}
}
