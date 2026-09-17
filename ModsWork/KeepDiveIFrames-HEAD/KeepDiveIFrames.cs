using Modding;
using System.Collections.Generic;
using UnityEngine;
using Satchel;

namespace KeepDiveIFrames
{
    public class KeepDiveIFrames : Mod, IMenuMod, ITogglableMod
    {
        internal static KeepDiveIFrames instance;

        public KeepDiveIFrames() : base("保持下冲无敌帧")
        {
            instance = this;
        }

        // 游戏内菜单显示为开关按钮（而非子菜单）
        public bool ToggleButtonInsideMenu => true;

        // 仅返回一条总开关条目（说明文写进 Description），避免重复条目导致菜单渲染时 NullReferenceException。
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> entries = new List<IMenuMod.MenuEntry>();
            if (toggleButtonEntry != null)
            {
                IMenuMod.MenuEntry entry = toggleButtonEntry.Value;
                entry.Description = "按住下冲键时移除前摇动作，从而保持下冲的无敌帧，避免在下冲期间受到伤害。";
                entries.Add(entry);
            }
            return entries;
        }

        public override string GetVersion() => "1.0.0.1";

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            instance.Log("Initializing");
            On.PlayMakerFSM.OnEnable += editFSM;
            instance.Log("Initialized");
        }

        // 关闭时移除钩子，实现真正的总开关
        public void Unload()
        {
            On.PlayMakerFSM.OnEnable -= editFSM;
            instance = null;
        }

        /// <summary>
        /// Once the player has beaten THK, the Infection enters stage 2 and starts to empower enemies throughout the map like it does in Crossroads
        /// </summary>
        private void editFSM(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);
            // 移除"Quake Antic"（下冲前摇）状态的第二个 Action，从而保持下冲时的无敌帧
            if (self.gameObject.name == "Knight" && self.FsmName == "Spell Control")
            {
                self.GetValidState("Quake Antic").RemoveAction(1);
            }
        }
    }
}
