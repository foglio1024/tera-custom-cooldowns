using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Data;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MEMBER_LIST : ParsedMessage
    {
        bool im, raid;
        uint unk1, unk2;
        ushort unk3, unk4;
        uint leaderServerId;
        uint leaderPlayerId;
        uint unk5, unk6;
        byte unk7;
        uint unk8;
        byte unk9;
        uint unk10;
        byte unk11;

        private List<User> members;

        public bool Im { get { return im; } }
        public bool Raid { get { return raid; } }
        public uint LeaderServerId { get { return leaderServerId; } }
        public uint LeaderPlayerId { get { return leaderPlayerId; } }

        public List<User> Members
        {
            get { return members; }
        }

        public S_PARTY_MEMBER_LIST(TeraMessageReader reader) : base(reader)
        {
            var count = reader.ReadUInt16();
            var offset = reader.ReadUInt16();

            im = reader.ReadBoolean();
            raid = reader.ReadBoolean();

            reader.Skip(12);
            //unk1 = reader.ReadUInt32();
            //unk2 = reader.ReadUInt32();
            //unk3 = reader.ReadUInt16();
            //unk4 = reader.ReadUInt16();

            leaderServerId = reader.ReadUInt32();
            leaderPlayerId = reader.ReadUInt32();

            reader.Skip(19);
            //unk5 = reader.ReadUInt32();
            //unk6 = reader.ReadUInt32();
            //unk7 = reader.ReadByte();
            //unk8 = reader.ReadUInt32();
            //unk9 = reader.ReadByte();
            //unk10 = reader.ReadUInt32();
            //unk11 = reader.ReadByte();

            members = new List<User>();

            for (int i = 0; i < count; i++)
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
                u.Alive = true;
                if (u.ServerId == LeaderServerId && u.PlayerId == LeaderPlayerId) u.IsLeader = true;
                members.Add(u);
            }
        }
    }
}
