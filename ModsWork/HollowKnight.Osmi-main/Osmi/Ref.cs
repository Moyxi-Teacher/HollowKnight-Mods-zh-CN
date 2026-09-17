namespace Osmi;

[PublicAPI]
public static class Ref {
	public static GameManager GM => GameManager.instance;

	public static HeroController HC => HeroController.instance;

	public static PlayerData PD => PlayerData.instance;

	public static GameCameras GC => GameCameras.instance;

	public static UIManager UM => UIManager.instance;

	public static ObjectPool OP => ObjectPool.instance;

	public static InputHandler IH => InputHandler.Instance;

	public static SceneData SD => SceneData.instance;

	public static Scene DDOL => GM.gameObject.scene;
}
