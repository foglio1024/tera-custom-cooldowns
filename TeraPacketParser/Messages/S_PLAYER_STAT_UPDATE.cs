


namespace TeraPacketParser.Messages
{
    public class S_PLAYER_STAT_UPDATE : ParsedMessage
    {
        public int CurrentHP { get; }
        public int CurrentMP { get; }
        public int CurrentST { get; }
        public int MaxHP { get; }
        public int MaxMP { get; }
        public int BonusHP { get; }
        public int BonusMP { get; }
        public int MaxST { get; }
        public int BonusST { get; }
        public int Level { get; }
        public float Ilvl { get; }
        public int Edge { get; }
        public float BonusCritFactor { get; }
        public bool Status { get; set; }
        public bool Fire { get; set; }
        public bool Ice { get; set; }
        public bool Arcane { get; set; }
        public uint MaxCoins { get; set; }
        public uint Coins { get; set; }

        public S_PLAYER_STAT_UPDATE(TeraMessageReader reader) : base(reader)
        {
            if (reader.Factory.ReleaseVersion >= 8600 && reader.Factory.ReleaseVersion < 9901)
            {
                CurrentHP = reader.ReadInt32();
                reader.Skip(4);
                CurrentMP = reader.ReadInt32();
                reader.Skip(8);
                MaxHP = reader.ReadInt32();
                reader.Skip(4);
                MaxMP = reader.ReadInt32();
                reader.Skip(120);

                BonusCritFactor = reader.ReadSingle();
                reader.Skip(72);
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

                reader.Skip(28);
                Fire = reader.ReadUInt32() == 4;
                Ice = reader.ReadUInt32() == 4;
                Arcane = reader.ReadUInt32() == 4;
                Coins = reader.ReadUInt32();
                MaxCoins = reader.ReadUInt32();

            }
            else
            {
                CurrentHP = reader.ReadInt32();
                reader.Skip(4);
                CurrentMP = reader.ReadInt32();
                reader.Skip(8);

                MaxHP = reader.ReadInt32();
                reader.Skip(4);
                MaxMP = reader.ReadInt32();
                reader.Skip(88);
                BonusCritFactor = reader.ReadSingle();
                                                      
                reader.Skip(40);

                Level = reader.ReadInt16();
                reader.Skip(2);
                reader.Skip(2);

                Status = reader.ReadBoolean();
                BonusHP = reader.ReadInt32();
                BonusMP = reader.ReadInt32();
                reader.Skip(8);

                CurrentST = reader.ReadInt32();
                MaxST = reader.ReadInt32();
                BonusST = reader.ReadInt32();

                reader.Skip(8);
                Ilvl = reader.ReadInt32();
                Edge = reader.ReadInt32();
                reader.Skip(28);
                Fire = reader.ReadUInt32() == 4;
                Ice = reader.ReadUInt32() == 4;
                Arcane = reader.ReadUInt32() == 4;
                Coins = reader.ReadUInt32();
                MaxCoins = reader.ReadUInt32();
            }
        }

    }
}
