namespace Osmi.Utils;

public static partial class FuncUtil {
	public static Func<bool> Negate(this Func<bool> self) => () => !self.Invoke();

	public static Func<A, bool> Negate<A>(this Func<A, bool> self) =>
		(a) => !self.Invoke(a);

	public static Func<A, B, bool> Negate<A, B>(this Func<A, B, bool> self) =>
		(a, b) => !self.Invoke(a, b);

	public static Func<A, B, C, bool> Negate<A, B, C>(this Func<A, B, C, bool> self) =>
		(a, b, c) => !self.Invoke(a, b, c);

	public static Func<A, B, C, D, bool> Negate<A, B, C, D>(this Func<A, B, C, D, bool> self) =>
		(a, b, c, d) => !self.Invoke(a, b, c, d);

	public static Func<A, B, C, D, E, bool> Negate<A, B, C, D, E>(this Func<A, B, C, D, E, bool> self) =>
		(a, b, c, d, e) => !self.Invoke(a, b, c, d, e);

	public static Func<A, B, C, D, E, G, bool> Negate<A, B, C, D, E, G>(this Func<A, B, C, D, E, G, bool> self) =>
		(a, b, c, d, e, g) => !self.Invoke(a, b, c, d, e, g);
}
