using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;

namespace InfiniteNotches
{
    /*
     * This is an example Hollow Knight mod.
     * Mods must be made using .NET Framework 4.7.2, and should output a (.DLL) class library.
     * Mods must reference the Modding API. If you have already installed the API, this is the Assembly-CSharp file
     * Then, place "using Modding;" in the header to begin your mod.
    */

    public class InfiniteNotches : Mod, ITogglableMod, IMenuMod
    {
        internal static InfiniteNotches instance;

        // 游戏内菜单显示为开关按钮（而非子菜单）
        public bool ToggleButtonInsideMenu => true;

        // 返回菜单条目：总开关 + 功能说明
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> entries = new List<IMenuMod.MenuEntry>();
            if (toggleButtonEntry != null)
            {
                entries.Add((IMenuMod.MenuEntry)toggleButtonEntry);
            }
            entries.Add(new IMenuMod.MenuEntry()
            {
                Name = "无限凹槽",
                Description = "将护符槽总数固定设置为 2000，相当于无限凹槽。"
            });
            return entries;
        }

        public InfiniteNotches() : base("无限凹槽")
        {
            instance = this;
        }

        /*
         * Every mod needs an override Initialize() method. This is what the API will call when it loads the mod.
        */
        public override void Initialize()
        {
            instance.Log("Initializing");
            ModHooks.GetPlayerIntHook += OverrideNotchCount;
        }

        /*
         * The override GetVersion() method is how the API retrieves the version number it displays for your mod.
        */
        public override string GetVersion()
        {
            return "2000";
        }

        /*
         * This is an example of a method which is used in a hook.
         * We hook the method in Initialize()
         * Then, whenever the game tries to retrieve an int from the player's save data,
         * we can intercept that call and replace it with our own int.
         * (Save data can be accessed in PlayerData.instance)
         * Here, when the game checks the number of charm notches, we always return 2000.
         * Otherwise, we send the call forwards with the value provided in the method parameters.
         * 
         * We could also retrieve save data with the PlayerData.instance.GetIntInternal method instead of using value
         * Avoid using PlayerData.instance.GetInt inside a method hooked to GetPlayerIntHook
            * This calls the hook again immediately, and can easily lead to infinite recursion.
         * Avoid using direct field access on PlayerData (e.g. PlayerData.instance.charmSlots)
            * This does not call any hooks, and so it prevents other mods from making changes.
        */
        private static int OverrideNotchCount(string fieldName, int value)
        {
            if (fieldName == nameof(PlayerData.charmSlots))
            {
                return 2000;
            }
            return value;
        }

        /*
         * The Unload() method is used by the IToggleableMod interface.
         * When the mod is toggled off in the menu, Unload() is called.
         * We use it here to unhook OverrideNotchCount, so the game can now access the number of notches in the save data.
         * When the mod is toggled on later, it will call Initialize() again, and rehook OverrideNotchCount.
        */

        public void Unload()
        {
            ModHooks.GetPlayerIntHook -= OverrideNotchCount;
        }

    }
}
