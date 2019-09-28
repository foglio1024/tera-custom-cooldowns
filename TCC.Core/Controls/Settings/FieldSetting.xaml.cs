using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

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

        public FieldSetting()
        {
            InitializeComponent();
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
