namespace Osmi.Utils.Tap;

[PublicAPI]
public static class TapUtil {
	public static T Tap<T>(this T self, Action<T> f) {
		f(self);
		return self;
	}

	public static U Thru<T, U>(this T self, Func<T, U> f) =>
		f(self);

	public static T CondTap<T>(this T self, Func<T, bool> predicate, Action<T> f) {
		if (predicate(self)) {
			f(self);
		}

		return self;
	}

	public static void Discard<T>(this T self) {
	}



	public static Func<T, Action<T>, T> TapFunc<T>() =>
		(t, f) => t.Tap(f);

	public static Func<Action<T>, T> TapFunc<T>(this T self) =>
		TapFunc<T>().Bind1(self);

	public static Func<T, T> TapFunc<T>(Action<T> f) =>
		TapFunc<T>().Bind2(f);
}
