// 编译期最小桩（仅解决 PaleCourtCharms 引用缺失）。运行时由真实 DebugMod 插件 DLL 解析，不影响任何功能。
using System;
namespace DebugMod
{
    public static class BindableFunctions
    {
        public static event Action OnGiveAllCharms;
        public static event Action OnRemoveAllCharms;
    }
}
