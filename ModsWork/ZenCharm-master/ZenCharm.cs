using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using SFCore;
using SFCore.Utils;
using HutongGames.PlayMaker;
namespace ZenCharm
{
    public class Settings {
        public EasyCharmState CharmState;
    }
    public class ZenCharm : Mod, ILocalSettings<Settings>, IMenuMod, ITogglableMod
    {
        internal static ZenCharm Instance { get; private set; }
        public ZenCharm() : base("禅意护符") { }
        private bool Initialized = false;
        // Joni Health 状态机的包装只需做一次
        private bool JoniFsmSetup = false;
        private EasyCharm Charm = new Charm();
        public override string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        public override void Initialize()
        {
            Instance = this;
            Initialized = false;
            On.GameManager.ReturnToMainMenu += OnReturnToMainMenu;
            On.GameManager.OnNextLevelReady += OnNextLevelReady;
            On.PlayerData.UpdateBlueHealth += OnUpdateBlueHealth;
            On.HeroController.CharmUpdate += OnCharmUpdate;
        }
        public void Unload()
        {
            On.GameManager.ReturnToMainMenu -= OnReturnToMainMenu;
            On.GameManager.OnNextLevelReady -= OnNextLevelReady;
            On.PlayerData.UpdateBlueHealth -= OnUpdateBlueHealth;
            On.HeroController.CharmUpdate -= OnCharmUpdate;
            On.HeroController.MaxHealth -= InterceptMaxHealth;
            On.GameManager.OnNextLevelReady -= OnNextLevelReady;
            Instance = null;
            Initialized = false;
            JoniFsmSetup = false;
        }
        public bool ToggleButtonInsideMenu => true;
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "启用禅意护符，允许在无长椅时更换其他护符。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
            };
        }
        private IEnumerator OnReturnToMainMenu(On.GameManager.orig_ReturnToMainMenu orig, global::GameManager self, global::GameManager.ReturnToMainMenuSaveModes saveMode, Action<bool> callback) {
            var result = orig(self, saveMode, callback);
            if (result != null) {
                Initialized = false;
                JoniFsmSetup = false;
                On.GameManager.OnNextLevelReady += OnNextLevelReady;
            }
            return result;
        }
        private void OnUpdateBlueHealth(On.PlayerData.orig_UpdateBlueHealth orig, global::PlayerData self) {
            if (PlayerData.instance.GetBool($"equippedCharm_{Charm.Id}") && !PlayerData.instance.GetBool("atBench")) {
                return;
            }
            orig(self);
        }
        private void OnNextLevelReady(On.GameManager.orig_OnNextLevelReady orig, global::GameManager self) {
            SetupCharmFSM();
            SetupJoniHealthFSM();
            GiveCharm();
            On.GameManager.OnNextLevelReady -= OnNextLevelReady;
            orig(self);
        }
        public void OnCharmUpdate(On.HeroController.orig_CharmUpdate orig, global::HeroController self)
        {
            if (PlayerData.instance.GetBool($"equippedCharm_{Charm.Id}") && !PlayerData.instance.GetBool("atBench")) {
                int currentHealth = self.playerData.health;
                int currentMaxHealth = self.playerData.maxHealth;
                var invGo = GameCameras.instance.hudCamera.gameObject.Find("Inventory");
                var charmsGo = invGo.Find("Charms");
                var charmFsm = charmsGo.LocateMyFSM("UI Charms");
                int charmTarget = charmFsm.FsmVariables.FindFsmInt("Current Item Number").Value;
                On.HeroController.MaxHealth += InterceptMaxHealth;
                orig(self);
                On.HeroController.MaxHealth -= InterceptMaxHealth;
                if (charmTarget == 27 && !PlayerData.instance.GetBool($"equippedCharm_23")) {
                    int currentJoniHealth = self.playerData.healthBlue;
                    self.playerData.maxHealth = self.playerData.maxHealthBase;
                    self.playerData.healthBlue -= (int)(self.playerData.maxHealthBase * 1.4);
                    self.playerData.health = (int)Math.Round(currentJoniHealth / 1.4);
                }
                if (PlayerData.instance.GetBool($"equippedCharm_{27}")) {
                    self.playerData.joniHealthBlue = 0;
                    self.playerData.maxHealth = currentMaxHealth;
                    if (PlayerData.instance.GetBool($"equippedCharm_23") && !PlayerData.instance.GetBool("broken_charm_23")) {
                        self.playerData.maxHealth = self.playerData.maxHealthBase + 2;
                    }
                }
                int overHealthStartIdx = currentMaxHealth + 1;
                for (int i=overHealthStartIdx; i <= self.playerData.maxHealth; i++) {
                    var healthGo = GameCameras.instance.hudCamera.gameObject.Find($"Health {i}");
                    var someRenderer = healthGo.GetComponent<MeshRenderer>();
                    someRenderer.enabled = true;
                }
            } else {
                orig(self);
            }
        }
        public void InterceptMaxHealth(On.HeroController.orig_MaxHealth orig, global::HeroController self) {
        }
        private void GiveCharm() {
            if (!PlayerData.instance.GetBool($"gotCharm_{Charm.Id}")) {
                Modding.Logger.LogDebug("Giving Player ZenCharm");
                Charm.GiveCharm(true);
            }
        }
        // handles when trying to equip charm from UI
        private void SetupCharmFSM() {
            if (Initialized) return;
            try {
                var invGo = GameCameras.instance.hudCamera.gameObject.Find("Inventory");
                var charmsGo = invGo.Find("Charms");
                var charmFsm = charmsGo.LocateMyFSM("UI Charms");
                FsmState deactivateUi = charmFsm.GetState("Deactivate UI");
                var oldAction = charmFsm.GetAction<HutongGames.PlayMaker.Actions.PlayerDataBoolTest>("Deactivate UI", 2);
                if (oldAction == null) return;   // 已经被替换过，避免重复包装
                // 动作一旦被替换出状态，其 Fsm 引用可能失效；这里直接捕获状态机本身，
                // 事件对象也一并取出，避免在 lambda 里访问已失效的原动作。
                var fsm = charmFsm.Fsm;
                var trueEvent = oldAction.isTrue;
                var falseEvent = oldAction.isFalse;
                var action = new MethodAction
                {
                    method = () =>
                    {
                        if (fsm == null) return;
                        if (!PlayerData.instance.GetBool("atBench") && (!PlayerData.instance.GetBool("equippedCharm_" + Charm.Id) || charmFsm.FsmVariables.FindFsmInt("Current Item Number").Value == Charm.Id))
                        {
                            fsm.Event(falseEvent);
                        }
                        else
                        {
                            fsm.Event(trueEvent);
                        }
                    }
                };
                deactivateUi.Actions[2] = action;
                action.Init(deactivateUi);
                Initialized = true;
            } catch {
            }
        }
        // stops health icon showing blue when Joni charm is equpped but not active
        private void SetupJoniHealthFSM() {
            if (JoniFsmSetup) return;   // 只包装一次；重复包装会因取不到原动作而在运行时抛空引用
            try {
                var healthGo = GameCameras.instance.hudCamera.gameObject.Find("Health");
                PlayMakerFSM joniHealthFsm = healthGo.LocateMyFSM("Joni Health");
                FsmState check = joniHealthFsm.GetState("Check");
                var oldAction = joniHealthFsm.GetAction<HutongGames.PlayMaker.Actions.PlayerDataBoolTest>("Check", 0);
                if (oldAction == null) return;
                // 同 SetupCharmFSM：捕获状态机与事件，避免访问已失效的原动作
                var fsm = joniHealthFsm.Fsm;
                var trueEvent = oldAction.isTrue;
                var falseEvent = oldAction.isFalse;
                var action = new MethodAction {
                    method = () => {
                        if (fsm == null) return;
                        if (PlayerData.instance.GetBool($"equippedCharm_27") && (!PlayerData.instance.GetBool($"equippedCharm_{Charm.Id}") || PlayerData.instance.GetBool("atBench"))) {
                            fsm.Event(trueEvent);
                        } else {
                            fsm.Event(falseEvent);
                        }
                    }
                };
                check.Actions[0] = action;
                action.Init(check);
                JoniFsmSetup = true;
            } catch (Exception e) {
                Modding.Logger.LogError("[禅意护符] Joni Health 状态机设置失败：" + e);
            }
        }
        public void OnLoadLocal(Settings settings) {
            if (settings.CharmState != null) {
                Charm.RestoreCharmState(settings.CharmState);
            }
        }
        public Settings OnSaveLocal() {
            Settings settings = new();
            settings.CharmState = Charm.GetCharmState();
            return settings;
        }
    }
    public class Charm : EasyCharm {
        protected override int GetCharmCost() => 3;
        protected override string GetDescription() => "进入禅定状态。\r\n\r\n在地面上可更换护符。";

        protected override string GetName() => "禅意";
        protected override Sprite GetSpriteInternal() {
            Texture2D tex = new Texture2D(2, 2);
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ZenCharm.ZenCharm.png")) {
                if (stream == null) {
                    Modding.Logger.LogError("Failed to find resource!");
                    return null;
                }
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                if (buffer == null) {
                    Modding.Logger.LogError("Failed to load ZenCharm sprite");
                    return null;
                }
                tex.LoadImage(buffer);
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 64f);
        }
    }
}