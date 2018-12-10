using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TCC.Data;
using TCC.ViewModels;

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

        public ICollectionView Dungeons => Utils.InitView(d => d != null, SessionManager.DungeonDatabase.Dungeons.Values, new[]
        {
            new SortDescription(nameof(Dungeon.Name), ListSortDirection.Ascending)
        });
        private void AddDungeon(object sender, RoutedEventArgs e)
        {
            var dg = (sender as FrameworkElement).DataContext as Dungeon;
            if (dg.Show) return;
            dg.Show = true;
            dg.Index = int.MaxValue;
            //WindowManager.Dashboard.VM.ExNPC(nameof(DashboardViewModel.SortedColumns));
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
