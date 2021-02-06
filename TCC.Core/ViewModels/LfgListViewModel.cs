using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Nostrum;
using Nostrum.Extensions;
using Nostrum.Factories;
using TCC.Data;
using TCC.Data.Pc;
using TCC.Interop.Proxy;
using TCC.Analysis;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TCC.Utils;
using TeraDataLite;
using TeraPacketParser.Messages;
using FocusManager = TCC.UI.FocusManager;

namespace TCC.ViewModels
{
    [TccModule]
    public class LfgListViewModel : TccWindowViewModel
    {
        #region Events
        public event Action<int>? Publicized;
        public event Action? CreatingStateChanged;
        public event Action? TempLfgCreated;
        public event Action? MyLfgStateChanged;

        #endregion

        // ReSharper disable once NotAccessedField.Local
        private DispatcherTimer _requestTimer;
        private readonly DispatcherTimer AutoPublicizeTimer;
        private static readonly Queue<uint> _requestQueue = new();
        private bool _creating;
        private bool _creatingRaid;
        private int _lastGroupSize;
        public Listing? LastClicked;
        private string _newMessage = "";
        private bool _isPopupOpen;

        public string LastSortDescr { get; set; } = "Message";
        public int PublicizeCooldown => 5;
        public int AutoPublicizeCooldown => 20;


        private bool _isAutoPublicizeEnabled;

        public bool IsAutoPublicizeEnabled
        {
            get => _isAutoPublicizeEnabled;
            set
            {
                if (_isAutoPublicizeEnabled == value) return;
                _isAutoPublicizeEnabled = value;
                N();
            }
        }

        public bool IsAutoPublicizeRunning => AutoPublicizeTimer.IsEnabled;
        private bool _stopAuto;
        public TSObservableCollection<Listing> Listings { get; }
        public SortCommand SortCommand { get; }
        public ICommand PublicizeCommand { get; }
        public ICommand ToggleAutoPublicizeCommand { get; }
        public ICommand CreateMessageCommand { get; }
        public ICommand RemoveMessageCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand ExpandAllCommand { get; }
        public ICommand CollapseAllCommand { get; }
        public ICommand OpenPopupCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand ToggleShowTradeListingsCommand { get; }

        public ICollectionViewLiveShaping ListingsView { get; }

        public bool Creating
        {
            get => _creating;
            set
            {
                if (_creating == value) return;
                _creating = value;
                N();
                CreatingStateChanged?.Invoke();
            }
        }

        public bool CreatingRaid
        {
            get => _creatingRaid;
            set
            {
                if (_creatingRaid == value) return;
                _creatingRaid = value;
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
                CreatingStateChanged?.Invoke();
            }
        }

        public bool AmIinLfg => Dispatcher.Invoke(() => Listings.ToSyncList().Any(listing =>
            listing.LeaderId == Game.Me.PlayerId
            || listing.LeaderName == Game.Me.Name
            || listing.Players.ToSyncList().Any(player => player.PlayerId == Game.Me.PlayerId)
            || Game.Group.Has(listing.LeaderId)));


        public bool AmILeader => Game.Group.AmILeader || (MyLfg != null && MyLfg?.LeaderId == Game.Me.PlayerId);

        public Listing? MyLfg => Dispatcher.Invoke(() => Listings.FirstOrDefault(listing =>
                listing.Players.Any(p => p.PlayerId == Game.Me.PlayerId)
                || listing.LeaderId == Game.Me.PlayerId
                || Game.Group.Has(listing.LeaderId)
        //|| WindowManager.ViewModels.GroupVM.Members.ToSyncList().Any(member => member.PlayerId == listing.LeaderId)
        ));

        public int MinLevel
        {
            get => ((LfgWindowSettings)Settings!).MinLevel;
            set
            {
                if (((LfgWindowSettings)Settings!).MinLevel == value) return;
                ((LfgWindowSettings)Settings).MinLevel = value;
                N();
                N(nameof(MaxLevel));
            }
        }

