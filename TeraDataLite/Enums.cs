using System.ComponentModel;

namespace TeraDataLite;

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
public enum CustomLaurel
{
    [Description("None")]
    None,
    [Description("From game")]
    Game,
    [Description("Bronze")]
    Bronze,
    [Description("Silver")]
    Silver,
    [Description("Gold")]
    Gold,
    [Description("Diamond")]
    Diamond,
    [Description("Champion")]
    Champion,
    [Description("Pink Champion")]
    ChampionPink,
    [Description("Black Champion")]
    ChampionBlack,

}

public enum Species
{
    Unknown = 0,
    Giant = 1,
    Argon = 2,
    Dragon = 3,
    God = 4,
    Faerie = 5,
    Azart = 6,
    [Description("Magical creature")]
    MagicalCreature = 7,
    Beast = 8,
    [Description("Magical device")]
    MagicalDevice = 9,
    Demon = 10
}
public enum ReadyStatus
{
    NotReady = 0,
    Ready = 1,
    None = 254,
    Undefined = 255 //arbitrary
}
public enum LangEnum : uint
{
    INT = 0,
    KR = 1,
    NA = 2,
    JP = 3,
    GER = 4,
    FR = 5,
    EN = 6,
    TW = 7,
    RU = 8,
    CH = 9,
    THA = 10,
    SE = 11
}
public enum RegionEnum
{
    EU,
    NA,
    KR,
    JP,
    TW,
    THA,
    RU
}
public enum NpcGuild
{
    Vanguard = 609,
    Guardian = 611
}
public enum DespawnType
{
    OutOfView = 1,
    Dead = 5
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
public enum GearTier
{
    Low = 0,
    Mid = 1,
    High = 2,
    Top = 3,
    Heroic
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