using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Nostrum;
using Newtonsoft.Json;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Utilities;
using TCC.ViewModels.Widgets;

namespace TCC.ViewModels
{
    public class TabInfo
    {
        public string Name { get; set; }
        public List<string> ShowedAuthors { get; set; }
        public List<string> HiddenAuthors { get; set; }
        public List<string> ShowedKeywords { get; set; }
        public List<string> HiddenKeywords { get; set; }
        public List<ChatChannel> ShowedChannels { get; set; }
        public List<ChatChannel> HiddenChannels { get; set; }

        public TabInfo()
        {
            ShowedAuthors = new List<string>();
            HiddenAuthors = new List<string>();
            ShowedKeywords = new List<string>();
            HiddenKeywords = new List<string>();
            ShowedChannels = new List<ChatChannel>();
            HiddenChannels = new List<ChatChannel>();
            
        }
        public TabInfo(string name) : this()
        {
            Name = name;
        }
    }
    public class TabInfoVM : TSPropertyChanged
    {
        private string _tabName;
        public string TabName
        {
            get => _tabName;
            set
            {
                if (_tabName == value) return;
                _tabName = value;
                N();
            }
        }

        public TSObservableCollection<string> Authors { get; set; }

        public TSObservableCollection<string> ExcludedAuthors { get; set; }

        public TSObservableCollection<ChatChannel> ShowedChannels { get; set; }

        public TSObservableCollection<ChatChannel> ExcludedChannels { get; set; }

        public TSObservableCollection<string> Keywords { get; set; }

        public TSObservableCollection<string> ExcludedKeywords { get; set; }

        public TabInfoVM()
        {
            Authors = new TSObservableCollection<string>(Dispatcher);
            ExcludedAuthors = new TSObservableCollection<string>(Dispatcher);
            Keywords = new TSObservableCollection<string>(Dispatcher);
            ExcludedKeywords = new TSObservableCollection<string>(Dispatcher);
            ShowedChannels = new TSObservableCollection<ChatChannel>(Dispatcher);
            ExcludedChannels = new TSObservableCollection<ChatChannel>(Dispatcher);
        }

        public TabInfoVM(string tabName) : this()
        {
            TabName = tabName;
        }
        public TabInfoVM(TabInfo info) : this()
        {
            TabName = info.Name;
            info.ShowedAuthors.ForEach(Authors.Add);
            info.HiddenAuthors.ForEach(ExcludedAuthors.Add);
            info.ShowedKeywords.ForEach(Keywords.Add);
            info.HiddenKeywords.ForEach(ExcludedKeywords.Add);
            info.ShowedChannels.ForEach(ShowedChannels.Add);
            info.HiddenChannels.ForEach(ExcludedChannels.Add);

            Authors.CollectionChanged += (s, ev) =>
            {
                switch (ev.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        info.ShowedAuthors.AddRange(ev.NewItems.Cast<string>());
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        ev.OldItems.Cast<string>().ToList().ForEach(i => info.ShowedAuthors.Remove(i));
                        break;
                }
            };
            ExcludedAuthors.CollectionChanged += (s, ev) =>
            {
                switch (ev.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        info.HiddenAuthors.AddRange(ev.NewItems.Cast<string>());
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        ev.OldItems.Cast<string>().ToList().ForEach(i => info.HiddenAuthors.Remove(i));
                        break;
                }
            };
            Keywords.CollectionChanged += (s, ev) =>
            {
                switch (ev.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        info.ShowedKeywords.AddRange(ev.NewItems.Cast<string>());
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        ev.OldItems.Cast<string>().ToList().ForEach(i => info.ShowedKeywords.Remove(i));
                        break;
                }
            };
            ExcludedKeywords.CollectionChanged += (s, ev) =>
            {
                switch (ev.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        info.HiddenKeywords.AddRange(ev.NewItems.Cast<string>());
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        ev.OldItems.Cast<string>().ToList().ForEach(i => info.HiddenKeywords.Remove(i));
                        break;
                }
            };
            ShowedChannels.CollectionChanged += (s, ev) =>
            {
                switch (ev.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        info.ShowedChannels.AddRange(ev.NewItems.Cast<ChatChannel>());
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        ev.OldItems.Cast<ChatChannel>().ToList().ForEach(i => info.ShowedChannels.Remove(i));
                        break;
                }
            };
            ExcludedChannels.CollectionChanged += (s, ev) =>
            {
                switch (ev.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        info.HiddenChannels.AddRange(ev.NewItems.Cast<ChatChannel>());
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        ev.OldItems.Cast<ChatChannel>().ToList().ForEach(i => info.HiddenChannels.Remove(i));
                        break;
                }
            };
        }
    }
    public class Tab : TSPropertyChanged
    {
        public TabInfo TabInfo { get; }
        public TabInfoVM TabInfoVM { get; set; }

        private ICollectionView _messages;
        private ChatMessage _pinnedMessage;


        public string TabName
        {
            get => TabInfo?.Name;
            set
            {
                if (TabInfo?.Name == value) return;
                if (TabInfo != null) TabInfo.Name = value;
                if (TabInfoVM != null) TabInfoVM.TabName = value;
                N();
            }
        }

        [JsonIgnore]
        public List<ChatChannelOnOff> AllChannels => TccUtils.GetEnabledChannelsList(); // needed for combobox in settings
        [JsonIgnore]
        public ICommand ScrollToMessageCommand { get; }
        [JsonIgnore]
        public ICommand RemoveImportantMessageCommand { get; }
        [JsonIgnore]
        public ICommand ClearAllCommand { get; }
        [JsonIgnore]
        public string ImportantMessagesLabel => ImportantMessages.Count > 9 ? "!" : ImportantMessages.Count.ToString();

