using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_SPAWN_NPC : ParsedMessage
    {

        ulong id;
        uint templateId;
        ushort huntingZoneId;

        public ulong EntityId { get => id;}
        public uint TemplateId { get => templateId;  }
        public ushort HuntingZoneId { get => huntingZoneId; }

        public S_SPAWN_NPC(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(10);
            id = reader.ReadUInt64();
            reader.Skip(26);
            templateId = reader.ReadUInt32();
            huntingZoneId = reader.ReadUInt16();

           //Console.WriteLine("[S_SPAWN NPC] id:{0} tId:{1} hzId:{2}", id, templateId, huntingZoneId);
        }
    }
}
