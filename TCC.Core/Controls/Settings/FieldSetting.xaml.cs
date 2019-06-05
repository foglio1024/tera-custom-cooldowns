using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Settings;

namespace TCC.Controls.Settings
{
    /// <summary>
    /// Interaction logic for FieldSetting.xaml
    /// </summary>
    public partial class FieldSetting
    {
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(FieldSetting));
        public string SettingName
        {
            get => (string)GetValue(SettingNameProperty);
            set => SetValue(SettingNameProperty, value);
        }
        public static readonly DependencyProperty SettingNameProperty = DependencyProperty.Register("SettingName", typeof(string), typeof(FieldSetting));
        public Geometry SvgIcon
        {
            get => (Geometry)GetValue(SvgIconProperty);
            set => SetValue(SvgIconProperty, value);
        }
        public static readonly DependencyProperty SvgIconProperty = DependencyProperty.Register("SvgIcon", typeof(Geometry), typeof(FieldSetting));

        private readonly ColorAnimation _glow;
        private readonly ColorAnimation _unglow;
        private readonly DoubleAnimation _fadeIn;
        private readonly DoubleAnimation _fadeOut;

        public FieldSetting()
        {
            InitializeComponent();
            _glow = new ColorAnimation(Colors.Transparent, Color.FromArgb(8, 255, 255, 255), TimeSpan.FromMilliseconds(50));
            _unglow = new ColorAnimation(Color.FromArgb(8, 255, 255, 255), Colors.Transparent, TimeSpan.FromMilliseconds(100));
            _fadeIn = new DoubleAnimation(.3, .9, TimeSpan.FromMilliseconds(200));
            _fadeOut = new DoubleAnimation(.9, .3, TimeSpan.FromMilliseconds(200));

            MainGrid.Background = new SolidColorBrush(Colors.Transparent);

        }
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Grid)?.Background.BeginAnimation(SolidColorBrush.ColorProperty, _glow);
            Img.BeginAnimation(OpacityProperty, _fadeIn);

        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Grid)?.Background.BeginAnimation(SolidColorBrush.ColorProperty, _unglow);
            Img.BeginAnimation(OpacityProperty, _fadeOut);

        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            App.Settings.Save();

        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb) Value = tb.Text;
        }


    }
}
