using System.Collections.Generic;
using System.Linq;
using TeraDataLite;

namespace TCC.Data
{
    public class GroupInfo
    {
        public bool InGroup { get; private set; }
        public bool IsRaid { get; private set; }
        public bool AmILeader => Game.Me.Name == Leader.Name;

        public GroupMemberData Leader { get; private set; }
        public List<GroupMemberData> Members { get; private set; } = new List<GroupMemberData>();

        public void SetGroup(List<GroupMemberData> members, bool raid)
        {
            Members = members;
            Leader = members.Find(m => m.IsLeader);
            IsRaid = raid;
            InGroup = true;
        }
        public void ChangeLeader(string name)
        {
            Members.ForEach(m => m.IsLeader = m.Name == name);
        }
        public void Remove(uint playerId, uint serverId)
        {
            var target = Members.Find(m => m.PlayerId == playerId && m.ServerId == serverId);
            Members.Remove(target);
        }
        public void Disband()
        {
            Members.Clear();
            Leader = default;
            IsRaid = false;
            InGroup = false;
        }
        public bool Has(string name)
        {
            return Members.Any(m => m.Name == name);
        }
        public bool HasPowers(string name)
        {
            return Has(name) && Members.FirstOrDefault(x => x.Name == name).CanInvite;
        }

    }
}