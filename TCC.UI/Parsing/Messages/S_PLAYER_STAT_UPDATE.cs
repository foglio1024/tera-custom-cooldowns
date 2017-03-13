using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Messages
{
    class S_PLAYER_STAT_UPDATE : ParsedMessage
    {
        public int currHp, currMp, unk1, maxHp, maxMp, basePower, baseEndu, baseImpactFactor, baseBalanceFactor;
        public short baseMovSpeed, unk2, baseAtkSpeed;
        public float baseCritRate, baseCritResist, baseCritPower;
        public int baseAttack, baseAttack2, baseDefense, baseImpact, baseBalance;
        public float baseResistWeak, baseResistPeriodic, baseResistStun;
        public int bonusPower, bonusEndu, bonusImpactFac, bonusBalanceFac;
        public short bonusMovSpeed, unk3, bonusAtkSpeed;
        public float bonusCritRate, bonusCritResist, bonusCritPower;
        public int bonusAttack, bonusAttack2, bonusDefense, bonusImpact, bonusBalance;
        public float bonusResistWeak, bonusResistPeriodic, bonusResistStun;
        public int level;
        public byte status;
        public short vitality;
        public int bonusHp, bonusMp, currStamina, maxStamina, currRe, maxRe, bonusRe, unk6, ilvlInven, ilvl, edge, unk8, unk9, unk10, unk11;

        public S_PLAYER_STAT_UPDATE(TeraMessageReader reader) : base(reader)
        {
            //199
            currHp = reader.ReadInt32();//195
            currMp = reader.ReadInt32();//191
            unk1 = reader.ReadInt32();//187
            maxHp = reader.ReadInt32();//183
            maxMp = reader.ReadInt32();//179
            basePower = reader.ReadInt32();//175
            baseEndu = reader.ReadInt32();//171
            baseImpactFactor = reader.ReadInt32();//167
            baseBalanceFactor = reader.ReadInt32();//163
            baseMovSpeed = reader.ReadInt16();//161
            unk2 = reader.ReadInt16();//159
            baseAtkSpeed = reader.ReadInt16();//157
            baseCritRate = reader.ReadSingle();//153
            baseCritResist = reader.ReadSingle();//149
            baseCritPower = reader.ReadSingle();//145
            baseAttack = reader.ReadInt32();//141
            baseAttack2 = reader.ReadInt32();//137
            baseDefense = reader.ReadInt32();//133
            baseImpact = reader.ReadInt32();//129
            baseBalance = reader.ReadInt32();//125
            baseResistWeak = reader.ReadSingle();//121
            baseResistPeriodic = reader.ReadSingle();//117
            baseResistStun = reader.ReadSingle();//113
            bonusPower = reader.ReadInt32();//109
            bonusEndu = reader.ReadInt32();//105
            bonusImpactFac = reader.ReadInt32();//101
            bonusBalanceFac = reader.ReadInt32();//97
            bonusMovSpeed = reader.ReadInt16();//95
            unk3 = reader.ReadInt16();//93
            bonusAtkSpeed = reader.ReadInt16();//91
            bonusCritRate = reader.ReadSingle();//87
            bonusCritResist = reader.ReadSingle();//83
            bonusCritPower = reader.ReadSingle();//79
            bonusAttack = reader.ReadInt32();//75
            bonusAttack2 = reader.ReadInt32();//71
            bonusDefense = reader.ReadInt32();//67
            bonusImpact = reader.ReadInt32();//64
            bonusBalance = reader.ReadInt32();//61
            bonusResistWeak = reader.ReadSingle();//57
            bonusResistPeriodic = reader.ReadSingle();//53
            bonusResistStun = reader.ReadSingle();//49
            level = reader.ReadInt32();//45
            vitality = reader.ReadInt16();//43
            status = reader.ReadByte();//42
            bonusHp = reader.ReadInt32();//38
            bonusMp = reader.ReadInt32();//34
            currStamina = reader.ReadInt32();//30
            maxStamina = reader.ReadInt32();//26
            currRe = reader.ReadInt32();//22
            maxRe = reader.ReadInt32();//18
            bonusRe = reader.ReadInt32();//14
            unk6 = reader.ReadInt32();//10
            ilvlInven = reader.ReadInt32();//6
            ilvl = reader.ReadInt32();//2
            edge = reader.ReadInt32();//0
            unk8 = reader.ReadInt32();//
            unk9 = reader.ReadInt32();//
            unk10 = reader.ReadInt32();//
            unk11 = reader.ReadInt32();//
            //reader.Skip(199);

            //edge = reader.ReadUInt16();
        }
    }
}