        [JsonIgnore]
        public bool Attention => ImportantMessages.Count > 0;

        [JsonIgnore]
        public TSObservableCollection<ChatMessage> ImportantMessages { get; set; }
        [JsonIgnore]
        public ICollectionView Messages
        {
            get => _messages;
            set
            {
                if (_messages == value) return;
                _messages = value;
                N(nameof(Messages));
            }
        }
        [JsonIgnore]
        public ChatMessage PinnedMessage
        {
            get => _pinnedMessage;
            set
            {
                if (_pinnedMessage == value) return;
                _pinnedMessage = value;
                N();
            }
        }
        public void Refresh()
        {
            Messages.Refresh();
        }

        public Tab()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Messages = new ListCollectionView(ChatManager.Instance.ChatMessages);
            ImportantMessages = new TSObservableCollection<ChatMessage>(Dispatcher);
            RemoveImportantMessageCommand = new RelayCommand(msg =>
            {
                RemoveImportantMessage((ChatMessage)msg);
                TabViewModel.InvokeImportantRemoved(this, new ImportantRemovedArgs(ImportantRemovedArgs.ActionType.Remove, (ChatMessage)msg));
            });
            ClearAllCommand = new RelayCommand(par =>
            {
                ClearImportant();
                TabViewModel.InvokeImportantRemoved(this, new ImportantRemovedArgs(ImportantRemovedArgs.ActionType.Clear));
            });
            ScrollToMessageCommand = new RelayCommand(msg => { ChatManager.Instance.ScrollToMessage(this, (ChatMessage)msg); });
            TabViewModel.ImportantRemoved += SyncImportant;

        }

        public Tab(TabInfo tabInfo) : this()
        {
            TabInfo = tabInfo;
            TabInfoVM = new TabInfoVM(TabInfo);
            ApplyFilter();
        }
        //public Tab(string n, ChatChannel[] ch, ChatChannel[] ex, string[] a, string[] exa) : this()
        //{
        //    if (n == null || ch == null || ex == null || a == null || exa == null) return;
        //    TabName = n;
        //    foreach (var auth in a)
        //    {
        //        Authors.Add(auth);
        //    }
        //    foreach (var auth in exa)
        //    {
        //        ExcludedAuthors.Add(auth);
        //    }
        //    foreach (var chan in ch)
        //    {
        //        Channels.Add(chan);
        //    }
        //    foreach (var chan in ex)
        //    {
        //        ExcludedChannels.Add(chan);
        //    }

        //    ApplyFilter();
        //}

        private void SyncImportant(Tab source, ImportantRemovedArgs e)
        {
            if (source == this) return;
            switch (e.Action)
            {
                case ImportantRemovedArgs.ActionType.Remove:
                    RemoveImportantMessage(e.Item);
                    break;
                case ImportantRemovedArgs.ActionType.Clear:
                    ClearImportant();
                    break;
            }
        }

        public bool Filter(ChatMessage m)
        {
            if (TabInfoVM.Authors == null 
                || TabInfoVM.ShowedChannels == null
                || TabInfoVM.ExcludedAuthors == null 
                || TabInfoVM.ExcludedChannels == null
                || TabInfoVM.Keywords == null
                || TabInfoVM.ExcludedKeywords == null)
                return true;

            return (TabInfoVM.Authors.Count == 0 || TabInfoVM.Authors.Any(x => x == m.Author)) &&
                   (TabInfoVM.ShowedChannels.Count == 0 || TabInfoVM.ShowedChannels.Any(x => x == m.Channel)) &&
                   (TabInfoVM.ExcludedChannels.Count == 0 || TabInfoVM.ExcludedChannels.All(x => x != m.Channel)) &&
                   (TabInfoVM.ExcludedAuthors.Count == 0 || TabInfoVM.ExcludedAuthors.All(x => x != m.Author)) &&
                   (TabInfoVM.Keywords.Count == 0 || TabInfoVM.Keywords.Any(x => m.PlainMessage.ToLower().Contains(x.ToLower()))) &&
                   (TabInfoVM.ExcludedKeywords.Count == 0 || TabInfoVM.ExcludedKeywords.Any(x => !m.PlainMessage.ToLower().Contains(x.ToLower())));

        }
        public void ApplyFilter()
        {
            //if (Channels?.Count == 0 && 
            //    Authors?.Count == 0 && 
            //    ExcludedChannels?.Count == 0 && 
            //    ExcludedAuthors?.Count == 0)
            //{
            //    Messages.Filter = null;
            //}
            //else
            //{
            //}
            Dispatcher.Invoke(() =>
            {
                Messages.Filter = f =>
                {
                    var m = f as ChatMessage;
                    return Filter(m);
                };
            });
        }

        public void AddImportantMessage(ChatMessage chatMessage)
        {
            ImportantMessages.Add(chatMessage);
            N(nameof(Attention));
            N(nameof(ImportantMessagesLabel));
        }

        public void RemoveImportantMessage(ChatMessage msg)
        {
            ImportantMessages.Remove(msg);
            N(nameof(Attention));
            N(nameof(ImportantMessagesLabel));
        }
        public void ClearImportant()
        {
            ImportantMessages.Clear();
            N(nameof(Attention));
            N(nameof(ImportantMessagesLabel));
        }
    }
}
