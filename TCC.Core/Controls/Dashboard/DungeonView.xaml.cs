using FoglioUtils;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data;
using TCC.Data.Pc;
using FoglioUtils.Extensions;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC.Controls.Dashboard
{
    /// <summary>
    /// Logica di interazione per DungeonView.xaml
    /// </summary>
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

            var dng = WindowManager.ViewModels.Dashboard.Columns.FirstOrDefault(x => x.Dungeon.Id == dung.Id);
            if (dng != null) dng.Hilight = true;

            var ch = WindowManager.ViewModels.Dashboard.CharacterViewModels.ToList().FirstOrDefault(x => x.Character.Id == chara.Id);
            if (ch != null) ch.Hilight = true;
        }
        private void OnEntryMouseLeave(object sender, MouseEventArgs e)
        {
            var cd = (sender as FrameworkElement)?.DataContext as DungeonCooldownViewModel;
            var chara = cd?.Owner;
            var dung = cd?.Cooldown?.Dungeon;
            if (dung == null) return;
            var col = WindowManager.ViewModels.Dashboard.Columns.FirstOrDefault(x => x.Dungeon.Id == dung.Id);
            if (col != null) col.Hilight = false;
            var chVM = WindowManager.ViewModels.Dashboard.CharacterViewModels.ToList().FirstOrDefault(x => chara != null && x.Character.Id == chara.Id);
            if (chVM != null) chVM.Hilight = false;
        }
        private void OnDungeonEditButtonClick(object sender, RoutedEventArgs e)
        {
            new DungeonEditWindow() { Topmost = true, Owner = WindowManager.DashboardWindow }.ShowDialog();
        }
    }

    public class DungeonColumnViewModel : TSPropertyChanged
    {
        private bool _hilight;

        public Dungeon Dungeon { get; set; }

        public SynchronizedObservableCollection<DungeonCooldownViewModel> DungeonsList { get; private set; }
        public ICollectionViewLiveShaping DungeonsListView { get; }
        public bool Hilight
        {
            get => _hilight;
            set
            {
                if (_hilight == value) return;
                _hilight = value;
                N();
            }
        }

        public DungeonColumnViewModel()
        {
            DungeonsList = new SynchronizedObservableCollection<DungeonCooldownViewModel>();
            DungeonsListView = CollectionViewUtils.InitLiveView(o => !((DungeonCooldownViewModel)o).Owner.Hidden, DungeonsList,
                new[] { "Owner.Hidden" },
                new[]
                {
                    new SortDescription("Owner.Position", ListSortDirection.Ascending)
                }
            );
        }
    }

    public class DungeonCooldownViewModel : TSPropertyChanged
    {
        public DungeonCooldown Cooldown { get; set; }
        public Character Owner { get; set; }
    }

    public class CharacterViewModel : TSPropertyChanged
    {
        private bool _hilight;

        public bool Hilight
        {
            get => _hilight;
            set
            {
                if (_hilight == value) return;
                _hilight = value;
                N();
            }
        }
        public Character Character { get; set; }

    }


}
