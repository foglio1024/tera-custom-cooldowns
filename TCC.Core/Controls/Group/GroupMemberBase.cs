using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data.Pc;
using TCC.Settings;
using TCC.ViewModels;

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

        public bool ShowHp => WindowManager.GroupWindow.VM.Size <= SettingsHolder.HideHpThreshold;
        public bool ShowMp => WindowManager.GroupWindow.VM.Size <= SettingsHolder.HideMpThreshold;
        public bool ShowBuffs => WindowManager.GroupWindow.VM.Size <= SettingsHolder.HideBuffsThreshold;
        public bool ShowDebuffs => WindowManager.GroupWindow.VM.Size <= SettingsHolder.HideDebuffsThreshold;
        public bool ShowLaurel => SettingsHolder.ShowMembersLaurels;
        public bool ShowAwaken => SettingsHolder.ShowAwakenIcon;
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
            Loaded += (_, __) =>
            {
                UpdateSettings();
                WindowManager.GroupWindow.VM.SettingsUpdated += UpdateSettings;
                WindowManager.GroupWindow.VM.PropertyChanged += (___, args) => { if (args.PropertyName == nameof(GroupWindowViewModel.Size)) UpdateSettings(); };
                SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
            };
            Unloaded += (_, __) => SettingsWindowViewModel.AbnormalityShapeChanged -= OnAbnormalityShapeChanged;
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
        }
        private void OnAbnormalityShapeChanged()
        {
            CurrentAbnormalityTemplateSelector = null;
            CurrentAbnormalityTemplateSelector = InitialAbnormalityDataTemplateSelector;
        }

        protected void ShowUserMenu(object sender, MouseButtonEventArgs e)
        {
            var dc = (User)DataContext;
            Proxy.Proxy.AskInteractive(dc.ServerId, dc.Name);
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