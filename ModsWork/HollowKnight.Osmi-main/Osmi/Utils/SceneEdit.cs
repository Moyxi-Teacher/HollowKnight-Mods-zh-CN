namespace Osmi.Utils;

public sealed class SceneEdit {
	private static readonly Dictionary<string, HashSet<SceneEdit>> handles = new();

	internal readonly GameObjectRef goRef;
	internal readonly Action<GameObject> action;
	private bool enabled = false;

	public SceneEdit(GameObjectRef goRef, Action<GameObject> action) {
		this.goRef = goRef;
		this.action = action;
	}

	public void Enable() {
		if (enabled) {
			throw new InvalidOperationException("Already enabled");
		}

		string sceneName = goRef.SceneName;

		if (!handles.TryGetValue(sceneName, out HashSet<SceneEdit> sceneHandles)) {
			sceneHandles = handles[sceneName] = new();
		}

		_ = sceneHandles.Add(this);
		enabled = true;
	}

	public void Disable() {
		if (!enabled) {
			throw new InvalidOperationException("Already Disabled");
		}

		string sceneName = goRef.SceneName;

		if (!handles.TryGetValue(sceneName, out HashSet<SceneEdit> sceneHandles)) {
			return;
		}

		_ = sceneHandles.Remove(this);
		enabled = false;

		if (sceneHandles.Count == 0) {
			_ = handles.Remove(sceneName);
		}
	}

	internal static void ApplyHandles(Scene _prev, Scene next) {
		if (handles.TryGetValue(GameObjectRef.WILDCARD_SCENE_NAME, out HashSet<SceneEdit> wildcardHandles)) {
			wildcardHandles.ForEach(handle => handle.action.Invoke(
				next.GetGameObjectByPath(handle.goRef.Path)!
			));
		}

		if (handles.TryGetValue(next.name, out HashSet<SceneEdit> sceneHandles)) {
			sceneHandles.ForEach(handle => handle.action.Invoke(
				next.GetGameObjectByPath(handle.goRef.Path)!
			));
		}
	}
}
