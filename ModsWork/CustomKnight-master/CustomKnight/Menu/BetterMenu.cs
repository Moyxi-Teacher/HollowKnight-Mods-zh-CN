using System.Linq;
using CustomKnight.NewUI;
using Satchel.BetterMenus;

namespace CustomKnight
{
    internal static class BetterMenu
    {
        internal static int selectedSkin = 0;
        internal static Menu MenuRef;

        internal static void ApplySkin()
        {
            var skinToApply = SkinManager.SkinsList[selectedSkin];
            SkinManager.SetSkinById(skinToApply.GetId());
        }

        internal static void SelectedSkin(string skinId)
        {
            selectedSkin = SkinManager.SkinsList.FindIndex(skin => skin.GetId() == skinId);
        }

        private static void OpenSkins()
        {
            IoUtils.OpenDefault(SkinManager.SKINS_FOLDER);
        }

        private static void OpenLink(string link)
        {
            Application.OpenURL(link);
        }
        internal static void ReloadSkins()
        {
            CustomKnight.Instance.Log("正在重新应用皮肤");
            // 清空纹理缓存
            TextureCache.clearAllTextureCache();
            // 清空并重置存档 HUD 缓存
            SaveHud.ClearCache();
            // 重置当前皮肤以便重新应用同一个皮肤
            SkinManager.CurrentSkin = null;
            ApplySkin();
            UpdateSkinList();
        }

        internal static void UpdateSkinList()
        {
            SkinManager.getSkinNames();
            MenuRef?.Find("SelectSkinOption").updateAfter((element) =>
            {
                ((HorizontalOption)element).Values = getSkinNameArray();
            });
            UIController.CreateUpdateGUI();
        }

        internal static string[] getSkinNameArray()
        {
            return SkinManager.SkinsList.Select(s => SkinManager.MaxLength(s.GetName(), CustomKnight.GlobalSettings.NameLength)).ToArray();
        }
        internal static Menu PrepareMenu(ModToggleDelegates toggleDelegates)
        {
            return new Menu("自定义小骑士", new Element[]{
                Blueprints.CreateToggle(toggleDelegates,"自定义小骑士", "加载自定义贴图皮肤替换小骑士外观", "开启","关闭"),
                new HorizontalOption(
                    "选择皮肤", "所选皮肤将用于当前存档以及之后新建的存档。",
                    getSkinNameArray(),
                    (setting) => { selectedSkin = setting; },
                    () => selectedSkin,
                    Id:"SelectSkinOption"),
                new TextPanel("想要添加更多皮肤？将皮肤文件夹复制到 Skins 目录即可。"),
                new MenuRow(
                    new List<Element>{
                        Blueprints.NavigateToMenu( "皮肤列表","打开皮肤列表",()=> SkinsList.GetMenu(MenuRef.menuScreen)),
                        Blueprints.NavigateToMenu("备用贴图","配置同一皮肤的备用贴图组",() => AltMenu.GetMenu(MenuRef.menuScreen)),
                    },
                    Id:"ApplyButtonGroup"
                ){ XDelta = 400f},
                new MenuButton("应用皮肤","应用当前选中的皮肤。",(_)=> ApplySkin()),
                new MenuRow(
                    new List<Element>{
                        new MenuButton("打开文件夹","打开皮肤存放文件夹",(_)=>OpenSkins()),
                        new MenuButton("重新加载","重新加载所有皮肤",(_)=>ReloadSkins()),
                        new MenuButton("需要帮助？","加入空洞骑士模组交流 Discord 频道",(_)=>OpenLink("https://discord.gg/J4SV6NFxAA")),
                    },
                    Id:"HelpButtonGroup"
                ){ XDelta = 425f},
                new MenuRow(
                    new List<Element>{
                        Blueprints.NavigateToMenu("高级设置","打开高级设置页面", ()=> AdvancedMenu.GetMenu(MenuRef.menuScreen)),
                        Blueprints.NavigateToMenu("按键绑定","设置各类快捷键", ()=> KeybindsMenu.GetMenu(MenuRef.menuScreen)),
                        Blueprints.NavigateToMenu("工具","供皮肤作者使用的工具", ()=> AuthorsMenu.GetMenu(MenuRef.menuScreen)),
                    },
                    Id:"SubMenuGroup"
                ){ XDelta = 425f},
            });
        }
        internal static MenuScreen GetMenu(MenuScreen lastMenu, ModToggleDelegates? toggleDelegates)
        {
            MenuRef ??= PrepareMenu((ModToggleDelegates)toggleDelegates);
            MenuRef.OnBuilt += (_, Element) =>
            {
                if (SkinManager.CurrentSkin != null)
                {
                    SelectedSkin(SkinManager.CurrentSkin.GetId());
                }
            };
            return MenuRef.GetMenuScreen(lastMenu);
        }
    }
}