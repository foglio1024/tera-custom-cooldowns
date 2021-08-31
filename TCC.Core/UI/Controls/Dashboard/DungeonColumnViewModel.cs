using System.ComponentModel;
using System.Windows.Input;
using Nostrum;
using Nostrum.WPF;
using Nostrum.WPF.Factories;
using Nostrum.WPF.ThreadSafe;
using TCC.Data;
using TCC.Data.Pc;

namespace TCC.UI.Controls.Dashboard
{
    public class DungeonColumnViewModel : ThreadSafePropertyChanged
    {
        private bool _hilight;

        public Dungeon Dungeon { get; }

        private void OnDungeonPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Dungeon.Show)) return;
            N(nameof(IsVisible));
        }

        public ThreadSafeObservableCollection<DungeonCooldownViewModel> DungeonsList { get; private set; }
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

        public DungeonColumnViewModel(Dungeon dungeon)
        {
            Dungeon = dungeon;
            Dungeon.PropertyChanged += OnDungeonPropertyChanged;
            DungeonsList = new ThreadSafeObservableCollection<DungeonCooldownViewModel>();
            DungeonsListView = CollectionViewFactory.CreateLiveCollectionView(DungeonsList,
                o => !o.Owner.Hidden,
                new[] { $"{nameof(DungeonCooldownViewModel.Owner)}.{nameof(Character.Hidden)}" },
                new[] { new SortDescription($"{nameof(DungeonCooldownViewModel.Owner)}.{nameof(Character.Position)}", ListSortDirection.Ascending) });
            RemoveDungeonCommand = new RelayCommand(_ => Dungeon.Show = false);
        }
    }
}