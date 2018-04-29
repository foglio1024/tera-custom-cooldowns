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
    public class S_SHOW_PARTY_MATCH_INFO : ParsedMessage
    {
        public static List<Listing> Listings { get; } = new List<Listing>();
        public bool IsLast { get; }
        public S_SHOW_PARTY_MATCH_INFO(TeraMessageReader reader) : base(reader)
        {
            var count = reader.ReadUInt16();
            var offset = reader.ReadUInt16();
            var page = reader.ReadInt16();
            var pages = reader.ReadInt16();

            if (page == 0) Listings.Clear();
            if (count == 0)
            {
                Listings.Clear();
                return;
            }

            reader.BaseStream.Position = offset - 4;
            for (int i = 0; i < count; i++)
            {
                var l = new Listing();
                var curr = reader.ReadUInt16();
                var next = reader.ReadUInt16();
                var msgOffset = reader.ReadUInt16();
                var leaderNameOffset = reader.ReadUInt16();
                var leaderId = reader.ReadUInt32();
                var isRaid = reader.ReadBoolean();
                var playerCount = reader.ReadInt16();

                reader.BaseStream.Position = msgOffset - 4;
                var msg = reader.ReadTeraString();
                reader.BaseStream.Position = leaderNameOffset - 4;
                var leaderName = reader.ReadTeraString();
                l.LeaderName = leaderName;
                l.LeaderId = leaderId;
                l.IsRaid = isRaid;
                l.Message = msg;
                l.PlayerCount = playerCount;
                Listings.Add(l);
                if(next != 0) reader.BaseStream.Position = next - 4;

            }

            if (page < pages) Proxy.RequestNextLfgPage(page + 1);
            if (page == pages) IsLast = true;
        }
    }
}
