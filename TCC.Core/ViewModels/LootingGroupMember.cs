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
        set
        {
            if (_roll == value) return;
            _roll = value;
            N();
        }
    }

    bool _isWinning;

    public bool IsWinning
    {
        get => _isWinning;
        set
        {
            if (_isWinning == value) return;
            _isWinning = value;
            N();
        }
    }

    BidAction _bidAction;

    public BidAction BidAction
    {
        get => _bidAction;
        set
        {
            if (_bidAction == value) return;
            _bidAction = value;
            N();
        }
    }

    public LootingGroupMember(GroupMemberData member)
    {
        Member = member;
    }
}