using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_PLAYER_STAT_UPDATE : ParsedMessage
    {
        int currHp, currMp, unk1, maxHp, maxMp, basePower, baseEndu, baseImpactFactor, baseBalanceFactor;
        short baseMovSpeed, unk2, baseAtkSpeed;
        float baseCritRate, baseCritResist, baseCritPower;
        int baseAttack, baseAttack2, baseDefense, baseImpact, baseBalance;
        float baseResistWeak, baseResistPeriodic, baseResistStun;
        int bonusPower, bonusEndu, bonusImpactFac, bonusBalanceFac;
        short bonusMovSpeed, unk3, bonusAtkSpeed;
        float bonusCritRate, bonusCritResist, bonusCritPower;
        int bonusAttack, bonusAttack2, bonusDefense, bonusImpact, bonusBalance;
        float bonusResistWeak, bonusResistPeriodic, bonusResistStun;
        int level;
        byte status;
        short vitality;
        int bonusHp, bonusMp, currStamina, maxStamina, currRe, maxRe, bonusRe, unk6, ilvlInven, ilvl, edge, unk8, unk9, unk10, unk11;

        public int CurrentHP { get => currHp; }
        public int CurrentMP { get => currMp; }
        public int CurrentST { get => currRe; }
        public int MaxHP { get => maxHp; }
        public int MaxMP { get => maxMp; }
        public int MaxST { get => maxRe; }
        public int BonusHP { get => bonusHp; }
        public int BonusMP { get => bonusMp; }
        public int BonusST { get => bonusRe; }
        public int Level { get => level; }
        public int Ilvl { get => ilvl; }
        public int Edge { get => edge; }

        public S_PLAYER_STAT_UPDATE(TeraMessageReader reader) : base(reader)
        {
            currHp = reader.ReadInt32();
            currMp = reader.ReadInt32();

            //unk1 = reader.ReadInt32();
            reader.Skip(4);

            maxHp = reader.ReadInt32();
            maxMp = reader.ReadInt32();

            //basePower = reader.ReadInt32();4
            //baseEndu = reader.ReadInt32();8
            //baseImpactFactor = reader.ReadInt32();12
            //baseBalanceFactor = reader.ReadInt32();16
            //baseMovSpeed = reader.ReadInt16();18
            //unk2 = reader.ReadInt16();20
            //baseAtkSpeed = reader.ReadInt16();22
            //baseCritRate = reader.ReadSingle();26
            //baseCritResist = reader.ReadSingle();30
            //baseCritPower = reader.ReadSingle();34
            //baseAttack = reader.ReadInt32();38
            //baseAttack2 = reader.ReadInt32();42
            //baseDefense = reader.ReadInt32();46
            //baseImpact = reader.ReadInt32();50
            //baseBalance = reader.ReadInt32();54
            //baseResistWeak = reader.ReadSingle();58
            //baseResistPeriodic = reader.ReadSingle();62
            //baseResistStun = reader.ReadSingle();66
            //bonusPower = reader.ReadInt32();70
            //bonusEndu = reader.ReadInt32();74
            //bonusImpactFac = reader.ReadInt32();78
            //bonusBalanceFac = reader.ReadInt32();82
            //bonusMovSpeed = reader.ReadInt16();84
            //unk3 = reader.ReadInt16();86
            //bonusAtkSpeed = reader.ReadInt16();88
            //bonusCritRate = reader.ReadSingle();92
            //bonusCritResist = reader.ReadSingle();96
            //bonusCritPower = reader.ReadSingle();100
            //bonusAttack = reader.ReadInt32();104
            //bonusAttack2 = reader.ReadInt32();108
            //bonusDefense = reader.ReadInt32();112
            //bonusImpact = reader.ReadInt32();116
            //bonusBalance = reader.ReadInt32();120
            //bonusResistWeak = reader.ReadSingle();124
            //bonusResistPeriodic = reader.ReadSingle();128
            //bonusResistStun = reader.ReadSingle();132
            reader.Skip(132);

            level = reader.ReadInt32();

            //vitality = reader.ReadInt16();
            reader.Skip(2);

            status = reader.ReadByte();
            bonusHp = reader.ReadInt32();
            bonusMp = reader.ReadInt32();

            //currStamina = reader.ReadInt32();
            //maxStamina = reader.ReadInt32();
            reader.Skip(8);

            currRe = reader.ReadInt32();
            maxRe = reader.ReadInt32();
            bonusRe = reader.ReadInt32();

            //unk6 = reader.ReadInt32();
            reader.Skip(4);

            ilvlInven = reader.ReadInt32();
            ilvl = reader.ReadInt32();
            edge = reader.ReadInt32();

            //unk8 = reader.ReadInt32();
            //unk9 = reader.ReadInt32();
            //unk10 = reader.ReadInt32();
            //unk11 = reader.ReadInt32();
            reader.Skip(16);
        }
    }
}
