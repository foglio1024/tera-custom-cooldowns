using System.Collections.Generic;
using System.Linq;
using FoglioUtils;
using TeraDataLite;

namespace TCC.Data
{
    public class GuildInfo
    {
        public bool InGuild { get; private set; }
        public GuildMemberData Master { get; private set; }

        public TSObservableCollection<GuildMemberData> Members { get; private set; } = new TSObservableCollection<GuildMemberData>();
        public bool AmIMaster { get; private set; }

        public string NameOf(uint id)
        {
            var found = Members.ToSyncList().FirstOrDefault(m => m.PlayerId == id);
            return found.Name != "" ? found.Name : "Unknown player";
        }

        public void Set(List<GuildMemberData> mMembers, uint masterId, string masterName)
        {
            mMembers.ForEach(m =>
            {
                if (Has(m.Name)) return;
                Members.Add(m);
            });

            //var toRemove = new List<GuildMemberData>();
            //Members.ToSyncList().ForEach(m =>
            //{
            //    if (mMembers.All(f => f.PlayerId != m.PlayerId)) toRemove.Add(m);
            //});
            //toRemove.ForEach(m => Members.Remove(m));

            InGuild = true;
            SetMaster(masterId, masterName);
        }

        public bool Has(string name)
        {
            return Members.ToSyncList().Any(m => m.Name == name);
        }

        public void Clear()
        {
            Members.Clear();
            InGuild = false;
            Master = default;
            AmIMaster = false;
        }

        public void SetMaster(uint playerId, string playerName)
        {
            Master = new GuildMemberData { Name = playerName, PlayerId = playerId };
            AmIMaster = Master.Name == Game.Me.Name;
        }
    }
}