namespace RandomCompanions
{

    internal static partial class BetterMenu
    {

        internal static GlobalModSettings defaultSettings = new GlobalModSettings();
        internal static Menu MenuRef;
         internal static Menu PrepareMenu(ModToggleDelegates toggleDelegates){
            return new Menu(RandomCompanions.Localize("随机伴侣", "Random Companions"),new Element[]{
                new TextPanel(RandomCompanions.Localize("提示：修改设置后需重启游戏才能正确生效。", "Note: Settings require a restart of the game to apply properly."),800f),
                Blueprints.CreateToggle(toggleDelegates, RandomCompanions.Localize("随机皮肤", "Random Skins"), "", RandomCompanions.Localize("启用", "Enabled"), RandomCompanions.Localize("关闭", "Disabled")),
                new HorizontalOption(
                    RandomCompanions.Localize("启用攻击", "Enable Attacks"), RandomCompanions.Localize("允许召唤物攻击。", "Allows minions to attack"),
                    new string[] { RandomCompanions.Localize("关闭", "Disabled"), RandomCompanions.Localize("启用", "Enabled") },
                    (setting) => { RandomCompanions.Settings.attackOption = (setting == 1); },
                    () => RandomCompanions.Settings.attackOption ? 1 : 0,
                    Id:"AttackEnabled"),
                Blueprints.NavigateToMenu(RandomCompanions.Localize("幼体设置", "Hatchlings Settings"),"",()=> GetHatchlingsMenu(MenuRef.menuScreen)),
                Blueprints.NavigateToMenu(RandomCompanions.Localize("织蛛设置", "Weaverlings Settings"),"",()=> GetWeaverlingsMenu(MenuRef.menuScreen)),
                Blueprints.NavigateToMenu(RandomCompanions.Localize("格林之子设置", "Grimmchild Settings"),"",()=> GetGrimmChildMenu(MenuRef.menuScreen))
            });
        }
        internal static MenuScreen GetMenu(MenuScreen lastMenu, ModToggleDelegates? toggleDelegates){
            if(MenuRef == null){
                MenuRef = PrepareMenu((ModToggleDelegates)toggleDelegates);
            }
            return MenuRef.GetCachedMenuScreen(lastMenu);
        }
    }
}