namespace Osmi.Utils;

[PublicAPI]
public static class StreamUtil {
	public static string ReadToString(this Stream self) {
		StreamReader reader = new(self);
		string content = reader.ReadToEnd();

		reader.Close();
		self.Close();

		return content;
	}

	public static byte[] ReadToBytes(this Stream self) {
		if (self is not MemoryStream ms) {
			ms = new((int) Math.Min(self.Length, int.MaxValue));
			self.CopyTo(ms);
			self.Close();
		}

		byte[] content = ms.TryGetBuffer(out ArraySegment<byte> buffer)
			&& buffer.Offset == 0 && buffer.Count == buffer.Array.Length
				? buffer.Array
				: ms.ToArray();
		ms.Close();

		return content;
	}
}
