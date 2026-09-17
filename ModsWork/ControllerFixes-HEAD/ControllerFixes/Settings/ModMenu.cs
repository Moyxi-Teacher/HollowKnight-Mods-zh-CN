namespace ControllerFixes;

public static class ModMenu
{
    private static Menu MenuRef;
    private static GlobalSettings Settings => ControllerFixes.Settings;
    private static readonly List<string> Descriptions =
    [
        "普通手柄提示",
        "Joy-Con 外观，任天堂提示",
        "DualShock 4、PlayStation 提示（菜单按键提示：触摸板）",
        "DualShock 4、PlayStation 提示（菜单按键提示：Share 按钮）",
        "DualShock 4、PlayStation 提示（菜单按键提示：Back/Select 按钮）",
        "Xbox One、Xbox 按钮提示（菜单按键提示：View）",
        "Xbox 360、Xbox 按钮提示（菜单按键提示：Back/Select）",
    ];

    public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
    {
        MenuRef = new Menu("控制器修正",
        [
            new MenuButton("链接至说明",
                "如果有任何不清楚之处，请阅读本模组所基于的 Steam 讨论串",
                _ => Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=2525497719")),

            Blueprints.HorizontalBoolOption("禁用施法",
                "从聚焦按键/键中移除施法动作",
                b => Settings.NoCast = b,
                () => Settings.NoCast),

            new HorizontalOption("按钮皮肤类型",
                Descriptions[Settings.ButtonSkinType + 1],
                ["默认", "1", "2", "3", "4", "5", "6"],
                (i) =>
                {
                    Settings.ButtonSkinType = i - 1;

                    (MenuRef.Find("ButtonPrompt") as HorizontalOption)!.Description = Descriptions[i];
                    MenuRef.Update();
                },
                () => Settings.ButtonSkinType + 1,
                Id: "ButtonPrompt"),

            Blueprints.HorizontalBoolOption("仅模拟 Xbox",
                "强制游戏使用 Xbox 360 手柄",
                b => Settings.EmulatedXboxOnly = b,
                () => Settings.EmulatedXboxOnly),

            Blueprints.HorizontalBoolOption("Steam 横掌布局",
                "使用 Steam 的横掌手柄布局",
                b => Settings.SteamNintendoLayout = b,
                () => Settings.SteamNintendoLayout),

            Blueprints.HorizontalBoolOption("覆盖菜单提示",
                "对于菜单按钮，总是显示菜单绑定，即使它与跳跃/聚焦映射冲突。",
                b => Settings.OverrideMenuPrompts = b,
                () => Settings.OverrideMenuPrompts)
        ]);

        return MenuRef.GetMenuScreen(modListMenu);
    }
}