using System.Collections.Generic;
using TeraDataLite;

namespace TeraPacketParser.Messages
{
    public class S_LEAVE_GUILD : ParsedMessage
    {
        public S_LEAVE_GUILD(TeraMessageReader reader) : base(reader)
        {
        }
    }
    public class S_PARTY_MEMBER_LIST : ParsedMessage
    {
        public bool Im { get; }
        public bool Raid { get; }
        public uint LeaderServerId { get; }
        public uint LeaderPlayerId { get; }

        public List<GroupMemberData> Members { get; }

        public S_PARTY_MEMBER_LIST(TeraMessageReader reader) : base(reader)
        {
            var count = reader.ReadUInt16();
            var offset = reader.ReadUInt16();

            Im = reader.ReadBoolean();
            Raid = reader.ReadBoolean();

            reader.Skip(4); // memberLimit
            reader.Skip(8); // id

            LeaderServerId = reader.ReadUInt32();
            LeaderPlayerId = reader.ReadUInt32();

            //reader.Skip(19);
            /*
                - uint32 mode                       # 0: free for all, 1: round robin
                - uint32 distributeMinRarity
                - bool   distributeAllEquipment
                - bool   distributeOnlyReqClass
                - uint32 distributeMode             # 0: random, 1: roll
                - uint32 distributeModeBoP          # 0: random, 1: roll
                - bool   disableCombatLooting
             */


            Members = new List<GroupMemberData>();

            for (var i = 0; i < count; i++)
            {
                var u = new GroupMemberData();
                reader.RepositionAt(offset);
                reader.Skip(2);
                var nextOffset = reader.ReadUInt16();
                var nameOffset = reader.ReadUInt16();
                u.ServerId = reader.ReadUInt32();
                u.PlayerId = reader.ReadUInt32();
                u.Level = reader.ReadUInt32();
                u.Class = (Class)reader.ReadUInt32();
                if (reader.Factory.ReleaseVersion / 100 >= 106)
                {
                    reader.Skip(8); // race, gender
                }
                u.Online = reader.ReadBoolean();
                u.EntityId = reader.ReadUInt64();
                u.Order = reader.ReadInt32();
                u.CanInvite = reader.ReadBoolean();
                u.Laurel = (Laurel)reader.ReadUInt32();
                u.Awakened = reader.ReadInt32() > 0;
                reader.RepositionAt(nameOffset);
                u.Name = reader.ReadTeraString();
                u.Alive = true;
                u.IsLeader = u.ServerId == LeaderServerId && u.PlayerId == LeaderPlayerId;
                Members.Add(u);
                offset = nextOffset;
            }
        }
    }
}
