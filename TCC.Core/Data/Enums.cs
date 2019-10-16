using System;
using System.ComponentModel;

namespace TCC.Data
{
    public enum CaptureMode
    {
        [Description("npcap")]
        Npcap,
        [Description("Raw sockets")]
        RawSockets,
        [Description("TERA Toolbox")]
        Toolbox
    }
    public enum GroupWindowLayout
    {
        [Description("Role separated")]
        RoleSeparated,
        [Description("Single column")]
        SingleColumn
    }
    public enum ItemLevelTier
    {
        [Description("None")]
        Tier0 = 0,
        [Description("412")]
        Tier1 = 412,
        [Description("431")]
        Tier2 = 431,
        [Description("439")]
        Tier3 = 439,
        [Description("446")]
        Tier4 = 446,
        [Description("453")]
        Tier5 = 453,
        [Description("455")]
        Tier6 = 455,
        [Description("456")]
        Tier7 = 456
    }

    public enum WarriorStance
    {
        None, Assault, Defensive
    }
    [Flags]
    public enum ModifierKeys : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8,
        None = 0
    }

    public enum CooldownMode
    {
        Normal,
        Pre
    }
    public enum ControlShape
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
        Rhomb,
        Arrow,
        Bar
    }

    public enum HpChangeSource
    {
        CreatureChangeHp,
        BossGage,
        SkillResult,
        Me
    }

    public enum DungeonTier
    {
        Solo = 1,
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
        Angler = 10,
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
        //Enchant12 = 304,
        //Enchant15 = 305,
        RaidLeader = 306,
        Bargain = 307,
        Apply = 308,
        Death = 309,
        Ress = 310,
        Quest = 311,
        Friend = 312,
        //Enchant7 = 313,  
        //Enchant8 = 314,  
        //Enchant9 = 315,  
        WorldBoss = 316,
        Laurel = 317,
        Damage = 318,
        Guardian = 319,
        Enchant = 320,
        LFG = 321, // not sure if old /u is still here
        TCC = 1000,
        Twitch = 1001
        // --custom
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
        Buff = 4,
        Special = 100
    }

    public enum Dragon
    {
        None,
        Ignidrax = 1100,
        Terradrax = 1101,
        Umbradrax = 1102,
        Aquadrax = 1103,
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
        Off = 0,
        On,
        Broken,
        Failed
    }



    public enum ClickThruMode
    {
        Never = 0,
        Always = 1,
        [Description("When dim")]
        WhenDim = 2,
        [Description("When undim")]
        WhenUndim = 3,
        [Description("Game-driven")]
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