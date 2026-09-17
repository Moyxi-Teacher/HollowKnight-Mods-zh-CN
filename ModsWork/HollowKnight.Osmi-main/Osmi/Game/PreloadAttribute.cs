namespace Osmi.Game;

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class PreloadAttribute : Attribute {
	public string SceneName { get; private init; }
	public string Path { get; private init; }

	public PreloadAttribute(string sceneName, string path) {
		SceneName = sceneName;
		Path = path;
	}
}
