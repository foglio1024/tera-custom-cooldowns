using System.Collections.Generic;
using TeraDataLite;

namespace TeraPacketParser.Messages
{
    public class S_FRIEND_LIST : ParsedMessage
    {
        public List<FriendData> Friends;
        public S_FRIEND_LIST(TeraMessageReader reader) : base(reader)
        {
            Friends = new List<FriendData>();
            var count = reader.ReadUInt16();
            reader.Skip(4);
            reader.ReadTeraString();
            for (var i = 0; i < count; i++)
            {
                try
                {
                    Friends.Add(ParseFriend(reader));

                }
                catch
                {
                    // ignored
                }
            }
        }

        private FriendData ParseFriend(TeraMessageReader reader)
        {
            reader.Skip(4);
            var nameOffset = reader.ReadUInt16();
            reader.Skip(4);
            var id = reader.ReadUInt32();
            reader.BaseStream.Position = nameOffset - 4;
            var name = reader.ReadTeraString();
            reader.ReadTeraString();
            reader.ReadTeraString();
            return new FriendData(id, name);
        }
    }
}
