using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public class LfgListViewModel : TSPropertyChanged
    {
        private bool _creating;
        public Listing _lastClicked;
        private string _newMessage;


        public SynchronizedObservableCollection<Listing> Listings { get; }
        public SortCommand SortCommand { get; }
        public ICollectionViewLiveShaping ListingsView { get; }
        public bool Creating
        {
            get => _creating;
            set
            {
                if (_creating == value) return;
                _creating = value;
                NPC();
            }
        }
        public string NewMessage
        {
            get => _newMessage;
            set
            {
                if (_newMessage == value) return;
                _newMessage = value;
                NPC();
            }
        }
        public bool AmIinLfg => (Listings.ToSyncArray().Any(listing =>  listing.LeaderId == SessionManager.CurrentPlayer.PlayerId 
                                                                     || listing.LeaderName == SessionManager.CurrentPlayer.Name
                                                                     || listing.Players.ToSyncArray().Any(player => player.PlayerId == SessionManager.CurrentPlayer.PlayerId)));
        public void NotifyMyLfg()
        {
            NPC(nameof(AmIinLfg));
            NPC(nameof(MyLfg));
        }

        public Listing MyLfg => Listings.FirstOrDefault(x => x.Players.Any(p => p.PlayerId == SessionManager.CurrentPlayer.PlayerId) ||
                                                             x.LeaderId == SessionManager.CurrentPlayer.PlayerId);
        public LfgListViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Listings = new SynchronizedObservableCollection<Listing>(_dispatcher);
            ListingsView = Utils.InitLiveView<Listing>(null, Listings, new string[] { }, new string[] { });
            SortCommand = new SortCommand(ListingsView);
            Listings.CollectionChanged += ListingsOnCollectionChanged;
        }

        private void ListingsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //NotifyMyLfg();
            Task.Delay(500).ContinueWith(t => { _dispatcher.Invoke(NotifyMyLfg); });

        }

        internal void RemoveDeadLfg()
        {
            Listings.Remove(_lastClicked);
        }

    }

    public class SortCommand : ICommand
    {
        private ICollectionViewLiveShaping _view;
        private ListSortDirection _direction = ListSortDirection.Ascending;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var f = (string)parameter;
            _direction = _direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            ((CollectionView)_view).SortDescriptions.Clear();
            ((CollectionView)_view).SortDescriptions.Add(new SortDescription(f, _direction));
        }
        public SortCommand(ICollectionViewLiveShaping view)
        {
            _view = view;
        }
    }

}
