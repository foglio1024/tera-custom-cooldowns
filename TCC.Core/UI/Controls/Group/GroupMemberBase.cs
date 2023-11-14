using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nostrum.WPF.Factories;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Pc;
using TCC.ViewModels;
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
    public ICollectionViewLiveShaping? BuffsSource => ShowBuffs ? _buffs : null;
    public ICollectionViewLiveShaping? DebuffsSource => ShowDebuffs ? _debuffs : null;

    ICollectionViewLiveShaping _buffs;
    ICollectionViewLiveShaping _debuffs;

    protected GroupMemberBase()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        PreviewMouseRightButtonDown += ShowUserMenu;
        MouseEnter += OnMouseEnter;
        MouseLeave += OnMouseLeave;
    }

    void OnMouseEnter(object sender, MouseEventArgs e)
    {
        SetAbnormalitiesVisibility(true);
    }
    void OnMouseLeave(object sender, MouseEventArgs e)
    {
        Task.Delay(1000).ContinueWith(_ => Dispatcher.InvokeAsync(() =>
        {
            if (IsMouseOver) return;
            SetAbnormalitiesVisibility(false);
        }));
    }

    void SetAbnormalitiesVisibility(bool visible)
    {
        if (DataContext is not User user) return;

        var buffs = user.Buffs.ToSyncList()
            .Where(x => x.CanBeHidden)
            .ToArray();
        var debuffs = user.Debuffs.ToSyncList()
            .Where(x => x.CanBeHidden)
            .ToArray();

        foreach (var abnormality in buffs)
        {
            abnormality.IsHidden = !visible;
        }
        foreach (var abnormality in debuffs)
        {
            abnormality.IsHidden = !visible;
        }
    }
    void OnLoaded(object _, RoutedEventArgs __)
    {
        UpdateSettings();

        if (DesignerProperties.GetIsInDesignMode(this)) return;

        Game.Group.CompositionChanged += OnGroupCompositionChanged;
        SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
        WindowManager.ViewModels.GroupVM.SettingsUpdated += UpdateSettings;

        if (DataContext is User user)
        {
            _buffs = CollectionViewFactory.CreateLiveCollectionView(user.Buffs,
                predicate: x => !x.IsHidden,
                filters: new[] { $"{nameof(AbnormalityDuration.IsHidden)}" },
                sortFilters: new[]
                {
                    new SortDescription($"{nameof(AbnormalityDuration.Abnormality)}.{nameof(Abnormality.Type)}", ListSortDirection.Descending),
                    new SortDescription($"{nameof(AbnormalityDuration.Abnormality)}.{nameof(Abnormality.Infinity)}", ListSortDirection.Descending),
                    new SortDescription($"{nameof(AbnormalityDuration.CanBeHidden)}", ListSortDirection.Ascending),
                    new SortDescription($"{nameof(AbnormalityDuration.TimeOfArrival)}", ListSortDirection.Ascending)
            }) ?? throw new Exception("Failed to create LiveCollectionView");

            _debuffs = CollectionViewFactory.CreateLiveCollectionView(user.Debuffs,
                predicate: x => !x.IsHidden,
                filters: new[] { $"{nameof(AbnormalityDuration.IsHidden)}" },
                sortFilters: new[]
                {
                    new SortDescription($"{nameof(AbnormalityDuration.Abnormality)}.{nameof(Abnormality.Type)}", ListSortDirection.Descending),
                    new SortDescription($"{nameof(AbnormalityDuration.Abnormality)}.{nameof(Abnormality.Infinity)}", ListSortDirection.Descending),
                    new SortDescription($"{nameof(AbnormalityDuration.CanBeHidden)}", ListSortDirection.Ascending),
                    new SortDescription($"{nameof(AbnormalityDuration.TimeOfArrival)}", ListSortDirection.Ascending)
            }) ?? throw new Exception("Failed to create LiveCollectionView");

            NPC(nameof(BuffsSource));
            NPC(nameof(DebuffsSource));
        }
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