using HarmonyLib;
using Modding;
using System.Collections.Generic;
using System.Reflection;

namespace UnofficialPatch
{
    /// <summary>
    /// 以空洞骑士 Mod API 加载的入口（替代原 BepInEx 的 BaseUnityPlugin）。
    /// 由 ModLoader 自动实例化，构造函数设置静态实例引用供补丁类日志使用。
    /// 实现 IMenuMod / ITogglableMod，提供模组列表中的开关与菜单。
    /// </summary>
    public class Plugin : Mod, IMenuMod, ITogglableMod
    {
        // ModLoader 实例化后保存引用，供静态日志方法使用
        private static Plugin _instance;

        private Harmony _harmony;

        private bool _enabled = true;

        /// <summary>
        /// 供补丁类访问的静态日志方法。
        /// 使用 new 关键字明确隐藏基类 Loggable.Log(string)。
        /// </summary>
        public static new void Log(string message) => ((Loggable)_instance)?.Log(message);

        public Plugin() : base("非官方补丁修正")
        {
            _instance = this;
        }

        /// <summary>
        /// Gets the mod's version from the assembly.
        /// </summary>
        public override string GetVersion() =>
            Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

        /// <summary>
        /// 在 Mod 被加载时调用，替代 BepInEx 的 Awake()。
        /// </summary>
        public override void Initialize()
        {
            _harmony = new Harmony(PluginInfo.GUID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Plugin.Log($"{PluginInfo.Name} v{PluginInfo.Version} 已加载。");
        }

        /// <summary>
        /// IMenuMod：在模组列表中显示一个开关条目。
        /// 此模组所有补丁由 Harmony 应用，开关即整体启用或卸载补丁。
        /// </summary>
        public bool ToggleButtonInsideMenu => true;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new()
            {
                new(toggleButtonEntry.Value.Name, toggleButtonEntry.Value.Values,
                    "启用以应用非官方补丁修正的各项错误修复。关闭后补丁将移除。",
                    toggleButtonEntry.Value.Saver, toggleButtonEntry.Value.Loader)
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
                if (_harmony == null)
                {
                    _harmony = new Harmony(PluginInfo.GUID);
                }
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                Plugin.Log("非官方补丁修正 已启用。");
            }
            else
            {
                _harmony?.UnpatchSelf();
                Plugin.Log("非官方补丁修正 已禁用。");
            }
        }

        /// <summary>
        /// ITogglableMod：卸载模组时移除全部 Harmony 补丁并清理静态引用。
        /// </summary>
        public void Unload()
        {
            _harmony?.UnpatchSelf();
            _harmony = null;
            _instance = null;
        }
    }

    /// <summary>
    /// 用于 HarmonyPatch 的 GUID/名称/版本引用。
    /// </summary>
    public static class PluginInfo
    {
        public const string GUID = "com.priler.unofficialpatch";
        public const string Name = "非官方补丁修正";
        public const string Version = "1.0.0";
    }
}