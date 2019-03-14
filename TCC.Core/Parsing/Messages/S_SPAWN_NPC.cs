using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_SPAWN_NPC : ParsedMessage
    {
        public ulong EntityId { get; }
        public uint TemplateId { get; }
        public ushort HuntingZoneId { get; }
        public bool Villager { get; }
        public int RemainingEnrageTime { get; }

        public S_SPAWN_NPC(TeraMessageReader reader) : base(reader)
        {
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
            EntityId = reader.ReadUInt64();
            reader.Skip(8); //var target = reader.ReadUInt64();
            reader.Skip(12); //var loc = reader.ReadVector3f();
            reader.Skip(2); //var angle = reader.ReadInt16();
            reader.Skip(4); //var relation = reader.ReadInt32();
            TemplateId = reader.ReadUInt32();
            HuntingZoneId = reader.ReadUInt16();
            reader.Skip(4  // shapeID
                      + 2  // walkSpeed
                      );
            reader.Skip(2); //var enrage = reader.ReadUInt16(); // 0/1
            if (reader.Factory.ReleaseVersion / 100 >= 79)
            {
                RemainingEnrageTime = reader.ReadInt32();
            }
            reader.Skip(2  // hpLevel  
                      + 2  // questInfo
                      + 1);  // visible
            Villager = reader.ReadBoolean();
            reader.Skip(4);
            //reader.Skip(4+8+4+4);
            //var aggressive = reader.ReadBoolean();

            //Console.WriteLine("[S_SPAWN NPC] id:{0} tId:{1} hzId:{2}", id, templateId, huntingZoneId);
        }
    }
}
