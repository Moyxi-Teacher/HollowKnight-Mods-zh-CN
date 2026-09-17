namespace Osmi.Utils;

[PublicAPI]
public static class VectorUtil {
	public static (float x, float y, float z) ToTuple(this Vector3 self) =>
		(self.x, self.y, self.z);

	public static (float x, float y) ToTuple(this Vector2 self) =>
		(self.x, self.y);


	public static Vector2 AsVector2(this Vector3 self) => new(self.x, self.y);

	public static Vector3 ToVector3(this Vector2 self, float z) => new(self.x, self.y, z);
}
