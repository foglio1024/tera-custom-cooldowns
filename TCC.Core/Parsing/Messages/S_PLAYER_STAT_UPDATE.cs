using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            //basePower = reader.ReadInt32();
            //baseEndu = reader.ReadInt32();
            //baseImpactFactor = reader.ReadInt32();
            //baseBalanceFactor = reader.ReadInt32();
            //baseMovSpeed = reader.ReadInt16();
            //unk2 = reader.ReadInt16();
            //baseAtkSpeed = reader.ReadInt16();
            //baseCritRate = reader.ReadSingle();
            //baseCritResist = reader.ReadSingle();
            //baseCritPower = reader.ReadSingle();
            //baseAttack = reader.ReadInt32();
            //baseAttack2 = reader.ReadInt32();
            //baseDefense = reader.ReadInt32();
            //baseImpact = reader.ReadInt32();
            //baseBalance = reader.ReadInt32();
            //baseResistWeak = reader.ReadSingle();
            //baseResistPeriodic = reader.ReadSingle();
            //baseResistStun = reader.ReadSingle();
            //bonusPower = reader.ReadInt32();
            //bonusEndu = reader.ReadInt32();
            //bonusImpactFac = reader.ReadInt32();
            //bonusBalanceFac = reader.ReadInt32();
            //bonusMovSpeed = reader.ReadInt16();
            //unk3 = reader.ReadInt16();
            //bonusAtkSpeed = reader.ReadInt16();
            //bonusCritRate = reader.ReadSingle();
            //bonusCritResist = reader.ReadSingle();
            //bonusCritPower = reader.ReadSingle();
            //bonusAttack = reader.ReadInt32();
            //bonusAttack2 = reader.ReadInt32();
            //bonusDefense = reader.ReadInt32();
            //bonusImpact = reader.ReadInt32();
            //bonusBalance = reader.ReadInt32();
            //bonusResistWeak = reader.ReadSingle();
            //bonusResistPeriodic = reader.ReadSingle();
            //bonusResistStun = reader.ReadSingle();
            reader.Skip(128);

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
