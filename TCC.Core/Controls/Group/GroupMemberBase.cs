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

namespace TCC.Controls.Group
{
    public class GroupMemberBase : UserControl, INotifyPropertyChanged
    {
        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NPC([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        protected DataTemplateSelector InitialAbnormalityDataTemplateSelector
        {
            private get => _initialAbnormalityDataTemplateSelector;
            set
            {
                _initialAbnormalityDataTemplateSelector = value;
                _currentAbnormalityTemplateSelector = value;
            }
        }

        private DataTemplateSelector _currentAbnormalityTemplateSelector;
        private DataTemplateSelector _initialAbnormalityDataTemplateSelector;

        public bool ShowHp => WindowManager.ViewModels.GroupVM.Size <= App.Settings.GroupWindowSettings.HideHpThreshold;
        public bool ShowMp => WindowManager.ViewModels.GroupVM.Size <= App.Settings.GroupWindowSettings.HideMpThreshold;
        public bool ShowBuffs => WindowManager.ViewModels.GroupVM.Size <= App.Settings.GroupWindowSettings.HideBuffsThreshold;
        public bool ShowDebuffs => WindowManager.ViewModels.GroupVM.Size <= App.Settings.GroupWindowSettings.HideDebuffsThreshold;
        public bool ShowLaurel => App.Settings.GroupWindowSettings.ShowLaurels;
        public bool ShowAwaken => App.Settings.GroupWindowSettings.ShowAwakenIcon;
        public bool ShowHpAmount => App.Settings.GroupWindowSettings.HpLabelMode == GroupHpLabelMode.Amount && ShowHp;
        public bool ShowHpPercentage => App.Settings.GroupWindowSettings.HpLabelMode == GroupHpLabelMode.Percentage && ShowHp;
        public DataTemplateSelector CurrentAbnormalityTemplateSelector
        {
            get => _currentAbnormalityTemplateSelector;
            protected set
            {
                if (_currentAbnormalityTemplateSelector == value) return;
                _currentAbnormalityTemplateSelector = value;
                NPC();
            }
        }
        public IEnumerable BuffsSource => ShowBuffs ? (DataContext as User)?.Buffs : null;
        public IEnumerable DebuffsSource => ShowDebuffs ? (DataContext as User)?.Debuffs : null;

        public GroupMemberBase()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object _, RoutedEventArgs __)
        {
            UpdateSettings();
            WindowManager.ViewModels.GroupVM.SettingsUpdated += UpdateSettings;
            WindowManager.ViewModels.GroupVM.PropertyChanged += OnGroupVmPropertyChanged;
            SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
        }

        private void OnUnloaded(object _, RoutedEventArgs __)
        {
            SettingsWindowViewModel.AbnormalityShapeChanged -= OnAbnormalityShapeChanged;
            WindowManager.ViewModels.GroupVM.SettingsUpdated -= UpdateSettings;
            WindowManager.ViewModels.GroupVM.PropertyChanged -= OnGroupVmPropertyChanged;
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
        }
        private void OnGroupVmPropertyChanged(object ___, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(GroupWindowViewModel.Size)) UpdateSettings();
        }

        private void UpdateSettings()
        {
            NPC(nameof(ShowHp));
            NPC(nameof(ShowMp));
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
            var dc = (User)DataContext;
            WindowManager.ViewModels.PlayerMenuVM.Open(dc.Name, dc.ServerId, (int)dc.Level, dc.UserClass, dc.EntityId);
            //ProxyInterface.Instance.Stub.AskInteractive(dc.ServerId, dc.Name); //ProxyOld.AskInteractive(dc.ServerId, dc.Name);
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