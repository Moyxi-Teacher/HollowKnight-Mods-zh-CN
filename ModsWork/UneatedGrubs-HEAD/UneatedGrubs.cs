using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace UneatedGrubs
{
    public class UneatedGrubs : Mod, IMenuMod, ITogglableMod
    {
        internal static UneatedGrubs Instance;

        //public override List<ValueTuple<string, string>> GetPreloadNames()
        //{
        //    return new List<ValueTuple<string, string>>
        //    {
        //        new ValueTuple<string, string>("White_Palace_18", "White Palace Fly")
        //    };
        //}

        public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public UneatedGrubs() : base("保护幼虫") { }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            if (UneatedGrubs.Instance != null) return;

            UneatedGrubs.Instance = this;

            // hooks
            On.DeactivateIfPlayerdataTrue.OnEnable += DeactivateIfPlayerdataTrue_OnEnable;

            this.Log("Loaded Uneaten Grubs!");
        }

        private void DeactivateIfPlayerdataTrue_OnEnable(On.DeactivateIfPlayerdataTrue.orig_OnEnable orig, DeactivateIfPlayerdataTrue self)
        {
            GameManager gm_hook = GameManager.instance;
            PlayerData pd_hook = gm_hook.playerData;
            pd_hook.SetBool("fatGrubKing", false);
            orig(self);
        }

        public void Unload()
        {
            UneatedGrubs.Instance = null;

            // destroy hooks
            On.DeactivateIfPlayerdataTrue.OnEnable -= DeactivateIfPlayerdataTrue_OnEnable;

            this.Log("Unloaded Uneaten Grubs!");
        }

        // 标准菜单：总开关 + 功能描述（≤25字）
        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values, "保护幼虫的小生命", toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
            };
        }
    }
}