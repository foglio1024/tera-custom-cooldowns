using System.Windows;
using System.Windows.Controls;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Interaction logic for SmallMobControl.xaml
    /// </summary>
    public partial class SmallMobControl : UserControl
    {
        public SmallMobControl()
        {
            InitializeComponent();
        }

        private void SmallMobControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dc = (Boss) DataContext;
            dc.DeleteEvent += Dc_DeleteEvent;
        }

        private void Dc_DeleteEvent() => Dispatcher.Invoke(() => BossGageWindowViewModel.Instance.RemoveMe((Boss)DataContext));
    }
}
