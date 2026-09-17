using System.Security.Cryptography;

namespace Osmi.Utils;

public class HashStream : Stream {
	private readonly HashAlgorithm hash;
	private readonly CryptoStream crypto;

	public byte[]? Hash { get; private set; }

	public string? HashString => Hash == null ? null : BitConverter.ToString(Hash).RemoveChar('-');

	public HashStream(HashAlgorithm hash) {
		this.hash = hash;
		crypto = new(Null, hash, CryptoStreamMode.Write);
	}

	private static HashStream ComputeHashInternal(Stream stream, HashAlgorithm hash) {
		if (!stream.CanRead) {
			throw new ArgumentException(nameof(stream), "Stream not readable");
		}

		HashStream hs = new(hash);
		stream.CopyTo(hs);
		stream.Dispose();
		hs.Dispose();
		return hs;
	}

	public static byte[] ComputeHash(Stream stream, HashAlgorithm hash) =>
		ComputeHashInternal(stream, hash).Hash!;

	public static string ComputeHashString(Stream stream, HashAlgorithm hash) =>
		ComputeHashInternal(stream, hash).HashString!;

	public byte[] Finalize() {
		Dispose();
		return Hash!;
	}

	public string FinalizeToString() {
		Dispose();
		return HashString!;
	}


	public override void Flush() { }
	public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
	public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
	public override void SetLength(long value) => throw new NotSupportedException();
	public override void Write(byte[] buffer, int offset, int count) =>
		crypto.Write(buffer, offset, count);

	public override bool CanRead => false;
	public override bool CanSeek => false;
	public override bool CanWrite => true;
	public override long Length => throw new NotSupportedException();
	public override long Position {
		get => throw new NotSupportedException();
		set => throw new NotSupportedException();
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			crypto.Dispose();
			Hash = hash.Hash!;
			hash.Dispose();
		}

		base.Dispose(disposing);
	}
}
