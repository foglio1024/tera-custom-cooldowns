using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;
using Nostrum.WPF.Factories;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Pc;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TCC.UI.Windows;
using TCC.Utils;

namespace TCC.ViewModels.Widgets;

//NOTE: hooks handled by Game
[TccModule]
[UsedImplicitly]
public class AbnormalityWindowViewModel : TccWindowViewModel
{
    public Player Player => Game.Me;
    public FlowDirection Direction => ((BuffWindowSettings)Settings!).Direction;

    Thickness _globalMargin;
    public Thickness GlobalMargin
    {
        get => _globalMargin;
        set
        {
            if (_globalMargin == value) return;
            _globalMargin = value;
            N();
        }
    }

    Thickness _containersMargin;
    public Thickness ContainersMargin
    {
        get => _containersMargin;
        set
        {
            if (_containersMargin == value) return;
            _containersMargin = value;
            N();
        }
    }

    public ControlShape Shape => App.Settings.AbnormalityShape;
    public ICollectionViewLiveShaping BuffsView { get; }
    public ICollectionViewLiveShaping InfBuffsView { get; }
    public ICollectionViewLiveShaping DebuffsView { get; }

    public AbnormalityWindowViewModel(WindowSettingsBase settings) : base(settings)
    {
        Player.InitAbnormalityCollections(_dispatcher);

        ((BuffWindowSettings)settings).DirectionChanged += () => ExN(nameof(Direction));
        ((BuffWindowSettings)settings).OverlapChanged += () =>
        {
            GlobalMargin = Direction switch // todo: check
            {
                FlowDirection.LeftToRight => new Thickness { Right = -((BuffWindowSettings)Settings!).Overlap },
                FlowDirection.RightToLeft => new Thickness { Left = -((BuffWindowSettings)Settings!).Overlap },
                _ => throw new ArgumentOutOfRangeException()
            };

            ContainersMargin = new Thickness { Right = ((BuffWindowSettings)Settings!).Overlap }; // todo: check
        };

        BuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.Buffs,
            predicate: x => !x.IsHidden,
            filters: new[]{ $"{nameof(AbnormalityDuration.IsHidden)}" },
            sortFilters: new []
            {
                new SortDescription($"{nameof(AbnormalityDuration.Abnormality)}.{nameof(Abnormality.Type)}", ListSortDirection.Descending),
                new SortDescription($"{nameof(AbnormalityDuration.CanBeHidden)}", ListSortDirection.Ascending),
                new SortDescription($"{nameof(AbnormalityDuration.TimeOfArrival)}", ListSortDirection.Ascending)
            }) ?? throw new Exception("Failed to create LiveCollectionView");
        InfBuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.InfBuffs,
            predicate: x => !x.IsHidden,
            filters: new[] { $"{nameof(AbnormalityDuration.IsHidden)}" },
            sortFilters: new[]
            {
                new SortDescription($"{nameof(AbnormalityDuration.Abnormality)}.{nameof(Abnormality.Type)}", ListSortDirection.Descending),
                new SortDescription($"{nameof(AbnormalityDuration.CanBeHidden)}", ListSortDirection.Ascending),
                new SortDescription($"{nameof(AbnormalityDuration.TimeOfArrival)}", ListSortDirection.Ascending)
            }) ?? throw new Exception("Failed to create LiveCollectionView");

        DebuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.Debuffs,
            predicate: x => !x.IsHidden,
            filters: new[] { $"{nameof(AbnormalityDuration.IsHidden)}" },
            sortFilters: new[]
            {
                new SortDescription($"{nameof(AbnormalityDuration.Abnormality)}.{nameof(Abnormality.Type)}", ListSortDirection.Descending),
                new SortDescription($"{nameof(AbnormalityDuration.CanBeHidden)}", ListSortDirection.Ascending),
                new SortDescription($"{nameof(AbnormalityDuration.TimeOfArrival)}", ListSortDirection.Ascending)
            }) ?? throw new Exception("Failed to create LiveCollectionView");

        KeyboardHook.Instance.RegisterCallback(App.Settings.AbnormalSettingsHotkey, OnShowAbnormalConfigHotkeyPressed);

    }

    void OnShowAbnormalConfigHotkeyPressed()
    {
        _dispatcher.InvokeAsync(() =>
        {
            MyAbnormalConfigWindow.Instance.ShowWindow();
        },
        DispatcherPriority.Background);
    }
}