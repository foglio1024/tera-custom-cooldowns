using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Dragablz;
using FoglioUtils;
using TCC.Controls.Dashboard;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Windows
{
    public partial class DungeonEditWindow
    {

        public DungeonEditWindow()
        {
            InitializeComponent();
            DataContext = WindowManager.ViewModels.DashboardVM;
        }
        public IEnumerable<ItemLevelTier> ItemLevelTiers => EnumUtils.ListFromEnum<ItemLevelTier>();
        public IEnumerable<ResetMode> ResetModes => EnumUtils.ListFromEnum<ResetMode>();

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Game.DB.DungeonDatabase.SaveCustomDefs();
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
