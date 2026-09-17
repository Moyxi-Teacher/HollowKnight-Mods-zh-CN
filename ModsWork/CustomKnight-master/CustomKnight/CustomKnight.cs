using CustomKnight.NewUI;
using CustomKnight.Skin.Swapper;

namespace CustomKnight
{
    /// <summary>
    /// 模组主类
    /// </summary>
    public class CustomKnight : Mod, IGlobalSettings<GlobalModSettings>, ILocalSettings<SaveModSettings>, ICustomMenuMod, ITogglableMod
    {
        /// <summary>
        /// 全局设置
        /// </summary>
        public static GlobalModSettings GlobalSettings { get; set; } = new GlobalModSettings();
        /// <summary>
        /// 当前存档的设置
        /// </summary>
        public static SaveModSettings SaveSettings { get; set; } = new SaveModSettings();
        /// <summary>
        /// 当前模组实例
        /// </summary>
        public static CustomKnight Instance { get; private set; }
        /// <summary>
        /// 当前 DumpManager 实例
        /// </summary>
        public static DumpManager dumpManager { get; private set; } = new DumpManager();
        /// <summary>
        /// 当前 SwapManager 实例
        /// </summary>
        public static SwapManager swapManager { get; private set; } = new SwapManager();
        /// <summary>
        /// 存储预加载的游戏对象
        /// </summary>
        public static readonly Dictionary<string, GameObject> GameObjects = new();
        /// <summary>
        /// 自定义小骑士完全就绪（应用皮肤完成）后触发的事件
        /// </summary>
        public static event EventHandler<EventArgs> OnReady;
        /// <summary>
        /// 初始化完成、尚未应用皮肤时触发的事件
        /// </summary>
        public static event EventHandler<EventArgs> OnInit;
        /// <summary>
        /// 模组被卸载时触发的事件
        /// </summary>
        public static event EventHandler<EventArgs> OnUnload;
        /// <summary>
        /// 总开关按钮是否位于模组菜单内部
        /// </summary>
        public bool ToggleButtonInsideMenu { get; } = true;
        internal static void touchSatchelVersion()
        {
            Satchel.AssemblyUtils.Version();
        }
        internal static bool isSatchelInstalled()
        {
            var isInstalled = false;
            try
            {
                touchSatchelVersion();
                isInstalled = true;
            }
            catch (Exception e)
            {
                Modding.Logger.Log(e);
            }
            return isInstalled;
        }
        internal void getVersionSafely()
        {
            version = Satchel.AssemblyUtils.GetAssemblyVersionHash();
        }

        internal string version;
        /// <summary>
        /// 获取模组显示名称
        /// </summary>
        /// <returns></returns>
        public new string GetName() => "自定义小骑士";

        /// <summary>
        /// 获取 Mods 列表中用于进入本模组菜单的按钮文本
        /// </summary>
        public override string GetMenuButtonText() => "自定义小骑士";

