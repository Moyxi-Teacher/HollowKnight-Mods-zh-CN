namespace Osmi.Utils;

public static partial class FuncUtil {
	public static Func<T> Bind<A, T>(this Func<A, T> self, A a) =>
		() => self(a);

	public static Func<B, T> Bind1<A, B, T>(this Func<A, B, T> self, A a) =>
		(b) => self(a, b);
	public static Func<A, T> Bind2<A, B, T>(this Func<A, B, T> self, B b) =>
		(a) => self(a, b);

	public static Func<B, C, T> Bind1<A, B, C, T>(this Func<A, B, C, T> self, A a) =>
		(b, c) => self(a, b, c);
	public static Func<A, C, T> Bind2<A, B, C, T>(this Func<A, B, C, T> self, B b) =>
		(a, c) => self(a, b, c);
	public static Func<A, B, T> Bind3<A, B, C, T>(this Func<A, B, C, T> self, C c) =>
		(a, b) => self(a, b, c);

	public static Func<B, C, D, T> Bind1<A, B, C, D, T>(this Func<A, B, C, D, T> self, A a) =>
		(b, c, d) => self(a, b, c, d);
	public static Func<A, C, D, T> Bind2<A, B, C, D, T>(this Func<A, B, C, D, T> self, B b) =>
		(a, c, d) => self(a, b, c, d);
	public static Func<A, B, D, T> Bind3<A, B, C, D, T>(this Func<A, B, C, D, T> self, C c) =>
		(a, b, d) => self(a, b, c, d);
	public static Func<A, B, C, T> Bind4<A, B, C, D, T>(this Func<A, B, C, D, T> self, D d) =>
		(a, b, c) => self(a, b, c, d);

	public static Func<B, C, D, E, T> Bind1<A, B, C, D, E, T>(this Func<A, B, C, D, E, T> self, A a) =>
		(b, c, d, e) => self(a, b, c, d, e);
	public static Func<A, C, D, E, T> Bind2<A, B, C, D, E, T>(this Func<A, B, C, D, E, T> self, B b) =>
		(a, c, d, e) => self(a, b, c, d, e);
	public static Func<A, B, D, E, T> Bind3<A, B, C, D, E, T>(this Func<A, B, C, D, E, T> self, C c) =>
		(a, b, d, e) => self(a, b, c, d, e);
	public static Func<A, B, C, E, T> Bind4<A, B, C, D, E, T>(this Func<A, B, C, D, E, T> self, D d) =>
		(a, b, c, e) => self(a, b, c, d, e);
	public static Func<A, B, C, D, T> Bind5<A, B, C, D, E, T>(this Func<A, B, C, D, E, T> self, E e) =>
		(a, b, c, d) => self(a, b, c, d, e);


	public static Action Bind<A>(this Action<A> self, A a) =>
		() => self(a);

	public static Action<B> Bind1<A, B>(this Action<A, B> self, A a) =>
		(b) => self(a, b);
	public static Action<A> Bind2<A, B>(this Action<A, B> self, B b) =>
		(a) => self(a, b);

	public static Action<B, C> Bind1<A, B, C>(this Action<A, B, C> self, A a) =>
		(b, c) => self(a, b, c);
	public static Action<A, C> Bind2<A, B, C>(this Action<A, B, C> self, B b) =>
		(a, c) => self(a, b, c);
	public static Action<A, B> Bind3<A, B, C>(this Action<A, B, C> self, C c) =>
		(a, b) => self(a, b, c);

	public static Action<B, C, D> Bind1<A, B, C, D>(this Action<A, B, C, D> self, A a) =>
		(b, c, d) => self(a, b, c, d);
	public static Action<A, C, D> Bind2<A, B, C, D>(this Action<A, B, C, D> self, B b) =>
		(a, c, d) => self(a, b, c, d);
	public static Action<A, B, D> Bind3<A, B, C, D>(this Action<A, B, C, D> self, C c) =>
		(a, b, d) => self(a, b, c, d);
	public static Action<A, B, C> Bind4<A, B, C, D>(this Action<A, B, C, D> self, D d) =>
		(a, b, c) => self(a, b, c, d);

	public static Action<B, C, D, E> Bind1<A, B, C, D, E>(this Action<A, B, C, D, E> self, A a) =>
		(b, c, d, e) => self(a, b, c, d, e);
	public static Action<A, C, D, E> Bind2<A, B, C, D, E>(this Action<A, B, C, D, E> self, B b) =>
		(a, c, d, e) => self(a, b, c, d, e);
	public static Action<A, B, D, E> Bind3<A, B, C, D, E>(this Action<A, B, C, D, E> self, C c) =>
		(a, b, d, e) => self(a, b, c, d, e);
	public static Action<A, B, C, E> Bind4<A, B, C, D, E>(this Action<A, B, C, D, E> self, D d) =>
		(a, b, c, e) => self(a, b, c, d, e);
	public static Action<A, B, C, D> Bind5<A, B, C, D, E>(this Action<A, B, C, D, E> self, E e) =>
		(a, b, c, d) => self(a, b, c, d, e);
}
