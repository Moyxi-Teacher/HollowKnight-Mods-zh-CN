namespace RandomCompanions
{

    internal static partial class BetterMenu
    {
        internal static Menu HatchlingMenu;
        internal static Menu PrepareHatchlingsMenu(){
            return new Menu(RandomCompanions.Localize("幼体设置", "Hatchling Settings"),new Element[]{
                new HorizontalOption(
                    RandomCompanions.Localize("开局拥有发光子宫", "Start With Glowing Womb"), RandomCompanions.Localize("游戏开始时自动给予护符。", "Automatically give charm at start of game."),
                    new string[] { RandomCompanions.Localize("关闭", "Disabled"), RandomCompanions.Localize("启用", "Enabled") },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.HatchlingCharmStart = true;
                        } else {
                            RandomCompanions.Settings.HatchlingCharmStart = false;
                        }
                    },
                    () => {
                         return RandomCompanions.Settings.HatchlingCharmStart ? 1 : 0;
                    },
                    Id:"AutoHatchings"),

                new HorizontalOption(
                    RandomCompanions.Localize("发光子宫免费装备", "Free Glowing Womb"), RandomCompanions.Localize("使护符无需护符槽即可装备。", "Make charm free to equip."),
                    new string[] { RandomCompanions.Localize("关闭", "Disabled"), RandomCompanions.Localize("启用", "Enabled") },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.HatchlingCharmFree = true;
                        } else {
                            RandomCompanions.Settings.HatchlingCharmFree = false;
                        } 
                    },
                    () => {
                        return RandomCompanions.Settings.HatchlingCharmFree ? 1 : 0;
                    },
                    Id:"FreeHatchlings"),

                new HorizontalOption(
                    RandomCompanions.Localize("快速生成", "Quick Spawn"), RandomCompanions.Localize("移除幼体生成时的延迟。", "Remove delays when spawning hatchlings."),
                    new string[] { RandomCompanions.Localize("关闭", "Disabled"), RandomCompanions.Localize("启用", "Enabled") },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.HatchlingSpawnTime = 0f;
                        } else {
                            RandomCompanions.Settings.HatchlingSpawnTime = defaultSettings.HatchlingSpawnTime;
                        } 
                    },
                    () => {
                        return RandomCompanions.Settings.HatchlingSpawnTime == 0 ? 1 : 0;
                    },
                    Id:"QuickHatchlings"),

                new CustomSlider(
                    RandomCompanions.Localize("幼体最大数量", "Max Hatchlings Count"),
                    (f)=>{
                        RandomCompanions.Settings.HatchlingMaxCount = (int)f;
                    },
                    () => (float)RandomCompanions.Settings.HatchlingMaxCount,
                    4,15,true,
                    Id:"HatchlingsCount"),
                    
                new CustomSlider(
                    RandomCompanions.Localize("幼体灵魂消耗", "Hatchling Soul Cost"),
                    (f)=>{
                        RandomCompanions.Settings.HatchlingSoulCost = (int)f;
                    },
                    () => (float)RandomCompanions.Settings.HatchlingSoulCost,
                    0,15,true,
                    Id:"HatchlingSoulCost")
                });
        }

        internal static MenuScreen GetHatchlingsMenu(MenuScreen lastMenu){
            if(HatchlingMenu == null){
                HatchlingMenu = PrepareHatchlingsMenu();
            }
            return HatchlingMenu.GetCachedMenuScreen(lastMenu);
        }
    }
}