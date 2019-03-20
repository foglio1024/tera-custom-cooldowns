using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using TCC.Data;
using TCC.ProxyInterop;

namespace TCC.ViewModels
{
    public class LfgListViewModel : TSPropertyChanged
    {
        public static DispatcherTimer RequestTimer;
        public static Queue<uint> RequestQueue = new Queue<uint>();
        private bool _creating;
        public Listing LastClicked;
        private string _newMessage;
        public string LastSortDescr { get; set; } = "Message";
        public void RefreshSorting()
        {
            SortCommand.Refresh(LastSortDescr);
        }

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
                N();
            }
        }
        public string NewMessage
        {
            get => _newMessage;
            set
            {
                if (_newMessage == value) return;
                _newMessage = value;
                N();
            }
        }
        public bool AmIinLfg => Dispatcher.Invoke(() => Listings.ToSyncList().Any(listing => listing.LeaderId == SessionManager.CurrentPlayer.PlayerId
                                                                                              || listing.LeaderName == SessionManager.CurrentPlayer.Name
                                                                                              || listing.Players.ToSyncList().Any(player => player.PlayerId == SessionManager.CurrentPlayer.PlayerId)
                                                                                              || WindowManager.GroupWindow.VM.Members.ToSyncList().Any(member => member.PlayerId == listing.LeaderId)));
        public void NotifyMyLfg()
        {
            N(nameof(AmIinLfg));
            N(nameof(MyLfg));
            foreach (var listing in Listings.ToSyncList())
            {
                listing?.NotifyMyLfg();
            }
            MyLfg?.NotifyMyLfg();
        }
        public bool AmILeader => WindowManager.GroupWindow.VM.AmILeader;
        public Listing MyLfg => Dispatcher.Invoke(() => Listings.FirstOrDefault(listing => listing.Players.Any(p => p.PlayerId == SessionManager.CurrentPlayer.PlayerId)
                                                                   || listing.LeaderId == SessionManager.CurrentPlayer.PlayerId
                                                                   || WindowManager.GroupWindow.VM.Members.ToSyncList().Any(member => member.PlayerId == listing.LeaderId)
                                                             ));

        public bool StayClosed { get; set; }

        public LfgListViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Listings = new SynchronizedObservableCollection<Listing>(Dispatcher);
            ListingsView = Utils.InitLiveView(null, Listings, new string[] { }, new SortDescription[] { });
            SortCommand = new SortCommand(ListingsView);
            Listings.CollectionChanged += ListingsOnCollectionChanged;
            WindowManager.GroupWindow.VM.PropertyChanged += OnGroupWindowVmPropertyChanged;
            RequestTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Background, RequestNextLfg, Dispatcher);
            RequestTimer.Start();
        }

        private void RequestNextLfg(object sender, EventArgs e)
        {
            if (!Settings.SettingsHolder.LfgEnabled) return;
            if (RequestQueue.Count == 0) return;
           
            var req = RequestQueue.Dequeue();
            if (req == 0)
            {
                StayClosed = true;
                Proxy.RequestLfgList();
            }
            else
            {
                Proxy.RequestPartyInfo(req);
            }
        }

        public void EnqueueRequest(uint id)
        {
            if ((SessionManager.IsInDungeon || SessionManager.CivilUnrestZone) && SessionManager.Combat) return;
            Dispatcher.Invoke(() =>
            {
                if (RequestQueue.Count > 0 && RequestQueue.Last() == id) return;
                RequestQueue.Enqueue(id);
            });
        }

        private void OnGroupWindowVmPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GroupWindowViewModel.AmILeader)) N(nameof(AmILeader));
        }

        private void ListingsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //NotifyMyLfg();
            Task.Delay(500).ContinueWith(t => { Dispatcher.Invoke(NotifyMyLfg); });

        }

        internal void RemoveDeadLfg()
        {
            Listings.Remove(LastClicked);
        }

        public void EnqueueListRequest()
        {
            Dispatcher.Invoke(() =>
            {
                if (RequestQueue.Count > 0 && RequestQueue.Last() == 0) return;
                RequestQueue.Enqueue(0);
            });

        }
    }

    public class SortCommand : ICommand
    {
        private readonly ICollectionViewLiveShaping _view;
        private bool _refreshing;
        private ListSortDirection _direction = ListSortDirection.Ascending;
#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var f = (string)parameter;
            if (!_refreshing) _direction = _direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            ((CollectionView)_view).SortDescriptions.Clear();
            ((CollectionView)_view).SortDescriptions.Add(new SortDescription(f, _direction));
            WindowManager.LfgListWindow.VM.LastSortDescr = parameter.ToString();
        }
        public SortCommand(ICollectionViewLiveShaping view)
        {
            _view = view;
        }

        public void Refresh(string lastSortDescr)
        {
            _refreshing = true;
            Execute(lastSortDescr);
            _refreshing = false;
        }
    }

}
