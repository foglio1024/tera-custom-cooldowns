


namespace TeraPacketParser.Messages
{
    public class S_PLAYER_STAT_UPDATE : ParsedMessage
    {
        public long CurrentHP { get; }
        public int CurrentMP { get; }
        public int CurrentST { get; }
        public long MaxHP { get; }
        public int MaxMP { get; }
        public int BonusHP { get; }
        public int BonusMP { get; }
        public int MaxST { get; }
        public int BonusST { get; }
        public int Level { get; }
        public float Ilvl { get; }
        public int Edge { get; }
        public float TotalCritFactor { get; }
        public bool Status { get; set; }
        public int TotalMagicalResistance { get; set; }
        public bool Fire { get; set; }
        public bool Ice { get; set; }
        public bool Arcane { get; set; }
        public uint MaxCoins { get; set; }
        public uint Coins { get; set; }

        public S_PLAYER_STAT_UPDATE(TeraMessageReader reader) : base(reader)
        {
            reader.BaseStream.Position = 0;
            CurrentHP = reader.ReadInt64();
            CurrentMP = reader.ReadInt32();
            reader.Skip(8);
            MaxHP = reader.ReadInt64();
            MaxMP = reader.ReadInt32();

            reader.Skip(2 * 4);

            var baseCrit = reader.ReadSingle();

            reader.Skip((14 * 4) + (3 * 2));

            var magRes = reader.ReadInt32();

            reader.Skip((12 * 4) + (3 * 2));

            TotalCritFactor = reader.ReadSingle() + baseCrit;

            reader.Skip(12 * 4);

            TotalMagicalResistance = magRes + reader.ReadInt32();

            reader.Skip(7 * 4);

            Level = reader.ReadInt16();

            reader.Skip(4);

            Status = reader.ReadBoolean();
            BonusHP = reader.ReadInt32();
            BonusMP = reader.ReadInt32();

            reader.Skip(8);

            CurrentST = reader.ReadInt32();
            MaxST = reader.ReadInt32();
            BonusST = reader.ReadInt32();

            reader.Skip(8);

            Ilvl = reader.ReadSingle();
            Edge = reader.ReadInt32();

            reader.Skip(reader.Factory.ReleaseVersion/100 >= 105 ? 32 : 28);

            Fire = reader.ReadUInt32() == 4;
            Ice = reader.ReadUInt32() == 4;
            Arcane = reader.ReadUInt32() == 4;
            Coins = reader.ReadUInt32();
            MaxCoins = reader.ReadUInt32();
        }

    }
}
