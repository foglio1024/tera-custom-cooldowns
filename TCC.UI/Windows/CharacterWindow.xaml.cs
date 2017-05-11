using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TCC.Parsing;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per HPbar.xaml
    /// </summary>
    public partial class CharacterWindow : TccWindow
    {
        public CharacterWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow();

            Left = SettingsManager.CharacterWindowSettings.X;
            Top = SettingsManager.CharacterWindowSettings.Y;
            Visibility = SettingsManager.CharacterWindowSettings.Visibility;
            SetClickThru(SettingsManager.CharacterWindowSettings.ClickThru);

            rootGrid.DataContext = CharacterWindowManager.Instance.Player;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

            SettingsManager.CharacterWindowSettings.X = this.Left;
            SettingsManager.CharacterWindowSettings.Y = this.Top;
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}

