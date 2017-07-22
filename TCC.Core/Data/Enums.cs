using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC
{
    public enum RareGrade
    {
        Common,
        Uncommon,
        Rare,
        Superior
    }
    public enum BoundType
    {
        None,
        Equip,
        Loot
    }
    public enum ChatChannel
    {
        Say = 0,
        Party = 1,
        Guild = 2,
        Area = 3,
        Trade = 4,
        Greet = 9,
        Private1 = 11,
        Private2 = 12,
        Private3 = 13,
        Private4 = 14,
        Private5 = 15,
        Private6 = 16,
        Private7 = 17,
        Private8 = 18,
        PartyNotice = 21,
        RaidNotice = 25,
        Emote = 26,
        Global = 27,
        Raid = 32,
        Notify = 201, //enchant, broker msgs, Discovered:, etc..
        Event = 202, //guild bam, civil unrest, 
        Error = 203,
        Group = 204,
        GuildNotice = 205,
        Deathmatch = 206,
        ContractAlert = 207,
        GroupAlerts = 208,
        Loot = 209,
        Exp = 210,
        Money = 211,
        Megaphone = 213,
        GuildAdvertising = 214,
        SentWhisper = 300, //arbitrary
        ReceivedWhisper = 301, //arbitrary
        System = 302, //arbitrary (missing in database)
        TradeRedirect = 303, //arbitrary
        Enchant12 = 304, //arbitrary
        Enchant15 = 305, //arbitrary
        RaidLeader = 306, //arbitrary
        Bargain = 307, //arbitrary
        Apply = 308, //arb
        Death = 309, //arb
        Ress = 310, //arb
        TCC = 1000,
    }

    public enum Class
    {
        Warrior = 0,
        Lancer = 1,
        Slayer = 2,
        Berserker = 3,
        Sorcerer = 4,
        Archer = 5,
        Priest = 6,
        Elementalist = 7,
        Soulless = 8,
        Engineer = 9,
        Fighter = 10,
        Assassin = 11,
        Glaiver = 12,
        Common = 255,
        None = 256
    }
    public enum Race
    {
        HumanMale = 1,
        HumanFemale = 2,
        ElfMale = 3,
        ElfFemale = 4,
        AmanMale = 5,
        AmanFemale = 6,
        CastanicMale = 7,
        CastanicFemale = 8,
        Popori = 9,
        Elin = 10,
        Baraka = 11
    }
    public enum Laurel
    {
        None = 0,
        Bronze = 1,
        Silver = 2,
        Gold = 3,
        Diamond = 4,
        Champion = 5
    }
    public enum CooldownType
    {
        Skill,
        Item
    }

    public enum AbnormalityType
    {
        WeakeningEffect = 1,
        DamageOverTime = 2,
        Stun = 3,
        Buff = 4
    }

    public enum Dragon
    {
        None,
        Ignidrax = 1100,
        Terradrax = 1101,
        Umbradrax = 1102,
        Aquadrax = 1103,
    }

    public enum AggroCircle
    {
        Main = 2,
        Secondary = 3,
        None = 255 //arbitrary
    }
    public enum AggroAction
    {
        Add = 1,
        Remove = 2
    }

    public enum ReadyStatus
    {
        NotReady = 0,
        Ready = 1,
        None = 254,
        Undefined = 255 //arbitrary
    }

    public enum HarrowholdPhase
    {
        None = 0,
        Phase1 = 1,
        Phase2 = 2,
        Phase3 = 3,
        Phase4 = 4,
        Balistas = 5,
    }
    public enum ShieldStatus
    {
        Off,
        On,
        Broken,
        Failed
    }
}
