using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Nostrum.WPF;
using Nostrum.WPF.Factories;
using Nostrum.WPF.ThreadSafe;
using TCC.Data;
using TCC.Interop.Proxy;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TCC.Utils;
using TCC.ViewModels.Widgets;
using TeraDataLite;
using TeraPacketParser.Analysis;
using TeraPacketParser.Messages;
using ModifierKeys = TCC.Data.ModifierKeys;

namespace TCC.ViewModels;

[TccModule]
public class LootDistributionViewModel : TccWindowViewModel
{

    readonly IReadOnlyCollection<uint> _itemExclusions =
    [
        8008, 8009, 8010, 8011, 8012, 8013, 8014, 8015,
        8016, 8017, 8018, 8019, 8020, 8021, 8022, 8023
    ];
    readonly Dictionary<GameId, DropItem> _droppedItems = new();
    readonly Dictionary<(uint, uint), uint> _amountsDistributed = new();
    readonly DispatcherTimer _countdown;
    readonly DispatcherTimer _commitDelay;
    readonly DispatcherTimer _clear;

    LootDistributionWindowSettings _settings => (LootDistributionWindowSettings)Settings!;

    LootItemViewModel? _itemInDistribution;
    bool _isListVisible;
    int _itemsLeftAmount;
    int _timeLeft = 59;
    float _delayFactor;

    public LootItemViewModel? ItemInDistribution
    {
        get => _itemInDistribution;
        set
        {
            if (_itemInDistribution == value) return;
            _itemInDistribution = value;
            N();
        }
    }

    public bool IsListVisible
    {
        get => _isListVisible;
        set
        {
            if (_isListVisible == value) return;
            _isListVisible = value;
            N();
        }
    }

    public int ItemsLeftAmount
    {
        get => _itemsLeftAmount;
        set
        {
            if (_itemsLeftAmount == value) return;
            _itemsLeftAmount = value;
            if (_itemsLeftAmount < 0) _itemsLeftAmount = 0;
            N();
        }
    }

    public int TimeLeft
    {
        get => _timeLeft;
        set
        {
            if (_timeLeft == value) return;
            _timeLeft = value;
            N();
        }
    }

    public float DelayFactor
    {
        get => _delayFactor;
        set
        {
            if (_delayFactor == value) return;
            _delayFactor = value;
            N();
        }
    }

    public ThreadSafeObservableCollection<LootItemViewModel> DistributionList { get; }
    public ThreadSafeObservableCollection<LootingGroupMember> Members { get; }

    public ICollectionView DistributionListView { get; }
    public ICollectionViewLiveShaping MembersView { get; }
    public ICommand SetRollForCategoryCommand { get; }
    public ICommand SetPassForCategoryCommand { get; }
    public ICommand SetWaitForCategoryCommand { get; }
    public ICommand ToggleListViewCommand { get; }

    public LootDistributionViewModel(LootDistributionWindowSettings settings) : base(settings)
    {
        DistributionList = new ThreadSafeObservableCollection<LootItemViewModel>();
        Members = new ThreadSafeObservableCollection<LootingGroupMember>();

        DistributionListView = CollectionViewFactory.CreateCollectionView(DistributionList, sortDescr: new[] {
            new SortDescription($"{nameof(LootItemViewModel.DbItem)}.{nameof(Item.RareGrade)}", ListSortDirection.Descending),
            new SortDescription($"{nameof(LootItemViewModel.Item)}.{nameof(DropItem.ItemId)}", ListSortDirection.Ascending)
        });

        MembersView = CollectionViewFactory.CreateLiveCollectionView(Members, sortFilters: [
            new SortDescription($"{nameof(LootingGroupMember.IsPlayer)}", ListSortDirection.Descending)
        ])!;

        DistributionListView.GroupDescriptions.Add(new PropertyGroupDescription($"{nameof(LootItemViewModel.DbItem)}.{nameof(Item.Id)}"));

        SetRollForCategoryCommand = new RelayCommand<int>(SetRollForCategory);
        SetPassForCategoryCommand = new RelayCommand<int>(SetPassForCategory);
        SetWaitForCategoryCommand = new RelayCommand<int>(SetWaitForCategory);
        ToggleListViewCommand = new RelayCommand(ToggleListView);

        Game.Group.CompositionChanged += OnGroupCompositionChanged;
        Game.LoggedChanged += OnLoginStatusChanged;

        _countdown = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _countdown.Tick += OnCountdownTick;

        settings.DelayChanged += OnDelaySettingChanged;
        settings.AutoRollPolicyChanged += OnAutoRollPolicyChanged;

        _commitDelay = new DispatcherTimer { Interval = TimeSpan.FromSeconds(settings.AutorollDelaySec) };
        _commitDelay.Tick += OnDelayTick;

        _clear = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
        _clear.Tick += OnClearTick;

        DelayFactor = settings.AutorollDelaySec / 59f;

        KeyboardHook.Instance.RegisterCallback(_settings.ToggleHotkey, OnShowLootWindowHotkeyPressed);
        KeyboardHook.Instance.RegisterCallback(_settings.RollHotKey with { Modifier = ModifierKeys.Shift }, SetRollForCurrentCategory);
        KeyboardHook.Instance.RegisterCallback(_settings.PassHotKey with { Modifier = ModifierKeys.Shift }, SetPassForCurrentCategory);
        KeyboardHook.Instance.RegisterCallback(_settings.PassHotKey with { Modifier = ModifierKeys.Shift | ModifierKeys.Control }, SetWaitForCurrentCategory);

        Game.LootDistributionWindowShowRequest += OnShowRequest;
    }

