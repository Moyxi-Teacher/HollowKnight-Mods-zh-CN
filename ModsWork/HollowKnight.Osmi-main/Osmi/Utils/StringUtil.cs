using System.Globalization;

namespace Osmi.Utils;

[PublicAPI]
public static class StringUtil {
	public static bool EnclosedWith(this string self, string start, string end) =>
		self.StartsWith(start) && self.EndsWith(end);

	public static string StripStart(this string self, string val) =>
		self.StartsWith(val) ? self.Substring(val.Length) : self;

	public static string StripEnd(this string self, string val) =>
		self.EndsWith(val) ? self.Substring(0, self.Length - val.Length) : self;


	public static string Repeat(this string self, int count) {
		StringBuilder sb = new(self.Length * count);

		for (int i = 0; i < count; i++) {
			_ = sb.Append(self);
		}

		return sb.ToString();
	}

	public static string Concat(this string self, string other) =>
		self + other;


	public static string Slice(this string self, int startIndex) =>
		self.Slice(startIndex, self.Length);

	public static string Slice(this string self, int startIndex, int endIndex) {
		startIndex = startIndex >= 0 ? startIndex : startIndex + self.Length;
		endIndex = endIndex >= 0 ? endIndex : endIndex + self.Length;

		return self.Substring(startIndex, Math.Max(0, endIndex - startIndex));
	}


	public static string RemoveChar(this string self, char val) =>
		self.Replace(val.ToString(), "");

	public static string Remove(this string self, string val) =>
		self.Replace(val, "");


	public static string Join(this IEnumerable<string> self, string sep) {
		using IEnumerator<string> enumerator = self.GetEnumerator();

		_ = enumerator.MoveNext();
		StringBuilder builder = new(enumerator.Current);

		while (enumerator.MoveNext()) {
			_ = builder.Append(sep);
			_ = builder.Append(enumerator.Current);
		}

		return builder.ToString();
	}

	public static string Join(this IEnumerable<string> self, char sep) =>
		self.Join("" + sep);

	public static string Join(this IEnumerable<string> self) {
		using IEnumerator<string> enumerator = self.GetEnumerator();

		_ = enumerator.MoveNext();
		StringBuilder builder = new(enumerator.Current);

		while (enumerator.MoveNext()) {
			_ = builder.Append(enumerator.Current);
		}

		return builder.ToString();
	}


	public static bool IsNullOrEmpty(this string self) =>
		string.IsNullOrEmpty(self);

	public static bool IsNullOrWhiteSpace(this string self) =>
		string.IsNullOrWhiteSpace(self);


	public static string FormatForLog(object message) {
		if (message is string s) {
			return s;
		}

		if (message is IConvertible convertible) {
			return convertible.ToString(CultureInfo.InvariantCulture);
		}

		if (message is IFormattable formattable) {
			return formattable.ToString(null, CultureInfo.InvariantCulture);
		}

		s = message.ToString();
		return s == message.GetType().ToString() ? JsonConvert.SerializeObject(message) : s;
	}
}
