using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Interaction logic for SmallMobControl.xaml
    /// </summary>
    public partial class SmallMobControl : UserControl
    {
        private DispatcherTimer t;
        public SmallMobControl()
        {
            InitializeComponent();
        }

        private void SmallMobControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dc = (Boss) DataContext;
            dc.DeleteEvent += Dc_DeleteEvent;
            t = new DispatcherTimer {Interval = TimeSpan.FromSeconds(4700)};
            t.Tick += (s,ev) => RootGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1,0,TimeSpan.FromMilliseconds(200)));
            RootGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0,1,TimeSpan.FromMilliseconds(200)));
        }

        private void Dc_DeleteEvent() => Dispatcher.Invoke(() =>
        {
            t.Start();
            try
            {
                BossGageWindowViewModel.Instance.RemoveMe((Boss) DataContext);
            }
            catch { }
        });

    }
}
