using TeraDataLite;

namespace TeraPacketParser.Messages
{
    public class S_UPDATE_NPCGUILD : ParsedMessage
    {
        internal S_UPDATE_NPCGUILD(TeraMessageReader reader) : base(reader)
        {
            User = reader.ReadEntityId();
            reader.Skip(8);
            var type = reader.ReadInt32();
            try { Guild = (NpcGuild)type; } catch { return; }
            reader.Skip(8);
            Credits = reader.ReadInt32();
        }

        public EntityId User { get; private set; }
        public int Credits { get; private set; }
        public NpcGuild Guild { get; private set; }
    }
}

