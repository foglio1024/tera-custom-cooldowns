﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TeraDataLite;

namespace TCC.Data;

public enum GroupCompositionChangeReason
{
    Created,
    Disbanded,
    Updated
}

public class GroupInfo
{
    GroupMemberData _leader = new() { Class = Class.None, Name = "", PlayerId = 0 };
    List<GroupMemberData> _members = new();

    public event Action? LeaderChanged;

    public event Action<ReadOnlyCollection<GroupMemberData>, GroupCompositionChangeReason>? CompositionChanged;

    public bool InGroup { get; private set; }
    public bool IsRaid { get; private set; }
    public bool AmILeader => Game.Me.Name == Leader.Name && InGroup;
    public int Size { get; private set; }

    public GroupMemberData Leader
    {
        get => _leader;
        private set
        {
            if (_leader == value) return;
            _leader = value;
            LeaderChanged?.Invoke();
        }
    }

    public void UpdateComposition(ReadOnlyCollection<GroupMemberData> members, bool raid)
    {
        var reason = Size == 0 ? GroupCompositionChangeReason.Created : GroupCompositionChangeReason.Updated;

        _members = members.ToList();
        Leader = members.FirstOrDefault(m => m.IsLeader)!;
        IsRaid = raid;
        InGroup = true;
        Size = _members.Count;

        CompositionChanged?.Invoke(_members.AsReadOnly(), reason);
    }

    public void ChangeLeader(string name)
    {
        _members.ForEach(x => x.IsLeader = x.Name == name);
        Leader = _members.FirstOrDefault(m => m.Name == name)!;
    }

    public void RemoveMember(uint playerId, uint serverId)
    {
        var target = _members.FirstOrDefault(m => m.PlayerId == playerId && m.ServerId == serverId);
        if (target == null) return;
        _members.Remove(target);
        Size = _members.Count;

        CompositionChanged?.Invoke(_members.AsReadOnly(), GroupCompositionChangeReason.Updated);
    }

    public void Disband()
    {
        _members.Clear();
        Leader = new GroupMemberData
        {
            Class = Class.None,
            Name = "",
            PlayerId = 0
        };
        IsRaid = false;
        InGroup = false;
        Size = _members.Count;

        CompositionChanged?.Invoke(_members.AsReadOnly(), GroupCompositionChangeReason.Disbanded);
    }

    public bool Has(string name)
    {
        return _members.Any(m => m.Name == name);
    }

    public bool Has(uint pId)
    {
        return _members.Any(m => m.PlayerId == pId);
    }

    public bool HasPowers(string name)
    {
        return Has(name) && _members.FirstOrDefault(x => x.Name == name)?.CanInvite == true;
    }

    public bool TryGetMember(uint playerId, uint serverId, out GroupMemberData? member)
    {
        member = _members.FirstOrDefault(m => m.PlayerId == playerId && m.ServerId == serverId);
        return member != null;
    }

    public bool TryGetMember(ulong entityId, out GroupMemberData? member)
    {
        member = _members.FirstOrDefault(m => m.EntityId == entityId);
        return member != null;
    }

    public bool TryGetMember(string name, out GroupMemberData? member)
    {
        member = _members.FirstOrDefault(m => m.Name == name);
        return member != null;
    }

    internal ReadOnlyCollection<GroupMemberData> GetMembers()
    {
        return _members.ToArray().AsReadOnly();
    }
}