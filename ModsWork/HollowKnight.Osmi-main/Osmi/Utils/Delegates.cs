namespace Osmi.Utils;

[PublicAPI]
public static class Delegates {
	public static Func<T, T> Identity<T>() => Funcs.Identity;

	public static Action<T> Drop<T>() => Funcs.Drop;

	public static Action NoOp() => Funcs.NoOp;
	public static Action<A> NoOp<A>() => (_) => { };
	public static Action<A, B> NoOp<A, B>() => (_, _) => { };
	public static Action<A, B, C> NoOp<A, B, C>() => (_, _, _) => { };

	public static Func<A, bool> Pass<A>() => (_) => true;
	public static Func<A, B, bool> Pass<A, B>() => (_, _) => true;
	public static Func<A, B, C, bool> Pass<A, B, C>() => (_, _, _) => true;

	public static Func<A, bool> Block<A>() => (_) => false;
	public static Func<A, B, bool> Block<A, B>() => (_, _) => false;
	public static Func<A, B, C, bool> Block<A, B, C>() => (_, _, _) => false;

	public static Func<T?> Default<T>() => Funcs.Default<T>;

	public static Func<T> New<T>() where T : new() => Funcs.New<T>;

	public static Func<T> Constant<T>(T val) => Funcs.Constant(val);
	public static Func<U> Constant<T, U>(T val) where T : U => Funcs.Constant<T, U>(val);
}
