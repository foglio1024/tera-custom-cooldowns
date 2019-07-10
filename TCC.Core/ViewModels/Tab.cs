using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json;
using TCC.Controls;
using TCC.Data;
using TCC.Data.Chat;
using TCC.ViewModels.Widgets;

namespace TCC.ViewModels
{
    public class Tab : TSPropertyChanged
    {
        // needed for combobox in settings
        [JsonIgnore]
        public List<ChatChannelOnOff> AllChannels => TccUtils.GetEnabledChannelsList();

        private ICollectionView _messages;
        private ChatMessage _pinnedMessage;
        [JsonIgnore]
        public ICommand ScrollToMessageCommand { get; }
        [JsonIgnore]
        public ICommand RemoveImportantMessageCommand { get; }
        [JsonIgnore]
        public ICommand ClearAllCommand { get; }
        private string _tabName;
        private SynchronizedObservableCollection<string> _authors;
        private SynchronizedObservableCollection<string> _excludedAuthors;
        private SynchronizedObservableCollection<ChatChannel> _channels;
        private SynchronizedObservableCollection<ChatChannel> _excludedChannels;

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

        [JsonIgnore]
        public string ImportantMessagesLabel => ImportantMessages.Count > 9 ? "!" : ImportantMessages.Count.ToString();

        [JsonIgnore]
        public bool Attention => ImportantMessages.Count > 0;

        public SynchronizedObservableCollection<string> Authors
        {
            get => _authors;
            set
            {
                _authors = value;
                ApplyFilter();
            }
        }

        public SynchronizedObservableCollection<string> ExcludedAuthors
        {
            get => _excludedAuthors;
            set
            {
                _excludedAuthors = value;
                ApplyFilter();
            }
        }

        public SynchronizedObservableCollection<ChatChannel> Channels
        {
            get => _channels;
            set
            {
                _channels = value;
                ApplyFilter();
            }
        }

        public SynchronizedObservableCollection<ChatChannel> ExcludedChannels
        {
            get => _excludedChannels;
            set
            {
                _excludedChannels = value;
                ApplyFilter();
            }
        }

        [JsonIgnore]
        public SynchronizedObservableCollection<ChatMessage> ImportantMessages { get; set; }
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
            Messages = new ListCollectionView(ChatWindowManager.Instance.ChatMessages);
            Authors = new SynchronizedObservableCollection<string>(Dispatcher);
            ExcludedAuthors = new SynchronizedObservableCollection<string>(Dispatcher);
            Channels = new SynchronizedObservableCollection<ChatChannel>(Dispatcher);
            ExcludedChannels = new SynchronizedObservableCollection<ChatChannel>(Dispatcher);
            ImportantMessages = new SynchronizedObservableCollection<ChatMessage>(Dispatcher);
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
            ScrollToMessageCommand = new RelayCommand(msg => { ChatWindowManager.Instance.ScrollToMessage(this, (ChatMessage)msg); });
            TabViewModel.ImportantRemoved += SyncImportant;

        }
        public Tab(string n, ChatChannel[] ch, ChatChannel[] ex, string[] a, string[] exa) : this()
        {
            if (n == null || ch == null || ex == null || a == null || exa == null) return;
            TabName = n;
            foreach (var auth in a)
            {
                Authors.Add(auth);
            }
            foreach (var auth in exa)
            {
                ExcludedAuthors.Add(auth);
            }
            foreach (var chan in ch)
            {
                Channels.Add(chan);
            }
            foreach (var chan in ex)
            {
                ExcludedChannels.Add(chan);
            }

            ApplyFilter();
        }

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
            if (Authors == null || Channels == null || ExcludedAuthors == null || ExcludedChannels == null)
                return true;
            return (Authors.Count == 0 || Authors.Any(x => x == m.Author)) &&
                   (Channels.Count == 0 || Channels.Any(x => x == m.Channel)) &&
                   (ExcludedChannels.Count == 0 || ExcludedChannels.All(x => x != m.Channel)) &&
                   (ExcludedAuthors.Count == 0 || ExcludedAuthors.All(x => x != m.Author));

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
            Messages.Filter = f =>
            {
                var m = f as ChatMessage;
                return Filter(m);
            };
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
