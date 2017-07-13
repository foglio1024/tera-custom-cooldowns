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
    /// Logica di interazione per ValueSetting.xaml
    /// </summary>
    public partial class ValueSetting : UserControl
    {

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ValueSetting));
        public string SettingName
        {
            get { return (string)GetValue(SettingNameProperty); }
            set { SetValue(SettingNameProperty, value); }
        }
        public static readonly DependencyProperty SettingNameProperty = DependencyProperty.Register("SettingName", typeof(string), typeof(ValueSetting));
        public ImageSource SettingImage
        {
            get { return (ImageSource)GetValue(SettingImageProperty); }
            set { SetValue(SettingImageProperty, value); }
        }
        public static readonly DependencyProperty SettingImageProperty = DependencyProperty.Register("SettingImage", typeof(ImageSource), typeof(ValueSetting));


        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(double), typeof(ValueSetting));



        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(ValueSetting));





        ColorAnimation glow;
        ColorAnimation unglow;



        public ValueSetting()
        {
            InitializeComponent();
            glow = new ColorAnimation(Colors.Transparent, Color.FromArgb(20, 255, 255, 255), TimeSpan.FromMilliseconds(50));
            unglow = new ColorAnimation(Color.FromArgb(10, 255, 255, 255), Colors.Transparent, TimeSpan.FromMilliseconds(100));
            mainGrid.Background = new SolidColorBrush(Colors.Transparent);

        }

        private void AddValue(object sender, MouseButtonEventArgs e)
        {
            Value = Math.Round(Value + 0.01, 2);
        }
        private void SubtractValue(object sender, MouseButtonEventArgs e)
        {
            Value = Math.Round(Value - 0.01, 2);
        }
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Grid).Background.BeginAnimation(SolidColorBrush.ColorProperty, glow);
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Grid).Background.BeginAnimation(SolidColorBrush.ColorProperty, unglow);

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var s = sender as Slider;
            Value = Math.Round(s.Value, 2);
            Console.WriteLine("Slider_ValueChanged set {1} value to {0}", Value, SettingName);

        }

        private void Slider_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Value = 1;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            double result;
            try
            {
                result = Double.Parse(tb.Text, CultureInfo.InvariantCulture);
                if (result > Max) Value = Max;
                else if (result < Min) Value = Min;
                else Value = result;
                Console.WriteLine("TextBox_LostFocus set {1} value to {0}", Value, SettingName);

            }
            catch (Exception)
            {
                tb.Text = Value.ToString();

            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (Double.TryParse(tb.Text, out double result))
            {
                 Value = result;
            }
            else
            {
                tb.Text = Value.ToString();
            }
        }
    }
}
