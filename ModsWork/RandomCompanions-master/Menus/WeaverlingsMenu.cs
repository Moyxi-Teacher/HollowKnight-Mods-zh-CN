namespace RandomCompanions
{

    internal static partial class BetterMenu
    {
        internal static Menu WeaverlingsMenu;
        internal static Menu PrepareWeaverlingsMenu(){
            return new Menu(RandomCompanions.Localize("织蛛设置", "Weaverlings Settings"),new Element[]{
                new HorizontalOption(
                    RandomCompanions.Localize("开局拥有编织者之歌", "Start With Weaversong"), RandomCompanions.Localize("游戏开始时自动给予护符。", "Automatically give charm at start of game."),
                    new string[] { RandomCompanions.Localize("关闭", "Disabled"), RandomCompanions.Localize("启用", "Enabled") },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.WeaverlingCharmStart = true;
                        } else {
                            RandomCompanions.Settings.WeaverlingCharmStart = false;
                        }
                    },
                    () => {
                         return RandomCompanions.Settings.WeaverlingCharmStart ? 1 : 0;
                    },
                    Id:"AutoWeaverlings"),

                new HorizontalOption(
                    RandomCompanions.Localize("编织者之歌免费装备", "Free Weaversong"), RandomCompanions.Localize("使护符无需护符槽即可装备。", "Make charm free to equip."),
                    new string[] { RandomCompanions.Localize("关闭", "Disabled"), RandomCompanions.Localize("启用", "Enabled") },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.WeaverlingCharmFree = true;
                        } else {
                            RandomCompanions.Settings.WeaverlingCharmFree = false;
                        } 
                    },
                    () => {
                         return RandomCompanions.Settings.WeaverlingCharmFree ? 1 : 0;
                    },
                    Id:"FreeWeaverlings"),

                new CustomSlider(
                    RandomCompanions.Localize("织蛛最大数量", "Max Weaverling Count"),
                    (f)=>{
                        RandomCompanions.Settings.WeaverlingMaxCount = (int)f;
                    },
                    () => (float)RandomCompanions.Settings.WeaverlingMaxCount,
                    3,15,true,
                    Id:"WeaverlingCount"),
                });
        }

        internal static MenuScreen GetWeaverlingsMenu(MenuScreen lastMenu){
            if(WeaverlingsMenu == null){
                WeaverlingsMenu = PrepareWeaverlingsMenu();
            }
            return WeaverlingsMenu.GetCachedMenuScreen(lastMenu);
        }
    }
}