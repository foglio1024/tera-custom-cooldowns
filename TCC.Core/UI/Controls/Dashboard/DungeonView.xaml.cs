using Nostrum.Extensions;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.UI.Windows;
using TCC.ViewModels;

namespace TCC.UI.Controls.Dashboard
{
    public partial class DungeonView
    {
        public DungeonView()
        {
            InitializeComponent();
            IsVisibleChanged += (_, __) => { (DataContext as DashboardViewModel)?.LoadDungeonsCommand.Execute(null); };
        }

        private void DungeonColumns_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var headerSw = DungeonHeaders.GetChild<ScrollViewer>();
            var namesSw = CharacterNames.GetChild<ScrollViewer>();

            headerSw.ScrollToHorizontalOffset(e.HorizontalOffset);
            namesSw.ScrollToVerticalOffset(e.VerticalOffset);

        }
        private void OnEntryMouseEnter(object sender, MouseEventArgs e)
        {
            if (!((sender as FrameworkElement)?.DataContext is DungeonCooldownViewModel cd)) return;
            var chara = cd.Owner;
            var dung = cd.Cooldown?.Dungeon;
            if (dung == null) return;

            var dng = WindowManager.ViewModels.DashboardVM.Columns.FirstOrDefault(x => x.Dungeon.Id == dung.Id);
            if (dng != null) dng.Hilight = true;

            var ch = WindowManager.ViewModels.DashboardVM.CharacterViewModels.ToList().FirstOrDefault(x => x.Character.Id == chara.Id);
            if (ch != null) ch.Hilight = true;
        }
        private void OnEntryMouseLeave(object sender, MouseEventArgs e)
        {
            var cd = (sender as FrameworkElement)?.DataContext as DungeonCooldownViewModel;
            var chara = cd?.Owner;
            var dung = cd?.Cooldown?.Dungeon;
            if (dung == null) return;
            var col = WindowManager.ViewModels.DashboardVM.Columns.FirstOrDefault(x => x.Dungeon.Id == dung.Id);
            if (col != null) col.Hilight = false;
            var chVM = WindowManager.ViewModels.DashboardVM.CharacterViewModels.ToList().FirstOrDefault(x => chara != null && x.Character.Id == chara.Id);
            if (chVM != null) chVM.Hilight = false;
        }
        private void OnDungeonEditButtonClick(object sender, RoutedEventArgs e)
        {
            new DungeonEditWindow() { Topmost = true, Owner = WindowManager.DashboardWindow }.ShowDialog();
        }
    }
}
