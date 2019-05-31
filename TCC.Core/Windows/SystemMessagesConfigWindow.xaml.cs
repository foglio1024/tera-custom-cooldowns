using FoglioUtils;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.Settings;

namespace TCC.Windows
{
    public partial class SystemMessagesConfigWindow
    {
        public SystemMessagesConfigWindow()
        {
            InitializeComponent();
            DataContext = this;
            _hiddenMessages = new SynchronizedObservableCollection<SystemMessageViewModel>();
            _showedMessages = new SynchronizedObservableCollection<SystemMessageViewModel>();

            SettingsHolder.UserExcludedSysMsg.ForEach(opc =>
            {
                _hiddenMessages.Add(new SystemMessageViewModel(opc, SessionManager.DB.SystemMessagesDatabase.Messages[opc]));
            });
            SessionManager.DB.SystemMessagesDatabase.Messages.ToList().ForEach(keyVal =>
            {
                if (SettingsHolder.UserExcludedSysMsg.Contains(keyVal.Key)) return;
                _showedMessages.Add(new SystemMessageViewModel(keyVal.Key, keyVal.Value));
            });

            _hiddenMessages.CollectionChanged += (_, args) =>
            {
                switch (args.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        foreach (var item in args.NewItems)
                        {
                            SettingsHolder.UserExcludedSysMsg.Add((item as SystemMessageViewModel)?.Opcode);
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        foreach (var item in args.OldItems)
                        {
                            SettingsHolder.UserExcludedSysMsg.Remove((item as SystemMessageViewModel)?.Opcode);
                        }
                        break;
                }
                SettingsWriter.Save();
            };

            ShowedMessagesView = CollectionViewUtils.InitLiveView(null, ShowedMessages, new string[] { }, new SortDescription[] { });
            HiddenMessagesView = CollectionViewUtils.InitLiveView(null, HiddenMessages, new string[] { }, new SortDescription[] { });
            ((ICollectionView)ShowedMessagesView).CollectionChanged += GcPls;
            ((ICollectionView)HiddenMessagesView).CollectionChanged += GcPls;
        }

        private void GcPls(object sender, EventArgs ev) { }

        SynchronizedObservableCollection<SystemMessageViewModel> _hiddenMessages;
        SynchronizedObservableCollection<SystemMessageViewModel> _showedMessages;
        public SynchronizedObservableCollection<SystemMessageViewModel> HiddenMessages
        {
            get
            {
                return _hiddenMessages;
            }
        }
        public SynchronizedObservableCollection<SystemMessageViewModel> ShowedMessages
        {
            get
            {
                return _showedMessages;
            }
        }

        public ICollectionViewLiveShaping ShowedMessagesView { get; }
        public ICollectionViewLiveShaping HiddenMessagesView { get; }

        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

        }
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExcludeMessage(object sender, RoutedEventArgs e)
        {
            var msgVm = (sender as FrameworkElement)?.DataContext as SystemMessageViewModel;
            if (HiddenMessages.Any(x => x.Opcode == msgVm?.Opcode)) return;
            HiddenMessages.Add(msgVm);
            ShowedMessages.Remove(msgVm);
        }

        private void RestoreMessage(object sender, RoutedEventArgs e)
        {
            var msgVm = (sender as FrameworkElement)?.DataContext as SystemMessageViewModel;
            if (HiddenMessages.All(x => x.Opcode != msgVm?.Opcode)) return;
            ShowedMessages.Add(msgVm);
            HiddenMessages.Remove(msgVm);
        }

        private void FilterShowedMessages(object sender, TextChangedEventArgs e)
        {
            var view = (ICollectionView)ShowedMessagesView;
            view.Filter = o =>
            {
                var msg = ((SystemMessageViewModel)o).SysMsg.Message;
                return msg.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
            };
            view.Refresh();

        }
        private void FilterHiddenMessages(object sender, TextChangedEventArgs e)
        {
            var view = (ICollectionView)HiddenMessagesView;
            view.Filter = o =>
            {
                var msg = ((SystemMessageViewModel)o).SysMsg.Message;
                return msg.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
            };
            view.Refresh();
        }
    }
}
