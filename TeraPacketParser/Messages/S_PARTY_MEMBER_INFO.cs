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
            reader.Skip(2);

            reader.BaseStream.Position = offset - 4;
            Members = new List<GroupMemberData>();
            for (var i = 0; i < count; i++)
            {
                var u = new GroupMemberData();
                reader.Skip(2);
                var next = reader.ReadUInt16();
                var nameOffset = reader.ReadUInt16();
                u.PlayerId = reader.ReadUInt32();
                u.Class = (Class)reader.ReadUInt16();
                reader.Skip(4);
                u.Level = Convert.ToUInt32(reader.ReadUInt16());
                reader.Skip(4);
                u.GuardId = reader.ReadUInt32();
                u.SectionId = reader.ReadUInt32();
                u.Order = Convert.ToInt32(reader.ReadInt16());
                reader.Skip(2);
                u.IsLeader = reader.ReadBoolean();
                if (u.IsLeader) Id = u.PlayerId;
                u.Online = reader.ReadBoolean();
                reader.BaseStream.Position = nameOffset - 4;
                u.Name = reader.ReadTeraString();
                Members.Add(u);
                if (next != 0) reader.BaseStream.Position = next - 4;
            }
        }
    }
}
