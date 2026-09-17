using MapChanger;
using MapChanger.UI;
using UnityEngine;

namespace VanillaMapMod.UI;

internal class ModDisabledButton : MainButton
{
    protected internal override void OnClick()
    {
        ModeManager.ToggleModEnabled();
    }

    protected internal override TextFormat GetTextFormat()
    {
        return new("地图模组\n已禁用".L(), Colors.GetColor(ColorSetting.UI_Disabled));
    }

    protected internal override Color GetBorderColor()
    {
        return Colors.GetColor(ColorSetting.UI_Neutral);
    }
}
