using System;

namespace TeraDataLite
{
    public struct ListingData
    {
        public string LeaderName { get; set; }
        public uint LeaderId { get; set; }
        public bool IsRaid { get; set; }
        public string Message { get; set; }
        public short PlayerCount { get; set; }
        public bool IsTrade => Message.IndexOf("WTS", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                               Message.IndexOf("WTB", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                               Message.IndexOf("WTT", StringComparison.InvariantCultureIgnoreCase) != -1;
    }
}
