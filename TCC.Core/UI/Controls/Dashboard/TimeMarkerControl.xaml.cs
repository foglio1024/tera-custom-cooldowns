using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using TCC.ViewModels;

namespace TCC.UI.Controls.Dashboard;

public partial class TimeMarkerControl
{
    TimeMarker? _dc;

    public TimeMarkerControl()
    {
        InitializeComponent();
    }

    void TimeMarkerControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        _dc = (TimeMarker)DataContext;
        _dc.PropertyChanged += Dc_PropertyChanged;
    }

    void Dc_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_dc == null) return;
        if (e.PropertyName != nameof(_dc.TimeFactor)) return;
        var w = TextBorder.ActualWidth;
        Dispatcher?.Invoke(() => TextBorder.LayoutTransform = new TranslateTransform(-w * _dc.TimeFactor, 0));
    }
}