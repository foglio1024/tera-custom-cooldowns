using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_FRIEND_LIST : ParsedMessage
    {
        public List<string> Friends;
        public S_FRIEND_LIST(TeraMessageReader reader) : base(reader)
        {
            Friends = new List<string>();
            var count = reader.ReadUInt16();
            reader.Skip(4);
            reader.ReadTeraString();
            for (int i = 0; i < count; i++)
            {
                try
                {
                    Friends.Add(ParseFriend(reader));

                }
                catch (Exception)
                {

                }
            }
        }

        private string ParseFriend(TeraMessageReader reader)
        {
            reader.Skip(4);
            var nameOffset = reader.ReadUInt16();
            reader.BaseStream.Position = nameOffset - 4;
            var name = reader.ReadTeraString();
            reader.ReadTeraString();
            reader.ReadTeraString();
            return name;
        }
    }
}
