using TCC.Data;
using TCC.UI.Windows.Widgets;

namespace TCC.Settings.WindowSettings;

public class NotificationAreaSettings : WindowSettingsBase
{
    private int _defaultNotificationDuration;
    public int MaxNotifications { get; set; }
    public int DefaultNotificationDuration
    {
        get => _defaultNotificationDuration;
        set => RaiseAndSetIfChanged(value, ref _defaultNotificationDuration);
    }

    public NotificationAreaSettings()
    {
        _visible = true;
        _clickThruMode = ClickThruMode.Never;
        _scale = 1;
        _autoDim = false;
        _dimOpacity = 1;
        _showAlways = true;
        _enabled = true;
        _allowOffScreen = false;
        PerClassPosition = false;
        Positions = new ClassPositions(0, .5, ButtonsPosition.Above);

        MaxNotifications = 5;
        DefaultNotificationDuration = 5;
    }
}