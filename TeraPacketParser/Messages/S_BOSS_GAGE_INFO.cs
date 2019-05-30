namespace TeraPacketParser.Messages
{
    public class S_BOSS_GAGE_INFO : ParsedMessage
    {
        //private ulong id, targetId;
        //private int templateId, huntingZoneId;//, unk1;
        //private float /*hpDiff,*/ currHp, maxHp;
        //private byte enrage/*, unk3*/;

        public ulong EntityId { get; }
        public int TemplateId { get; }
        public int HuntingZoneId { get; }
        public float CurrentHP { get; }
        public float MaxHP { get; }
        public ulong Target { get; }
        public bool Enrage { get; }

        public S_BOSS_GAGE_INFO(TeraMessageReader reader) : base(reader)
        {
            EntityId = reader.ReadUInt64();
            HuntingZoneId = reader.ReadInt32();
            TemplateId = reader.ReadInt32();
            Target = reader.ReadUInt64();
            reader.Skip(4); //unk1 = reader.ReadInt32();
                            //if (reader.Version < 321550 || reader.Version > 321600)
                            //{
            Enrage = reader.ReadBoolean();
            CurrentHP = reader.ReadInt64();
            MaxHP = reader.ReadInt64();

            //}
            //else
            //{

            //    hpDiff = reader.ReadSingle();
            //    enrage = reader.ReadByte();
            //    currHp = reader.ReadSingle();
            //    maxHp = reader.ReadSingle();
            //}

            //unk3 = reader.ReadByte();
        }
    }
}