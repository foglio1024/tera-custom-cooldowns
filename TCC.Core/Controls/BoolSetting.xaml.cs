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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per BoolSetting.xaml
    /// </summary>
    public partial class BoolSetting : UserControl
    {
        public BoolSetting()
        {
            InitializeComponent();
            glow = new ColorAnimation(Colors.Transparent, Color.FromArgb(8, 255, 255, 255), TimeSpan.FromMilliseconds(50));
            unglow = new ColorAnimation(Color.FromArgb(8, 255, 255, 255), Colors.Transparent, TimeSpan.FromMilliseconds(100));
            fadeIn = new DoubleAnimation(.3, .9, TimeSpan.FromMilliseconds(200));
            fadeOut = new DoubleAnimation(.9, .3, TimeSpan.FromMilliseconds(200));
            mainGrid.Background = new SolidColorBrush(Colors.Transparent);
        }

        ColorAnimation glow;
        ColorAnimation unglow;
        DoubleAnimation fadeIn;
        DoubleAnimation fadeOut;
        public string SettingName
        {
            get { return (string)GetValue(SettingNameProperty); }
            set { SetValue(SettingNameProperty, value); }
        }
        public static readonly DependencyProperty SettingNameProperty =
            DependencyProperty.Register("SettingName", typeof(string), typeof(BoolSetting));

        public bool IsOn
        {
            get { return (bool)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register("IsOn", typeof(bool), typeof(BoolSetting), new PropertyMetadata(false));

        public ImageSource SettingImage
        {
            get { return (ImageSource)GetValue(SettingImageProperty); }
            set { SetValue(SettingImageProperty, value); }
        }
        public static readonly DependencyProperty SettingImageProperty =
            DependencyProperty.Register("SettingImage", typeof(ImageSource), typeof(BoolSetting));



        private void ToggleSetting(object sender, MouseButtonEventArgs e)
        {
            IsOn = !IsOn;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Grid).Background.BeginAnimation(SolidColorBrush.ColorProperty, glow);
            img.BeginAnimation(OpacityProperty, fadeIn);
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Grid).Background.BeginAnimation(SolidColorBrush.ColorProperty, unglow);
            img.BeginAnimation(OpacityProperty, fadeOut);

        }
    }
}
