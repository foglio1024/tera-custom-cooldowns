using System;
using System.Windows.Input;
using System.Windows.Threading;
using Nostrum.WPF;
using Nostrum.WPF.ThreadSafe;
using TCC.Data;
using TCC.Utils;
using TCC.ViewModels.Widgets;
using FocusManager = TCC.UI.FocusManager;

namespace TCC.ViewModels;

public class LootItemViewModel : ThreadSafeObservableObject
{
    private readonly DispatcherTimer _commitCheckTimer;
    private BidAction _bidIntent;
    private DistributionStatus _distributionStatus;
    private bool _bidSent;
    private string _winnerName = "";
    private int _winnerRoll = -1;

    public DropItem Item { get; }
    public Item DbItem { get; }

    public DistributionStatus DistributionStatus
    {
        get => _distributionStatus;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _distributionStatus)) return;

            if (_distributionStatus != DistributionStatus.Distributing
                && _commitCheckTimer.IsEnabled)
            {
                _commitCheckTimer.Stop();
            }

            Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
        }
    }

    public string WinnerName
    {
        get => _winnerName;
        set => RaiseAndSetIfChanged(value, ref _winnerName);
    }

    public BidAction BidIntent
    {
        get => _bidIntent;
        set => RaiseAndSetIfChanged(value, ref _bidIntent);
    }

    public bool BidSent
    {
        get => _bidSent;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _bidSent)) return;

            if (_bidSent)
            {
                _commitCheckTimer.Stop();
            }
            Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
        }
    }

    public int Index { get; set; } = -1;

    public int WinnerRoll
    {
        get => _winnerRoll;
        set => RaiseAndSetIfChanged(value, ref _winnerRoll);
    }

    public ICommand SetBidIntentCommand { get; }


    public LootItemViewModel(DropItem item)
    {
        Item = item;

        Game.DB!.ItemsDatabase.TryGetItem(item.ItemId, out var dbItem);
        DbItem = dbItem;

        SetBidIntentCommand = new RelayCommand<BidAction>(SetBidIntent,
            _ => DistributionStatus is not (DistributionStatus.Distributed or DistributionStatus.Discarded) && !BidSent);

        _commitCheckTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _commitCheckTimer.Tick += OnCommitCheck;
    }

    private void OnCommitCheck(object? sender, EventArgs e)
    {
        _commitCheckTimer.Stop();

        if (BidSent) return;
        Log.CW("Intent commit failed, retrying...");
        CommitIntent();
    }

    private void SetBidIntent(BidAction intent)
    {
        BidIntent = BidIntent == intent ? BidAction.Unset : intent;
        CommitIntent();
    }

    public void CommitIntent()
    {
        // TERA doesn't accept keystrokes while in loading screen
        // so abort commit and subscribe to LoadingScreenChanged event
        // and try again when the event is triggered
        if (!Game.LoadingScreen)
        {
            Game.LoadingScreenChanged -= CommitIntent;
        }
        else
        {
            Game.LoadingScreenChanged += CommitIntent;
            return;
        }

        // if chat is open, keystrokes are intercepted by chat input box
        if (!Game.InGameChatOpen)
        {
            Game.ChatModeChanged -= CommitIntent;
        }
        else
        {
            Log.N("Auto-loot", "Cannot send roll/pass action while chat input is open. Trying again when chat input will be closed.", NotificationType.Warning, 5000);
            Game.ChatModeChanged += CommitIntent;
            return;
        }

        if (DistributionStatus is not DistributionStatus.Distributing) return;

        switch (BidIntent)
        {
            case BidAction.Unset:
                return;

            case BidAction.Pass:
                FocusManager.SendPgDown(500);
                break;

            case BidAction.Roll:
                FocusManager.SendPgUp(500);
                break;
        }

        _commitCheckTimer.Start();
    }
}