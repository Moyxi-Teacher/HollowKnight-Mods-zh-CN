using MapChanger.Input;

namespace MapChanger.UI;

internal class ModDisabledTitle : PauseMenuTitle
{
    protected internal override TextFormat GetDefaultTextFormat()
    {
        return new(
            $"{"按".L()} {ModEnabledInput.Instance.GetBindingsText()} {"启用地图模组".L()}",
            Colors.GetColor(ColorSetting.UI_Neutral)
        );
    }
}
