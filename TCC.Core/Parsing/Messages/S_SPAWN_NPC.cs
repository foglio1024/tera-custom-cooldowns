using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_SPAWN_NPC : ParsedMessage
    {
        private ulong id;
        private uint templateId;
        private ushort huntingZoneId;

        public ulong EntityId => id;
        public uint TemplateId => templateId;
        public ushort HuntingZoneId => huntingZoneId;

        public S_SPAWN_NPC(TeraMessageReader reader) : base(reader)
        {
            reader.BaseStream.Position = 0;
            //reader.Skip(10);
            //id = reader.ReadUInt64();
            //var target = reader.ReadUInt64();
            //var pos = reader.ReadVector3f();
            //var angle = reader.ReadUInt16();
            //var relation = reader.ReadUInt32();
            //templateId = reader.ReadUInt32();
            //huntingZoneId = reader.ReadUInt16();
            //var unk1 = reader.ReadInt32();
            //var walkSpeed = reader.ReadInt16();
            //var runSpeed = reader.ReadInt16();
            //var unk5 = reader.ReadInt16();
            //var unk6 = reader.ReadBoolean();
            //var unk7 = reader.ReadInt32();
            //var visible = reader.ReadBoolean();
            //var villager = reader.ReadBoolean();
            //var spawnType = reader.ReadUInt32();
            //var unk11 = reader.ReadUInt64();
            //var unk12 = reader.ReadUInt64();
            //var unk13 = reader.ReadUInt16();
            //var unk14 = reader.ReadUInt32();
            //var unk15 = reader.ReadBoolean();
            //var owner = reader.ReadUInt64();
            //var unk16 = reader.ReadUInt32();
            //var unk17 = reader.ReadUInt32();
            //var unk18 = reader.ReadUInt64();
            //var unk19 = reader.ReadByte();
            //var unk20 = reader.ReadUInt32();
            //var unk25 = reader.ReadUInt32();

            reader.Skip(10);
            id = reader.ReadUInt64();
            reader.Skip(26);
            templateId = reader.ReadUInt32();
            huntingZoneId = reader.ReadUInt16();

            //Console.WriteLine("[S_SPAWN NPC] id:{0} tId:{1} hzId:{2}", id, templateId, huntingZoneId);
        }
    }
}
