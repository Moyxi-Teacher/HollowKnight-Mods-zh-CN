namespace Osmi.Utils;

public sealed record GameObjectRef {
	// Scene names cannot be longer than 244 characters, so a string with the length of 245 will definitely have no collision
	public const string WILDCARD_SCENE_NAME = "*****************************************************************************************************************************************************************************************************************************************************";

	public const string DONT_DESTROY_ON_LOAD = "DontDestroyOnLoad";

	public string SceneName { get; private init; }

	public string[] Path { get; private init; }

	public GameObjectRef(string sceneName, params string[] path) {
		if (sceneName.Length > 244 && sceneName != WILDCARD_SCENE_NAME) {
			throw new ArgumentException(nameof(sceneName));
		}

		if (path.Length < 1) {
			throw new ArgumentException(nameof(path));
		}

		SceneName = sceneName;
		Path = path.ToArray();
	}

	public bool MatchScene(Scene scene) => SceneName == WILDCARD_SCENE_NAME || SceneName == scene.name;

	public bool MatchGameObject(GameObject go) => MatchScene(go.scene) && go.GetPath().SeqEqual(Path);
}
