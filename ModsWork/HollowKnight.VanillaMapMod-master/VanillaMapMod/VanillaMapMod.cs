using System;
using System.Collections.Generic;
using MapChanger;
using Modding;
using UnityEngine;
using VanillaMapMod.Settings;
using VanillaMapMod.UI;

namespace VanillaMapMod;

/// <summary>
/// 地图加强模组 —— 为地图添加额外标记与全图模式。
/// </summary>
public sealed class VanillaMapMod : Mod,
    ILocalSettings<LocalSettings>,
    IGlobalSettings<GlobalSettings>,
    IMenuMod,
    ITogglableMod
{
    private static readonly string[] _dependencies = ["地图变更", "CMICore"];

    private static readonly MapMode[] _modes = [new NormalMode(), new FullMapMode()];

    public VanillaMapMod() : base("地图加强")
    {
        Instance = this;
    }

    public static VanillaMapMod Instance { get; private set; }
    internal static LocalSettings LS { get; private set; } = new();
    internal static GlobalSettings GS { get; private set; } = new();

    internal static VmmPauseMenu PauseMenu { get; private set; }

    public override string GetVersion()
    {
        return "2.1.6";
    }

    public override int LoadPriority()
    {
        return 10;
    }

    public void OnLoadLocal(LocalSettings ls)
    {
        LS = ls;
    }

    public LocalSettings OnSaveLocal()
    {
        return LS;
    }

    public void OnLoadGlobal(GlobalSettings gs)
    {
        GS = gs;
    }

    public GlobalSettings OnSaveGlobal()
    {
        return GS;
    }

    /// <summary>
    /// IMenuMod：在模组列表中显示一个开关条目。
    /// </summary>
    public bool ToggleButtonInsideMenu => true;

    public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
    {
        return [new(
            toggleButtonEntry.Value.Name,
            toggleButtonEntry.Value.Values,
            "启用地图加强模组，添加地图标记功能。",
            toggleButtonEntry.Value.Saver,
            toggleButtonEntry.Value.Loader)];
    }

    /// <summary>
    /// ITogglableMod：支持模组列表中的开关切换。
    /// </summary>
    public bool IsToggleable() => true;

    public void Toggle()
    {
        ModeManager.ToggleModEnabled();
    }

    /// <summary>
    /// ITogglableMod：卸载模组时清理资源。
    /// </summary>
    public void Unload()
    {
        PauseMenu?.Destroy();
        PauseMenu = null;
        Instance = null;
    }

    public override void Initialize()
    {
        LogDebug("Initializing");

        foreach (var dependency in _dependencies)
        {
            if (ModHooks.GetMod(dependency) is not Mod)
            {
                Instance.LogWarn($"Dependency not found for {GetType().Name}: {dependency}");
                return;
            }
        }

        Localization.Load();

        Events.OnEnterGame += OnEnterGame;
        Events.OnQuitToMenu += OnQuitToMenu;

        LogDebug("初始化完成。");
    }

    private static void OnEnterGame()
    {
        ModeManager.AddModes(_modes);

        Events.OnSetGameMap += OnSetGameMap;
    }

    private static void OnQuitToMenu()
    {
        Events.OnSetGameMap -= OnSetGameMap;

        PauseMenu?.Destroy();
        PauseMenu = null;
    }

    private static void OnSetGameMap(GameObject goMap)
    {
        try
        {
            VmmPinManager.MakePins(goMap);
            PauseMenu = new();
        }
        catch (Exception e)
        {
            Instance.LogError(e);
        }
    }
}
