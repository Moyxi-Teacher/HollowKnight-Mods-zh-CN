using Modding;
using SFCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ExtraNotchesForExtraCharms
{
    public class ExtraNotchesForExtraCharms : Mod, IGlobalSettings<GlobalSettings>, ITogglableMod, IMenuMod
    {
        internal static ExtraNotchesForExtraCharms Instance;

        public override string GetVersion() => "1.1.0.0";

        public ExtraNotchesForExtraCharms() : base("额外护符槽位") {}

        #region Global Settings
        internal static GlobalSettings globalSettings = new GlobalSettings();

        public void OnLoadGlobal(GlobalSettings s)
        {
            globalSettings = s;
        }

        public GlobalSettings OnSaveGlobal()
        {
            return globalSettings;
        }
        #endregion

        /// <summary>
        /// Stores the number of extra notches currently applied
        /// </summary>
        private int extraNotches = 0;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;

            On.HeroController.Start += OnStart;
            On.GameManager.SaveGame += OnSave;
            On.GameManager.ReturnToMainMenu += OnQuit;

            Log("Initialized");
        }

        /// <summary>
        /// 卸载时移除所有钩子，确保能正确禁用模组效果
        /// </summary>
        public void Unload()
        {
            Instance = null;
            On.HeroController.Start -= OnStart;
            On.GameManager.SaveGame -= OnSave;
            On.GameManager.ReturnToMainMenu -= OnQuit;
        }

        /// <summary>
        /// We will add charm notches once, when a save is loaded
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void OnStart(On.HeroController.orig_Start orig, HeroController self)
        {
            orig(self);

            Log($"Starting slots: {PlayerData.instance.charmSlots}");
            extraNotches = GetExtraNotches();
            PlayerData.instance.charmSlots += extraNotches;
            //Log($"New slots: {PlayerData.instance.charmSlots}");
            GameManager.instance.RefreshOvercharm();
        }

        private void OnSave(On.GameManager.orig_SaveGame orig, GameManager self)
        {
            PlayerData.instance.charmSlots -= extraNotches;
            //Log($"Slots reset to {PlayerData.instance.charmSlots}");
            orig(self);

            extraNotches = GetExtraNotches();
            PlayerData.instance.charmSlots += extraNotches;
            //Log($"Slots increased by {extraNotches} -> {PlayerData.instance.charmSlots}");
            GameManager.instance.RefreshOvercharm();
        }

        /// <summary>
        /// Naturally, we will remove the notches when a save is closed so they don't stack between runs
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="saveMode"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator OnQuit(On.GameManager.orig_ReturnToMainMenu orig, GameManager self, 
                                    GameManager.ReturnToMainMenuSaveModes saveMode, Action<bool> callback)
        {
            PlayerData.instance.charmSlots -= extraNotches;
            extraNotches = 0;
            //Log($"Slots reset on quit: {PlayerData.instance.charmSlots}");
            GameManager.instance.RefreshOvercharm();
            return orig(self, saveMode, callback);
        }

        /// <summary>
        /// 计算本次应额外增加的护符槽位数量。
        /// 修改(2026-09-20)：只统计玩家**已经解锁**的额外护符，
        /// 每解锁一个就增加 notchesPerCharm 个槽位；一个都没解锁则不增加。
        /// </summary>
        /// <returns></returns>
        private int GetExtraNotches()
        {
            FieldInfo fieldInfo = typeof(CharmHelper).GetField("CustomSprites",
                                                                BindingFlags.NonPublic | BindingFlags.Static);
            List<Sprite> customCharmList = (List<Sprite>)fieldInfo.GetValue(null);
            int maxCharms = customCharmList.Count;

            int unlocked = CountUnlockedModdedCharms(maxCharms);
            int extraNotches = unlocked * globalSettings.notchesPerCharm;
            Log($"已解锁 {unlocked}/{maxCharms} 个额外护符，每个 +{globalSettings.notchesPerCharm} 槽位 -> 共 {extraNotches} 个额外护符槽位");

            return extraNotches;
        }

        /// <summary>
        /// 统计玩家已经解锁（获得）的额外护符数量。
        /// 模组新增的护符编号从 41 开始（SFCore 的约定：第 i 个护符的编号是 41 + i），
        /// 解锁状态记录在 PlayerData 的 gotCharm_41、gotCharm_42 …… 字段中。
        /// 苍白宫廷、禅意护符等模组都按这个约定读写（部分模组用钩子映射到自己的存档字段）。
        /// </summary>
        /// <param name="maxCharms">模组新增的护符总数</param>
        /// <returns></returns>
        private int CountUnlockedModdedCharms(int maxCharms)
        {
            int unlocked = 0;
            for (int i = 0; i < maxCharms; i++)
            {
                int charmId = 41 + i;
                try
                {
                    if (PlayerData.instance.GetBool("gotCharm_" + charmId))
                    {
                        unlocked++;
                    }
                }
                catch (Exception e)
                {
                    // 极少数护符没有对应的存档字段：按未解锁处理，不影响其余护符
                    Log($"读取 gotCharm_{charmId} 失败，按未解锁处理：{e.Message}");
                }
            }
            return unlocked;
        }

        #region Menu Options
        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "启用或禁用额外护符槽位功能。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader),
                new("每个护符增加的槽位数", new[]{"1","2","3","4","5","6","7","8","9","10"}, "每解锁一个额外护符，增加的护符槽位数量。", i => globalSettings.notchesPerCharm = i + 1, () => globalSettings.notchesPerCharm - 1)
            };
        }
        #endregion
    }
}