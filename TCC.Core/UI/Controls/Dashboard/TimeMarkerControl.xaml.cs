using System.Windows;
using System.Windows.Media;
using TCC.ViewModels;

namespace TCC.UI.Controls.Dashboard
{
    /// <summary>
    /// Interaction logic for TimeMarkerControl.xaml
    /// </summary>
    public partial class TimeMarkerControl
    {
        public TimeMarkerControl()
        {
            InitializeComponent();
        }

/*
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            var p = Utils.FindVisualParent<Grid>(this);
            Panel.SetZIndex(this, int.MaxValue);
        }
*/

/*
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            var p = Utils.FindVisualParent<Grid>(this);
            Panel.SetZIndex(this, int.MinValue);

        }
*/

        private TimeMarker _dc;
        private void TimeMarkerControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dc = (TimeMarker)DataContext;
            _dc.PropertyChanged += Dc_PropertyChanged;
        }

        private void Dc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_dc.TimeFactor)) return;
            var w = TextBorder.ActualWidth;
            Dispatcher?.Invoke(() => TextBorder.LayoutTransform = new TranslateTransform(-w * _dc.TimeFactor, 0));
        }
    }
}
