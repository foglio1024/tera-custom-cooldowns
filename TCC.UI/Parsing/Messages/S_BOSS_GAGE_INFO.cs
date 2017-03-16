using TCC.Data;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Messages
{
    public class S_BOSS_GAGE_INFO : ParsedMessage
    {
        ulong id, targetId;
        int type, npc, unk1;
        float hpDiff, currHp, maxHp;
        byte enrage, unk3;

        public ulong EntityId { get => id; } 
        public int Type { get => type; }
        public int Npc { get => npc; }
        public float CurrentHP { get => currHp; }
        public float MaxHP { get => maxHp; }

        public S_BOSS_GAGE_INFO(TeraMessageReader reader) : base(reader)
        {
            id = reader.ReadUInt64();
            type = reader.ReadInt32();
            npc = reader.ReadInt32();
            targetId = reader.ReadUInt64();
            reader.Skip(4); //unk1 = reader.ReadInt32();
            hpDiff = reader.ReadSingle();
            enrage = reader.ReadByte();
            currHp = reader.ReadSingle();
            maxHp = reader.ReadSingle();

            //System.Console.WriteLine("[S_BOSS_GAGE_INFO] id:{0} name:{1}", EntityId, MonsterDatabase.GetName((uint)npc, (uint)type));
            //unk3 = reader.ReadByte();
        }
    }
}