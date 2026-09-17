using MenuButton = Satchel.BetterMenus.MenuButton;

namespace EnemyHPBar;

// Thanks to CustomKnight menu: https://github.com/PrashantMohta/HollowKnight.CustomKnight/blob/moreskin/CustomKnight/Menu/BetterMenu.cs
internal static class BetterMenu {
	internal static int selectedSkin = 0;
	internal static Menu MenuRef = null;

	internal static MenuScreen GetMenu(MenuScreen lastMenu, ModToggleDelegates? toggleDelegates) {
		MenuRef ??= PrepareMenu((ModToggleDelegates) toggleDelegates);

		MenuRef.OnBuilt += (_, Element) => {
			if (EnemyHPBar.CurrentSkin != null) {
				BetterMenu.SelectedSkin(EnemyHPBar.CurrentSkin.GetId());
			}
		};

		return MenuRef.GetMenuScreen(lastMenu);
	}

	internal static void ApplySkin() {
		ISelectableSkin skinToApply = EnemyHPBar.SkinList[selectedSkin];
		BetterMenu.SetSkinById(skinToApply.GetId());
		EnemyHPBar.CompleteImage(Path.Combine(EnemyHPBar.DATA_DIR, EnemyHPBar.CurrentSkin.GetId()));
		EnemyHPBar.bossol = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetBossOutlineImage());
		EnemyHPBar.bossbg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetBossBackgroundImage());
		EnemyHPBar.bossfg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetBossForegroundImage());
		EnemyHPBar.ol = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetOutlineImage());
		EnemyHPBar.fg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetForegroundImage());
		EnemyHPBar.mg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetMiddlegroundImage());
		EnemyHPBar.bg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetBackgroundImage());
		AnimJson.animDict.Clear();
		AnimJson.Initdic();
		AnimJson.LoadAllConfig();
		AnimJson.SaveAllConfig();
	}

	internal static string[] GetSkinNameArray() => EnemyHPBar.SkinList.Select(s => HPBarList.MaxLength(s.GetName(), EnemyHPBar.globalSettings.NameLength)).ToArray();

	internal static Menu PrepareMenu(ModToggleDelegates toggleDelegates) => new("敌人血条", new Element[] {
		Blueprints.CreateToggle(toggleDelegates,"血条开关","","已启用","已禁用"),
			new HorizontalOption(
				"选择皮肤",
				"当前将使用的皮肤",
				GetSkinNameArray(),
				(setting) => selectedSkin = setting,
				() => selectedSkin,
				Id: "SelectSkinOption"
			),
			Blueprints.HorizontalBoolOption(
				"与CK联动",
				"是否与 CustomKnight 集成？（使用此项请确保已安装 CK，并在切换为开启时重置皮肤；请在你的皮肤文件夹中创建名为 \"HPBar\" 的文件夹）",
				(choose) => EnemyHPBar.globalSettings.Intergration = choose,
				() => EnemyHPBar.globalSettings.Intergration,
				Id:"CKIntergration"
			),
			new MenuRow(
				new List<Element>{
					Blueprints.NavigateToMenu(
						"皮肤列表",
						"打开皮肤列表",
						() => HPBarList.GetMenu(MenuRef.menuScreen)),
						new MenuButton("应用皮肤","应用当前选中的皮肤。",
						_ => ApplySkin()
					),
				},
				Id: "ApplyButtonGroup"
			){ XDelta = 400f },
		});

	internal static void SelectedSkin(string skinId) => selectedSkin = EnemyHPBar.SkinList.FindIndex(skin => skin.GetId() == skinId);

	public static ISelectableSkin GetSkinById(string id) => EnemyHPBar.SkinList.Find(skin => skin.GetId() == id) ?? GetDefaultSkin();

	public static ISelectableSkin GetDefaultSkin() {
		EnemyHPBar.DefaultSkin ??= GetSkinById("Default");
		return EnemyHPBar.DefaultSkin;
	}

	public static void SetSkinById(string id) {
		ISelectableSkin Skin = GetSkinById(id);
		if (EnemyHPBar.CurrentSkin.GetId() == Skin.GetId()) { return; }

		EnemyHPBar.CurrentSkin = Skin;
	}
}
