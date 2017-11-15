using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_BOSS_GAGE_INFO : ParsedMessage
    {
        ulong id, targetId;
        int templateId, huntingZoneId, unk1;
        float hpDiff, currHp, maxHp;
        byte enrage, unk3;

        public ulong EntityId { get => id; } 
        public int TemplateId { get => templateId; }
        public int HuntingZoneId { get => huntingZoneId; }
        public float CurrentHP { get => currHp; }
        public float MaxHP { get => maxHp; }
        public ulong Target { get => targetId; }

        public S_BOSS_GAGE_INFO(TeraMessageReader reader) : base(reader)
        {
            id = reader.ReadUInt64();
            huntingZoneId = reader.ReadInt32();
            templateId = reader.ReadInt32();
            targetId = reader.ReadUInt64();
            reader.Skip(4); //unk1 = reader.ReadInt32();
            if (reader.Version < 321550 || reader.Version > 321600)
            {
                enrage = reader.ReadByte();
                currHp = reader.ReadInt64();
                maxHp = reader.ReadInt64();

            }
            else
            {

                hpDiff = reader.ReadSingle();
                enrage = reader.ReadByte();
                currHp = reader.ReadSingle();
                maxHp = reader.ReadSingle();
            }

            //System.Console.WriteLine("[S_BOSS_GAGE_INFO] id:{0} name:{1}", EntityId, MonsterDatabase.GetName((uint)npc, (uint)type));
            //unk3 = reader.ReadByte();
        }
    }
}