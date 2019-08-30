using FoglioUtils;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per NewDungeonDialog.xaml
    /// </summary>
    public partial class NewDungeonDialog
    {
        public NewDungeonDialog()
        {
            InitializeComponent();
        }

        public ICollectionView Dungeons => CollectionViewUtils.InitView(d => d != null, Game.DB.DungeonDatabase.Dungeons.Values, new[]
        {
            new SortDescription(nameof(Dungeon.Name), ListSortDirection.Ascending)
        });
        private void AddDungeon(object sender, RoutedEventArgs e)
        {
            if (!((sender as FrameworkElement)?.DataContext is Dungeon dg)) return;
            if (dg.Show) return;
            dg.Show = true;
            dg.Index = int.MaxValue;
            dg.Cost = 0;
            dg.DoublesOnElite = true;
            dg.ItemLevel = 0;
            dg.HasDef = true;
            //WindowManager.ViewModels.Dashboard.RefreshDungeons();
        }

        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