    void OnShowRequest()
    {
        _settings.Visible = true;
    }

    void SetWaitForCurrentCategory()
    {
        if (ItemInDistribution == null) return;
        SetWaitForCategory((int)ItemInDistribution.DbItem.Id);
    }

    void SetPassForCurrentCategory()
    {
        if (ItemInDistribution == null) return;
        SetPassForCategory((int)ItemInDistribution.DbItem.Id);
    }

    void SetRollForCurrentCategory()
    {
        if (ItemInDistribution == null) return;
        SetRollForCategory((int)ItemInDistribution.DbItem.Id);
    }

    void OnAutoRollPolicyChanged()
    {
        var policy = _settings.AlwaysRoll ? BidAction.Roll : _settings.AlwaysPass ? BidAction.Pass : BidAction.Unset;

        var items = DistributionList.ToSyncList().Where(x => !x.BidSent).ToArray();

        foreach (var item in items)
        {
            item.BidIntent = policy;
        }
    }

    void OnClearTick(object? sender, EventArgs e)
    {
        _clear.Stop();
        _settings.Visible = false;
        ClearLoot();
    }

    void OnShowLootWindowHotkeyPressed()
    {
        if (!Game.Group.InGroup) return;
        _settings.Visible = true;
    }

    void OnLoginStatusChanged()
    {
        if (Game.Logged) return;
        _settings.Visible = false;
        ClearAll();
    }

    protected override void OnEnabledChanged(bool enabled)
    {
        base.OnEnabledChanged(enabled);
        if (!enabled)
        {
            _settings.Visible = false;
            ClearLoot();
        }

        StubInterface.Instance.StubClient.UpdateSetting("LootWindowEnabled", enabled);
    }

    void ClearAll()
    {
        Members.Clear();
        ClearLoot();
    }

    void ClearLoot()
    {
        DistributionList.Clear();
        _droppedItems.Clear();
        _amountsDistributed.Clear();
        _countdown.Stop();
        _commitDelay.Stop();
        if (ItemInDistribution != null)
        {
            ItemInDistribution.DistributionStatus = DistributionStatus.Waiting;
            ItemInDistribution = null;
        }
        TimeLeft = 59;
        ItemsLeftAmount = 0;
    }

    void OnDelaySettingChanged(int newValue)
    {
        _commitDelay.Interval = TimeSpan.FromSeconds(newValue);
        DelayFactor = newValue / 59f;
    }

    void OnDelayTick(object? sender, EventArgs e)
    {
        if (ItemInDistribution != null && ItemInDistribution.BidIntent != BidAction.Unset)
        {
            ItemInDistribution?.CommitIntent();
        }
        _commitDelay.Stop();
    }

    void OnCountdownTick(object? sender, EventArgs e)
    {
        TimeLeft--;
        if (TimeLeft == 0) _countdown.Stop();
    }

    void ResetTimeLeft()
    {
        _countdown.Stop();
        TimeLeft = 59;
        _countdown.Start();
    }

    void ToggleListView()
    {
        IsListVisible = !IsListVisible;
    }

