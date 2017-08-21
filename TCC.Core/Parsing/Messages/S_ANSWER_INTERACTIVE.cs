using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_ANSWER_INTERACTIVE : ParsedMessage
    {
        public string Name { get; private set; }
        public bool HasGuild { get; private set; }
        public bool HasParty { get; private set; }
        public uint Level { get; private set; }
        public uint Model { get; private set; }

        public S_ANSWER_INTERACTIVE(TeraMessageReader reader) : base(reader)
        {
            var nameOffset = reader.ReadUInt16();
            reader.Skip(4);
            Model = reader.ReadUInt32();
            Level = reader.ReadUInt32();
            HasParty = reader.ReadBoolean();
            HasGuild = reader.ReadBoolean();
            reader.Skip(4); //server ID
            Name = reader.ReadTeraString();
        }
    }
}
