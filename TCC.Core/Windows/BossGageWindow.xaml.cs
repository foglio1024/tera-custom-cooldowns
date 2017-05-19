using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using TCC.ViewModels;

namespace TCC.Converters
{

    public class HarrowholdBossesVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

namespace TCC.Windows
{

    public partial class BossGageWindow : TccWindow
    {

        public BossGageWindow()
        {
            InitializeComponent();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow();

            Bosses.DataContext = BossGageWindowManager.Instance.CurrentNPCs;
            Bosses.ItemsSource = BossGageWindowManager.Instance.CurrentNPCs;

            Left = SettingsManager.BuffBarWindowSettings.X;
            Top = SettingsManager.BossGaugeWindowSettings.Y;
            Visibility = SettingsManager.BossGaugeWindowSettings.Visibility;
            SetClickThru(SettingsManager.BossGaugeWindowSettings.ClickThru);
        }


        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
            SettingsManager.BossGaugeWindowSettings.X = this.Left;
            SettingsManager.BossGaugeWindowSettings.Y = this.Top;
            SettingsManager.SaveSettings();

        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}

