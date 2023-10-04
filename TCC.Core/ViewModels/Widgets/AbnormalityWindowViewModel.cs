using System;
using Nostrum.WPF.Factories;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Pc;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TCC.UI.Windows;
using TCC.Utils;

namespace TCC.ViewModels.Widgets;

//NOTE: hooks handled by Game
[TccModule]
public class AbnormalityWindowViewModel : TccWindowViewModel
{
    public Player Player => Game.Me;
    public FlowDirection Direction => ((BuffWindowSettings)Settings!).Direction;

    public Thickness GlobalMargin
    {
        get
        {
            return Direction switch
            {
                FlowDirection.LeftToRight => new Thickness(2, 2, 2 * (1 - ((BuffWindowSettings) Settings!).Overlap), 2),
                FlowDirection.RightToLeft => new Thickness(2 * (1 - ((BuffWindowSettings) Settings!).Overlap), 2, 2, 2),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public Thickness ContainersMargin => new(0, 0, ((BuffWindowSettings)Settings!).Overlap * 2, 0);
    public ControlShape Shape => App.Settings.AbnormalityShape;
    public ICollectionViewLiveShaping BuffsView { get; }
    public ICollectionViewLiveShaping SpecBuffsView { get; }
    public ICollectionViewLiveShaping InfBuffsView { get; }
    public ICollectionViewLiveShaping SpecInfBuffsView { get; }

    public AbnormalityWindowViewModel(WindowSettingsBase settings) : base(settings)
    {
        Player.InitAbnormalityCollections(_dispatcher);

        ((BuffWindowSettings)settings).DirectionChanged += () => ExN(nameof(Direction));
        ((BuffWindowSettings)settings).OverlapChanged += () =>
        {
            ExN(nameof(GlobalMargin));
            ExN(nameof(ContainersMargin));
        };

        BuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.Buffs, a => a.Abnormality.Type != AbnormalityType.Special) ?? throw new Exception("Failed to create LiveCollectionView");
        SpecBuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.Buffs, a => a.Abnormality.Type == AbnormalityType.Special) ?? throw new Exception("Failed to create LiveCollectionView");
        InfBuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.InfBuffs, a => a.Abnormality.Type != AbnormalityType.Special) ?? throw new Exception("Failed to create LiveCollectionView");
        SpecInfBuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.InfBuffs, a => a.Abnormality.Type == AbnormalityType.Special) ?? throw new Exception("Failed to create LiveCollectionView");

        KeyboardHook.Instance.RegisterCallback(App.Settings.AbnormalSettingsHotkey, OnShowAbnormalConfigHotkeyPressed);

    }

    void OnShowAbnormalConfigHotkeyPressed()
    {
        //if (!Game.Logged) return;
        _dispatcher.InvokeAsync(() =>
            {
                MyAbnormalConfigWindow.Instance.ShowWindow();
            },
            DispatcherPriority.Background);
    }
}