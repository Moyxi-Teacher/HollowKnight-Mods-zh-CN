namespace Osmi.Game;

public static class PreloadUtil {
	private static readonly LazyMap<Type, (string sceneName, string path, Action<GameObject> setter)[]> entries = new(t => t
		.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		.Filter(fi => Attribute.IsDefined(fi, typeof(PreloadAttribute)))
		.Filter(fi => {
			if (fi.IsLiteral) {
				LogError(
					$"[PreloadUtil] Preload field {fi.Name} on type {t.FullName} is const"
				);
				return false;
			}

			if (fi.FieldType != typeof(GameObject)) {
				LogError(
					$"[PreloadUtil] Type of preload field {fi.Name} on type {t.FullName} is not GameObject"
				);
				return false;
			}

			return true;
		})
		.Map(fi => {
			PreloadAttribute attr = fi.GetCustomAttribute<PreloadAttribute>();
			return (attr.SceneName, attr.Path, fi.GetFastStaticSetter<GameObject>());
		})
		.ToArray()
	);

	public static List<(string, string)> GetPreloadNames<T>() => GetPreloadsNames(typeof(T));

	public static List<(string, string)> GetPreloadsNames(Type type) => entries[type]
		.Map(tuple => (tuple.sceneName, tuple.path))
		.ToList();

	public static void SavePreloads<T>(Dictionary<string, Dictionary<string, GameObject>>? preloadedObjects) =>
		SavePreloads(typeof(T), preloadedObjects);

	public static void SavePreloads(Type type, Dictionary<string, Dictionary<string, GameObject>>? preloadedObjects) {
		if (preloadedObjects == null) {
			return;
		}

		entries[type].ForEach(
			tuple => tuple.setter.Invoke(preloadedObjects[tuple.sceneName][tuple.path])
		);

		entries.Remove(type);
	}

	static PreloadUtil() => OsmiHooks.GameInitializedHook += entries.Clear;
}
