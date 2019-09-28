using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TCC.Controls.Settings
{
    public partial class ValueSetting
    {

        public string SettingName
        {
            get => (string)GetValue(SettingNameProperty);
            set => SetValue(SettingNameProperty, value);
        }
        public static readonly DependencyProperty SettingNameProperty = 
            DependencyProperty.Register("SettingName", typeof(string), typeof(ValueSetting));

        public Visibility TextBoxVisibility
        {
            get => (Visibility)GetValue(TextBoxVisibilityProperty);
            set => SetValue(TextBoxVisibilityProperty, value);
        }
        public static readonly DependencyProperty TextBoxVisibilityProperty = 
            DependencyProperty.Register("TextBoxVisibility", typeof(Visibility), typeof(ValueSetting));

        public double Max
        {
            get => (double)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(double), typeof(ValueSetting));

        public double Min
        {
            get => (double)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(ValueSetting));

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register("Value", typeof(double), typeof(ValueSetting));

        public Geometry SvgIcon
        {
            get => (Geometry)GetValue(SvgIconProperty);
            set => SetValue(SvgIconProperty, value);
        }
        public static readonly DependencyProperty SvgIconProperty =
            DependencyProperty.Register("SvgIcon", typeof(Geometry), typeof(ValueSetting));


        public ValueSetting()
        {
            InitializeComponent();
        }

        private void AddValue(object sender, MouseButtonEventArgs e)
        {
            Value = Math.Round(Value + 0.01, 2);
        }
        private void SubtractValue(object sender, MouseButtonEventArgs e)
        {
            Value = Math.Round(Value - 0.01, 2);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed) return;

            var s = (Slider)sender;
            if (!s.IsMouseOver) return;

            Value = Math.Round(s.Value, 2);
        }

        private void Slider_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Value = 1;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox) sender;
            try
            {
                var result = double.Parse(tb.Text, CultureInfo.InvariantCulture);
                if (result > Max) Value = Max;
                else if (result < Min) Value = Min;
                else Value = result;
            }
            catch (Exception)
            {
                tb.Text = Value.ToString(CultureInfo.InvariantCulture);

            }
        }

/*
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (double.TryParse(tb.Text, out var result))
            {
                Value = result;
            }
            else
            {
                tb.Text = Value.ToString();
            }
        }
*/

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            Keyboard.ClearFocus();
        }
    }
}
