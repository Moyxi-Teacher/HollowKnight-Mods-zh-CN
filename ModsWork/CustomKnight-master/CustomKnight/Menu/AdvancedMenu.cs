using CustomKnight.NewUI;
using Satchel.BetterMenus;

namespace CustomKnight
{
    internal class AdvancedMenu
    {
        private static Menu MenuRef;
        private static MenuScreen MenuScreenRef;
        // 自动补齐缺失贴图子菜单（点进自动补齐缺失贴图后展开）
        private static Menu AutoFillMenuRef;
        internal static MenuScreen GetMenu(MenuScreen lastMenu)
        {
            if (MenuScreenRef == null)
            {
                if (MenuRef == null)
                {
                    MenuRef = PrepareMenu();
                }
                MenuScreenRef = MenuRef.GetMenuScreen(lastMenu);
            }
            else
            {
                MenuRef.returnScreen = lastMenu;
            }
            return MenuScreenRef;
        }

        private static Menu PrepareMenu()
        {

            var menu = new Menu("高级选项", new Element[]{
                new HorizontalOption(
                    "敌人贴图替换", "将皮肤应用到 Boss、敌人和场景区域",
                    new string[] { "关闭", "开启" },
                    (setting) => { CustomKnight.toggleSwap(setting == 1); },
                    () => CustomKnight.swapManager.enabled ? 1 : 0,
                    Id:"SwapperEnabled"),
                Blueprints.HorizontalBoolOption(
                    "为事件预加载游戏对象","(需要重启游戏)",
                    (v) => {
                        CustomKnight.GlobalSettings.Preloads = v;
                    },
                    () => CustomKnight.GlobalSettings.Preloads),
                Blueprints.HorizontalBoolOption(
                    "启用存档 HUD","(需要重启游戏)",
                    (v) => {
                        CustomKnight.GlobalSettings.EnableSaveHuds = v;
                    },
                    () => CustomKnight.GlobalSettings.EnableSaveHuds),
                Blueprints.HorizontalBoolOption(
                    "启用暂停菜单界面","(需要重启游戏)",
                    (v) => {
                        CustomKnight.GlobalSettings.EnablePauseMenu = v;
                        if (CustomKnight.GlobalSettings.EnablePauseMenu)
                        {
                            UIController.EnableMenu();
                        } else
                        {
                            UIController.DisableMenu();
                        }
                    },
                    () => CustomKnight.GlobalSettings.EnablePauseMenu),
                // ===== 皮肤加载性能优化（平滑加载）：将换肤时的贴图读取解码分散到逐帧完成，避免一次全量解码造成卡顿 =====
                new HorizontalOption(
                    "平滑皮肤加载", "将换肤贴图分散到逐帧解码，切换皮肤更流畅；关闭则恢复原版一次性同步加载（切换结束稍快但会卡顿）",
                    new string[] { "关闭", "开启" },
                    (setting) => { CustomKnight.GlobalSettings.SmoothTextureLoading = setting == 1; },
                    () => CustomKnight.GlobalSettings.SmoothTextureLoading ? 1 : 0,
                    Id:"SmoothTextureLoading"),
                new CustomSlider(
                    "每帧解码贴图张数",
                    (v) => { CustomKnight.GlobalSettings.SmoothTextureBatchSize = Mathf.RoundToInt(v); },
                    () => CustomKnight.GlobalSettings.SmoothTextureBatchSize,
                    1f, 10f, true,
                    "平滑加载时每帧最多解码的贴图张数；数字越大换肤越快、单帧占用越高"),
                new CustomSlider(
                    "解码时间预算（毫秒）",
                    (v) => { CustomKnight.GlobalSettings.SmoothTextureBudgetMs = Mathf.RoundToInt(v); },
                    () => CustomKnight.GlobalSettings.SmoothTextureBudgetMs,
                    2f, 64f, true,
                    "平滑加载时每帧解码允许的时间预算；数字越大换肤越快、卡顿越明显"),
                // ===== 自动补齐缺失贴图：点进该选项进入子菜单，现出四个子选项，每个一行 =====
                Blueprints.NavigateToMenu("自动补齐缺失贴图", "点击进入自动补齐缺失贴图分类设置",
                    () => GetAutoFillSubMenu(MenuRef.menuScreen)),
                new MenuRow(
                    new List<Element>{
                        new MenuButton("生成默认皮肤","下次重启时将创建默认皮肤",(_)=>RegenerateDefaultSkin()),
                        new MenuButton("修复皮肤","尝试修复皮肤的目录结构",(_)=>FixSkins()),
                        new MenuButton("重置设置","重置自定义小骑士的全部设置",(_)=>ResetSettings()),
                    },
                    Id:"AdditonalButtonGroup"
                ){ XDelta = 425f},

            });
            return menu;
        }

        /// <summary>
        /// 自动补齐缺失贴图子菜单：点进"自动补齐缺失贴图"选项后进入此页面，
        /// 内含四个单行 HorizontalBoolOption，分别是总开关、小骑士、护符、血条。
        /// </summary>
        private static MenuScreen GetAutoFillSubMenu(MenuScreen lastMenu)
        {
            if (AutoFillMenuRef == null)
            {
                AutoFillMenuRef = PrepareAutoFillMenu();
            }
            else
            {
                AutoFillMenuRef.returnScreen = lastMenu;
            }
            return AutoFillMenuRef.GetMenuScreen(lastMenu);
        }

        private static Menu PrepareAutoFillMenu()
        {
            return new Menu("自动补齐缺失贴图", new Element[]{
                Blueprints.HorizontalBoolOption(
                    "自动补齐缺失贴图总开关", "关闭后所有分类均不自动补充",
                    (v) => { CustomKnight.GlobalSettings.AutoFill = v; },
                    () => CustomKnight.GlobalSettings.AutoFill),
                Blueprints.HorizontalBoolOption(
                    "小骑士", "Geo / Hatchling / Hud / Knight / OrbFull / Sprint / VoidSpells / VS / Wraiths",
                    (v) => { CustomKnight.GlobalSettings.AutoFillKnight = v; },
                    () => CustomKnight.GlobalSettings.AutoFillKnight),
                Blueprints.HorizontalBoolOption(
                    "护符", "Charms 子目录下的护符贴图（Charm_*.png）",
                    (v) => { CustomKnight.GlobalSettings.AutoFillCharms = v; },
                    () => CustomKnight.GlobalSettings.AutoFillCharms),
                Blueprints.HorizontalBoolOption(
                    "血条", "血条/魂条相关贴图（HPBar/ 或 Inventory/Heart_*、Inventory/Vessel_*）",
                    (v) => { CustomKnight.GlobalSettings.AutoFillHPBar = v; },
                    () => CustomKnight.GlobalSettings.AutoFillHPBar),
            });
        }

        private static void Back()
        {
            Utils.GoToMenuScreen(MenuRef.returnScreen);
        }

        private static void RegenerateDefaultSkin()
        {
            CustomKnight.GlobalSettings.GenerateDefaultSkin = true;
            GlobalModSettings.ResetProfileSkins();
            Back();
        }

        private static void ResetSettings()
        {
            CustomKnight.GlobalSettings = new GlobalModSettings();
            if (CustomKnight.SaveSettings != null)
            {
                CustomKnight.SaveSettings = new SaveModSettings();
            }
            Back();
        }


        private static void FixSkins()
        {
            FixSkinStructure.FixSkins();
            BetterMenu.ReloadSkins();

            Back();
        }
    }
}
