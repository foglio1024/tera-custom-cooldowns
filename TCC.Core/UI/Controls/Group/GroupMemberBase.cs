using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data;
using TCC.Data.Pc;
using TCC.ViewModels;
using TCC.ViewModels.Widgets;
using TeraDataLite;

namespace TCC.UI.Controls.Group
{
    public class GroupMemberBase : UserControl, INotifyPropertyChanged
    {
        #region INPC
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NPC([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        private DataTemplateSelector? _currentAbnormalityTemplateSelector;
        private DataTemplateSelector? _initialAbnormalityDataTemplateSelector;
        protected DataTemplateSelector? InitialAbnormalityDataTemplateSelector
        {
            private get => _initialAbnormalityDataTemplateSelector;
            set
            {
                _initialAbnormalityDataTemplateSelector = value;
                _currentAbnormalityTemplateSelector = value;
            }
        }
        public DataTemplateSelector? CurrentAbnormalityTemplateSelector
        {
            get => _currentAbnormalityTemplateSelector;
            protected set
            {
                if (_currentAbnormalityTemplateSelector == value) return;
                _currentAbnormalityTemplateSelector = value;
                NPC();
            }
        }

        public bool ShowHp => WindowManager.ViewModels.GroupVM.Size <= App.Settings.GroupWindowSettings.HideHpThreshold;
        public bool ShowMp => WindowManager.ViewModels.GroupVM.Size <= App.Settings.GroupWindowSettings.HideMpThreshold;

        public bool ShowSt =>
            (WindowManager.ViewModels.GroupVM.Size <= App.Settings.GroupWindowSettings.HideStThreshold)
            && Game.Server.Region == "EU"
            && (DataContext is User u)
            && (u.UserClass == Class.Valkyrie
                || u.UserClass == Class.Archer
                || u.UserClass == Class.Brawler
                || u.UserClass == Class.Gunner
                || u.UserClass == Class.Lancer
                || u.UserClass == Class.Ninja
                || u.UserClass == Class.Warrior);
        public bool ShowBuffs => WindowManager.ViewModels.GroupVM.Size <= App.Settings.GroupWindowSettings.HideBuffsThreshold;
        public bool ShowDebuffs => WindowManager.ViewModels.GroupVM.Size <= App.Settings.GroupWindowSettings.HideDebuffsThreshold;
        public bool ShowLaurel => App.Settings.GroupWindowSettings.ShowLaurels;
        public bool ShowAwaken => App.Settings.GroupWindowSettings.ShowAwakenIcon;
        public bool ShowHpAmount => App.Settings.GroupWindowSettings.HpLabelMode == GroupHpLabelMode.Amount && ShowHp;
        public bool ShowHpPercentage => App.Settings.GroupWindowSettings.HpLabelMode == GroupHpLabelMode.Percentage && ShowHp;
        public IEnumerable? BuffsSource => ShowBuffs ? (DataContext as User)?.Buffs : null;
        public IEnumerable? DebuffsSource => ShowDebuffs ? (DataContext as User)?.Debuffs : null;

        protected GroupMemberBase()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            PreviewMouseRightButtonDown += ShowUserMenu;
        }

        private void OnLoaded(object _, RoutedEventArgs __)
        {
            UpdateSettings();

            if (DesignerProperties.GetIsInDesignMode(this)) return;

            WindowManager.ViewModels.GroupVM.SettingsUpdated += UpdateSettings;
            WindowManager.ViewModels.GroupVM.PropertyChanged += OnGroupVmPropertyChanged;
            SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
        }
        private void OnUnloaded(object _, RoutedEventArgs __)
        {
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;

            if (DesignerProperties.GetIsInDesignMode(this)) return;

            SettingsWindowViewModel.AbnormalityShapeChanged -= OnAbnormalityShapeChanged;
            WindowManager.ViewModels.GroupVM.SettingsUpdated -= UpdateSettings;
            WindowManager.ViewModels.GroupVM.PropertyChanged -= OnGroupVmPropertyChanged;
        }
        private void OnGroupVmPropertyChanged(object ___, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(GroupWindowViewModel.Size)) UpdateSettings();
        }

        private void UpdateSettings()
        {
            NPC(nameof(ShowHp));
            NPC(nameof(ShowMp));
            NPC(nameof(ShowSt));
            NPC(nameof(ShowBuffs));
            NPC(nameof(ShowDebuffs));
            NPC(nameof(BuffsSource));
            NPC(nameof(DebuffsSource));
            NPC(nameof(ShowLaurel));
            NPC(nameof(ShowAwaken));
            NPC(nameof(ShowHpAmount));
            NPC(nameof(ShowHpPercentage));
        }
        private void OnAbnormalityShapeChanged()
        {
            CurrentAbnormalityTemplateSelector = null;
            CurrentAbnormalityTemplateSelector = InitialAbnormalityDataTemplateSelector;
        }

        protected void ShowUserMenu(object sender, MouseButtonEventArgs e)
        {
            if (!(DataContext is User user)) return;
            WindowManager.ViewModels.PlayerMenuVM.Open(user.Name, user.ServerId, (int)user.Level, user.UserClass, user.EntityId);
        }
        protected void ToolTip_OnOpened(object sender, RoutedEventArgs e)
        {
            FocusManager.PauseTopmost = true;
        }
        protected void ToolTip_OnClosed(object sender, RoutedEventArgs e)
        {
            FocusManager.PauseTopmost = false;
        }
    }
}