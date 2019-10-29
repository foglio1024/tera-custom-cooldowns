using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows.Threading;
using FoglioUtils;
using TCC.Data.Pc;
using TCC.Utils;

namespace TCC.Data.Chat
{
    public class LfgMessage : ChatMessage
    {
        private int _tries = 10;
        private readonly TSObservableCollection<User> _members;
        private Timer _timer;
        private ICollectionView _membersView;

        private Listing _linkedListing;
        public ICollectionView MembersView
        {
            get => _membersView;
            private set
            {
                if (_membersView == value) return;
                _membersView = value;
                N();
            }
        }
        public Listing LinkedListing
        {
            get => _linkedListing;
            set
            {
                if (_linkedListing == value) return;
                if (value == null)
                {
                    _members.Clear();
                    if (_linkedListing != null) _linkedListing.Players.CollectionChanged -= OnMembersChanged;
                }
                else
                {
                    value.Players.CollectionChanged += OnMembersChanged;
                    foreach (var p in value.Players.ToSyncList())
                    {
                        _members.Add(p);
                    }
                }
                _linkedListing = value;
                N();
            }
        }
        public uint AuthorId { get; }
        public bool ShowMembers => LinkedListing != null && LinkedListing.Players.Count <= 7;

        public LfgMessage(uint authorId, string author, string msg) : base(ChatChannel.LFG, author, msg)
        {
            AuthorId = authorId;
            _members = new TSObservableCollection<User>();
            MembersView = CollectionViewUtils.InitView(_members, null, new List<SortDescription>());
        }

        private void OnMembersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems.Cast<User>())
                    {
                        _members.Add(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.Cast<User>())
                    {
                        _members.Remove(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                {
                    _members.Clear();
                }
                    break;
            }
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_tries == 0)
            {
                Log.CW($"Unable to find linked listing for [{Author}]{RawMessage}, stopping tries.");
                _timer.Stop();
                return;
            }
            LinkedListing = FindListing();
            if (LinkedListing == null)
            {
                Log.CW($"Linked listing ({Author}/{AuthorId}) is still null! ({_tries})");
                _tries--;
                return;
            }
            _timer.Stop();
            WindowManager.ViewModels.LfgVM.EnqueueRequest(LinkedListing.LeaderId);

        }
        private Listing FindListing()
        {
            return WindowManager.ViewModels.LfgVM.Listings.ToSyncList().FirstOrDefault(x =>
                x.Players.ToSyncList().Any(p => p.Name == Author) ||
                x.LeaderName == Author ||
                x.Message == RawMessage);
        }
        public void LinkListing()
        {
            Dispatcher.InvokeAsync(() =>
            {
                LinkedListing = FindListing();
                if (LinkedListing != null) return;
                //Log.CW($"Linked listing ({Author}/{AuthorId}) is null! Requesting list.");
                WindowManager.ViewModels.LfgVM.EnqueueListRequest();
                _timer = new Timer(1500);
                _timer.Elapsed += OnTimerTick;
                _timer.Start();
            });
        }
    }
}