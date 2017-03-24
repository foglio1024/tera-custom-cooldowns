using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing
{
    public class S_NPC_STATUS : ParsedMessage
    {
        ulong entityId, targetId;
        byte enraged;

        public ulong EntityId { get => entityId; }
        public bool IsEnraged
        {
            get
            {
                if (enraged == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public S_NPC_STATUS(TeraMessageReader reader) : base(reader)
        {
            entityId = reader.ReadUInt64();
            enraged = reader.ReadByte();
        }
    }
}