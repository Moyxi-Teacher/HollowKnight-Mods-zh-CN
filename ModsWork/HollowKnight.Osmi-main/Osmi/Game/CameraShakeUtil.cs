namespace Osmi.Game;

public static class CameraShakeUtil {
	public static PlayMakerFSM CameraShakeFSM => Ref.GC.cameraShakeFSM;

	public static void Small() => CameraShakeFSM.SendEvent("SmallShake");
	public static void Big() => CameraShakeFSM.SendEvent("BigShake");
	public static void Huge() => CameraShakeFSM.SendEvent("HugeShake");
	public static void SuperDash() => CameraShakeFSM.SendEvent("SuperDashShake");
	public static void EnemyKill() => CameraShakeFSM.SendEvent("EnemyKillShake");
	public static void Tram() => CameraShakeFSM.SendEvent("TramShake");
	public static void Average() => CameraShakeFSM.SendEvent("AverageShake");
	public static void Blizzard() => CameraShakeFSM.SendEvent("BlizzardShake");

	public static void Cancel() => CameraShakeFSM.SendEvent("CANCEL SHAKE");
}
