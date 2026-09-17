using Satchel.BetterMenus;

namespace MapChanger;

internal static class ModMenu
{
    internal static MenuScreen GetMenuScreen(MenuScreen modListMenu)
    {
        Menu menu =
            new(
                "地图变更".L(),
                [
                    new HorizontalOption(
                        "显示暂停菜单".L(),
                        string.Empty,
                        ["关闭".L(), "开启".L()],
                        i => MapChangerMod.GS.ToggleShowPauseMenu(),
                        () => MapChangerMod.GS.ShowPauseMenu ? 1 : 0
                    ),
                ]
            );

        return menu.GetMenuScreen(modListMenu);
    }
}
