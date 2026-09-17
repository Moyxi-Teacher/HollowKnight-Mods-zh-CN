namespace Osmi.Utils;

[PublicAPI]
public static partial class FuncUtil {
	public static T Combine<T>(T a, T b) where T : Delegate =>
		(T) Delegate.Combine(a, b);

	public static T Combine<T>(params T[] delegates) where T : Delegate =>
		(T) Delegate.Combine(delegates);
}
