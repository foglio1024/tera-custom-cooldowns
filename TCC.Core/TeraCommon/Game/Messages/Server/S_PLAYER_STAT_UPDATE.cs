namespace Tera.Game.Messages
{
    public class S_PLAYER_STAT_UPDATE : ParsedMessage
    {
        internal S_PLAYER_STAT_UPDATE(TeraMessageReader reader) : base(reader)
        {
            HpRemaining = reader.ReadInt32();
            MpRemaining = reader.ReadInt32();
            reader.Skip(4);
            TotalHp = reader.ReadInt32();
            TotalMp = reader.ReadInt32();
            return; //we don't need all other things now, but if we need - just remove return.
            BasePower = reader.ReadInt32();
            BaseEndurance = reader.ReadInt32();
            BaseImpactFactor = reader.ReadInt32();
            BaseBalanceFactor = reader.ReadInt32();
            BaseMovementSpeed = reader.ReadInt16();
            reader.Skip(2);
            BaseAttackSpeed = reader.ReadInt16();
            BaseCritRate = reader.ReadSingle();
            BaseCritResist = reader.ReadSingle();
            BaseCritPower = reader.ReadSingle();
            BaseAttack = reader.ReadInt32();
            BaseAttack2 = reader.ReadInt32();
            BaseDefence = reader.ReadInt32();
            BaseImpcat = reader.ReadInt32();
            BaseBalance = reader.ReadInt32();
            BaseResistWeakening = reader.ReadSingle();
            BaseResistPeriodic = reader.ReadSingle();
            BaseResistStun = reader.ReadSingle();
            BonusPower = reader.ReadInt32();
            BonusEndurance = reader.ReadInt32();
            BonusImpactFactor = reader.ReadInt32();
            BonusBalanceFactor = reader.ReadInt32();
            BonusMovementSpeed = reader.ReadInt16();
            reader.Skip(2);
            BonusAttackSpeed = reader.ReadInt16();
            BonusCritRate = reader.ReadSingle();
            BonusCritResist = reader.ReadSingle();
            BonusCritPower = reader.ReadSingle();
            BonusAttack = reader.ReadInt32();
            BonusAttack2 = reader.ReadInt32();
            BonusDefence = reader.ReadInt32();
            BonusImpcat = reader.ReadInt32();
            BonusBalance = reader.ReadInt32();
            BonusResistWeakening = reader.ReadSingle();
            BonusResistPeriodic = reader.ReadSingle();
            BonusResistStun = reader.ReadSingle();
            Level = reader.ReadInt32();
            Vitality = reader.ReadInt16();
            Status = reader.ReadByte();
            BonusHp = reader.ReadInt32();
            BonusMp = reader.ReadInt32();
            Stamina = reader.ReadInt32();
            TotalStamina = reader.ReadInt32();
            ReRemaining = reader.ReadInt32();
            TotalRe = reader.ReadInt32();
            reader.Skip(8);
            ItemLevelInventory = reader.ReadInt32();
            ItemLevel = reader.ReadInt32();

            // Something else unknown later
        }

        public bool Slaying => TotalHp > HpRemaining*2 && HpRemaining > 0;
        public int BaseAttack { get; private set; }
        public int BaseAttack2 { get; private set; }
        public short BaseAttackSpeed { get; private set; }
        public int BaseBalance { get; private set; }
        public int BaseBalanceFactor { get; private set; }
        public float BaseCritPower { get; private set; }
        public float BaseCritRate { get; private set; }
        public float BaseCritResist { get; private set; }
        public int BaseDefence { get; private set; }
        public int BaseEndurance { get; private set; }
        public int BaseImpactFactor { get; private set; }
        public int BaseImpcat { get; private set; }
        public short BaseMovementSpeed { get; private set; }
        public int BasePower { get; private set; }
        public float BaseResistPeriodic { get; private set; }
        public float BaseResistStun { get; private set; }
        public float BaseResistWeakening { get; private set; }
        public int BonusAttack { get; private set; }
        public int BonusAttack2 { get; private set; }
        public short BonusAttackSpeed { get; private set; }
        public int BonusBalance { get; private set; }
        public int BonusBalanceFactor { get; private set; }
        public float BonusCritPower { get; private set; }
        public float BonusCritRate { get; private set; }
        public float BonusCritResist { get; private set; }
        public int BonusDefence { get; private set; }
        public int BonusEndurance { get; private set; }
        public int BonusHp { get; private set; }
        public int BonusImpactFactor { get; private set; }
        public int BonusImpcat { get; private set; }
        public short BonusMovementSpeed { get; private set; }
        public int BonusMp { get; private set; }
        public int BonusPower { get; private set; }
        public float BonusResistPeriodic { get; private set; }
        public float BonusResistStun { get; private set; }
        public float BonusResistWeakening { get; private set; }
        public int HpRemaining { get; }
        public int ItemLevel { get; private set; }
        public int ItemLevelInventory { get; private set; }
        public int Level { get; private set; }
        public int MpRemaining { get; private set; }
        public int ReRemaining { get; private set; }
        public int Stamina { get; private set; }
        public byte Status { get; private set; }
        public int TotalHp { get; }
        public int TotalMp { get; private set; }
        public int TotalRe { get; private set; }
        public int TotalStamina { get; private set; }
        public int Vitality { get; private set; }
    }
}