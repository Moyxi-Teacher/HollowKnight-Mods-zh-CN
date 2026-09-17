namespace RandomCompanions
{

    internal static partial class BetterMenu
    {
        internal static Menu GrimmChildMenu;
        internal static Menu PrepareGrimmChildMenu(){
            return new Menu(RandomCompanions.Localize("格林之子设置", "Grimmchild Settings"),new Element[]{
                new HorizontalOption(
                    RandomCompanions.Localize("开局拥有格林之子", "Start With Grimmchild"), RandomCompanions.Localize("游戏开始时自动给予护符。", "Automatically give charm at start of game."),
                    new string[] { RandomCompanions.Localize("关闭", "Disabled"), RandomCompanions.Localize("启用", "Enabled") },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.GrimmChildCharmStart = true;
                        } else {
                            RandomCompanions.Settings.GrimmChildCharmStart = false;
                        }
                    },
                    () => {
                         return RandomCompanions.Settings.GrimmChildCharmStart ? 1 : 0;
                    },
                    Id:"AutoGrimmChild"),

                new HorizontalOption(
                    RandomCompanions.Localize("格林之子免费装备", "Free Grimmchild"), RandomCompanions.Localize("使护符无需护符槽即可装备。", "Make charm free to equip."),
                    new string[] { RandomCompanions.Localize("关闭", "Disabled"), RandomCompanions.Localize("启用", "Enabled") },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.GrimmChildCharmFree = true;
                        } else {
                            RandomCompanions.Settings.GrimmChildCharmFree = false;
                        } 
                    },
                    () => {
                         return RandomCompanions.Settings.GrimmChildCharmFree ? 1 : 0;
                    },
                    Id:"FreeGrimmChild"),

                new CustomSlider(
                    RandomCompanions.Localize("格林之子最大数量", "Max Grimmchild Count"),
                    (f)=>{
                        RandomCompanions.Settings.GrimmChildMaxCount = (int)f;
                    },
                    () => (float)RandomCompanions.Settings.GrimmChildMaxCount,
                    1,15,true,
                    Id:"GrimmChildCount"),

                new HorizontalOption(
                    RandomCompanions.Localize("多等级格林之子", "Multi-Level Grimmchildren"), RandomCompanions.Localize("依次循环格林之子的所有等级。", "Cycle through all levels of Grimmchild"),
                    new string[] { RandomCompanions.Localize("关闭", "Disabled"), RandomCompanions.Localize("启用", "Enabled") },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.GrimmChildMultiLevel = true;
                        } else {
                            RandomCompanions.Settings.GrimmChildMultiLevel = false;
                        } 
                    },
                    () => {
                         return RandomCompanions.Settings.GrimmChildMultiLevel ? 1 : 0;
                    },
                    Id:"GrimmChildMultiLevel"),

                });
        }

        internal static MenuScreen GetGrimmChildMenu(MenuScreen lastMenu){
            if(GrimmChildMenu == null){
                GrimmChildMenu = PrepareGrimmChildMenu();
            }
            return GrimmChildMenu.GetCachedMenuScreen(lastMenu);
        }
    }
}