    void SetRollForCategory(int itemId)
    {
        DistributionList.ToSyncList().Where(x => x.DbItem.Id == itemId && !x.BidSent).ToList().ForEach(x =>
        {
            x.BidIntent = BidAction.Roll;
            if (x.DistributionStatus is DistributionStatus.Distributing) x.CommitIntent();
        });
    }

    void SetPassForCategory(int itemId)
    {
        DistributionList.ToSyncList().Where(x => x.DbItem.Id == itemId && !x.BidSent).ToList().ForEach(x =>
        {
            x.BidIntent = BidAction.Pass;
            if (x.DistributionStatus is DistributionStatus.Distributing) x.CommitIntent();
        });
    }

    void SetWaitForCategory(int itemId)
    {
        DistributionList.ToSyncList().Where(x => x.DbItem.Id == itemId && !x.BidSent).ToList().ForEach(x =>
        {
            x.BidIntent = BidAction.Unset;
        });
    }

    void OnGroupCompositionChanged(ReadOnlyCollection<GroupMemberData> members, GroupCompositionChangeReason reason)
    {
        switch (reason)
        {
            case GroupCompositionChangeReason.Created:
                PacketAnalyzer.Processor.Hook<S_SPAWN_DROPITEM>(OnSpawnDropitem);
                break;

            case GroupCompositionChangeReason.Disbanded:
                PacketAnalyzer.Processor.Unhook<S_SPAWN_DROPITEM>(OnSpawnDropitem);

                _settings.Visible = false;
                ClearLoot();
                if (_clear.IsEnabled) _clear.Stop();

                break;
        }

        Dispatcher.InvokeAsync(() => UpdateGroupMembers(members, reason));
    }

    void UpdateGroupMembers(ReadOnlyCollection<GroupMemberData> members, GroupCompositionChangeReason reason)
    {
        switch (reason)
        {
            case GroupCompositionChangeReason.Created:
                foreach (var member in members)
                {
                    Members.Add(new LootingGroupMember(member));
                }
                break;

            case GroupCompositionChangeReason.Disbanded:
                Members.Clear();
                break;

            case GroupCompositionChangeReason.Updated:
                var current = Members.ToSyncList();
                var removed = current.Where(x => members.All(y => y.EntityId != x.Member.EntityId)).ToArray();
                var added = members.Where(x => current.All(y => y.Member.EntityId != x.EntityId)).ToArray();

                Dispatcher.Invoke(() =>
                {
                    foreach (var member in removed)
                    {
                        Members.Remove(member);
                    }

                    foreach (var member in added)
                    {
                        Members.Add(new LootingGroupMember(member));
                    }
                });

                break;
        }
    }

    protected override void InstallHooks()
    {
        PacketAnalyzer.Processor.Hook<S_ASK_BIDDING_RARE_ITEM>(OnAskBiddingRareItem);
        PacketAnalyzer.Processor.Hook<S_RESULT_BIDDING_DICE_THROW>(OnResultBiddingDiceThrow);
        PacketAnalyzer.Processor.Hook<S_RESULT_ITEM_BIDDING>(OnResultItemBidding);
        PacketAnalyzer.Processor.Hook<S_SET_ITEM_BIDDING_FLAG>(OnSetItemBiddingFlag);
        PacketAnalyzer.Processor.Hook<S_UPDATE_BIDDING_COUNT>(OnUpdateItemBiddingCount);
    }

    protected override void RemoveHooks()
    {
        PacketAnalyzer.Processor.Unhook<S_ASK_BIDDING_RARE_ITEM>(OnAskBiddingRareItem);
        PacketAnalyzer.Processor.Unhook<S_RESULT_BIDDING_DICE_THROW>(OnResultBiddingDiceThrow);
        PacketAnalyzer.Processor.Unhook<S_RESULT_ITEM_BIDDING>(OnResultItemBidding);
        PacketAnalyzer.Processor.Unhook<S_SET_ITEM_BIDDING_FLAG>(OnSetItemBiddingFlag);
        PacketAnalyzer.Processor.Unhook<S_UPDATE_BIDDING_COUNT>(OnUpdateItemBiddingCount);
    }

    #region Hooks

