// 编译期最小桩（仅解决 PaleCourtCharms 引用缺失）。运行时由真实 RSM 插件 DLL 解析，不影响任何功能。
using System;
namespace RandoSettingsManager
{
    public class RandoSettingsManagerMod : Modding.Mod
    {
        public static RandoSettingsManagerMod Instance { get; internal set; }
        public void RegisterConnection(object connection) { }
    }
}
namespace RandoSettingsManager.SettingsManagement
{
    public abstract class RandoSettingsProxy<TSettings, TKey>
    {
        public abstract string ModKey { get; }
        public abstract Versioning.VersioningPolicy<TKey> VersioningPolicy { get; }
        public abstract bool TryProvideSettings(out TSettings settings);
        public abstract void ReceiveSettings(TSettings settings);
    }
    }
namespace RandoSettingsManager.SettingsManagement.Versioning
{
    public abstract class VersioningPolicy<T> { }
    public class EqualityVersioningPolicy<T> : VersioningPolicy<T>
    {
        public EqualityVersioningPolicy(string version) { }
    }
}
