using System;
using System.Collections.Generic;
using System.Linq;
using TCC.Data.Chat;
using TeraDataLite;

namespace TCC.Data;

public class FriendList
{

    bool _waitingForFirstUpdate = true;
    readonly List<FriendEntry> _friends  = new();

    internal void UpdateFriendInfo(List<FriendInfoUpdate> friendUpdates)
    {
        friendUpdates.ForEach(update =>
        {
            var friend = _friends.Find(x => x.Id == update.Id);
            if (friend == default) return;
            var fireMessage = update.Status != friend.Status
                              && update.Status is FriendStatus.Online
                              && !_waitingForFirstUpdate;
            //Log.Chat($"Updating {friend.Name} from {friend.Status} to {update.Status} ({nameof(_waitingForFirstUpdate)}:{_waitingForFirstUpdate})");
            var idx = _friends.IndexOf(friend);
            friend.UpdateFrom(update);
            _friends[idx] = friend;
            if (!fireMessage) return;
            SystemMessagesProcessor.AnalyzeMessage($"@0\vUserName\v{friend.Name}", "SMT_FRIEND_IS_CONNECTED");
        });
        _waitingForFirstUpdate = false;
    }

    internal void Clear()
    {
        _friends.Clear();
        _waitingForFirstUpdate = true;
    }

    internal void NotifyWalkInSameArea(uint playerId, uint worldId, uint guardId, uint sectionId)
    {
        var friend = _friends.Find(f => f.Id == playerId);
        if (friend == default) return;
        var areaName = sectionId.ToString();
        try
        {
            areaName = Game.DB!.RegionsDatabase.GetZoneName(Game.DB.MapDatabase.Worlds[worldId].Guards[guardId].Sections[sectionId].NameId);
        }
        catch (Exception)
        {
            // ignored
        }

        SystemMessagesProcessor.AnalyzeMessage($"@0\vUserName\v{friend.Name}\vAreaName\v{areaName}", "SMT_FRIEND_WALK_INTO_SAME_AREA");
    }

    internal void SetFrom(List<FriendEntry> friends)
    {
        var toRemove = _friends.Where(x => friends.All(f => f.Id != x.Id)).ToArray();

        foreach (var item in toRemove)
        {
            _friends.Remove(item);
        }

        friends.ForEach(updated =>
        {
            var existing = _friends.Find(x => x.Id == updated.Id);
            if (existing == default)
                _friends.Add(updated);
            else
            {
                var idx = _friends.IndexOf(existing);
                existing.UpdateFrom(updated);
                _friends[idx] = existing;
            }
        });
    }

    internal void Remove(string name)
    {
        _friends.RemoveAll(x => x.Name == name);
    }

    internal bool Has(string name)
    {
        return _friends.Any(x => x.Name == name && x.Type is FriendEntryType.Friend);
    }
}