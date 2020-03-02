using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Nostrum.Factories;
using TCC.Data;

namespace TCC.UI.Windows
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

        public ICollectionView Dungeons => CollectionViewFactory.CreateCollectionView(Game.DB.DungeonDatabase.Dungeons.Values, d => d != null, new[]
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
