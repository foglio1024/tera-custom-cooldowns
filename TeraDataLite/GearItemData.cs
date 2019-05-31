namespace TeraDataLite
{
    public struct GearItemData
    {
        public uint Id { get; }
        public GearTier Tier { get; }
        public GearPiece Piece { get; }

        public GearItemData(uint itemId, GearTier tier, GearPiece piece)
        {
            Id = itemId;
            Tier = tier;
            Piece = piece;
        }
    }
}
