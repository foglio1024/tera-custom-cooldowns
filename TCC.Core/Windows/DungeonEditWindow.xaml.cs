using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Dragablz;
using TCC.Controls.Dashboard;
using TCC.Data;

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
        public IEnumerable<ResetMode> ResetModes => Utils.ListFromEnum<ResetMode>();

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            SessionManager.CurrentDatabase.DungeonDatabase.SaveCustomDefs();
            Close();

        }
        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void RemoveDungeon(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement) sender).DataContext is DungeonColumnViewModel dng) dng.Dungeon.Show = false;
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
