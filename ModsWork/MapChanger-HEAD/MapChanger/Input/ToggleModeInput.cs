using UnityEngine;

namespace MapChanger.Input;

public class ToggleModeInput : GlobalHotkeyInput
{
    internal ToggleModeInput()
        : base("切换地图模式", "地图模组", KeyCode.T)
    {
        Instance = this;
    }

    public static ToggleModeInput Instance { get; private set; }

    public override bool UseCondition()
    {
        return true;
    }

    public override bool ActiveCondition()
    {
        return true;
    }

    public override void DoAction()
    {
        ModeManager.ToggleMode();
    }
}
