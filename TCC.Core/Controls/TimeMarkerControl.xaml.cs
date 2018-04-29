using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Interaction logic for TimeMarkerControl.xaml
    /// </summary>
    public partial class TimeMarkerControl : UserControl
    {
        public TimeMarkerControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            var p = Utils.FindVisualParent<Grid>(this);
            Grid.SetZIndex(this, int.MaxValue);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            var p = Utils.FindVisualParent<Grid>(this);
            Grid.SetZIndex(this, int.MinValue);

        }

        private TimeMarker dc;
        private void TimeMarkerControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            dc = (TimeMarker)DataContext;
            dc.PropertyChanged += Dc_PropertyChanged;
        }

        private void Dc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(dc.TimeFactor))
            {
                var w = TextBorder.ActualWidth;
                Dispatcher.Invoke(() => TextBorder.LayoutTransform = new TranslateTransform(-w * dc.TimeFactor, 0));
            }
        }
    }
}
