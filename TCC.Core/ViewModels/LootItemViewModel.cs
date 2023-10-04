using Nostrum.WPF;
using Nostrum.WPF.ThreadSafe;
using System.Windows.Input;
using TCC.Data;
using TCC.ViewModels.Widgets;

namespace TCC.ViewModels;

public class LootItemViewModel : ThreadSafeObservableObject
{
    BidAction _bidIntent;
    DistributionStatus _distributionStatus;
    bool _bidSent;
    string _winnerName = "";
    int _winnerRoll = -1;

    public DropItem Item { get; }
    public Item DbItem { get; }

    public DistributionStatus DistributionStatus
    {
        get => _distributionStatus;
        set
        {
            if (_distributionStatus == value) return;
            _distributionStatus = value;
            N();
            Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
        }
    }

    public string WinnerName
    {
        get => _winnerName;
        set
        {
            if (_winnerName == value) return;
            _winnerName = value;
            N();
        }
    }

    public BidAction BidIntent
    {
        get => _bidIntent;
        set
        {
            if (_bidIntent == value) return;
            _bidIntent = value;
            N();

            if (DistributionStatus == DistributionStatus.Distributing
                && !_bidSent
                && Index != -1
                && _bidIntent != BidAction.Unset)
            {
                //StubInterface.Instance.StubClient.BidItem(Index, _bidIntent == BidIntent.Roll);
                switch (_bidIntent)
                {
                    case BidAction.Unset:
                        return;

                    case BidAction.Pass:
                        UI.FocusManager.SendPgDown(200);
                        break;

                    case BidAction.Roll:
                        UI.FocusManager.SendPgUp(200);
                        break;
                }

                BidSent = true;
            }
        }
    }

    public bool BidSent
    {
        get => _bidSent;
        set
        {
            if (_bidSent == value) return;
            _bidSent = value;
            N();
            Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
        }
    }

    public int Index { get; set; } = -1;

    public int WinnerRoll
    {
        get => _winnerRoll;
        set
        {
            if (_winnerRoll == value) return;
            _winnerRoll = value;
            N();
        }
    }

    public ICommand SetBidIntentCommand { get; }

    public LootItemViewModel(DropItem item)
    {
        Item = item;

        Game.DB!.ItemsDatabase.TryGetItem(item.ItemId, out var dbItem);
        DbItem = dbItem;

        SetBidIntentCommand = new RelayCommand<BidAction>(SetBidIntent,
            _ => DistributionStatus is not (DistributionStatus.Distributed or DistributionStatus.Discarded) && !BidSent);
    }

    void SetBidIntent(BidAction intent)
    {
        if (BidIntent == intent) BidIntent = BidAction.Unset;
        else BidIntent = intent;
    }
}