using System;
using System.Collections.Generic;
using System.Linq;

namespace Tera.Game
{
    public class HotDot : IEquatable<object>
    {
        public enum DotType
        {
            swch = 0, // switch on for noctineum ? other strange uses.
            seta = 1, // ?set abs stat value
            abs = 2, // each tick  HP +=HPChange ; MP += MPChange
            perc = 3, // each tick  HP += MaxHP*HPChange; MP += MaxMP*MPChange
            setp = 4 // ?set % stat value
        }

        public enum Types
        {
            Unknown = 0,
            MaxHP = 1,
            Power = 3,
            Endurance = 4,
            MovSpd = 5,
            Crit = 6,
            CritResist = 7,
            ImpactEffective = 8,
            Ballance = 9,
            WeakResist = 14,
            DotResist = 15,
            StunResist = 16,
            //something strange, internal itemname sleep_protect, but user string is stun resist, russian user string is "control effect resist"
            AllResist = 18,
            CritPower = 19,
            CritPower1 = 36, // Dragon blood bugged critpower.
            Aggro = 20,
            NoMPDecay = 21, //slayer
            Attack = 22, //total damage modificator
            XPBoost = 23,
            ASpd = 24,
            MovSpdInCombat = 25,
            CraftTime = 26,
            OutOfCombatMovSpd = 27,
            HPDrain = 28, //drain hp on attack
            //28 = Something comming with MovSpd debuff skills, fxp 32% MovSpd debuff from Lockdown Blow IV, give also 12% of this kind
            //29 = something strange when using Lethal Strike
            Stamina = 30,
            Gathering = 31,
            HPChange = 51,
            MPChange = 52,
            RageChange = 53,
            KnockDownChance = 103,
            DefPotion = 104, //or glyph: - incoming damage %
            IncreasedHeal = 105,
            PVPDef = 108,
            AtkPotion = 162, //or glyph: + outgoing damage %
            CritChance = 167,
            PVPAtk = 168,
            Noctenium = 203, //different values for different kinds of Noctenium, not sure what for =)
            StaminaDecay = 207,
            CDR = 208,
            Block = 210, //frontal block ? Not sure, the ability to use block, or blocking stance
            HPLoss = 221, //loss hp at the and of debuff
            Absorb = 227, //or may be I've messed it with 281
            Resurrect = 229,
            Mark = 231, // Velik's Mark/Curse of Kaprima = increase received damage when marked
            CastSpeed = 236,
            CrystalBind = 237,
            CCrystalBind = 249,
            DropUp=255,
            Range = 259, //increase melee range? method 0 value 0.1= +10%
            HPChange2 = 260, //used by instant death on Curse stacks.
            //264 = redirect abnormality, value= new abnormality, bugged due to wrong float format in xml.
            Rage = 280, //tick - RageChange, notick (one change) - Rage 
            SuperArmor = 283,
            ForceCrit = 316, //archer's Find Weakness = next hit will trigger critpower crystalls
            Charm = 65535
        }

        public struct Effect
        {
            public Types Type;
            public DotType Method;
            public double Amount;

        }
        public HotDot(int id, string type, double hp, double mp, double amount, DotType method, int time, int tick,
            string name, string itemName, string tooltip, string iconName)
        {
            Id = id;
            Types rType;
            rType = Enum.TryParse(type, out rType) ? rType : Types.Unknown;
            Hp = hp;
            Mp = mp;
            Time = time;
            Tick = tick;
            Effects.Add(new Effect
            {
                Type = rType,
                Amount = amount,
                Method = method,
            });
            Name = name;
            ShortName = name;
            ItemName = itemName;
            Tooltip = tooltip;
            IconName = iconName;
            Debuff = (rType == Types.Endurance || rType == Types.CritResist) && amount <= 1 || rType == Types.Mark || (rType == Types.DefPotion && amount > 1);
            HPMPChange = rType == Types.HPChange || rType == Types.MPChange;
            Buff = rType != Types.HPChange && rType != Types.MPChange;
        }

        public void Update(int id, string type, double hp, double mp, double amount, DotType method, int time, int tick,
            string name, string itemName, string tooltip, string iconName)
        {
            Types rType;
            rType = Enum.TryParse(type, out rType) ? rType : Types.Unknown;
            if (Effects.Any(x => x.Type == rType)) return; // already added - duplicate strings with the same id and type in tsv (different item names - will be deleted soon)
            Hp = rType==Types.HPChange ? hp : Hp;
            Mp = rType==Types.MPChange ? mp : Mp;
            Tick = rType == Types.MPChange|| rType == Types.HPChange ? tick : Tick; //assume that hp and mp tick times should be the same for one abnormality id
            Effects.Add(new Effect
            {
                Type = rType,
                Amount = amount,
                Method = method
            });
            Debuff = Debuff || (rType == Types.Endurance || rType == Types.CritResist) && amount < 1 || rType == Types.Mark || (rType == Types.DefPotion && amount > 1);
            HPMPChange = HPMPChange || rType == Types.HPChange || rType == Types.MPChange;
            Buff = Buff || (rType != Types.HPChange && rType != Types.MPChange);
        }


        public List<Effect> Effects = new List<Effect>();
        public int Id { get; }
        public double Hp { get; private set; }
        public double Mp { get; private set; }
        public int Time { get; }
        public int Tick { get; private set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string ItemName { get; }
        public string Tooltip { get; set; }
        public string IconName { get; }
        public bool Buff { get; private set; }
        public bool Debuff { get; private set; }
        public bool HPMPChange { get; private set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((HotDot) obj);
        }


        public bool Equals(HotDot other)
        {
            return Id == other.Id;
        }

        public static bool operator ==(HotDot a, HotDot b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(HotDot a, HotDot b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} {Id}";
        }
    }
}