        /// <summary>
        /// 获取模组版本
        /// </summary>
        /// <returns></returns>
        public override string GetVersion()
        {
            version = "未找到 Satchel";
            if (isSatchelInstalled())
            {
                getVersionSafely();
            }
            return version;
        }
        /// <summary>
        /// 读取全局设置时调用
        /// </summary>
        /// <param name="s"></param>
        public void OnLoadGlobal(GlobalModSettings s)
        {
            if (s.Version == GetVersion())
            {
                CustomKnight.GlobalSettings = s;
            }
            else
            {
                var DefaultSettings = new GlobalModSettings();
                CustomKnight.GlobalSettings = s;
                CustomKnight.GlobalSettings.Version = GetVersion();
                CustomKnight.GlobalSettings.NameLength = DefaultSettings.NameLength;
            }
        }
        /// <summary>
        /// 保存全局设置时调用
        /// </summary>
        /// <returns></returns>
        public GlobalModSettings OnSaveGlobal()
        {
            return CustomKnight.GlobalSettings;
        }
        /// <summary>
        /// 读取存档设置时调用
        /// </summary>
        /// <param name="s"></param>
        public void OnLoadLocal(SaveModSettings s)
        {
            CustomKnight.SaveSettings = s;
        }
        /// <summary>
        /// 保存存档设置时调用
        /// </summary>
        /// <returns></returns>
        public SaveModSettings OnSaveLocal()
        {
            return CustomKnight.SaveSettings;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomKnight() : base("自定义小骑士")
        {
            // 需要尽早挂接的钩子
            PreloadedTk2dSpritesHandler.Hook();
            SpriteFlashManager.Hook();
            CinematicsManager.Hook();
        }

        /// <summary>
        /// 获取需要预加载的场景对象
        /// </summary>
        /// <returns></returns>
        public override List<(string, string)> GetPreloadNames()
        {
            List<(string, string)> preloadsList;
            if (GlobalSettings.Preloads || GlobalSettings.GenerateDefaultSkin)
            {
                preloadsList = new List<(string, string)>
                {
                    ("Abyss_10", "higher_being/Dish Plat/Knight Dummy"),
                    ("Abyss_12", "Scream 2 Get/Cutscene Knight"),
                    ("Abyss_21", "Shiny Item DJ/Knight Cutscene"),
                    ("Crossroads_50", "Quirrel Lakeside/Sit Region/Knight Sit"),
                    ("Deepnest_East_12", "Hornet Blizzard Return Scene"),
                    ("Deepnest_Spider_Town", "RestBench Spider/Webbed Knight"),
                    ("Dream_Abyss", "End Cutscene/Dummy"),
                    ("GG_Door_5_Finale", "abyss_door_5_cutscene_sequence/main_chars"),
                    ("GG_Vengefly", "Boss Scene Controller/Dream Entry/Knight Dream Arrival"),
                    ("RestingGrounds_07", "Dream Moth/Knight Dummy"),

                };
            }
            else
            {
                preloadsList = new List<(string, string)>();
            }
            if (GlobalSettings.GenerateDefaultSkin)
            {
                preloadsList.AddRange(new List<(string, string)>
                {
                    ("GG_Vengefly","Giant Buzzer Col")
                });
            }
            return preloadsList;
        }
        /// <summary>
        /// 模组初始化
        /// </summary>
        /// <param name="preloadedObjects"></param>
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            Log($"正在初始化 自定义小骑士 {version}");
            // 确保 skinables 列表已就绪
            Log($"发现 {SkinManager.Skinables.Count} 个可替换部件（Skinables）");
            PreloadedTk2dSpritesHandler.Unhook();
            PreloadedTk2dSpritesHandler.Enable();
            SaveHud.Hook();
            OnInit?.Invoke(this, null);

            if (!isSatchelInstalled())
            {
                return;
            }
            SkinManager.checkDirectoryStructure();
            SkinManager.getSkinNames();
            SaveHud.LoadAll();
            SkinManager.CurrentSkin = SkinManager.GetSkinById(CustomKnight.GlobalSettings.DefaultSkin);
            SkinManager.SetSkinById(CustomKnight.GlobalSettings.DefaultSkin);
            // 首次加载
            if (preloadedObjects != null)
            {
                if (preloadedObjects["GG_Vengefly"].TryGetValue("Giant Buzzer Col", out var enemy))
                {
                    var hm = enemy.GetComponent<HealthManager>();
                    var prefab = ReflectionHelper.GetField<HealthManager, GameObject>(hm, "largeGeoPrefab");
                    (SkinManager.Skinables[Geo.NAME] as Geo).SetGeoDefaultTexture(prefab);
                }
                GameObjects.Add("Cloak", preloadedObjects["Abyss_10"]["higher_being/Dish Plat/Knight Dummy"]);
                GameObjects.Add("Shriek", preloadedObjects["Abyss_12"]["Scream 2 Get/Cutscene Knight"]);
                GameObjects.Add("Wings", preloadedObjects["Abyss_21"]["Shiny Item DJ/Knight Cutscene"]);
                GameObjects.Add("Quirrel", preloadedObjects["Crossroads_50"]["Quirrel Lakeside/Sit Region/Knight Sit"]);
                GameObjects.Add("Hornet", preloadedObjects["Deepnest_East_12"]["Hornet Blizzard Return Scene"]);
                GameObjects.Add("Webbed", preloadedObjects["Deepnest_Spider_Town"]["RestBench Spider/Webbed Knight"]);
                GameObjects.Add("Birthplace", preloadedObjects["Dream_Abyss"]["End Cutscene/Dummy"]);
                GameObjects.Add("DreamArrival", preloadedObjects["GG_Vengefly"]["Boss Scene Controller/Dream Entry/Knight Dream Arrival"]);
                GameObjects.Add("Dreamnail", preloadedObjects["RestingGrounds_07"]["Dream Moth/Knight Dummy"]);
            }

            if (CustomKnight.GlobalSettings.SwapperEnabled)
            {
                swapManager.enabled = true;
                swapManager.active = true;
            }


            InputListener.Start();
            if (CustomKnight.GlobalSettings.EnablePauseMenu)
            {
                UIController.EnableMenu();
            }

            if (CustomKnight.GlobalSettings.GenerateDefaultSkin)
            {
                DefaultSkin.SaveSkin();
            }
            else
            {
                DefaultSkin.isGeneratingDefaultSkin = false;
            }
            On.HeroController.Start += HeroControllerStart;
        }

        /// <summary>
        /// 获取本模组的设置菜单（Mods 列表的二级菜单）
        /// </summary>
        /// <param name="modListMenu"></param>
        /// <param name="toggle"></param>
        /// <returns></returns>
        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggle)
        {
            return BetterMenu.GetMenu(modListMenu, toggle);
        }

        internal void HeroControllerStart(On.HeroController.orig_Start orig, HeroController self)
        {
            orig(self);
            Log("英雄控制器启动（HeroController.Start）");
            var currentSkinId = (SaveSettings.DefaultSkin != GlobalSettings.DefaultSkin && SaveSettings.DefaultSkin != null) ? SaveSettings.DefaultSkin : GlobalSettings.DefaultSkin;
            SkinManager.CurrentSkin = SkinManager.GetSkinById(currentSkinId);
            GlobalModSettings.SetSkinForProfileID(GameManager.instance.profileID, currentSkinId);
            SkinManager.LoadSkin();
            OnReady?.Invoke(this, null);
        }


        internal static void toggleSwap(bool enable)
        {
            swapManager.enabled = enable;
            if (!enable)
            {
                swapManager.Unhook();
                dumpManager.Unhook();
                PreloadedTk2dSpritesHandler.Disable();
            }
            else
            {
                swapManager.Hook();
                dumpManager.Hook();
                PreloadedTk2dSpritesHandler.Enable();
            }
        }

        /// <summary>
        /// 卸载模组
        /// </summary>
        public void Unload()
        {
            SaveHud.UnHook();
            SkinManager.Unload();
            OnUnload?.Invoke(this, null);
            On.HeroController.Start -= HeroControllerStart;
        }

        internal static void SaveSprite(Sprite s, string str)
        {
            var tex = SpriteUtils.ExtractTextureFromSprite(s);
            CustomKnight.dumpManager.SaveTextureByPath("Debug", str, tex);
        }
    }
}
