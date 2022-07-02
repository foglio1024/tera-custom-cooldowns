using System;
using System.Collections.Generic;
using TeraDataLite;
namespace TeraPacketParser.Messages
{
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
                var u = new GroupMemberData();
                reader.Skip(2); // curr
                var next = reader.ReadUInt16();
                var nameOffset = reader.ReadUInt16();
                u.PlayerId = reader.ReadUInt32();

                if (reader.Factory.ReleaseVersion / 100 >= 108)
                    u.ServerId = reader.ReadUInt32();

                u.Class = (Class)reader.ReadUInt16();
                reader.Skip(4); // race + gender
                u.Level = Convert.ToUInt32(reader.ReadUInt16());
                var worldId = reader.ReadUInt32(); // worldId
                u.GuardId = reader.ReadUInt32();
                u.SectionId = reader.ReadUInt32();

                if (reader.Factory.ReleaseVersion / 100 >= 108)
                    reader.Skip(4); // dungeonGauntletDifficultyId

                u.IsLeader = reader.ReadBoolean();
                u.Online = reader.ReadBoolean();

                reader.RepositionAt(nameOffset);
                u.Name = reader.ReadTeraString();

                if (u.IsLeader) Id = u.PlayerId;
                Members.Add(u);
                if (next != 0) reader.BaseStream.Position = next - 4;
            }
        }
    }
}