        public int MaxLevel
        {
            get => ((LfgWindowSettings)Settings!).MaxLevel;
            set
            {
                if (((LfgWindowSettings)Settings!).MaxLevel == value) return;
                ((LfgWindowSettings)Settings).MaxLevel = value;
                N();
                N(nameof(MinLevel));
            }
        }

        public bool HideTradeListings
        {
            get => ((LfgWindowSettings)Settings!).HideTradeListings;
            set
            {
                if (((LfgWindowSettings)Settings!).HideTradeListings == value) return;
                ((LfgWindowSettings)Settings).HideTradeListings = value;
                N();
            }
        }

        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set
            {
                if (_isPopupOpen == value) return;
                _isPopupOpen = value;
                FocusManager.PauseTopmost = _isPopupOpen;
                N();
            }
        }


        public bool StayClosed { get; set; }

        public LfgListViewModel(LfgWindowSettings settings) : base(settings)
        {

            Listings = new TSObservableCollection<Listing>(Dispatcher);
            ListingsView = CollectionViewFactory.CreateLiveCollectionView(Listings, sortFilters: new[]
            {
                new SortDescription(nameof(Listing.IsTwitch), ListSortDirection.Ascending),
                new SortDescription(nameof(Listing.IsTrade), ListSortDirection.Ascending),
                new SortDescription(nameof(Listing.IsMyLfg), ListSortDirection.Descending),
                new SortDescription(LastSortDescr, ListSortDirection.Ascending),
            });

            Listings.CollectionChanged += ListingsOnCollectionChanged;
            settings.HideTradeListingsChangedEvent += OnHideTradeChanged;

            KeyboardHook.Instance.RegisterCallback(App.Settings.LfgHotkey, OnShowLfgHotkeyPressed);

            _requestTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Background, RequestNextLfg, Dispatcher);
            AutoPublicizeTimer = new DispatcherTimer(TimeSpan.FromSeconds(AutoPublicizeCooldown), DispatcherPriority.Background, OnAutoPublicizeTimerTick, Dispatcher) { IsEnabled = false };

