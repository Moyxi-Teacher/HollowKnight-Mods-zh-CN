using Modding;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Pogonest {
    public class Pogonest: Mod, IMenuMod, ITogglableMod {
        public Pogonest() : base("打击跳跃") { }
        public override string GetVersion() => "1.0.0.0";

        private static MethodInfo origNailTriggerEnter = typeof(NailSlash).GetMethod("orig_OnTriggerEnter2D", BindingFlags.NonPublic | BindingFlags.Instance);
        private ILHook nailTriggerHook;
        private bool _enabled = true;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            nailTriggerHook = new ILHook(origNailTriggerEnter, nailTrigger);
        }

        /// <summary>
        /// IMenuMod：在模组列表中显示一个开关条目，由模组列表统一控制启用/禁用。
        /// </summary>
        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry) {
            return new() {
                new(
                    toggleButtonEntry.Value.Name,
                    toggleButtonEntry.Value.Values,
                    "启用打击跳跃：向下劈砍时获得下坠加速跳跃，将纳拉（Nail）的下劈命中角度扩大。",
                    toggleButtonEntry.Value.Saver,
                    toggleButtonEntry.Value.Loader)
            };
        }

        /// <summary>
        /// ITogglableMod：支持模组列表中的开关切换。
        /// </summary>
        public bool IsToggleable() => true;

        public void Toggle() {
            _enabled = !_enabled;
            if (_enabled) {
                if (nailTriggerHook == null)
                    nailTriggerHook = new ILHook(origNailTriggerEnter, nailTrigger);
            } else {
                nailTriggerHook?.Dispose();
                nailTriggerHook = null;
            }
        }

        /// <summary>
        /// ITogglableMod：卸载模组时释放 IL 钩子。
        /// </summary>
        public void Unload() {
            nailTriggerHook?.Dispose();
            nailTriggerHook = null;
        }

        private void nailTrigger(ILContext il) {
            ILCursor cursor = new ILCursor(il).Goto(0);
            cursor.GotoNext(i => i.MatchLdcR4(270));
            cursor.GotoNext(i => i.MatchStloc(0));
            cursor.EmitDelegate<Func<Int32, Int32>>(j => { return 11; });
        }
    }
}