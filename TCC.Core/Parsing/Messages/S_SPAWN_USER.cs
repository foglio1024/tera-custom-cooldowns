using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_SPAWN_USER : ParsedMessage
    {
        private ulong entityId;
        public ulong EntityId
        {
            get { return entityId; }
            set { entityId = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public S_SPAWN_USER(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(8);
            var nameOffset = reader.ReadUInt16() - 4;
            reader.Skip(22);
            entityId = reader.ReadUInt64();
            reader.BaseStream.Position = nameOffset;
            name = reader.ReadTeraString();

            //System.Console.WriteLine("[S_SPAWN_USER] {0} {1}", EntityId, Name);
        }
    }
}