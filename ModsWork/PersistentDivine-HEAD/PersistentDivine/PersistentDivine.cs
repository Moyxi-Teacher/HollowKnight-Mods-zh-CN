using Modding;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PersistentDivine
{
    public class PersistentDivineMod : Mod,
        IMenuMod,
        ITogglableMod
    {
        private static PersistentDivineMod? _instance;
        private string? loadingScene;
        private string? previousScene;

        private bool _enabled = true;

        internal static PersistentDivineMod Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"An instance of {nameof(PersistentDivineMod)} was never constructed");
                }
                return _instance;
            }
        }

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public PersistentDivineMod() : base("常驻迪万")
        {
            _instance = this;
        }

        /// <summary>
        /// IMenuMod：在模组列表中显示一个开关条目，由模组列表统一控制启用/禁用。
        /// </summary>
        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new()
            {
                new(
                    toggleButtonEntry.Value.Name,
                    toggleButtonEntry.Value.Values,
                    "启用常驻迪万：驱逐格里姆 troupe 后，迪万的帐篷仍会留在 towns 口袋中。",
                    toggleButtonEntry.Value.Saver,
                    toggleButtonEntry.Value.Loader)
            };
        }

        /// <summary>
        /// ITogglableMod：支持模组列表中的开关切换。
        /// </summary>
        public bool IsToggleable() => true;

        public void Toggle()
        {
            _enabled = !_enabled;
            if (_enabled)
            {
                ModHooks.GetPlayerBoolHook += OnGetPlayerBoolHook;
                ModHooks.BeforeSceneLoadHook += BeforeSceneLoad;
                Log("常驻迪万 已启用。");
            }
            else
            {
                ModHooks.GetPlayerBoolHook -= OnGetPlayerBoolHook;
                ModHooks.BeforeSceneLoadHook -= BeforeSceneLoad;
                Log("常驻迪万 已禁用。");
            }
        }

        /// <summary>
        /// ITogglableMod：卸载模组时移除全部钩子并清理静态引用。
        /// </summary>
        public void Unload()
        {
            ModHooks.GetPlayerBoolHook -= OnGetPlayerBoolHook;
            ModHooks.BeforeSceneLoadHook -= BeforeSceneLoad;
            _instance = null;
        }

        public override void Initialize()
        {
            Log("Initializing");

            ModHooks.GetPlayerBoolHook += OnGetPlayerBoolHook;
            ModHooks.BeforeSceneLoadHook += BeforeSceneLoad;

            Log("Initialized");
        }
        private bool OnGetPlayerBoolHook(string target, bool orig)
        {

            if (target == "divineInTown")
            {
                PlayerData pd = PlayerData.instance;
                return !(
                            (
                                pd.GetBool("legEaterLeft") &&
                                (
                                    pd.GetBool("defeatedNightmareGrimm") ||
                                    pd.GetBool("destroyedNightmareLantern")
                                ) &&
                                pd.GetBool("divineFinalConvo") &&
                                !(
                                    previousScene == "Grimm_Divine" &&
                                    loadingScene == "Town"
                                 )
                            ) ||
                            !pd.GetBool("nightmareLanternLit")
                       );
            }
            return orig;
        }
        private string BeforeSceneLoad(string scName)
        {
            previousScene = loadingScene;
            loadingScene = scName;
            return scName;
        }
    }
}
