using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.Controls.Settings
{
    /// <summary>
    /// Logica di interazione per SelectionSetting.xaml
    /// </summary>
    public partial class SelectionSetting
    {
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
            DependencyProperty.Register("SettingName", typeof(string), typeof(SelectionSetting));
        public ImageSource SettingImage
        {
            get => (ImageSource)GetValue(SettingImageProperty);
            set => SetValue(SettingImageProperty, value);
        }
        public static readonly DependencyProperty SettingImageProperty =
            DependencyProperty.Register("SettingImage", typeof(ImageSource), typeof(SelectionSetting));

        public IEnumerable Choices
        {
            get => (IEnumerable)GetValue(ChoicesProperty);
            set => SetValue(ChoicesProperty, value);
        }
        public static readonly DependencyProperty ChoicesProperty =
            DependencyProperty.Register("Choices", typeof(IEnumerable), typeof(SelectionSetting));

        public string SelectedItem
        {
            get => (string)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(string), typeof(SelectionSetting));

        public Type ChoicesType
        {
            get => (Type)GetValue(ChoicesTypeProperty);
            set => SetValue(ChoicesTypeProperty, value);
        }
        public static readonly DependencyProperty ChoicesTypeProperty =
            DependencyProperty.Register("ChoicesType", typeof(Type), typeof(SelectionSetting));



        public SelectionSetting()
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
            ((Grid) sender).Background.BeginAnimation(SolidColorBrush.ColorProperty, _glow);
            Img.BeginAnimation(OpacityProperty, _fadeIn);
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Grid) sender).Background.BeginAnimation(SolidColorBrush.ColorProperty, _unglow);
            Img.BeginAnimation(OpacityProperty, _fadeOut);

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
            var i = 0;
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
