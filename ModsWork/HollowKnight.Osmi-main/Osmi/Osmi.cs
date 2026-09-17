using Osmi.Exports;
using Osmi.Game;

namespace Osmi;

[PublicAPI]
public sealed class Osmi : Mod {
	public static Osmi? Instance { get; private set; }

	public static Osmi UnsafeInstance => Instance!;

	private static readonly Lazy<string> version = AssemblyUtil
#if DEBUG
		.GetMyDefaultVersionWithHash();
#else
		.GetMyDefaultVersion();
#endif

	public override string GetVersion() => version.Value;

	public Osmi() : base("Osmi（模组框架）") =>
		Instance = this;

	public override void Initialize() =>
		ModInteropExports.Init();

	static Osmi() {
		GameObjectUtil.Init();
		OsmiHooks.SceneChangeHook += SceneEdit.ApplyHandles;
		ModHooks.FinishedLoadingModsHook += () => GlobalCoroutineExecutor.Start(OsmiHooks.CheckGameInitialized());
	}
}
