


namespace TeraPacketParser.Messages
{
    public readonly record struct GameId(int Id, short Unk1, byte Type, sbyte Unk2)
    {
        public static GameId Parse(ulong id)
        {
            return new GameId(
                (int)id,
                (short)(id >> 32),
                (byte)(id >> 48),
                (sbyte)(id >> 56));
        }
    }
}
