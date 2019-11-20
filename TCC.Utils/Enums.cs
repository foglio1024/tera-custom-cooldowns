namespace TCC.Utils
{
    public enum NotificationType
    {
        Normal = 0,
        Success,
        Warning,
        Error
    }

    public enum NotificationTemplate
    {
        Default,
        Progress,
        Confirm
    }

}

namespace TCC.Data
{
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
}
