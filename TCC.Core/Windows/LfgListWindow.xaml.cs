using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per LfgListWindow.xaml
    /// </summary>
    public partial class LfgListWindow : Window
    {
        public LfgListViewModel VM => Dispatcher.Invoke(() => this.DataContext as LfgListViewModel);


        public LfgListWindow()
        {
            InitializeComponent();
            DataContext = new LfgListViewModel();
            VM.PropertyChanged += VM_PropertyChanged;
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VM.Creating) || e.PropertyName == nameof(VM.NewMessage))
            {
                var colAn = new ColorAnimation() { Duration = TimeSpan.FromMilliseconds(200) };
                if (VM.Creating)
                {
                    if (string.IsNullOrEmpty(VM.NewMessage)) colAn.To = (App.Current.FindResource("Colors.App.HP") as SolidColorBrush).Color;
                    else colAn.To = (App.Current.FindResource("Colors.App.Green") as SolidColorBrush).Color;
                }
                else
                {
                    colAn.To = (App.Current.FindResource("BackgroundDarkColor") as SolidColorBrush).Color;
                }
                var currBg = CreateMessageBtn.Background as SolidColorBrush;
                var currCol = currBg.Color;
                var newBg = new SolidColorBrush(currCol);
                CreateMessageBtn.Background = newBg;
                CreateMessageBtn.Background.BeginAnimation(SolidColorBrush.ColorProperty, colAn);
            }
            if (e.PropertyName == nameof(VM.AmIinLfg))
            {
                if (VM.AmIinLfg)
                {
                    LfgMgmtBtn.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                    CreateMessageBtn.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                    CreateMessageBtn.BeginAnimation(MarginProperty, new ThicknessAnimation(new Thickness(4,0,4,0), TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                }
                else
                {
                    LfgMgmtBtn.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                    CreateMessageBtn.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                    CreateMessageBtn.BeginAnimation(MarginProperty, new ThicknessAnimation(new Thickness(4), TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                }
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }
        private void CloseWindow()
        {
            var a = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));
            a.Completed += (s, ev) => { Hide(); };
            this.BeginAnimation(OpacityProperty, a);
        }

        internal void ShowWindow()
        {
            Dispatcher.Invoke(() =>
            {
                if (this.IsVisible) return;
                Opacity = 0;
                Show();
                Activate();
                BeginAnimation(Window.OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));
            });
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var l = ((sender as FrameworkElement).DataContext as Listing);
            if (l.IsExpanded)
            {
                l.IsExpanded = false;
                return;
            }
            var id = l.LeaderId;
            VM._lastClicked = l;
            Proxy.RequestPartyInfo(id);
        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dc = (sender as FrameworkElement).DataContext as User;
            Proxy.AskInteractive(SessionManager.CurrentPlayer.ServerId, dc.Name);
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var l = ((sender as FrameworkElement).DataContext as Listing);
            Proxy.ApplyToLfg(l.LeaderId);
        }
        private void CreateMessageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!VM.Creating)
            {
                NewMessageGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                NewMessageTextBox.Focus();
                VM.Creating = true;
            }
            else if (VM.Creating && !string.IsNullOrEmpty(VM.NewMessage))
            {
                NewMessageGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                Proxy.RegisterLfg(VM.NewMessage, RaidSwitch.IsOn);
                VM.Creating = false;
                VM.NewMessage = "";
                Task.Delay(200).ContinueWith(t => Proxy.RequestLfgList());
            }
            else
            {
                NewMessageGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                VM.NewMessage = "";
                VM.Creating = false;
            }
        }

        private void PublicizeBtn_Click(object sender, RoutedEventArgs e)
        {
            PublicizeBtn.IsEnabled = false;
            PublicizeBtn.IsHitTestVisible = false;
            var an = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(3000));
            an.Completed += (s, ev) =>
            {
                PublicizeBarGovernor.LayoutTransform = new ScaleTransform(0, 1);
                PublicizeBtn.IsEnabled = true;
                PublicizeBtn.IsHitTestVisible = true;
            };
            PublicizeBarGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, an);
            Proxy.PublicizeLfg();
        }

        private void RemoveMessageButton_Click(object sender, RoutedEventArgs e)
        {
            Proxy.RemoveLfg();
            Proxy.RequestLfgList();
            Proxy.RequestLfgList();
        }
    }
}
