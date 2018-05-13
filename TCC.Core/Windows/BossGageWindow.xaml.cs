using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TCC.ViewModels;

namespace TCC.Windows
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

    public partial class BossWindow : TccWindow
    {

        public BossWindow()
        {
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContentRef = content;
            InitWindow(SettingsManager.BossWindowSettings, ignoreSize: true);


        }

        private void TccWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BossGageWindowViewModel.Instance.CopyToClipboard();
        }
    }
}