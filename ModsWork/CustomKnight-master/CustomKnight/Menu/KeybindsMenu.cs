using Satchel.BetterMenus;

namespace CustomKnight
{
    internal class KeybindsMenu
    {
        private static Menu MenuRef;
        private static MenuScreen MenuScreenRef;

        internal static Menu PrepareMenu()
        {
            var menu = new Menu("按键绑定", new Element[]{
                new KeyBind("暂停菜单快捷键" ,CustomKnight.GlobalSettings.Keybinds.OpenSkinList,Id:"PauseMenuHotkey"),
                new KeyBind("重新加载皮肤快捷键" ,CustomKnight.GlobalSettings.Keybinds.ReloadSkins,Id:"ReloadSkinsHotkey"),
            });
            return menu;
        }

        internal static MenuScreen GetMenu(MenuScreen lastMenu)
        {
            if (MenuScreenRef == null)
            {
                MenuRef ??= PrepareMenu();
                MenuScreenRef = MenuRef.GetMenuScreen(lastMenu);

            }
            else
            {
                MenuRef.returnScreen = lastMenu;
            }
            return MenuScreenRef;
        }
    }
}
