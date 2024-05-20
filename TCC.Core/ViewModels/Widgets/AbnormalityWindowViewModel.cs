using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using JetBrains.Annotations;
using Nostrum.WPF;
using Nostrum.WPF.Factories;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Pc;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TCC.UI.Windows;
using TCC.UI.Windows.Configuration.Abnormalities;
using TCC.Utils;

namespace TCC.ViewModels.Widgets;

//NOTE: hooks handled by Game
[TccModule]
[UsedImplicitly]
public class AbnormalityWindowViewModel : TccWindowViewModel
{
    public Player Player => Game.Me;
    public FlowDirection Direction => ((BuffWindowSettings)Settings!).Direction;

    private Thickness _globalMargin;
    public Thickness GlobalMargin
    {
        get => _globalMargin;
        set => RaiseAndSetIfChanged(value, ref _globalMargin);
    }

    private Thickness _containersMargin;
    public Thickness ContainersMargin
    {
        get => _containersMargin;
        set => RaiseAndSetIfChanged(value, ref _containersMargin);
    }

    public ControlShape Shape => App.Settings.AbnormalityShape;
    public ICollectionViewLiveShaping BuffsView { get; }
    public ICollectionViewLiveShaping InfBuffsView { get; }
    public ICollectionViewLiveShaping DebuffsView { get; }

    public ICommand ConfigureAbnormalitiesCommand{ get; }

    public AbnormalityWindowViewModel(WindowSettingsBase settings) : base(settings)
    {
        Player.InitAbnormalityCollections(_dispatcher);

        ((BuffWindowSettings)settings).DirectionChanged += () => ExN(nameof(Direction));
        ((BuffWindowSettings)settings).OverlapChanged += OnOverlapChanged;

        BuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.Buffs,
            predicate: x => !x.IsHidden,
            filters: [$"{nameof(AbnormalityDuration.IsHidden)}"],
            sortFilters:
            [
                new SortDescription($"{nameof(AbnormalityDuration.Abnormality)}.{nameof(Abnormality.Type)}", ListSortDirection.Descending),
                new SortDescription($"{nameof(AbnormalityDuration.CanBeHidden)}", ListSortDirection.Ascending),
                new SortDescription($"{nameof(AbnormalityDuration.TimeOfArrival)}", ListSortDirection.Ascending)
            ]) ?? throw new Exception("Failed to create LiveCollectionView");
        InfBuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.InfBuffs,
            predicate: x => !x.IsHidden,
            filters: [$"{nameof(AbnormalityDuration.IsHidden)}"],
            sortFilters:
            [
                new SortDescription($"{nameof(AbnormalityDuration.Abnormality)}.{nameof(Abnormality.Type)}", ListSortDirection.Descending),
                new SortDescription($"{nameof(AbnormalityDuration.CanBeHidden)}", ListSortDirection.Ascending),
                new SortDescription($"{nameof(AbnormalityDuration.TimeOfArrival)}", ListSortDirection.Ascending)
            ]) ?? throw new Exception("Failed to create LiveCollectionView");

        DebuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.Debuffs,
            predicate: x => !x.IsHidden,
            filters: [$"{nameof(AbnormalityDuration.IsHidden)}"],
            sortFilters:
            [
                new SortDescription($"{nameof(AbnormalityDuration.Abnormality)}.{nameof(Abnormality.Type)}", ListSortDirection.Descending),
                new SortDescription($"{nameof(AbnormalityDuration.CanBeHidden)}", ListSortDirection.Ascending),
                new SortDescription($"{nameof(AbnormalityDuration.TimeOfArrival)}", ListSortDirection.Ascending)
            ]) ?? throw new Exception("Failed to create LiveCollectionView");

        KeyboardHook.Instance.RegisterCallback(App.Settings.AbnormalSettingsHotkey, ConfigureAbnormalities);

        OnOverlapChanged();

        ConfigureAbnormalitiesCommand = new RelayCommand(ConfigureAbnormalities);
    }

    private void ConfigureAbnormalities()
    {
        _dispatcher.InvokeAsync(() =>
            {
                if (TccWindow.Exists(typeof(AbnormalityConfigWindow))) return;
                new AbnormalityConfigWindow().Show();
            },
            DispatcherPriority.Background);
    }

    private void OnOverlapChanged()
    {
        GlobalMargin = Direction switch
        {
            FlowDirection.LeftToRight => new Thickness { Right = -((BuffWindowSettings)Settings!).Overlap },
            FlowDirection.RightToLeft => new Thickness { Left = -((BuffWindowSettings)Settings!).Overlap },
            _ => throw new ArgumentOutOfRangeException()
        };

        ContainersMargin = new Thickness { Right = ((BuffWindowSettings)Settings!).Overlap };
    }
}