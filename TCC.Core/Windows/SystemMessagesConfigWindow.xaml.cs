using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data.Chat;
using TCC.Data.Databases;
using TCC.Settings;

namespace TCC.Windows
{
    public class SystemMessageViewModel
    {
        public SystemMessage SysMsg { get; }
        public string Opcode { get; }

        public SystemMessageViewModel(string opc, SystemMessage msg)
        {
            Opcode = opc;
            SysMsg = msg;
        }
    }
    public partial class SystemMessagesConfigWindow : Window
    {
        public SystemMessagesConfigWindow()
        {
            InitializeComponent();
            DataContext = this;
            _hiddenMessages = new SynchronizedObservableCollection<SystemMessageViewModel>();
            _showedMessages = new SynchronizedObservableCollection<SystemMessageViewModel>();

            SettingsHolder.UserExcludedSysMsg.ForEach(opc =>
            {
                _hiddenMessages.Add(new SystemMessageViewModel(opc, SessionManager.CurrentDatabase.SystemMessagesDatabase.Messages[opc]));
            });
            SessionManager.CurrentDatabase.SystemMessagesDatabase.Messages.ToList().ForEach(keyVal =>
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
                            SettingsHolder.UserExcludedSysMsg.Add((item as SystemMessageViewModel).Opcode);
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        foreach (var item in args.OldItems)
                        {
                            SettingsHolder.UserExcludedSysMsg.Remove((item as SystemMessageViewModel).Opcode);
                        }
                        break;
                    default:
                        break;
                }
                SettingsWriter.Save();
            };

            ShowedMessagesView = Utils.InitLiveView(null, ShowedMessages, new string[] { }, new SortDescription[] { });
            HiddenMessagesView = Utils.InitLiveView(null, HiddenMessages, new string[] { }, new SortDescription[] { });
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
            var msgVm = ((sender as FrameworkElement).DataContext) as SystemMessageViewModel;
            if (!HiddenMessages.Any(x => x.Opcode == msgVm.Opcode))
            {
                HiddenMessages.Add(msgVm);
                ShowedMessages.Remove(msgVm);
            }
        }

        private void RestoreMessage(object sender, RoutedEventArgs e)
        {
            var msgVm = ((sender as FrameworkElement).DataContext) as SystemMessageViewModel;
            if (HiddenMessages.Any(x => x.Opcode == msgVm.Opcode))
            {
                ShowedMessages.Add(msgVm);
                HiddenMessages.Remove(msgVm);
            }
        }

        private void FilterShowedMessages(object sender, TextChangedEventArgs e)
        {
            var view = ((ICollectionView)ShowedMessagesView);
            view.Filter = o =>
            {
                var msg = ((SystemMessageViewModel)o).SysMsg.Message;
                return msg.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
            };
            view.Refresh();

        }
        private void FilterHiddenMessages(object sender, TextChangedEventArgs e)
        {
            var view = ((ICollectionView)HiddenMessagesView);
            view.Filter = o =>
            {
                var msg = ((SystemMessageViewModel)o).SysMsg.Message;
                return msg.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
            };
            view.Refresh();
        }
    }
}
