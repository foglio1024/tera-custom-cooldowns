using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_UPDATE_FRIEND_INFO : ParsedMessage
    {
        public bool Online { get; private set; }
        public string Name { get; private set; }
        public S_UPDATE_FRIEND_INFO(TeraMessageReader reader) : base(reader)
        {
            reader.BaseStream.Position = 0;
            var count = reader.ReadUInt16();
            if (count != 1) return;
            var friendsOffset = reader.ReadUInt16();
            reader.BaseStream.Position = friendsOffset;
            var nameOffset = reader.ReadUInt16();
            reader.Skip(4 + 4 + 4 + 4 + 4);
            var status = reader.ReadInt32();
            if(status == 0)
            {
                Online = true;
            }
            reader.BaseStream.Position = nameOffset - 4;
            Name = reader.ReadTeraString();

        }
    }
}
