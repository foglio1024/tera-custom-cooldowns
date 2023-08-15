using System;
using System.Collections.Generic;
using System.Linq;
using TeraDataLite;

namespace TCC.Data;

public class GroupInfo
{
    GroupMemberData leader = new();

    public event Action? LeaderChanged;
    public bool InGroup { get; private set; }
    public bool IsRaid { get; private set; }
    public bool AmILeader => Game.Me.Name == Leader.Name && InGroup;
    public int Size { get; private set; }

    public GroupMemberData Leader
    {
        get => leader;
        private set
        {
            if (leader == value) return;
            leader = value;
            LeaderChanged?.Invoke();
        }
    }
    List<GroupMemberData> Members { get; set; } = new();

    public void SetGroup(List<GroupMemberData> members, bool raid)
    {
        Members = members;
        Leader = members.FirstOrDefault(m => m.IsLeader)!;
        IsRaid = raid;
        InGroup = true;
        Size = Members.Count;
    }
    public void ChangeLeader(string name)
    {
        Members.ForEach(x => x.IsLeader = x.Name == name);
        Leader = Members.FirstOrDefault(m => m.Name == name)!;
    }
    public void Remove(uint playerId, uint serverId)
    {
        var target = Members.FirstOrDefault(m => m.PlayerId == playerId && m.ServerId == serverId);
        if (target == null) return;
        Members.Remove(target);
        Size = Members.Count;
    }
    public void Disband()
    {
        Members.Clear();
        Leader = new GroupMemberData();
        IsRaid = false;
        InGroup = false;
        Size = Members.Count;
    }
    public bool Has(string name)
    {
        return Members.Any(m => m.Name == name);
    }
    public bool Has(uint pId)
    {
        return Members.Any(m => m.PlayerId == pId);
    }
    public bool HasPowers(string name)
    {
        return Has(name) && Members.FirstOrDefault(x => x.Name == name)?.CanInvite == true;
    }
    public bool TryGetMember(uint playerId, uint serverId, out GroupMemberData? member)
    {
        member = Members.FirstOrDefault(m => m.PlayerId == playerId && m.ServerId == serverId);
        return member != null;
    }
    public bool TryGetMember(string name, out GroupMemberData? member)
    {
        member = Members.FirstOrDefault(m => m.Name == name);
        return member != null;
    }
}