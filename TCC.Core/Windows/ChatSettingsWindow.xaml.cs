using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Windows
{
    public partial class ChatSettingsWindow
    {
        public ChatSettingsWindow(Tab dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
            Opacity = 0;
        }

        private void CloseChannelSettings(object sender, RoutedEventArgs e)
        {
            FocusManager.ForceFocused = false;

            var an = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
            an.Completed += (s,ev) => Close();
            BeginAnimation(OpacityProperty, an);
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FocusManager.ForceFocused = true;

            BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(500)));
        }
    }
}
