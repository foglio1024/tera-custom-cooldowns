using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for FieldSetting.xaml
    /// </summary>
    public partial class FieldSetting : UserControl
    {
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(FieldSetting));
        public string SettingName
        {
            get { return (string)GetValue(SettingNameProperty); }
            set { SetValue(SettingNameProperty, value); }
        }
        public static readonly DependencyProperty SettingNameProperty = DependencyProperty.Register("SettingName", typeof(string), typeof(FieldSetting));
        public ImageSource SettingImage
        {
            get { return (ImageSource)GetValue(SettingImageProperty); }
            set { SetValue(SettingImageProperty, value); }
        }
        public static readonly DependencyProperty SettingImageProperty = DependencyProperty.Register("SettingImage", typeof(ImageSource), typeof(FieldSetting));

        ColorAnimation glow;
        ColorAnimation unglow;
        DoubleAnimation fadeIn;
        DoubleAnimation fadeOut;

        public FieldSetting()
        {
            InitializeComponent();
            glow = new ColorAnimation(Colors.Transparent, Color.FromArgb(8, 255, 255, 255), TimeSpan.FromMilliseconds(50));
            unglow = new ColorAnimation(Color.FromArgb(8, 255, 255, 255), Colors.Transparent, TimeSpan.FromMilliseconds(100));
            fadeIn = new DoubleAnimation(.3, .9, TimeSpan.FromMilliseconds(200));
            fadeOut = new DoubleAnimation(.9, .3, TimeSpan.FromMilliseconds(200));

            mainGrid.Background = new SolidColorBrush(Colors.Transparent);

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
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            SettingsManager.SaveSettings();

        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            Value = tb.Text;
        }


    }
}