    /// <summary>
    /// A new item is presented for distribution.
    /// </summary>
    void OnAskBiddingRareItem(S_ASK_BIDDING_RARE_ITEM m)
    {
        Log.CW($"> S_ASK_BIDDING_RARE_ITEM: itemId:{m.ItemId} amount:{m.Amount}");

        if (_clear.IsEnabled) _clear.Stop();

        foreach (var member in Members)
        {
            member.Roll = 0;
            member.BidAction = BidAction.Unset;
            member.IsWinning = false;
        }

        var items = DistributionList.ToSyncList().Where(x => x.Item.ItemId == m.ItemId
                                                          && x.Item.Amount == m.Amount).ToArray();
        Log.CW($"  found {items.Length} candidates");

        LootItemViewModel? item;

        if (_amountsDistributed.TryGetValue((m.ItemId, m.Amount), out var amount) && amount < items.Length)
        {
            item = items[amount];
            Log.CW($"  found {item.DbItem.Name} in distributed items");
        }
        else
        {
            item = items.FirstOrDefault();
            Log.CW($"  item not found in distributed items at idx={amount}, trying to use first");
        }

        if (item != null)
        {
            item.DistributionStatus = DistributionStatus.Distributing;
            Log.CW($"  item {item.DbItem.Name} set as Distributing");
        }
        else
        {
            Log.CW("! item is still null, creating new one as Distributing");

            Dispatcher.Invoke(() =>
            {
                item = new LootItemViewModel(new DropItem(GameId.Zero, m.ItemId, m.Amount))
                {
                    DistributionStatus = DistributionStatus.Distributing
                };
                DistributionList.Add(item);
            });
        }

        ItemInDistribution = item;

        if (ItemInDistribution!.BidIntent == BidAction.Unset)
        {
            if (_settings.AlwaysPass)
            {
                ItemInDistribution!.BidIntent = BidAction.Pass;
            }
            if (_settings.AlwaysRoll)
            {
                ItemInDistribution!.BidIntent = BidAction.Roll;
            }
        }

        ResetTimeLeft();
        ItemInDistribution.Index = m.Index;
        _commitDelay.Start();

        if (_settings.AutoShowUponRoll)
        {
            _settings.Visible = true;
        }
    }

    /// <summary>
    /// Updats the amount of items left.
    /// </summary>
    void OnUpdateItemBiddingCount(S_UPDATE_BIDDING_COUNT p)
    {
        Log.CW($"> S_UPDATE_BIDDING_COUNT.count = {p.Count}");
        ItemsLeftAmount = p.Count + 1;
    }

    /// <summary>
    /// A player rolled or passed for the item.
    /// </summary>
    void OnResultBiddingDiceThrow(S_RESULT_BIDDING_DICE_THROW m)
    {
        Log.CW($"> S_RESULT_BIDDING_DICE_THROW: gameId:{m.EntityId} result:{m.RollResult}");

        if (Game.IsMe(m.EntityId) || Game.IsMe(m.PlayerId, m.ServerId))
        {
            Log.CW("  roll result is from player");
            if (ItemInDistribution != null)
            {
                Log.CW("  setting BidSent to true");
                ItemInDistribution.BidIntent = m.RollResult != -1 ? BidAction.Roll : BidAction.Pass;
                ItemInDistribution.BidSent = true;
            }
            else
            {
                Log.CW("!  no items in distribution not found");
            }

            _commitDelay.Stop();
        }

        var members = Members.ToSyncList().ToArray();

        var member = members.FirstOrDefault(x =>
                (x.Member.PlayerId == m.PlayerId && x.Member.ServerId == m.ServerId)
                || x.Member.EntityId == m.EntityId
            );

        if (member != null)
        {
            Log.CW($"  saving roll for group member {member.Member.Name}");
            member.Roll = m.RollResult;
            member.BidAction = m.RollResult == -1 ? BidAction.Pass : BidAction.Roll;
        }
        else
        {
            var csv = members.Aggregate("", (current, m2) => current + $"{m2.Member.EntityId}:{m2.Member.Name} | ");

            Log.CW($"! group member ({m.EntityId}) not found: {csv}");
        }

        // refresh winning status
        var winningMember = members.MaxBy(x => x.Roll);

        if (winningMember?.Roll < 1) return;

        foreach (var mb in members)
        {
            mb.IsWinning = mb == winningMember;
        }
    }

