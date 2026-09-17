using Satchel.BetterMenus;

namespace ExtraNotchesForExtraCharms
{
    public static class ModMenu
    {
        private static Menu menu;
        private static MenuScreen menuScreen;

        /// <summary>
        /// Builds the Exaltation Expanded menu
        /// </summary>
        /// <param name="modListMenu"></param>
        /// <returns></returns>
        public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
        {
            // Declare the menu
            menu = new Menu("额外护符槽位", new Element[] { });

            // Populate main menu
            menu.AddElement(new HorizontalOption("每护符槽位数",
                                                    "每个护符槽位可容纳的额外护符数量",
                                                    CostValues(),
                                                    value => ExtraNotchesForExtraCharms.globalSettings.charmsPerNotch = value + 1,
                                                    () => ExtraNotchesForExtraCharms.globalSettings.charmsPerNotch - 1));

            // Insert the menu into the overall menu
            menuScreen = menu.GetMenuScreen(modListMenu);

            return menuScreen;
        }

        /// <summary>
        /// String values for int options
        /// </summary>
        /// <returns></returns>
        private static string[] CostValues()
        {
            // 中文数字映射
            return new string[] { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
        }
    }
}
