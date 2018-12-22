using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TCC.Data;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per NewDungeonDialog.xaml
    /// </summary>
    public partial class NewDungeonDialog : Window
    {
        public NewDungeonDialog()
        {
            InitializeComponent();
        }

        public ICollectionView Dungeons => Utils.InitView(d => d != null, SessionManager.CurrentDatabase.DungeonDatabase.Dungeons.Values, new[]
        {
            new SortDescription(nameof(Dungeon.Name), ListSortDirection.Ascending)
        });
        private void AddDungeon(object sender, RoutedEventArgs e)
        {
            var dg = (sender as FrameworkElement).DataContext as Dungeon;
            if (dg.Show) return;
            dg.Show = true;
            dg.Index = int.MaxValue;
            //WindowManager.Dashboard.VM.ExN(nameof(DashboardViewModel.SortedColumns));
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
