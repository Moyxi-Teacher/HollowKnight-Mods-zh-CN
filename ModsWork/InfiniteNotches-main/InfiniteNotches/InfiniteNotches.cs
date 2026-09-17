using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;

namespace InfiniteNotches
{
    /*
     * 这是一个《空洞骑士》模组示例。
     * 模组必须使用 .NET Framework 4.7.2 开发，并输出 DLL 类库。
     * 模组必须引用 Modding API；安装 API 后，应引用 Assembly-CSharp 程序集。
     * 然后在文件头部添加“using Modding;”即可开始编写模组。
    */

    public class InfiniteNotches : Mod, ITogglableMod, IMenuMod
    {
        /*
         * 保存模组的静态引用，便于在静态方法和其他类中使用模组名称记录日志。
        */ 
        internal static InfiniteNotches instance;
        private readonly Localization localization;

        /*
         * 构造函数中的代码会在模组加载前执行，应谨慎使用。
         * 构造函数不能包含参数。
         */ 
        public InfiniteNotches() : base(Localization.Load().Name)
        {
            instance = this;
            localization = Localization.Load();

            if (!string.IsNullOrEmpty(localization.Error))
            {
                LogError($"无法读取 zh.json，已使用默认文本：{localization.Error}");
            }
        }

        public bool ToggleButtonInsideMenu => true;

        public override string GetMenuButtonText()
        {
            return localization.Name;
        }

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            var entries = new List<IMenuMod.MenuEntry>();
            if (toggleButtonEntry.HasValue)
            {
                var toggle = toggleButtonEntry.Value;
                entries.Add(new IMenuMod.MenuEntry(
                    localization.Name,
                    new[] { localization.Disabled, localization.Enabled },
                    localization.Description,
                    toggle.Saver,
                    toggle.Loader));
            }

            return entries;
        }

        /*
         * 每个模组都需要重写 Initialize() 方法，API 加载模组时会调用此方法。
        */
        public override void Initialize()
        {
            instance.Log("Initializing");
            ModHooks.GetPlayerIntHook += OverrideNotchCount;
        }

        /*
         * API 通过重写的 GetVersion() 方法获取并显示模组版本号。
        */
        public override string GetVersion()
        {
            return "2000";
        }

        /*
         * 这是一个钩子方法示例。
         * 我们在 Initialize() 中注册此钩子。
         * 每当游戏尝试从玩家存档中读取整数时，
         * 我们就可以拦截调用并替换返回值。
         * （可以通过 PlayerData.instance 访问存档数据。）
         * 在此示例中，游戏检查护符槽数量时始终返回 2000。
         * 其他字段则继续返回方法参数中提供的原始值。
         *
         * 也可以使用 PlayerData.instance.GetIntInternal 方法读取存档数据，而不是直接使用 value。
         * 不要在挂接到 GetPlayerIntHook 的方法中调用 PlayerData.instance.GetInt，
         * 因为它会立即再次触发钩子，容易造成无限递归。
         * 也不要直接访问 PlayerData 的字段（例如 PlayerData.instance.charmSlots），
         * 因为这种访问不会触发钩子，从而会阻止其他模组修改数值。
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
         * ITogglableMod 接口使用 Unload() 方法卸载模组。
         * 在菜单中关闭模组时会调用 Unload()。
         * 这里解除 OverrideNotchCount 钩子，使游戏恢复读取存档中的护符槽数量。
         * 之后重新开启模组时，会再次调用 Initialize() 并重新注册 OverrideNotchCount 钩子。
        */

        public void Unload()
        {
            ModHooks.GetPlayerIntHook -= OverrideNotchCount;
        }

    }
}
