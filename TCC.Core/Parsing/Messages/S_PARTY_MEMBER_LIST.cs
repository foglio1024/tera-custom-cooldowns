using System.Collections.Generic;
using TCC.Data;
using TCC.Data.Pc;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MEMBER_LIST : ParsedMessage
    {
        private bool _im, _raid;
        private uint _leaderServerId;
        private uint _leaderPlayerId;

        public bool Im => _im;
        public bool Raid => _raid;
        public uint LeaderServerId => _leaderServerId;
        public uint LeaderPlayerId => _leaderPlayerId;

        public List<User> Members { get; }

        public S_PARTY_MEMBER_LIST(TeraMessageReader reader) : base(reader)
        {
            var count = reader.ReadUInt16();
            var offset = reader.ReadUInt16();

            _im = reader.ReadBoolean();
            _raid = reader.ReadBoolean();

            reader.Skip(12);
            //unk1 = reader.ReadUInt32();
            //unk2 = reader.ReadUInt32();
            //unk3 = reader.ReadUInt16();
            //unk4 = reader.ReadUInt16();

            _leaderServerId = reader.ReadUInt32();
            _leaderPlayerId = reader.ReadUInt32();

            reader.Skip(19);
            //unk5 = reader.ReadUInt32();
            //unk6 = reader.ReadUInt32();
            //unk7 = reader.ReadByte();
            //unk8 = reader.ReadUInt32();
            //unk9 = reader.ReadByte();
            //unk10 = reader.ReadUInt32();
            //unk11 = reader.ReadByte();

            Members = new List<User>();

            for (var i = 0; i < count; i++)
            {
                var u = new User(WindowManager.GroupWindow.Dispatcher);

                reader.BaseStream.Position = offset - 4;
                reader.Skip(2); // var pointer = reader.ReadUInt16();
                var nextOffset = reader.ReadUInt16();
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
                u.Awakened = reader.ReadInt32() > 0;
                
                reader.BaseStream.Position = nameOffset - 4;
                u.Name = reader.ReadTeraString();
                u.Alive = true;
                u.IsLeader = u.ServerId == LeaderServerId && u.PlayerId == LeaderPlayerId;
                Members.Add(u);
                offset = nextOffset;
            }
        }
    }
}
