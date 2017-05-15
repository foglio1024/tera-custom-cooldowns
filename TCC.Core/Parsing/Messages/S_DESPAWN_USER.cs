using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_DESPAWN_USER : ParsedMessage
    {
        private ulong entityId;
        public ulong EntityId
        {
            get { return entityId; }
            set { entityId = value; }
        }

        public S_DESPAWN_USER(TeraMessageReader reader) : base(reader)
        {
            entityId = reader.ReadUInt64();
        }
    }
}