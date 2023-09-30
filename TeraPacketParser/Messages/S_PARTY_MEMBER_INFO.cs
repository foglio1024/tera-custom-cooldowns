using System;
using System.Collections.Generic;
using TeraDataLite;

namespace TeraPacketParser.Messages;

public class S_PARTY_MEMBER_INFO : ParsedMessage
{
    public uint Id { get; }
    public List<GroupMemberData> Members { get; }

    public S_PARTY_MEMBER_INFO(TeraMessageReader reader) : base(reader)
    {
        var count = reader.ReadUInt16();
        var offset = reader.ReadUInt16();

        reader.Skip(2); // bool isRaid + bool adminInspectionUI

        reader.RepositionAt(offset);
        Members = new List<GroupMemberData>();
        for (var i = 0; i < count; i++)
        {
            reader.Skip(2); // curr
            var next = reader.ReadUInt16();
            var nameOffset = reader.ReadUInt16();
            var playerId = reader.ReadUInt32();

            var serverId = 0U;
            if (reader.Factory.ReleaseVersion / 100 >= 108)
                serverId = reader.ReadUInt32();

            var cl = (Class)reader.ReadUInt16();
            reader.Skip(4); // race + gender
            var level = Convert.ToUInt32(reader.ReadUInt16());
            var worldId = reader.ReadUInt32(); // worldId
            var guardId = reader.ReadUInt32();
            var sectionId = reader.ReadUInt32();

            if (reader.Factory.ReleaseVersion / 100 >= 108)
                reader.Skip(4); // dungeonGauntletDifficultyId

            var isLeader = reader.ReadBoolean();
            var online = reader.ReadBoolean();

            reader.RepositionAt(nameOffset);
            var name = reader.ReadTeraString();

            var u = new GroupMemberData
            {
                PlayerId = playerId,
                Name = name,
                ServerId = serverId,
                Class = cl,
                Level = level,
                GuardId = guardId,
                SectionId = sectionId,
                IsLeader = isLeader,
                Online = online,
            };

            if (u.IsLeader) Id = u.PlayerId;
            Members.Add(u);
            if (next != 0) reader.BaseStream.Position = next - 4;
        }
    }
}