namespace Osmi.Utils;

public sealed record Box<T>(T Value) {
	public T Value { get; set; } = Value;
	public static implicit operator Box<T>(T value) => new(value);
	public static explicit operator T(Box<T> box) => box.Value;
}
