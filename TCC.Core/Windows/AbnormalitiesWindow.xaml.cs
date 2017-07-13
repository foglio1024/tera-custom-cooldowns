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
            InitWindow(SettingsManager.BuffBarWindowSettings);

            buffs.ItemsSource = BuffBarWindowViewModel.Instance.Player.Buffs;
            debuffs.ItemsSource = BuffBarWindowViewModel.Instance.Player.Debuffs;
            infBuffs.ItemsSource = BuffBarWindowViewModel.Instance.Player.InfBuffs;
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}
