namespace CompanionCloth;

public sealed class CompanionCloth :
	Mod, ITogglableMod, IMenuMod, IGlobalSettings<GlobalSettings> {
	public CompanionCloth() : base("陪伴的布料") { }
	private const string clothScene = "Fungus3_23_boss";
	private const string clothPath = "Battle Scene/Cloth Entry/Cloth Fighter";

	public static CompanionCloth? Instance { get; private set; }

	internal static GameObject? clothFighter = null;


	public override string GetVersion() => Assembly
		.GetExecutingAssembly()
		.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
		.InformationalVersion;

	public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloads) {
		if (Instance != null) {
			return;
		}

		Instance = this;

		if (preloads != null) {
			clothFighter = preloads[clothScene][clothPath];
			LogDebug("Preloads saved");
		}

		ModHooks.HeroUpdateHook += Listener.ListenKey;
		ModHooks.SlashHitHook += EnemyMarker.MarkAttacked;
		On.PlayMakerFSM.OnEnable += ClothFSM.ModifyFSM;
	}

	public void Unload() {
		ModHooks.HeroUpdateHook -= Listener.ListenKey;
		ModHooks.SlashHitHook -= EnemyMarker.MarkAttacked;
		On.PlayMakerFSM.OnEnable -= ClothFSM.ModifyFSM;

		Instance = null;
	}

	public bool ToggleButtonInsideMenu => true;

	public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
	{
		return new List<IMenuMod.MenuEntry>
		{
			new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "在战斗中召唤布料伙伴协助攻击敌人并提供护盾。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
		};
	}


	public override List<(string, string)> GetPreloadNames() => new() {
		(clothScene, clothPath)
	};


	public GlobalSettings GlobalSettings { get; private set; } = new();
	public void OnLoadGlobal(GlobalSettings s) => GlobalSettings = s;
	public GlobalSettings OnSaveGlobal() => GlobalSettings;
}
