using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.Controls
{
    /// <inheritdoc cref="UserControl" />
    /// <summary>
    /// Logica di interazione per BoolSetting.xaml
    /// </summary>
    public partial class BoolSetting
    {
        public BoolSetting()
        {
            InitializeComponent();
            _glow = new ColorAnimation(Colors.Transparent, Color.FromArgb(8, 255, 255, 255), TimeSpan.FromMilliseconds(50));
            _unglow = new ColorAnimation(Color.FromArgb(8, 255, 255, 255), Colors.Transparent, TimeSpan.FromMilliseconds(100));
            _fadeIn = new DoubleAnimation(.3, .9, TimeSpan.FromMilliseconds(200));
            _fadeOut = new DoubleAnimation(.9, .3, TimeSpan.FromMilliseconds(200));
            MainGrid.Background = new SolidColorBrush(Colors.Transparent);
        }

        private readonly ColorAnimation _glow;
        private readonly ColorAnimation _unglow;
        private readonly DoubleAnimation _fadeIn;
        private readonly DoubleAnimation _fadeOut;

        public string SettingName
        {
            get => (string)GetValue(SettingNameProperty);
            set => SetValue(SettingNameProperty, value);
        }
        public static readonly DependencyProperty SettingNameProperty =
            DependencyProperty.Register("SettingName", typeof(string), typeof(BoolSetting));

        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register("IsOn", typeof(bool), typeof(BoolSetting), new PropertyMetadata(false));

        public ImageSource SettingImage
        {
            get => (ImageSource)GetValue(SettingImageProperty);
            set => SetValue(SettingImageProperty, value);
        }
        public static readonly DependencyProperty SettingImageProperty =
            DependencyProperty.Register("SettingImage", typeof(ImageSource), typeof(BoolSetting));

        private void ToggleSetting(object sender, MouseButtonEventArgs e)
        {
            IsOn = !IsOn;
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
    }
}
