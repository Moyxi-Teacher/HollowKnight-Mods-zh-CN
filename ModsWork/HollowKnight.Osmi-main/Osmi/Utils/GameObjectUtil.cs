namespace Osmi.Utils;

[PublicAPI]
public static class GameObjectUtil {
	private static readonly GameObject holder = New("Osmi MonoBehaviour Holder", true);

	internal static void Init() {
	}


	public static void SetParent(this GameObject self, Transform parent) =>
		self.transform.parent = parent;

	public static void SetParent(this GameObject self, GameObject parent) =>
		self.SetParent(parent.transform);

	public static void SetRoot(this GameObject self) =>
		self.SetParent((null as Transform)!);


	public static GameObject? Child(this GameObject self, string name) =>
		self.transform.Find(name)?.gameObject;

	public static GameObject? Child(this GameObject self, params string[] path) =>
		self.transform.Find(path.Join('/'))?.gameObject;

	public static GameObject? Child(this GameObject self, ArraySegment<string> path) =>
		self.transform.Find(path.Join('/'))?.gameObject;

	public static IEnumerable<GameObject> GetChildren(this GameObject self) {
		foreach (Transform child in self.transform) {
			yield return child.gameObject;
		}
	}

	public static string[] GetPath(this GameObject self) {
		List<string> path = new();
		GetPathInternal(self.transform, path);
		path.Reverse();
		return path.ToArray();
	}

	private static void GetPathInternal(Transform tf, List<string> path) {
		path.Add(tf.name);

		if (tf.parent is Transform parent) {
			GetPathInternal(parent, path);
		}
	}


	public static GameObject? GetRootGameObject(this Scene scene, string name) => scene
		.GetRootGameObjects()
		.FirstOrDefault(go => go.name == name);

	public static GameObject? GetGameObjectByPath(this Scene scene, params string[] path) =>
		scene.GetGameObjectByPath(new ArraySegment<string>(path));

	public static GameObject? GetGameObjectByPath(this Scene scene, ArraySegment<string> path) {
		if (path.Count == 0) {
			throw new ArgumentException(nameof(path));
		}

		GameObject? rootGo = scene.GetRootGameObject((path as IList<string>)[0]);

		return path.Count > 1 ? rootGo?.Child(new ArraySegment<string>(path.Array, path.Offset + 1, path.Count - 1)) : rootGo;
	}

	public static GameObject? FindGameObjectByRef(this Scene scene, GameObjectRef goRef) =>
		goRef.MatchScene(scene) ? scene.GetGameObjectByPath(goRef.Path) : null;


	public static TransformDelegate GetDelegate(this Transform self) => new(self);

	public static TransformDelegate GetTransformDelegate(this GameObject self) =>
		self.transform.GetDelegate();


	public static GameObject New(bool dontDestroyOnLoad = false) {
		GameObject go = new();

		if (dontDestroyOnLoad) {
			UObject.DontDestroyOnLoad(go);
		}

		return go;
	}

	public static GameObject New(string name, bool dontDestroyOnLoad = false) =>
		New(name, (null as Transform)!, dontDestroyOnLoad);

	public static GameObject New(string name, Transform parent, bool dontDestroyOnLoad = false) {
		GameObject go = new(name);
		go.SetParent(parent);

		if (dontDestroyOnLoad) {
			UObject.DontDestroyOnLoad(go);
		}

		return go;
	}

	public static GameObject New(string name, GameObject parent, bool dontDestroyOnLoad = false) =>
		New(name, parent.transform, dontDestroyOnLoad);

	public static GameObject New(string name, Transform parent, params Type[] components) {
		GameObject go = new(name, components);
		go.SetParent(parent);
		return go;
	}

	public static GameObject New(string name, GameObject parent, params Type[] components) =>
		New(name, parent.transform, components);

	public static GameObject New(string name, Transform parent, bool dontDestroyOnLoad, params Type[] components) {
		GameObject go = new(name, components);
		go.SetParent(parent);

		if (dontDestroyOnLoad) {
			UObject.DontDestroyOnLoad(go);
		}

		return go;
	}

	public static GameObject New(string name, GameObject parent, bool dontDestroyOnLoad, params Type[] components) =>
		New(name, parent.transform, dontDestroyOnLoad, components);


	public static T Instantiate<T>(T original, Vector3 position)
		where T : UObject =>
		UObject.Instantiate(original, position, Quaternion.identity);

	public static T Instantiate<T>(T original, Vector3 position, Transform parent)
		where T : UObject =>
		UObject.Instantiate(original, position, Quaternion.identity, parent);

	public static T Instantiate<T>(T original, Vector3 position, GameObject parent)
		where T : UObject =>
		UObject.Instantiate(original, position, Quaternion.identity, parent.transform);


	public static T CreateHolder<T>(string name = "") where T : MonoBehaviour =>
		New(string.IsNullOrWhiteSpace(name) ? typeof(T).Name + " Holder" : name, holder.transform).AddComponent<T>();
}
