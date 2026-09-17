using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;
using Satchel;
using System.Linq;

namespace EasySoulRefill
{
    public class EasySoulRefill : Mod, IMenuMod, ITogglableMod
    {
        internal static EasySoulRefill Instance;

        public override string GetVersion() => "v1.0.0.2";

        private string[] dream_boss_scene_names = {
            "Dream_01_False_Knight",
            "Dream_02_Mage_Lord",
            "Dream_03_Infected_Knight",
            "Dream_04_White_Defender",
            "Dream_Mighty_Zote",
            "dryya overworld",
            "zemer overworld arena",
            "hegemol overworld arena",
            "isma overworld"};

        public EasySoulRefill() : base("轻松灵魂填充")
        {
            Instance = this;
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            ModHooks.SetPlayerBoolHook += OnSetBool;
            On.GameManager.OnNextLevelReady += OnSceneLoad;
        }

        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "在长椅休息或进入梦境 Boss 房间后自动恢复灵魂。", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
            };
        }

        public void Unload()
        {
            ModHooks.SetPlayerBoolHook -= OnSetBool;
            On.GameManager.OnNextLevelReady -= OnSceneLoad;
        }

        public bool OnSetBool(string name, bool orig)
        {
            if ((name == "atBench") && orig)
            {
                Satchel.CoroutineHelper.WaitForSecondsBeforeInvoke(0.5f, RefillSoul);
            }
            return orig;
        }

        
        public void OnSceneLoad(On.GameManager.orig_OnNextLevelReady orig, global::GameManager self)
        {
            orig(self);
            string scene_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            if (dream_boss_scene_names.Contains(scene_name))
            {
                Satchel.CoroutineHelper.WaitForSecondsBeforeInvoke(0.5f, RefillSoul);
            }
        }

        public void RefillSoul()
        {
            HeroController.instance.AddMPCharge(200);
            // 原版已知缺陷：玩家持有灵魂容器时，重生后灵魂计量器 HUD 会显示错误
            // （原作者 issue #1 "HUD issues"）。TakeReserveMP(0) 让游戏"以为"灵魂
            // 容器被抽取从而刷新 HUD，实际抽取量为 0，不会损失灵魂。
            HeroController.instance.TakeReserveMP(0);
        }
    }
}