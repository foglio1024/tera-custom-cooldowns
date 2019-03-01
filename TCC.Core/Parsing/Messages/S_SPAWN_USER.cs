using TCC.Data;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_SPAWN_USER : ParsedMessage
    {
        public ulong EntityId { get; private set; }
        public uint ServerId { get; private set; }
        public uint PlayerId { get; private set; }
        public string Name { get; private set; }
        public GearItem Weapon { get; private set; }
        public GearItem Armor { get; private set; }
        public GearItem Gloves { get; private set; }
        public GearItem Boots { get; private set; }

        public S_SPAWN_USER(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(2+2+2+2);
            var nameOffset = reader.ReadUInt16() - 4;
            reader.Skip(2+2+2+2+2+2+2);
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();
            //if(!WindowManager.GroupWindow.VM.Exists(PlayerId,ServerId)) return;
            EntityId = reader.ReadUInt64();
            reader.Skip(4+4+4+2+4+4+2+2+2+2+2+1+1+4+4);
            var weaponId = reader.ReadUInt32();
            var armorId = reader.ReadUInt32();
            var glovesId = reader.ReadUInt32();
            var bootsId = reader.ReadUInt32();

            Weapon = new GearItem(weaponId, GearTier.Low, GearPiece.Weapon, 0, 0);
            Armor = new GearItem(armorId, GearTier.Low, GearPiece.Armor, 0, 0);
            Gloves = new GearItem(glovesId, GearTier.Low, GearPiece.Hands, 0, 0);
            Boots = new GearItem(bootsId, GearTier.Low, GearPiece.Feet, 0, 0);
            

            reader.Skip(4+4+4+4+4+4+4+8+4+1+1+1+4+4+4+4+4+4+4+4+4+4+4+4);

            reader.BaseStream.Position = nameOffset;
            Name = reader.ReadTeraString();

            //System.Console.WriteLine("[S_SPAWN_USER] {0} {1}", EntityId, Name);
        }
    }
}