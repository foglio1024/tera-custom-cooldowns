using System.Collections.Generic;
using TCC.Data;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MEMBER_INFO : ParsedMessage
    {
        ushort membersCount, membersOffset;

        public ushort MembersCount { get => membersCount; }
        public int Id { get; private set; }
        private List<User> members;
        public List<User> Members
        {
            get { return members; }
        }

        public S_PARTY_MEMBER_INFO(TeraMessageReader reader) : base(reader)
        {
            membersCount = reader.ReadUInt16();
            reader.Skip(10);
            Id = reader.ReadInt32();
            return;

            members = new List<User>();
            for (int i = 0; i < membersCount; i++)
            {
                var u = new User(WindowManager.GroupWindow.Dispatcher);
                reader.Skip(4);
                var nameOffset = reader.ReadUInt16();
                u.ServerId = reader.ReadUInt32();
                u.PlayerId = reader.ReadUInt32();
                u.Level = reader.ReadUInt32();
                u.UserClass = (Class)reader.ReadUInt32();
                u.Online = reader.ReadBoolean();
                u.EntityId = reader.ReadUInt64();
                u.Order = reader.ReadInt32();
                u.CanInvite = reader.ReadBoolean();
                u.Laurel = (Laurel)reader.ReadUInt32();
                u.Name = reader.ReadTeraString();
                members.Add(u);
            }

        }
    }
}
