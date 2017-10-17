using System;
using System.Collections;
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
    /// Logica di interazione per SelectionSetting.xaml
    /// </summary>
    public partial class SelectionSetting : UserControl
    {
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
            DependencyProperty.Register("SettingName", typeof(string), typeof(SelectionSetting));
        public ImageSource SettingImage
        {
            get { return (ImageSource)GetValue(SettingImageProperty); }
            set { SetValue(SettingImageProperty, value); }
        }
        public static readonly DependencyProperty SettingImageProperty =
            DependencyProperty.Register("SettingImage", typeof(ImageSource), typeof(SelectionSetting));



        public IEnumerable Choices
        {
            get { return (IEnumerable)GetValue(ChoicesProperty); }
            set { SetValue(ChoicesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Choices.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChoicesProperty =
            DependencyProperty.Register("Choices", typeof(IEnumerable), typeof(SelectionSetting));



        public string SelectedItem
        {
            get { return (string)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(string), typeof(SelectionSetting));



        public Type ChoicesType
        {
            get { return (Type)GetValue(ChoicesTypeProperty); }
            set { SetValue(ChoicesTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChoicesType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChoicesTypeProperty =
            DependencyProperty.Register("ChoicesType", typeof(Type), typeof(SelectionSetting));



        public SelectionSetting()
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

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = (ComboBox)sender;
            SelectedItem = cb.SelectedItem.ToString();
        }

        private void ComboBox_OnDropDownOpened(object sender, EventArgs e)
        {
        }

        private void SelectionSetting_OnLoaded(object sender, RoutedEventArgs e)
        {
            int i = 0;
            if(Choices == null) return;
            foreach (var choice in Choices)
            {
                if (choice.ToString() == SelectedItem)
                {
                    Cbox.SelectedIndex = i;
                }
                i++;
            }

        }
    }
}
