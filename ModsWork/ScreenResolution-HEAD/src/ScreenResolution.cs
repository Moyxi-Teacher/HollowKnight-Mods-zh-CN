using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InControl;
using Modding;
using Modding.Converters;
using Modding.Menu.Config;
using Newtonsoft.Json;
using SFCore.Generics;
using SFCore.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using UObject = UnityEngine.Object;
using UScenes = UnityEngine.SceneManagement;
using Satchel.BetterMenus;

namespace ScreenResolution;

/// <summary>
/// Base class.
/// </summary>
public class ScreenResolution : Mod, ICustomMenuMod
{
    bool ICustomMenuMod.ToggleButtonInsideMenu => true;
    public Menu MenuRef;

    private List<int> width_list = new();
    private List<int> height_list = new();
    private List<int> refreshRate_list = new();

    private int width;
    private int height;
    private int refreshRate;
    private UnityEngine.FullScreenMode fullScreenMode;

    public override string GetVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }

    public ScreenResolution() : base("屏幕分辨率")
    {
    }

    public override void Initialize()
    {
        Log("Initializing");

        // prepare lists of widths, heights and refreshrates to support
        for (int i = 1; i <= 4; i++)
        {
            foreach (Resolution res in UnityEngine.Screen.resolutions)
            {
                if (!width_list.Contains(res.width / i))
                {
                    width_list.Add(res.width / i);
                }
                if (!height_list.Contains(res.height / i))
                {
                    height_list.Add(res.height / i);
                }
                if ((i == 1) && !refreshRate_list.Contains(res.refreshRate))
                {
                    // only base list of refresh rates
                    refreshRate_list.Add(res.refreshRate);
                }
            }
        }
        // sort them
        width_list.Sort();
        height_list.Sort();
        refreshRate_list.Sort();

        // current values
        width = UnityEngine.Screen.currentResolution.width;
        height = UnityEngine.Screen.currentResolution.height;
        refreshRate = UnityEngine.Screen.currentResolution.refreshRate;
        fullScreenMode = UnityEngine.Screen.fullScreenMode;

        Log($"current width: {width}");
        Log($"current height: {height}");
        Log($"current refreshRate: {refreshRate}");
        Log($"current fullScreenMode: {fullScreenMode}");

        foreach (Resolution res in UnityEngine.Screen.resolutions)
        {
            Log($"Available supported fullscreen resolution: {res.width}x{res.height} at {res.refreshRate} FPS");
        }

        Log("Initialized");
    }

    private void SetResolution()
    {
        Log($"set width to {width}");
        Log($"set height to {height}");
        Log($"set refreshRate to {refreshRate}");
        Log($"set fullScreenMode to {fullScreenMode}");
        UnityEngine.Screen.SetResolution(width, height, fullScreenMode, refreshRate);
    }

    private static string Localize(string chinese, string english)
    {
        return Language.Language.CurrentLanguage().ToString() == "ZH" ? chinese : english;
    }

    public Menu PrepareMenu()
    {
        return new Menu(Localize("屏幕分辨率", "Screen Resolution"), new Element[]
        {
            new HorizontalOption(Localize("宽度", "Width"),
                Localize("要使用的屏幕宽度。", "The width used."),
                width_list.Select(x => $"{x}").ToArray(),
                (option) =>
                {
                    width = width_list[option];
                    SetResolution();
                },
                () => width_list.IndexOf(width)),
            new HorizontalOption(Localize("高度", "Height"),
                Localize("要使用的屏幕高度。", "The height used."),
                height_list.Select(x => $"{x}").ToArray(),
                (option) =>
                {
                    height = height_list[option];
                    SetResolution();
                },
                () => height_list.IndexOf(height)),
            new HorizontalOption(Localize("帧率", "Framerate"),
                Localize("要使用的刷新率。", "The framerate used."),
                refreshRate_list.Select(x => $"{x}").ToArray(),
                (option) =>
                {
                    refreshRate = refreshRate_list[option];
                    SetResolution();
                },
                () => refreshRate_list.IndexOf(refreshRate)),
            new HorizontalOption(Localize("全屏模式", "Fullscreen"),
                Localize("要使用的全屏显示模式。", "The fullscreen mode used."),
                Language.Language.CurrentLanguage().ToString() == "ZH"
                    ? new[] { "独占全屏", "无边框全屏", "最大化窗口", "窗口化" }
                    : new[] { "Exclusive Fullscreen", "Borderless Fullscreen", "Maximized Window", "Windowed" },
                (option) =>
                {
                    fullScreenMode = (UnityEngine.FullScreenMode)option;
                    SetResolution();
                },
                () => (int)fullScreenMode),
        });
    }

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
    {
        MenuRef ??= PrepareMenu();

        return MenuRef.GetMenuScreen(modListMenu);
    }
}