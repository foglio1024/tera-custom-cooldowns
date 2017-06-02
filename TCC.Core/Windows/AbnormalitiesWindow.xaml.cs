using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per AbnormalitiesWindow.xaml
    /// </summary>
    public partial class AbnormalitiesWindow : TccWindow
    {
        public AbnormalitiesWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow();

            //buffs.DataContext = BuffBarWindowManager.Instance.Player.Buffs;
            buffs.ItemsSource = BuffBarWindowManager.Instance.Player.Buffs;
            //debuffs.DataContext = BuffBarWindowManager.Instance.Player.Debuffs;
            debuffs.ItemsSource = BuffBarWindowManager.Instance.Player.Debuffs;
            //infBuffs.DataContext = BuffBarWindowManager.Instance.Player.InfBuffs;
            infBuffs.ItemsSource = BuffBarWindowManager.Instance.Player.InfBuffs;

            Left = SettingsManager.BuffBarWindowSettings.X;
            Top = SettingsManager.BuffBarWindowSettings.Y;
            Visibility = SettingsManager.BuffBarWindowSettings.Visibility;
            SetClickThru(SettingsManager.BuffBarWindowSettings.ClickThru);

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            SettingsManager.BuffBarWindowSettings.X = Left - Left % Left;
            SettingsManager.BuffBarWindowSettings.Y = Top - Top%Top;
            SettingsManager.SaveSettings();


        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}
