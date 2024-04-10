using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nostrum.WPF.Factories;
using Nostrum.WPF.ThreadSafe;

namespace TCC.UI.Windows;

public partial class SystemMessagesConfigWindow
{
    public ThreadSafeObservableCollection<SystemMessageViewModel> HiddenMessages { get; }
    public ThreadSafeObservableCollection<SystemMessageViewModel> ShowedMessages { get; }
    public ICollectionViewLiveShaping ShowedMessagesView { get; }
    public ICollectionViewLiveShaping HiddenMessagesView { get; }

    public SystemMessagesConfigWindow()
    {
        InitializeComponent();
        DataContext = this;
        HiddenMessages = new ThreadSafeObservableCollection<SystemMessageViewModel>();
        ShowedMessages = new ThreadSafeObservableCollection<SystemMessageViewModel>();

        App.Settings.UserExcludedSysMsg.ForEach(opc =>
        {
            HiddenMessages.Add(new SystemMessageViewModel(opc, Game.DB!.SystemMessagesDatabase.Messages[opc]));
        });
        Game.DB!.SystemMessagesDatabase.Messages.ToList().ForEach(keyVal =>
        {
            if (App.Settings.UserExcludedSysMsg.Contains(keyVal.Key)) return;
            ShowedMessages.Add(new SystemMessageViewModel(keyVal.Key, keyVal.Value));
        });

        HiddenMessages.CollectionChanged += (_, args) =>
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in args.NewItems!)
                    {
                        var opcode = ((SystemMessageViewModel?) item)?.Opcode;
                        if (opcode != null)
                            App.Settings.UserExcludedSysMsg.Add(opcode);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in args.OldItems!)
                    {
                        var opcode = ((SystemMessageViewModel?) item)?.Opcode;
                        if (opcode != null)
                            App.Settings.UserExcludedSysMsg.Remove(opcode);
                    }
                    break;
            }
            App.Settings.Save();
        };

        ShowedMessagesView = CollectionViewFactory.CreateLiveCollectionView(ShowedMessages)
                             ?? throw new Exception("Failed to create LiveCollectionView");
        HiddenMessagesView = CollectionViewFactory.CreateLiveCollectionView(HiddenMessages)
                             ?? throw new Exception("Failed to create LiveCollectionView");
    }

    private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();

    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
    //TODO: use commands
    private void ExcludeMessage(object sender, RoutedEventArgs e)
    {
        var msgVm = (SystemMessageViewModel) ((FrameworkElement) sender).DataContext;
        if (HiddenMessages.Any(x => x.Opcode == msgVm?.Opcode)) return;
        HiddenMessages.Add(msgVm);
        ShowedMessages.Remove(msgVm);
    }

    private void RestoreMessage(object sender, RoutedEventArgs e)
    {
        var msgVm = (SystemMessageViewModel) ((FrameworkElement) sender).DataContext;
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