


namespace TeraPacketParser.Messages
{
    public class S_PLAYER_CHANGE_EXP : ParsedMessage
    {
        public ulong NextLevelExp { get; set; }
        public ulong KilledMobEntityId { get; set; }
        public uint RestedExp { get; set; }
        public uint GainedRestedExp { get; set; }
        public ulong LevelExp { get; set; }
        public ulong TotalExp { get; set; }
        public ulong GainedTotalExp { get; set; }

        public S_PLAYER_CHANGE_EXP(TeraMessageReader reader) : base(reader)
        {
            GainedTotalExp = reader.ReadUInt64();
            TotalExp = reader.ReadUInt64();
            LevelExp = reader.ReadUInt64();
            NextLevelExp = reader.ReadUInt64();
            KilledMobEntityId = reader.ReadUInt64();
            reader.Skip(4);
            GainedRestedExp = reader.ReadUInt32();
            RestedExp = reader.ReadUInt32();
            // float, u32, u32 (unks)
        }

    }
}
