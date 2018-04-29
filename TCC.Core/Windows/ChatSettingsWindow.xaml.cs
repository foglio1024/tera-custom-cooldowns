using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per ChatSettingsWindow.xaml
    /// </summary>
    public partial class ChatSettingsWindow : Window
    {
        public ChatSettingsWindow(Tab dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
            Opacity = 0;
        }

        private void CloseChannelSettings(object sender, RoutedEventArgs e)
        {
            var an = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
            an.Completed += (s,ev) => this.Close();
            BeginAnimation(OpacityProperty, an);
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(500)));
        }
    }
}
