using TeraDataLite;

namespace TeraPacketParser.Messages
{
    public class S_SPAWN_USER : ParsedMessage
    {
        public ulong EntityId { get; }
        public uint ServerId { get; }
        public uint PlayerId { get; }
        public string Name { get; }
        public GearItemData Weapon { get; }
        public GearItemData Armor { get; }
        public GearItemData Gloves { get; }
        public GearItemData Boots { get; }

        public S_SPAWN_USER(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(2 + 2 + 2 + 2);
            var nameOffset = reader.ReadUInt16() - 4;
            reader.Skip(2 + 2 + 2 + 2 + 2 + 2 + 2);
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();
            //if(!WindowManager.GroupWindow.VM.Exists(PlayerId,ServerId)) return;
            EntityId = reader.ReadUInt64();
            reader.Skip(4 + 4 + 4 + 2 + 4 + 4 + 2 + 2 + 2 + 2 + 2 + 1 + 1 + 4 + 4);
            var weaponId = reader.ReadUInt32();
            var armorId = reader.ReadUInt32();
            var glovesId = reader.ReadUInt32();
            var bootsId = reader.ReadUInt32();

            Weapon = new GearItemData(weaponId, GearTier.Low, GearPiece.Weapon);
            Armor = new GearItemData(armorId, GearTier.Low, GearPiece.Armor);
            Gloves = new GearItemData(glovesId, GearTier.Low, GearPiece.Hands);
            Boots = new GearItemData(bootsId, GearTier.Low, GearPiece.Feet);


            reader.Skip(4 + 4 + 4 + 4 + 4 + 4 + 4 + 8 + 4 + 1 + 1 + 1 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4);

            reader.BaseStream.Position = nameOffset;
            Name = reader.ReadTeraString();

            //System.Console.WriteLine("[S_SPAWN_USER] {0} {1}", EntityId, Name);
        }
    }
}