namespace Osmi.Utils;

[PublicAPI]
public static class DeconstructUtil {
	public static void Deconstruct(this Vector3 self, out float x, out float y, out float z) =>
		(x, y, z) = self.ToTuple();

	public static void Deconstruct(this Vector2 self, out float x, out float y) =>
		(x, y) = self.ToTuple();

	public static void Deconstruct(this RaycastHit2D hit, out Collider2D? collider) =>
		collider = hit.collider;

	public static void Deconstruct(this RaycastHit2D hit, out Vector2 point, out float distance) {
		point = hit.point;
		distance = hit.distance;
	}

	public static void Deconstruct(this RaycastHit2D hit, out Collider2D? collider, out Vector2 point, out float distance) {
		collider = hit.collider;
		point = hit.point;
		distance = hit.distance;
	}

	public static void Deconstruct<T>(this T[] array, out T first, out T[] rest) {
		if (array.Length < 1) {
			throw new ArgumentException(nameof(array), "Array does not have enough items");
		}

		first = array[0];
		rest = new T[array.Length - 1];
		Array.Copy(array, 1, rest, 0, array.Length - 1);
	}

	public static void Deconstruct<T>(this T[] array, out T first, out T second, out T[] rest) {
		if (array.Length < 2) {
			throw new ArgumentException(nameof(array), "Array does not have enough items");
		}

		first = array[0];
		second = array[1];
		rest = new T[array.Length - 2];
		Array.Copy(array, 2, rest, 0, array.Length - 2);
	}

	public static void Deconstruct<T>(this T[] array, out T first, out T second, out T third, out T[] rest) {
		if (array.Length < 3) {
			throw new ArgumentException(nameof(array), "Array does not have enough items");
		}

		first = array[0];
		second = array[1];
		third = array[2];
		rest = new T[array.Length - 3];
		Array.Copy(array, 3, rest, 0, array.Length - 3);
	}

	public static void Deconstruct<T>(this T[] array, out T first, out T second, out T third, out T fourth, out T[] rest) {
		if (array.Length < 4) {
			throw new ArgumentException(nameof(array), "Array does not have enough items");
		}

		first = array[0];
		second = array[1];
		third = array[2];
		fourth = array[3];
		rest = new T[array.Length - 4];
		Array.Copy(array, 4, rest, 0, array.Length - 4);
	}

	public static void Deconstruct<T>(this T[] array, out T first, out T second, out T third, out T fourth, out T fifth, out T[] rest) {
		if (array.Length < 5) {
			throw new ArgumentException(nameof(array), "Array does not have enough items");
		}

		first = array[0];
		second = array[1];
		third = array[2];
		fourth = array[3];
		fifth = array[4];
		rest = new T[array.Length - 5];
		Array.Copy(array, 5, rest, 0, array.Length - 5);
	}
}
