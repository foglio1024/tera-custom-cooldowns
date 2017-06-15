using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public enum ChatChannel
    {
        Say = 0,
        Party = 1,
        Guild = 2,
        Area = 3,
        Trade = 4,
        Greet = 9,
        Private1 = 11,
        Private2 = 12,
        Private3 = 13,
        Private4 = 14,
        Private5 = 15,
        Private6 = 16,
        Private7 = 17,
        Private8 = 18,
        PartyNotice = 21,
        RaidNotice = 25,
        Emote = 26,
        Global = 27,
        Raid = 32,
        Megaphone = 213,
        GuildAdvertising = 214
    }
    public class S_CHAT : ParsedMessage
    {
        ushort authorNameOffset, messageOffset;
        uint ch;
        ulong authorId;
        byte unk1, gm, unk2;
        string authorName;
        string message;

        public ChatChannel Channel { get => (ChatChannel)ch; }
        public ulong AuthorId { get => authorId; }
        public string AuthorName { get => authorName; }
        public string Message { get => message; }

        public S_CHAT(TeraMessageReader reader) : base(reader)
        {
            authorNameOffset = reader.ReadUInt16();
            messageOffset = reader.ReadUInt16();
            ch = reader.ReadUInt32();
            authorId = reader.ReadUInt64();
            reader.Skip(3);
            authorName = reader.ReadTeraString();
            message = reader.ReadTeraString();
        }
    }
}