            SortCommand = new SortCommand(ListingsView);
            PublicizeCommand = new RelayCommand(_ => Publicize(), _ => CanPublicize());
            ToggleAutoPublicizeCommand = new RelayCommand(_ => ToggleAutoPublicize(), _ => CanToggleAutoPublicize());
            ReloadCommand = new RelayCommand(_ => StubInterface.Instance.StubClient.RequestListings());
            RemoveMessageCommand = new RelayCommand(_ => RemoveMessage());
            CreateMessageCommand = new RelayCommand(_ => CreateMessage(), _ => CanCreateMessage());
            ExpandAllCommand = new RelayCommand(_ => ExpandAll());
            CollapseAllCommand = new RelayCommand(_ => CollapseAll());
            OpenPopupCommand = new RelayCommand(_ => IsPopupOpen = true);
            OpenSettingsCommand = new RelayCommand(_ => WindowManager.SettingsWindow.ShowDialogAtPage(11));
            ToggleShowTradeListingsCommand = new RelayCommand(_ => HideTradeListings = !HideTradeListings);
        }

        private void CollapseAll()
        {
            Listings.ToSyncList().ForEach(l => l.ExpandCollapseCommand.Execute(false));
        }

        private void ExpandAll()
        {
            Listings.ToSyncList().ForEach(l => l.ExpandCollapseCommand.Execute(true));
        }

        private bool CanCreateMessage()
        {
            return Listings.All(l => !l.Temp && !AmIinLfg);
        }

        private void CreateMessage()
        {
            var listing = new Listing(new ListingData
            {
                LeaderId = Game.Group.InGroup ? Game.Group.Leader.PlayerId : Game.Me.PlayerId,
                LeaderName = Game.Group.InGroup ? Game.Group.Leader.Name : Game.Me.Name,
                Message = "Looking for group members!",
                PlayerCount = Game.Group.InGroup ? Game.Group.Size : 1,
                Temp = true
            });
            Listings.Insert(0, listing);
            //RefreshSorting();
            TempLfgCreated?.Invoke();
        }

        private void RemoveMessage()
        {
            ForceStopPublicize();
            StubInterface.Instance.StubClient.RemoveListing();
            StubInterface.Instance.StubClient.RequestListings();
        }

        private void OnShowLfgHotkeyPressed()
        {
            if (!Game.Logged) return;
            if (!StubInterface.Instance.IsStubAvailable) return;
            if (!WindowManager.LfgListWindow.IsVisible)
                WindowManager.LfgListWindow.ShowWindow();
            else WindowManager.LfgListWindow.HideWindow();
        }

        private void OnAutoPublicizeTimerTick(object? sender, EventArgs e)
        {
            if (Game.IsInDungeon || !AmIinLfg) _stopAuto = true;

            if (_stopAuto)
            {
                AutoPublicizeTimer.Stop();
                N(nameof(IsAutoPublicizeRunning)); //notify UI that CanPublicize changed
                //N(nameof(IsPublicizeEnabled)); //notify UI that CanPublicize changed
                _stopAuto = false;
            }
            else
            {
                if (!StubInterface.Instance.IsStubAvailable) return;
                StubInterface.Instance.StubClient.PublicizeListing();
                Publicized?.Invoke(AutoPublicizeCooldown);
            }
        }

        private void Publicize()
        {
            if (Game.IsInDungeon) return;
            //PublicizeTimer.Start();
            //N(nameof(IsPublicizeEnabled)); //notify UI that CanPublicize changed
            if (!StubInterface.Instance.IsStubAvailable) return;
            StubInterface.Instance.StubClient.PublicizeListing();
            Publicized?.Invoke(PublicizeCooldown);
        }

        private bool CanPublicize()
        {
            return /*IsPublicizeEnabled &&*/ !IsAutoPublicizeRunning;
        }

        private void ToggleAutoPublicize()
        {
            if (IsAutoPublicizeRunning)
            {
                if (IsAutoPublicizeEnabled)
                {
                    IsAutoPublicizeEnabled = false;
                    _stopAuto = true;
                }
                else
                {
                    _stopAuto = false;
                    IsAutoPublicizeEnabled = true;

                }
            }
            else
            {
                if (!AmIinLfg)
                {
                    _stopAuto = true;
                    return;
                }

                IsAutoPublicizeEnabled = true;

                AutoPublicizeTimer.Start();
                N(nameof(IsAutoPublicizeRunning)); //notify UI that CanPublicize changed
                //N(nameof(IsPublicizeEnabled)); //notify UI that CanPublicize changed
                if (!StubInterface.Instance.IsStubAvailable) return;
                StubInterface.Instance.StubClient.PublicizeListing();
                Publicized?.Invoke(AutoPublicizeCooldown);
            }
        }

        private bool CanToggleAutoPublicize()
        {
            if (!IsAutoPublicizeEnabled)
            {

                return StubInterface.Instance.IsStubAvailable &&
                       !Game.LoadingScreen &&
                       Game.Logged &&
                       !Game.IsInDungeon;
            }
            else
            {
                return StubInterface.Instance.IsStubAvailable;

            }
        }

        private void RequestNextLfg(object? sender, EventArgs e)
        {
            if (!App.Settings.LfgWindowSettings.Enabled) return;
            if (_requestQueue.Count == 0) return;

            var req = _requestQueue.Dequeue();
            if (req == 0)
            {
                StayClosed = true;
                StubInterface.Instance.StubClient.RequestListings(); //ProxyOld.RequestLfgList();
            }
            else
            {
                StubInterface.Instance.StubClient.RequestPartyInfo(req); //ProxyOld.RequestPartyInfo(req);
            }
        }

        public void EnqueueRequest(uint id)
        {
            if ((Game.IsInDungeon || Game.CivilUnrestZone) && Game.Combat) return;
            Dispatcher.InvokeAsyncIfRequired(() =>
            {
                if (_requestQueue.Count > 0 && _requestQueue.Last() == id) return;
                _requestQueue.Enqueue(id);
            }, DispatcherPriority.Background);
        }

        private void ListingsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Task.Delay(500).ContinueWith(_ => Dispatcher.Invoke(NotifyMyLfg));
        }

        internal void RemoveDeadLfg()
        {
            if (LastClicked == null) return;
            Listings.Remove(LastClicked);
        }

        public void EnqueueListRequest()
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (_requestQueue.Count > 0 && _requestQueue.Last() == 0) return;
                _requestQueue.Enqueue(0);
            });
        }

        public void ForceStopPublicize()
        {
            //PublicizeTimer.Stop();
            AutoPublicizeTimer.Stop();
            //N(nameof(IsPublicizeEnabled)); //notify UI that CanPublicize changed
            N(nameof(IsAutoPublicizeRunning)); //notify UI that CanPublicize changed
        }

        private void SyncListings(List<ListingData> listings)
        {
            Task.Factory.StartNew(() =>
            {
                RemoveMissingListings();
                listings.ForEach(l => Dispatcher.InvokeAsync(() => AddOrRefreshListing(l)));
            });


            void RemoveMissingListings()
            {
                var toRemove = new List<uint>();

                Listings.ToSyncList().ForEach(l =>
                {
                    if (listings.All(f => f.LeaderId != l.LeaderId) && !l.Temp) toRemove.Add(l.LeaderId);
                });
                toRemove.ForEach(r =>
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        var target = Listings.FirstOrDefault(rm => rm.LeaderId == r);
                        if (target != null) Listings.Remove(target);
                    });
                });
            }
        }

        public void AddOrRefreshListing(Listing l)
        {
            if (Listings.ToSyncList().Any(toFind => toFind.LeaderId == l.LeaderId))
            {
                var target = Listings.ToSyncList().FirstOrDefault(t => t.LeaderId == l.LeaderId);
                if (target == null) return;
                target.LeaderId = l.LeaderId;
                target.Message = l.Message;
                target.IsRaid = l.IsRaid;
                target.LeaderName = l.LeaderName;
                target.ExN(nameof(Listing.AliveSinceMs));
                if (target.PlayerCount != l.PlayerCount) EnqueueRequest(l.LeaderId);
            }
            else
            {
                if (l.IsTrade && ((LfgWindowSettings)Settings!).HideTradeListings) return;
                Listings.Add(l);
                EnqueueRequest(l.LeaderId);
            }
        }
        private void AddOrRefreshListing(ListingData l)
        {
            AddOrRefreshListing(new Listing(l));
        }

        private void OnHideTradeChanged()
        {
            if (!((LfgWindowSettings)Settings!).HideTradeListings) return;
            var toRemove = Listings.ToSyncList().Where(l => l.IsTrade).Select(s => s.LeaderId).ToList();
            toRemove.ForEach(r =>
            {
                var target = Listings.FirstOrDefault(rm => rm.LeaderId == r);
                if (target != null) Listings.Remove(target);
            });
        }

        private void NotifyMyLfg()
        {
            N(nameof(AmIinLfg));
            MyLfgStateChanged?.Invoke();
            N(nameof(MyLfg));
            foreach (var listing in Listings.ToSyncList()) listing?.UpdateIsMyLfg();
            MyLfg?.UpdateIsMyLfg();
            N(nameof(AmILeader));
        }

        protected override void InstallHooks()
        {
            PacketAnalyzer.Processor.Hook<S_LOGIN>(OnLogin);
            PacketAnalyzer.Processor.Hook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.Processor.Hook<S_SHOW_PARTY_MATCH_INFO>(OnShowPartyMatchInfo);
            PacketAnalyzer.Processor.Hook<S_OTHER_USER_APPLY_PARTY>(OnOtherUserApplyParty);
            PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_LIST>(OnPartyMemberList);
            PacketAnalyzer.Processor.Hook<S_LEAVE_PARTY>(OnLeaveParty);
            PacketAnalyzer.Processor.Hook<S_BAN_PARTY>(OnBanParty);
            PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_INFO>(OnPartyMemberInfo);
            PacketAnalyzer.Processor.Hook<S_SHOW_CANDIDATE_LIST>(OnShowCandidateList);
            PacketAnalyzer.Processor.Hook<S_CHANGE_PARTY_MANAGER>(OnChangePartyManager);
        }
        protected override void RemoveHooks()
        {
            PacketAnalyzer.Processor.Unhook<S_LOGIN>(OnLogin);
            PacketAnalyzer.Processor.Unhook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.Processor.Unhook<S_SHOW_PARTY_MATCH_INFO>(OnShowPartyMatchInfo);
            PacketAnalyzer.Processor.Unhook<S_OTHER_USER_APPLY_PARTY>(OnOtherUserApplyParty);
            PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_LIST>(OnPartyMemberList);
            PacketAnalyzer.Processor.Unhook<S_LEAVE_PARTY>(OnLeaveParty);
            PacketAnalyzer.Processor.Unhook<S_BAN_PARTY>(OnBanParty);
            PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_INFO>(OnPartyMemberInfo);
            PacketAnalyzer.Processor.Unhook<S_SHOW_CANDIDATE_LIST>(OnShowCandidateList);
            PacketAnalyzer.Processor.Unhook<S_CHANGE_PARTY_MANAGER>(OnChangePartyManager);
        }

        #region Hooks
        private void OnLogin(S_LOGIN m)
        {
            //Listings.Clear();
            EnqueueListRequest(); // need invoke?
        }

        private void OnShowCandidateList(S_SHOW_CANDIDATE_LIST p)
        {
            if (MyLfg == null) return;

            var dest = MyLfg.Applicants;
            foreach (var applicant in p.Candidates)
                if (dest.All(x => x.PlayerId != applicant.PlayerId))
                    dest.Add(new User(applicant));

            var toRemove = new List<User>();
            foreach (var user in dest)
                if (p.Candidates.All(x => x.PlayerId != user.PlayerId))
                    toRemove.Add(user);

            toRemove.ForEach(r => dest.Remove(r));
        }

        private void OnPartyMemberInfo(S_PARTY_MEMBER_INFO m)
        {
            //if (!App.Settings.LfgWindowSettings.Enabled) return;
            try
            {
                var lfg = Listings.ToSyncList().FirstOrDefault(listing => listing.LeaderId == m.Id
                                                                          || m.Members.Any(member =>
                                                                              member.PlayerId == listing.LeaderId));
                if (lfg == null) return;
                Task.Factory.StartNew(() =>
                {
                    m.Members.ForEach(member =>
                    {
                        if (lfg.Players.ToSyncList().Any(toFind => toFind.PlayerId == member.PlayerId))
                        {
                            var target = lfg.Players.ToSyncList()
                                .FirstOrDefault(player => player.PlayerId == member.PlayerId);
                            if (target == null) return;
                            target.IsLeader = member.IsLeader;
                            target.Online = member.Online;
                            target.Location = Game.DB!.GetSectionName(member.GuardId, member.SectionId);
                            if (!member.IsLeader) return;
                            lfg.LeaderId = member.PlayerId;
                            lfg.LeaderName = member.Name;
                        }
                        else
                        {
                            Dispatcher.InvokeAsync(() =>
                            {
                                lfg.Players.Add(new User(member));
                                if (!member.IsLeader) return;
                                lfg.LeaderId = member.PlayerId;
                                lfg.LeaderName = member.Name;
                            });
                        }
                    });

                    var toDelete = new List<uint>();
                    lfg.Players.ToSyncList().ForEach(player =>
                    {
                        if (m.Members.All(newMember => newMember.PlayerId != player.PlayerId))
                            toDelete.Add(player.PlayerId);
                    });
                    toDelete.ForEach(targetId =>
                    {
                        var target = lfg.Players.ToSyncList()
                            .FirstOrDefault(playerToRemove => playerToRemove.PlayerId == targetId);
                        if (target != null) lfg.Players.Remove(target);
                    });

                    lfg.LeaderId = m.Id;
                    var leader = lfg.Players.ToSyncList().FirstOrDefault(u => u.IsLeader);
                    if (leader != null) lfg.LeaderName = leader.Name;
                    if (LastClicked != null && LastClicked.LeaderId == lfg.LeaderId) lfg.IsExpanded = true;
                    lfg.PlayerCount = m.Members.Count;
                    NotifyMyLfg();
                });
            }
            catch (Exception e)
            {
                Log.CW(e.ToString());
            }
        }

        private void OnLeaveParty(S_LEAVE_PARTY m)
        {
            NotifyMyLfg();
        }

        private void OnBanParty(S_BAN_PARTY m)
        {
            NotifyMyLfg();
        }

        private void OnPartyMemberList(S_PARTY_MEMBER_LIST m)
        {
            if (_lastGroupSize == 0) NotifyMyLfg();
            _lastGroupSize = m.Members.Count;
            if (!StubInterface.Instance.IsStubAvailable || !App.Settings.LfgWindowSettings.Enabled ||
                !Game.InGameUiOn) return;
            StubInterface.Instance.StubClient.RequestListingCandidates();
            if (!WindowManager.LfgListWindow.IsVisible) return;
            StubInterface.Instance.StubClient.RequestListings();
        }

        private void OnOtherUserApplyParty(S_OTHER_USER_APPLY_PARTY m)
        {
            //if (!App.Settings.LfgWindowSettings.Enabled) return;
            if (MyLfg == null) return;
            var dest = MyLfg.Applicants;
            if (dest.Any(u => u.PlayerId == m.PlayerId)) return;
            dest.Add(new User(Dispatcher)
            {
                PlayerId = m.PlayerId,
                UserClass = m.Class,
                Level = Convert.ToUInt32(m.Level),
                Name = m.Name,
                Online = true,
                ServerId = Game.Me.ServerId
            });
        }

        private void OnShowPartyMatchInfo(S_SHOW_PARTY_MATCH_INFO m)
        {
            if (!m.IsLast && StubInterface.Instance.IsStubAvailable && m.Page <= m.Pages)
                StubInterface.Instance.StubClient.RequestListingsPage(m.Page + 1);

            if (!m.IsLast)
            {
                Log.CW($"S_SHOW_PARTY_MATCH_INFO is not last {m.Page + 1}/{m.Pages + 1}");
                return;
            }

            Log.CW($"S_SHOW_PARTY_MATCH_INFO is last {m.Page + 1}/{m.Pages + 1}");

            if (S_SHOW_PARTY_MATCH_INFO.Listings.Count != 0)
                SyncListings(S_SHOW_PARTY_MATCH_INFO.Listings);
            else Log.CW("No listings to show.");

            NotifyMyLfg();
            //Dispatcher?.InvokeAsync(RefreshSorting, DispatcherPriority.Background);
        }

        private void OnChangePartyManager(S_CHANGE_PARTY_MANAGER obj)
        {
            N(nameof(AmILeader));
        }

        private void OnReturnToLobby(S_RETURN_TO_LOBBY m)
        {
            ForceStopPublicize();
        }

        #endregion    
    }

    public class SortCommand : ICommand
    {
        private readonly ICollectionViewLiveShaping _view;
        private bool _refreshing;
        private ListSortDirection _direction = ListSortDirection.Ascending;
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            parameter ??= "Message";
            var f = (string)parameter;
            if (!_refreshing)
                _direction = _direction == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            ((CollectionView)_view).SortDescriptions.Clear();
            ((CollectionView)_view).SortDescriptions.Add(new SortDescription(nameof(Listing.IsTwitch), ListSortDirection.Ascending));
            ((CollectionView)_view).SortDescriptions.Add(new SortDescription(nameof(Listing.IsTrade), ListSortDirection.Ascending));
            ((CollectionView)_view).SortDescriptions.Add(new SortDescription(nameof(Listing.IsMyLfg), ListSortDirection.Descending));
            ((CollectionView)_view).SortDescriptions.Add(new SortDescription(f, _direction));
            WindowManager.ViewModels.LfgVM.LastSortDescr = parameter!.ToString()!;
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