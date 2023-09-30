using System.Collections.Generic;
using System.Collections.ObjectModel;
using TeraDataLite;

namespace TeraPacketParser.Messages;

public class S_PARTY_MEMBER_LIST : ParsedMessage
{
    public bool Im { get; }
    public bool Raid { get; }
    public uint LeaderServerId { get; }
    public uint LeaderPlayerId { get; }

    public ReadOnlyCollection<GroupMemberData> Members { get; }

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

        var members = new List<GroupMemberData>();

        for (var i = 0; i < count; i++)
        {
            reader.RepositionAt(offset);
            reader.Skip(2);
            var nextOffset = reader.ReadUInt16();
            var nameOffset = reader.ReadUInt16();
            var serverId = reader.ReadUInt32();
            var playerId = reader.ReadUInt32();
            var level = reader.ReadUInt32();
            var cl = (Class)reader.ReadUInt32();
            if (reader.Factory.ReleaseVersion / 100 >= 106)
            {
                reader.Skip(8); // race, gender
            }
            var online = reader.ReadBoolean();
            var entityId = reader.ReadUInt64();
            var order = reader.ReadInt32();
            var canInvite = reader.ReadBoolean();
            var laurel = (Laurel)reader.ReadUInt32();
            var awakened = reader.ReadInt32() > 0;
            reader.RepositionAt(nameOffset);
            var name = reader.ReadTeraString();
            var alive = true;
            var isLeader = serverId == LeaderServerId && playerId == LeaderPlayerId;

            var u = new GroupMemberData
            {
                Name = name,
                Class = cl,
                PlayerId = playerId,
                ServerId = serverId,
                Level = level,
                Online = online,
                EntityId = entityId,
                Order = order,
                CanInvite = canInvite,
                Laurel = laurel,
                Awakened = awakened,
                Alive = alive,
                IsLeader = isLeader
            };

            members.Add(u);
            offset = nextOffset;
        }

        Members = members.AsReadOnly();
    }
}