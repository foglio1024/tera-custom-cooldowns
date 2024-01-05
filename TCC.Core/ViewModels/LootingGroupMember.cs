using Nostrum.WPF.ThreadSafe;
using TeraDataLite;

namespace TCC.ViewModels;

public class LootingGroupMember : ThreadSafeObservableObject
{
    public GroupMemberData Member { get; }

    int _roll;

    public int Roll
    {
        get => _roll;
        set => RaiseAndSetIfChanged(value, ref _roll);
    }

    bool _isWinning;

    public bool IsWinning
    {
        get => _isWinning;
        set => RaiseAndSetIfChanged(value, ref _isWinning);
    }

    BidAction _bidAction;

    public BidAction BidAction
    {
        get => _bidAction;
        set => RaiseAndSetIfChanged(value, ref _bidAction);
    }

    public bool IsPlayer { get; }
    
    public LootingGroupMember(GroupMemberData member)
    {
        IsPlayer = Game.IsMe(member.EntityId);
        Member = member;
    }
}