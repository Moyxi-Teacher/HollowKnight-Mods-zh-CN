using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
using MonoMod.ModInterop;
using UnityEngine;

namespace InvincibilityMonitor
{
    public class InvincibilityMonitor : Mod, IGlobalSettings<GlobalSettings>, IMenuMod, ITogglableMod
    {
        internal static InvincibilityMonitor Instance;

        public bool ToggleButtonInsideMenu => true;

        // 仅返回一条总开关条目（说明文写进 Description），避免重复条目导致菜单渲染时 NullReferenceException。
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> entries = new List<IMenuMod.MenuEntry>();
            if (toggleButtonEntry != null)
            {
                // MenuEntry 是 struct，修改副本后再加入，才不会动到传入的 toggleButtonEntry。
                IMenuMod.MenuEntry entry = toggleButtonEntry.Value;
                entry.Description = "显示玩家当前无敌状态及相关条件的监控面板。";
                entries.Add(entry);
            }
            return entries;
        }

        public void Unload()
        {
            InvincibilityCondition.Unload();
            Instance = null;
        }

        public InvincibilityMonitor() : base("无敌监控") { Instance = this; }

        public static GlobalSettings GS = new();
        public void OnLoadGlobal(GlobalSettings gs) => GS = gs;
        public GlobalSettings OnSaveGlobal() => GS;

        public override string GetVersion()
        {
            return Vasi.VersionUtil.GetVersion<InvincibilityMonitor>();
        }

        public override void Initialize()
        {
            // Doesn't change anything
            DebugMod.CreateSimpleInfoPanel("InvincibilityMonitor.Info", 220);

            DebugMod.AddInfoToSimplePanel("InvincibilityMonitor.Info", "Invincibility State", GetInvincibilityState);
            DebugMod.AddInfoToSimplePanel("InvincibilityMonitor.Info", null, null);

            InvincibilityCondition.Setup();
        }

        public string GetInvincibilityState()
        {
            return InvincibilityCondition.AnyConditionInvincible ? "Invincible"
                : InvincibilityCondition.TimerSafety ? "Waiting"
                : "Vulnerable";
        }
    }
}