using Satchel.BetterMenus;

namespace CustomKnight
{
    internal class AuthorsMenu
    {
        private static Menu MenuRef;
        private static MenuScreen MenuScreenRef;

        internal static Menu PrepareMenu()
        {
            var menu = new Menu("作者工具", new Element[]{
               new MenuButton("导出贴图","导出 Swapper 所支持替换的精灵图（游戏可能会卡顿）",(_)=>ToggleDumping(),Id:"DumpButton"),
               Blueprints.HorizontalBoolOption(
                    "禁用目录替换","(需要重启游戏)",
                    (v) => {
                        CustomKnight.GlobalSettings.DisableDirectorySwaps = v;
                    },
                    () => CustomKnight.GlobalSettings.DisableDirectorySwaps),
            });

            return menu;
        }

        internal static MenuScreen GetMenu(MenuScreen lastMenu)
        {
            if (MenuScreenRef == null)
            {
                MenuRef ??= PrepareMenu();
                MenuRef.OnBuilt += (_, Element) =>
                {
                    SetDumpButton();
                };
                MenuScreenRef = MenuRef.GetMenuScreen(lastMenu);

            }
            else
            {
                MenuRef.returnScreen = lastMenu;
            }
            return MenuScreenRef;
        }


        internal static void SetDumpButton()
        {
            var btn = MenuRef?.Find("DumpButton");
            btn.Name = CustomKnight.dumpManager.GetIsEnabled() ? "正在导出贴图" : "导出贴图";
            btn.Update();
        }
        internal static void ToggleDumping()
        {
            if (CustomKnight.dumpManager.GetIsEnabled())
            {
                //关闭导出
                CustomKnight.dumpManager.Unhook();
                CustomKnight.swapManager.Hook();
            }
            else
            {
                //开启导出
                CustomKnight.dumpManager.Hook();
                CustomKnight.swapManager.Unhook();
                SkinManager.SetDefaultSkin();
                CustomKnight.dumpManager.DumpAllSprites();
            }
            SetDumpButton();
        }
    }
}
