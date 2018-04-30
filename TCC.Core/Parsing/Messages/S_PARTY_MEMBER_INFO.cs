using System;
using System.Collections.Generic;
using TCC.Data;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MEMBER_INFO : ParsedMessage
    {

        public uint Id { get; private set; }
        public List<User> Members { get; }

        public S_PARTY_MEMBER_INFO(TeraMessageReader reader) : base(reader)
        {
            var count = reader.ReadUInt16();
            var offset = reader.ReadUInt16();
            reader.Skip(8);
            Id = reader.ReadUInt32();

            reader.BaseStream.Position = offset - 4;
            Members = new List<User>();
            for (var i = 0; i < count; i++)
            {
                var u = new User(WindowManager.GroupWindow.Dispatcher);
                var current = reader.ReadUInt16();
                var next = reader.ReadUInt16();
                var nameOffset = reader.ReadUInt16();
                u.PlayerId = reader.ReadUInt32();
                u.UserClass = (Class)reader.ReadUInt16();
                reader.Skip(2 + 2);
                u.Level = Convert.ToUInt32(reader.ReadUInt16());
                reader.ReadBoolean();
                reader.Skip(1);
                u.Order = Convert.ToInt32(reader.ReadInt16());
                var gId = reader.ReadUInt32();
                var aId = reader.ReadUInt32();
                u.Location = SessionManager.MapDatabase.GetName(gId, aId);
                u.IsLeader = reader.ReadBoolean();
                //u.Laurel = (Laurel)reader.ReadUInt32();
                Console.WriteLine("---");
                u.Online = reader.ReadBoolean();
                Console.WriteLine(reader.ReadByte());
                Console.WriteLine(reader.ReadByte());
                reader.BaseStream.Position = nameOffset - 4;
                u.Name = reader.ReadTeraString();
                //u.IsLeader = u.PlayerId == Id;
                Members.Add(u);
                if (next != 0) reader.BaseStream.Position = next - 4;
            }

        }
    }
}
