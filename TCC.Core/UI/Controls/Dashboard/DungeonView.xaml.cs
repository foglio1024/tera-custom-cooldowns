using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nostrum;
using Nostrum.Extensions;
using Nostrum.Factories;
using TCC.Data;
using TCC.Data.Pc;
using TCC.UI.Windows;
using TCC.ViewModels;

namespace TCC.UI.Controls.Dashboard
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

    public class DungeonColumnViewModel : TSPropertyChanged
    {
        private bool _hilight;
        private Dungeon _dungeon;

        public Dungeon Dungeon
        {
            get => _dungeon;
            set
            {
                if(_dungeon == value)return;
                if(_dungeon != null) _dungeon.PropertyChanged -= OnDungeonPropertyChanged;
                _dungeon = value;
                _dungeon.PropertyChanged += OnDungeonPropertyChanged;
            }
        }

        private void OnDungeonPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Dungeon.Show)) N(nameof(IsVisible));
        }

        public TSObservableCollection<DungeonCooldownViewModel> DungeonsList { get; private set; }
        public ICollectionViewLiveShaping DungeonsListView { get; }
        public ICommand RemoveDungeonCommand { get; }
        public bool IsVisible => Dungeon.Show;
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
            DungeonsList = new TSObservableCollection<DungeonCooldownViewModel>();
            DungeonsListView = CollectionViewFactory.CreateLiveCollectionView(DungeonsList,
                o => !o.Owner.Hidden,
                new[] { $"{nameof(DungeonCooldownViewModel.Owner)}.{nameof(Character.Hidden)}" }, 
                new[] { new SortDescription($"{nameof(DungeonCooldownViewModel.Owner)}.{nameof(Character.Position)}", ListSortDirection.Ascending) });
            RemoveDungeonCommand = new RelayCommand(_ => Dungeon.Show = false);
        }
    }

    public class DungeonCooldownViewModel : TSPropertyChanged
    {
        public DungeonCooldownData Cooldown { get; set; }
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
