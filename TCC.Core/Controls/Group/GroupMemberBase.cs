using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data.Pc;
using TCC.Interop.Proxy;
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

        public bool ShowHp => WindowManager.ViewModels.Group.Size <= App.Settings.GroupWindowSettings.HideHpThreshold;
        public bool ShowMp => WindowManager.ViewModels.Group.Size <= App.Settings.GroupWindowSettings.HideMpThreshold;
        public bool ShowBuffs => WindowManager.ViewModels.Group.Size <= App.Settings.GroupWindowSettings.HideBuffsThreshold;
        public bool ShowDebuffs => WindowManager.ViewModels.Group.Size <= App.Settings.GroupWindowSettings.HideDebuffsThreshold;
        public bool ShowLaurel => App.Settings.GroupWindowSettings.ShowLaurels;
        public bool ShowAwaken => App.Settings.GroupWindowSettings.ShowAwakenIcon;
        public bool ShowHpNumbers => App.Settings.GroupWindowSettings.ShowHpLabels && ShowHp;
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
                WindowManager.ViewModels.Group.SettingsUpdated += UpdateSettings;
                WindowManager.ViewModels.Group.PropertyChanged += (___, args) => { if (args.PropertyName == nameof(GroupWindowViewModel.Size)) UpdateSettings(); };
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
            NPC(nameof(ShowHpNumbers));
        }
        private void OnAbnormalityShapeChanged()
        {
            CurrentAbnormalityTemplateSelector = null;
            CurrentAbnormalityTemplateSelector = InitialAbnormalityDataTemplateSelector;
        }

        protected void ShowUserMenu(object sender, MouseButtonEventArgs e)
        {
            var dc = (User)DataContext;
            ProxyInterface.Instance.Stub.AskInteractive(dc.ServerId, dc.Name); //ProxyOld.AskInteractive(dc.ServerId, dc.Name);
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