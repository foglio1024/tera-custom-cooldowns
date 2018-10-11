using System;

namespace TCC.Parsing
{
    [Flags]
    public enum ModifierKeys : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8,
        None = 0
    }
    public enum LangEnum : uint
    {
        INT = 0,
        KR = 1,
        USA = 2,
        JPN = 3,
        GER = 4,
        FR = 5,
        EN = 6,
        TW = 7,
        RUS = 8,
        CHN = 9,
        THA = 10
    }
}
namespace TCC.Data
{
    public enum AbnormalityShape
    {
        Round = 0,
        Square = 1
    }
    public enum FlightStackType
    {
        None,
        Air,
        Fire,
        Spark
    }
    public enum WarriorEdgeMode
    {
        Bar,
        Arrow
    }

    public enum HpChangeSource
    {
        CreatureChangeHp,
        BossGage,
        SkillResult
    }

    public enum NotificationType
    {
        Normal = 0,
        Success,
        Warning,
        Error
    }
    public enum DungeonTier
    {
        Solo,
        Tier2,
        Tier3,
        Tier4,
        Tier5
    }
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
        TeamAlert = 22,
        SystemDefault = 24,
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
        //  custom--
        SentWhisper = 300,
        ReceivedWhisper = 301,
        System = 302, //missing in db
        TradeRedirect = 303, 
        Enchant12 = 304,
        Enchant15 = 305,
        RaidLeader = 306,
        Bargain = 307, 
        Apply = 308, 
        Death = 309,     
        Ress = 310,      
        Quest = 311,     
        Friend = 312,    
        Enchant7 = 313,  
        Enchant8 = 314,  
        Enchant9 = 315,  
        WorldBoss = 316, 
        Laurel = 317,
        Damage = 318,
        Guardian = 319,
        TCC = 1000,
        Twitch = 1001
        // --custom
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
        Mystic = 7,
        Reaper = 8,
        Gunner = 9,
        Brawler = 10,
        Ninja = 11,
        Valkyrie = 12,
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
        Item,
        Passive
    }

    public enum AbnormalityType
    {
        Debuff = 1,
        DOT = 2,
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

    public enum GearPiece
    {
        Weapon = 1,
        Armor = 2,
        Hands = 3,
        Feet = 4,
        CritNecklace = 5,
        CritEarring = 6,
        CritRing = 7,
        PowerNecklace = 8,
        PowerEarring = 9,
        PowerRing = 10,
        Circlet = 11,
        Belt = 12,
    }

    public enum GearTier
    {
        Low = 0,
        Mid = 1,
        High = 2,
        Top = 3
    }

    public enum ClickThruMode
    {
        Never = 0,
        Always = 1,
        WhenDim = 2,
        WhenUndim = 3,
        GameDriven = 4
    }

    public enum ButtonsPosition
    {
        Above = 0,
        Below = 1,
    }

    public enum CooldownBarMode
    {
        Normal = 0,
        Fixed = 1
    }

    public enum EnrageLabelMode
    {
        Next = 0,
        Remaining = 1
    }

    public enum DespawnType
    {
        OutOfView = 1,
        Dead = 5
    }

    public enum Role
    {
        Dps = 0,
        Tank = 1,
        Healer = 2
    }

    public enum MessageBoxType
    {
        ConfirmationWithYesNo = 0,
        ConfirmationWithYesNoCancel,
        Information,
        Error,
        Warning
    }

    public enum MessageBoxImage
    {
        Warning = 0,
        Question,
        Information,
        Error,
        None
    }
}
