using TCC.Data;
using TCC.UI.Windows.Widgets;

namespace TCC.Settings.WindowSettings;

public class FloatingButtonWindowSettings : WindowSettingsBase
{
    private bool _showNotificationBubble;

    public bool ShowNotificationBubble
    {
        get => _showNotificationBubble;
        set => RaiseAndSetIfChanged(value, ref _showNotificationBubble);
    }

    public FloatingButtonWindowSettings()
    {
        _visible = true;
        _clickThruMode = ClickThruMode.Never;
        _scale = 1;
        _autoDim = false;
        _dimOpacity = 1;
        _showAlways = false;
        _enabled = true;
        _allowOffScreen = true;
        Positions = new ClassPositions(0, 0, ButtonsPosition.Above);

        PerClassPosition = false;

        ShowNotificationBubble = true;

    }
}