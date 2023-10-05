using System;
using System.ComponentModel;

namespace TCC.Data;

public enum NoticeTrigger
{
    // before setup, only MessageBox
    Startup = 0,
    // after splashscreen is closed
    Ready = 1,
    // upon server connection
    Connection = 2,
    // upon character login
    Login = 3
}
public enum MentionMode
{
    [Description("Disabled")]
    Disabled,
    [Description("Current character")]
    Current,
    [Description("All characters")]
    All
}

public enum GroupWindowLayout
{
    [Description("Role separated")]
    RoleSeparated,
    [Description("Single column")]
    SingleColumn
}
public enum GroupHpLabelMode
{
    None,
    Amount,
    Percentage
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
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Superior = 3,
    Heroic = 4
}
public enum BoundType
{
    None,
    Equip,
    Loot
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
    Aquadrax = 1103
}



public enum HarrowholdPhase
{
    None = 0,
    Phase1 = 1,
    Phase2 = 2,
    Phase3 = 3,
    Phase4 = 4,
    Balistas = 5
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
    Below = 1
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