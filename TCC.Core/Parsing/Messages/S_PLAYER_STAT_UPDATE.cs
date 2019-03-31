using System.Windows.Forms;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
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
        public int Ilvl { get; }
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
            CurrentHP = reader.ReadInt32();
            /*if (reader.Version < 321550 || reader.Version > 321600)*/
            reader.Skip(4);
            CurrentMP = reader.ReadInt32();
            /*if (reader.Version < 321550 || reader.Version > 321600)*/
            reader.Skip(4);
            //unk1 = reader.ReadInt32();
            reader.Skip(4);

            MaxHP = reader.ReadInt32();
            /*if (reader.Version < 321550 || reader.Version > 321600)*/
            reader.Skip(4);
            MaxMP = reader.ReadInt32();
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
            reader.Skip(88);
            BonusCritFactor = reader.ReadSingle();//92
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
            reader.Skip(128 - 88);

            Level = reader.ReadInt16();
            reader.Skip(2);
            //vitality = reader.ReadInt16();
            reader.Skip(2);

            Status = reader.ReadBoolean();
            BonusHP = reader.ReadInt32();
            BonusMP = reader.ReadInt32();

            //currStamina = reader.ReadInt32();
            //maxStamina = reader.ReadInt32();
            reader.Skip(8);

            CurrentST = reader.ReadInt32();
            MaxST = reader.ReadInt32();
            BonusST = reader.ReadInt32();

            //unk6 = reader.ReadInt32();
            reader.Skip(4);

            //ilvlInven = reader.ReadInt32();
            reader.Skip(4);
            Ilvl = reader.ReadInt32();
            Edge = reader.ReadInt32();

            //unk8 = reader.ReadInt32();
            //unk9 = reader.ReadInt32();
            //unk10 = reader.ReadInt32();
            //unk11 = reader.ReadInt32();
            reader.Skip(2+2+4+4+4+4+4+4);
            if (reader.Factory.ReleaseVersion/100 < 75) return;
            Fire = reader.ReadUInt32() == 4;
            Ice = reader.ReadUInt32() == 4;
            Arcane = reader.ReadUInt32() == 4;
            if (reader.Factory.ReleaseVersion < 8000) return;
            Coins = reader.ReadUInt32();
            MaxCoins = reader.ReadUInt32();
            //Log.CW($"F/I/A {fire}/{ice}/{arcane}");
        }

    }
}
