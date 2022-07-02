using System;
using System.Collections.Generic;
using System.Linq;
using TCC.Data.Chat;
using TeraDataLite;

namespace TCC.Data
{
    public class FriendList
    {

        bool _waitingForFirstUpdate = true;
        public List<FriendEntry> Friends { get; private set; } = new();

        internal void UpdateFriendInfo(List<FriendInfoUpdate> friendUpdates)
        {
            friendUpdates.ForEach(update =>
            {
                if (update.IsUpdated && update.Status is FriendStatus.Online)
                {
                    var friend = Friends.Find(x => x.Id == update.Id);
                    if (friend == default) return;
                    var fireMessage = update.Status != friend.Status
                                   && update.Status is FriendStatus.Online
                                   && !_waitingForFirstUpdate;
                    var idx = Friends.IndexOf(friend);
                    friend.UpdateFrom(update);
                    Friends[idx] = friend;
                    if (!fireMessage) return;
                    SystemMessagesProcessor.AnalyzeMessage($"@0\vUserName\v{friend.Name}", "SMT_FRIEND_IS_CONNECTED");
                }
            });
            _waitingForFirstUpdate = false;
        }

        internal void Clear()
        {
            Friends.Clear();
            _waitingForFirstUpdate = true;
        }

        internal void NotifyWalkInSameArea(uint playerId, uint worldId, uint guardId, uint sectionId)
        {
            var friend = Friends.Find(f => f.Id == playerId);
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
            var toRemove = Friends.Where(x => !friends.Any(f => f.Id == x.Id));

            foreach (var item in toRemove)
            {
                Friends.Remove(item);
            }

            friends.ForEach(updated =>
            {
                var existing = Friends.Find(x => x.Id == updated.Id);
                if (existing == default)
                    Friends.Add(updated);
                else
                {
                    var idx = Friends.IndexOf(existing);
                    existing.UpdateFrom(updated);
                    Friends[idx] = existing;
                }
            });
        }

        internal void Remove(string name)
        {
            Friends.RemoveAll(x => x.Name == name);
        }

        internal bool Has(string name)
        {
            return Friends.Any(x => x.Name == name && x.Type is FriendEntryType.Friend);
        }
    }
}