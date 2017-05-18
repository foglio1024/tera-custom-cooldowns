using DamageMeter.Sniffing;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC
{
    public partial class CooldownWindow : TccWindow
    {
        public CooldownWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow();

            Left = SettingsManager.CooldownWindowSettings.X;
            Top = SettingsManager.CooldownWindowSettings.Y;
            Visibility = SettingsManager.CooldownWindowSettings.Visibility;
            SetClickThru(SettingsManager.CooldownWindowSettings.ClickThru);


            LongSkillsPanel.ItemsSource = CooldownBarWindowManager.Instance.LongSkills;
            LongSkillsPanel.DataContext = CooldownBarWindowManager.Instance.LongSkills;
            NormalSkillsPanel.ItemsSource = CooldownBarWindowManager.Instance.ShortSkills;
            NormalSkillsPanel.DataContext = CooldownBarWindowManager.Instance.ShortSkills;
        }


        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
            SettingsManager.CooldownWindowSettings.X = this.Left;
            SettingsManager.CooldownWindowSettings.Y = this.Top;
            SettingsManager.SaveSettings();

        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}

/*
* C_START_SKILL            0xCBC5
* S_START_COOLTIME_SKILL   0x7BA8
* C_USE_ITEM               0x750F
* S_START_COOLTIME_ITEM    0xAD63
*/