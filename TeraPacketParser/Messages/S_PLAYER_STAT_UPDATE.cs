namespace TeraPacketParser.Messages
{
    public enum UserStatus
    {
        Normal = 0,
        InCombat = 1,
        Campfire = 2,
        OnPegasus = 3
    }
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
        public UserStatus Status { get; set; }
        public int TotalMagicalResistance { get; set; }
        public bool Fire { get; set; }
        public bool Ice { get; set; }
        public bool Arcane { get; set; }
        public uint MaxCoins { get; set; }
        public uint Coins { get; set; }

        public S_PLAYER_STAT_UPDATE(TeraMessageReader reader) : base(reader)
        {
            CurrentHP = reader.ReadInt64();
            CurrentMP = reader.ReadInt32();
            reader.Skip(8); // drain
            MaxHP = reader.ReadInt64();
            MaxMP = reader.ReadInt32();

            reader.Skip(4); // power
            reader.Skip(4); // endurance

            var baseCrit = reader.ReadSingle();

            reader.Skip(4); // critRes
            reader.Skip(4); // critPower
            reader.Skip(4); // critPowerPhy
            reader.Skip(4); // critPowerMag

            reader.Skip(2); // atkSpd
            reader.Skip(2); // runSpd
            reader.Skip(2); // wlkSpd

            reader.Skip(4); // impactFac
            reader.Skip(4); // balanceFac
            reader.Skip(4); // atkMin
            reader.Skip(4); // atkMax
            reader.Skip(4); // atkPhyMin
            reader.Skip(4); // atkPhyMax
            reader.Skip(4); // atkMagMin
            reader.Skip(4); // atkMagMax

            reader.Skip(4); // def
            reader.Skip(4); // defPhy
            var defMag = reader.ReadInt32();

            reader.Skip(4); // impact
            reader.Skip(4); // balance

            reader.Skip(4); // resWeak
            reader.Skip(4); // resPeriodic
            reader.Skip(4); // resStun

            reader.Skip(4); // powerBonus
            reader.Skip(4); // enduranceBonus
            reader.Skip(4); // impactFacBonus
            reader.Skip(4); // balanceFacBonus

            reader.Skip(2); // runSpdBonus
            reader.Skip(2); // wlkSpdBonus
            reader.Skip(2); // atkSpdBonus

            var critRateBonus = reader.ReadSingle();
            TotalCritFactor = critRateBonus + baseCrit;

            reader.Skip(4); // critResBonus
            reader.Skip(4); // critPowBonus
            reader.Skip(4); // critPowPhyBonus
            reader.Skip(4); // critPowMagBonus

            reader.Skip(4); // atkMinBonus
            reader.Skip(4); // atkMaxBonus
            reader.Skip(4); // defBonus

            reader.Skip(4); // atkPhyMinBonus
            reader.Skip(4); // atkPhyMaxBonus
            reader.Skip(4); // defPhyBonus

            reader.Skip(4); // atkMagMinBonus
            reader.Skip(4); // atkMagMaxBonus
            var defMagBonus = reader.ReadInt32();
            TotalMagicalResistance = defMag + defMagBonus;

            reader.Skip(4); // impactBonus
            reader.Skip(4); // balanceBonus

            reader.Skip(4); // resWeakBonus
            reader.Skip(4); // resPeriodicBonus
            reader.Skip(4); // resStunBonus

            Level = reader.ReadInt16();
            Status = (UserStatus)reader.ReadInt16();

            reader.Skip(2); // conditionLevel
            reader.Skip(1); // alive

            BonusHP = reader.ReadInt32();
            BonusMP = reader.ReadInt32();

            reader.Skip(4); // condition
            reader.Skip(4); // conditionMax

            CurrentST = reader.ReadInt32();
            MaxST = reader.ReadInt32();
            BonusST = reader.ReadInt32();

            reader.Skip(4); // infamy

            reader.Skip(4); // ilvlInventory
            Ilvl = reader.ReadSingle();

            Edge = reader.ReadInt32();
            reader.Skip(4); // edgePerc
            reader.Skip(4); // edgeTimeRemaining
            reader.Skip(4); // edgeMin

            reader.Skip(4); // trueLevel

            reader.Skip(4); // flightEnergy
            reader.Skip(4); // flightId
            reader.Skip(4); // flightSpeedMul

            Fire = reader.ReadUInt32() == 4;
            Ice = reader.ReadUInt32() == 4;
            Arcane = reader.ReadUInt32() == 4;

            Coins = reader.ReadUInt32();
            MaxCoins = reader.ReadUInt32();
        }

    }
}
