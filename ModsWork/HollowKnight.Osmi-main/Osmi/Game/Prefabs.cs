namespace Osmi.Game;

[PublicAPI]
public static class Prefabs {
	private static readonly Dictionary<string, GameObject?> prefabs = new();

	public static GameObject? Find(string name) => prefabs.TryGetValue(name, out GameObject? gameObject)
		? gameObject
		: prefabs[name] = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => !go.scene.IsValid() && go.name == name);

	public static GameObject?[] Find(params string[] names) {
		var result = new GameObject?[names.Length];
		Dictionary<string, int> lookup = new();

		for (int i = 0; i < names.Length; i++) {
			if (prefabs.TryGetValue(names[i], out GameObject? gameObject)) {
				result[i] = gameObject;
			} else if (!lookup.TryAdd(names[i], i)) {
				throw new ArgumentException(nameof(names), "Redundant values");
			}
		}

		if (lookup.Count == 0) {
			return result;
		}

		foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>()) {
			if (go.scene.IsValid()) {
				continue;
			}

			string name = go.name;

			if (lookup.ContainsKey(name)) {
				result[lookup[name]] = prefabs[name] = go;
				_ = lookup.Remove(name);

				if (lookup.Count == 0) {
					return result;
				}
			}
		}

		foreach (string name in lookup.Keys) {
			prefabs[name] = null;
		}

		return result;
	}

	public static void ClearCache(string name) => prefabs.Remove(name);

	public static void ClearCache(params string[] names) =>
		names.ForEach(name => prefabs.Remove(name));

	public static void ClearCahce() => prefabs.Clear();


	public static GameObject AudioPlayer => Find("Audio Player Actor")!;

	public static GameObject RoarWaveEmitter => Find("Roar Wave Emitter")!;

	public static GameObject Shockwave => Find("Shockwave Wave Z")!;

	static Prefabs() => _ = Find("Audio Player Actor", "Roar Wave Emitter", "Shockwave Wave Z");
}
