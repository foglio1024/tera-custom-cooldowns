using Nostrum;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TCC.Windows
{
    public partial class SystemMessagesConfigWindow
    {
        public TSObservableCollection<SystemMessageViewModel> HiddenMessages { get; }
        public TSObservableCollection<SystemMessageViewModel> ShowedMessages { get; }
        public ICollectionViewLiveShaping ShowedMessagesView { get; }
        public ICollectionViewLiveShaping HiddenMessagesView { get; }

        public SystemMessagesConfigWindow()
        {
            InitializeComponent();
            DataContext = this;
            HiddenMessages = new TSObservableCollection<SystemMessageViewModel>();
            ShowedMessages = new TSObservableCollection<SystemMessageViewModel>();

            App.Settings.UserExcludedSysMsg.ForEach(opc =>
            {
                HiddenMessages.Add(new SystemMessageViewModel(opc, Game.DB.SystemMessagesDatabase.Messages[opc]));
            });
            Game.DB.SystemMessagesDatabase.Messages.ToList().ForEach(keyVal =>
            {
                if (App.Settings.UserExcludedSysMsg.Contains(keyVal.Key)) return;
                ShowedMessages.Add(new SystemMessageViewModel(keyVal.Key, keyVal.Value));
            });

            HiddenMessages.CollectionChanged += (_, args) =>
            {
                switch (args.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        foreach (var item in args.NewItems)
                        {
                            App.Settings.UserExcludedSysMsg.Add((item as SystemMessageViewModel)?.Opcode);
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        foreach (var item in args.OldItems)
                        {
                            App.Settings.UserExcludedSysMsg.Remove((item as SystemMessageViewModel)?.Opcode);
                        }
                        break;
                }
                App.Settings.Save();
            };

            ShowedMessagesView = CollectionViewUtils.InitLiveView(ShowedMessages);
            HiddenMessagesView = CollectionViewUtils.InitLiveView(HiddenMessages);
        }

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
                var msg = ((SystemMessageViewModel)o).SysMsg.Template;
                return msg.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
            };
            view.Refresh();

        }
        private void FilterHiddenMessages(object sender, TextChangedEventArgs e)
        {
            var view = (ICollectionView)HiddenMessagesView;
            view.Filter = o =>
            {
                var msg = ((SystemMessageViewModel)o).SysMsg.Template;
                return msg.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
            };
            view.Refresh();
        }
    }
}
