using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Nostrum.WPF.Factories;
using TCC.Data;

namespace TCC.UI.Windows;

public partial class NewDungeonDialog
{
    public NewDungeonDialog()
    {
        InitializeComponent();
    }

    public ICollectionView Dungeons => CollectionViewFactory.CreateCollectionView(Game.DB!.DungeonDatabase.Dungeons.Values, null , new[]
    {
        new SortDescription(nameof(Dungeon.Name), ListSortDirection.Ascending)
    });

    void AddDungeon(object sender, RoutedEventArgs e)
    {
        if (((FrameworkElement)sender).DataContext is not Dungeon dg) return;
        if (dg.Show) return;
        dg.Show = true;
        dg.Index = int.MaxValue;
        dg.Cost = 0;
        dg.DoublesOnElite = true;
        dg.ItemLevel = 0;
        dg.HasDef = true;
        //WindowManager.ViewModels.Dashboard.RefreshDungeons();
    }

    void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();

    }

    void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}