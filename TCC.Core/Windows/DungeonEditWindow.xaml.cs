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
using Dragablz;
using TCC.Controls.Dashboard;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per DungeonEditWindow.xaml
    /// </summary>
    public partial class DungeonEditWindow
    {

        public DungeonEditWindow()
        {
            InitializeComponent();
            DataContext = WindowManager.Dashboard.VM;
        }
        public IEnumerable<ItemLevelTier> ItemLevelTiers => Utils.ListFromEnum<ItemLevelTier>();

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            SessionManager.DungeonDatabase.SaveCustomDefs();
            Close();

        }
        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void RemoveDungeon(object sender, RoutedEventArgs e)
        {
            var dng = (sender as FrameworkElement).DataContext as DungeonColumnViewModel;
            dng.Dungeon.Show = false;
        }

        private void OnDungeonsOrderChanged(object sender, OrderChangedEventArgs e)
        {
            foreach (DungeonColumnViewModel dcvm in e.NewOrder)
            {
                dcvm.Dungeon.Index = e.NewOrder.ToList().IndexOf(dcvm);
            }
        }

        private void AddDungeon(object sender, RoutedEventArgs e)
        {
            new NewDungeonDialog { Topmost = true, Owner = this}.ShowDialog();
        }
    }
}