    /// <summary>
    /// Distribution is finished for the item.
    /// </summary>
    void OnResultItemBidding(S_RESULT_ITEM_BIDDING p)
    {
        Log.CW("> S_RESULT_ITEM_BIDDING");

        var members = Members.ToSyncList();

        LootingGroupMember? winner = null;

        if (members.All(m => m.Roll < 1))
        {
            members.ForEach(m => m.BidAction = BidAction.Pass);
            Log.CW("  no rolls for current item");
        }
        else
        {
            winner = members.SingleOrDefault(m => m.IsWinning);
        }

        if (ItemInDistribution != null)
        {
            if (winner != null)
            {
                ItemInDistribution.WinnerName = winner.Member.Name;
                ItemInDistribution.WinnerRoll = winner.Roll;
                ItemInDistribution.DistributionStatus = DistributionStatus.Distributed;
                Log.CW($"  item {ItemInDistribution.DbItem.Name} won by {winner.Member.Name}");
            }
            else
            {
                ItemInDistribution.DistributionStatus = DistributionStatus.Discarded;
                Log.CW($"  winner is null, item {ItemInDistribution.DbItem.Name} set as Discarded");
            }

            if (_amountsDistributed.TryGetValue((ItemInDistribution.Item.ItemId, ItemInDistribution.Item.Amount), out var amount))
            {
                _amountsDistributed[(ItemInDistribution.Item.ItemId, ItemInDistribution.Item.Amount)] = amount + 1;
                Log.CW($"  incrementing distributed amount of item {ItemInDistribution.Item.ItemId},{ItemInDistribution.Item.Amount}");
            }
            else
            {
                _amountsDistributed[(ItemInDistribution.Item.ItemId, ItemInDistribution.Item.Amount)] = 1;
                Log.CW($"  item {ItemInDistribution.Item.ItemId},{ItemInDistribution.Item.Amount} not distributed, setting distributed amount to 1");
            }
        }
        else
        {
            Log.CW("!  item in distribution not found");
        }

        _countdown.Stop();

        ItemsLeftAmount--;

        if (ItemsLeftAmount == 0)
        {
            _clear.Start();
        }
    }

    /// <summary>
    /// An item is set/unset for distribution.
    /// </summary>
    void OnSetItemBiddingFlag(S_SET_ITEM_BIDDING_FLAG p)
    {
        Log.CW($"> S_SET_ITEM_BIDDING_FLAG: gameId:{p.GameId} flag:{p.Flag}");

        if (!p.Flag)
        {
            Log.CW("  flag is false, returning");
            return;
        }

        // move item from available to distribution list
        var existing = DistributionList.FirstOrDefault(x => x.Item.GameId == p.GameId);

        if (existing != null)
        {
            Log.CW("  item was in DistributionList, reverting it to Waiting");
            existing.DistributionStatus = DistributionStatus.Waiting;

            if (_settings.AlwaysPass) existing.BidIntent = BidAction.Pass;
            if (_settings.AlwaysRoll) existing.BidIntent = BidAction.Roll;
        }
        else
        {
            Log.CW("  item was not in DistributionList, moving it from dropped items");
            var dropItem = _droppedItems.TryGetValue(p.GameId, out var di) ? di : new DropItem(p.GameId, 0, 0);

            Log.CW($"  dropped item is {dropItem.ItemId}");

            Dispatcher.Invoke(() =>
            {
                var itemVm = new LootItemViewModel(dropItem);
                DistributionList.Add(itemVm);

                if (_settings.AlwaysPass) itemVm.BidIntent = BidAction.Pass;
                if (_settings.AlwaysRoll) itemVm.BidIntent = BidAction.Roll;

                Log.CW($"  created item viewmodel {itemVm.Item.GameId}");
            });
        }
    }

    /// <summary>
    /// An item is spawned on the ground.
    /// </summary>
    void OnSpawnDropitem(S_SPAWN_DROPITEM p)
    {
        if (_itemExclusions.Contains(p.ItemId)) return;
        Log.CW($"> S_SPAWN_DROPITEM: gameId:{p.GameId} itemId:{p.ItemId} amount:{p.Amount}");
        _droppedItems[p.GameId] = new DropItem(p.GameId, p.ItemId, p.Amount);
    }

    #endregion Hooks
}

public enum BidAction
{
    Unset,
    Pass,
    Roll
}

public enum DistributionStatus
{
    Waiting,
    Distributing,
    Distributed,
    Discarded
}