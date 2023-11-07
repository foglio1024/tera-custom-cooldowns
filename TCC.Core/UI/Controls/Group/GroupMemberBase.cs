using System.Collections;
using System.Collections.Generic;
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

namespace TCC.UI.Controls.Group;

public class GroupMemberBase : UserControl, INotifyPropertyChanged
{
    #region INPC
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void NPC([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion


    DataTemplateSelector? _currentAbnormalityTemplateSelector;
    DataTemplateSelector? _initialAbnormalityDataTemplateSelector;
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

    public bool ShowHp => Game.Group.Size <= App.Settings.GroupWindowSettings.HideHpThreshold;
    public bool ShowMp => Game.Group.Size <= App.Settings.GroupWindowSettings.HideMpThreshold;

    public bool ShowSt => Game.Group.Size <= App.Settings.GroupWindowSettings.HideStThreshold
        && Game.Server.Region == "EU"
        && DataContext is User
        {
            UserClass: Class.Valkyrie
                    or Class.Archer
                    or Class.Brawler 
                    or Class.Gunner 
                    or Class.Lancer 
                    or Class.Ninja 
                    or Class.Warrior
        };
    public bool ShowBuffs => Game.Group.Size <= App.Settings.GroupWindowSettings.HideBuffsThreshold;
    public bool ShowDebuffs => Game.Group.Size <= App.Settings.GroupWindowSettings.HideDebuffsThreshold;
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

    void OnLoaded(object _, RoutedEventArgs __)
    {
        UpdateSettings();

        if (DesignerProperties.GetIsInDesignMode(this)) return;

        Game.Group.CompositionChanged += OnGroupCompositionChanged;
        SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
        WindowManager.ViewModels.GroupVM.SettingsUpdated += UpdateSettings;
    }

    void OnUnloaded(object _, RoutedEventArgs __)
    {
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;

        if (DesignerProperties.GetIsInDesignMode(this)) return;

        Game.Group.CompositionChanged -= OnGroupCompositionChanged;
        SettingsWindowViewModel.AbnormalityShapeChanged -= OnAbnormalityShapeChanged;
        WindowManager.ViewModels.GroupVM.SettingsUpdated -= UpdateSettings;
    }

    void OnGroupCompositionChanged(IReadOnlyCollection<GroupMemberData> members, GroupCompositionChangeReason reason)
    {
        UpdateSettings();
    }

    void UpdateSettings()
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

    void OnAbnormalityShapeChanged()
    {
        CurrentAbnormalityTemplateSelector = null;
        CurrentAbnormalityTemplateSelector = InitialAbnormalityDataTemplateSelector;
    }

    protected void ShowUserMenu(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not User user) return;
        WindowManager.ViewModels.PlayerMenuVM.Open(user.Name, user.ServerId, (int)user.Level, user.UserClass);
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