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

        public override string GetVersion() => "1.0.0.0";

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
        /// Calculates the number of extra notches to add
        /// </summary>
        /// <returns></returns>
        private int GetExtraNotches()
        {
            FieldInfo fieldInfo = typeof(CharmHelper).GetField("CustomSprites",
                                                                BindingFlags.NonPublic | BindingFlags.Static);
            List<Sprite> customCharmList = (List<Sprite>)fieldInfo.GetValue(null);
            int maxCharms = customCharmList.Count;
            int extraNotches = maxCharms / globalSettings.charmsPerNotch;
            Log($"{maxCharms} modded charms -> {extraNotches} extra charm notches");

            return extraNotches;
        }

        #region Menu Options
        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "启用或禁用额外护符槽位功能。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader),
                new("每护符槽位数", new[]{"1","2","3","4","5","6","7","8","9","10"}, "每个护符槽位可容纳的额外护符数量。", i => globalSettings.charmsPerNotch = i + 1, () => globalSettings.charmsPerNotch - 1)
            };
        }
        #endregion
    }
}