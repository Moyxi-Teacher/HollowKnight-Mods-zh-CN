using Satchel.BetterMenus;

namespace CustomKnight
{
    internal class AltMenu
    {
        private static Menu MenuRef;
        private static MenuScreen MenuScreenRef;

        internal static void UpdateMenu()
        {
            var skin = SkinManager.GetCurrentSkin();
            if (skin is ISupportsOverrides SkinWithOverrides)
            {
                var hasOverrides = false;
                foreach (var kvp in SkinManager.Skinables)
                {
                    var btn = MenuRef.Find(kvp.Value.name + ".png");
                    if (btn != null)
                    {
                        btn.isVisible = SkinWithOverrides.HasOverrides(kvp.Value.name + ".png");
                        hasOverrides = hasOverrides || btn.isVisible;
                    }
                }
                MenuRef.Find("helpText").isVisible = !hasOverrides;
                MenuRef.Update();
            }
        }
        internal static Menu PrepareMenu()
        {
            var menu = new Menu("皮肤的备用贴图", new Element[]{
                new MenuButton("应用备用贴图", "应用当前选中的备用贴图",ApplyAlts),
                new TextPanel("未检测到备用贴图，请更新 skin-config.json 配置文件",Id : "helpText") { isVisible = false}
            });
            foreach (var kvp in SkinManager.Skinables)
            {
                menu.AddElement(ChangeAltButton(kvp.Value.name + ".png"));
            }
            return menu;
        }

        private static void ApplyAlts(UnityEngine.UI.MenuButton obj)
        {
            TextureCache.clearSkinTextureCache(SkinManager.CurrentSkin.GetId());
            SkinManager.RefreshSkin(true);
        }

        private static MenuButton ChangeAltButton(string name)
        {
            var ButtonText = name;
            return new MenuButton(ButtonText, "", (mb) =>
            {
                var skin = SkinManager.GetCurrentSkin();
                if (skin is ISupportsOverrides SkinWithOverrides)
                {
                    if (SkinWithOverrides.HasOverrides(name))
                    {
                        var options = SkinWithOverrides.GetAllOverrides(name);
                        var currentOverride = SkinWithOverrides.GetOverride(name);
                        var currentOverrideIndex = Array.FindIndex(options, (i) => i == currentOverride);
                        currentOverrideIndex++;
                        if (currentOverrideIndex > options.Length - 1)
                        {
                            currentOverrideIndex = 0;
                        }
                        SkinWithOverrides.SetOverride(name, options[currentOverrideIndex]);
                        var btn = MenuRef.Find(name);
                        btn?.updateAfter((e) =>
                            {
                                btn.Name = SkinWithOverrides.GetOverride(name);
                            });
                    }
                }
            }, Id: name)
            { isVisible = false };
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
            UpdateMenu();
            return MenuScreenRef;
        }
    }
}
