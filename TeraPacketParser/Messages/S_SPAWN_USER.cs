using System.Runtime.Remoting.Activation;
using TeraDataLite;

namespace TeraPacketParser.Messages
{
    public class S_SPAWN_USER : ParsedMessage
    {
        public ulong EntityId { get; }
        public uint ServerId { get; }
        public uint PlayerId { get; }
        public int TemplateId { get; }
        public int Level { get; }
        public string Name { get; }
        public string GuildName { get; }
        public string GuildRank { get; }
        public GearItemData Weapon { get; }
        public GearItemData Armor { get; }
        public GearItemData Gloves { get; }
        public GearItemData Boots { get; }

        public S_SPAWN_USER(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(2 + 2 + 2 + 2);
            var nameOffset = reader.ReadUInt16();
            var guildNameOffset = reader.ReadUInt16();
            var guildRankOffset = reader.ReadUInt16();
            reader.Skip(2+2+2+2+2);
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();
            //if(!WindowManager.GroupWindow.VM.Exists(PlayerId,ServerId)) return;
            EntityId = reader.ReadUInt64();
            reader.Skip(12);
            reader.Skip(2);
            reader.Skip(4);
            TemplateId = reader.ReadInt32();
            reader.Skip(2 + 2 + 2 + 2 + 2 + 1 + 1 + 4 + 4);

            var weaponId = reader.ReadUInt32();
            var armorId = reader.ReadUInt32();
            var glovesId = reader.ReadUInt32();
            var bootsId = reader.ReadUInt32();

            Weapon = new GearItemData(weaponId, GearTier.Low, GearPiece.Weapon);
            Armor = new GearItemData(armorId, GearTier.Low, GearPiece.Armor);
            Gloves = new GearItemData(glovesId, GearTier.Low, GearPiece.Hands);
            Boots = new GearItemData(bootsId, GearTier.Low, GearPiece.Feet);

            var underwear = reader.ReadUInt32();
            var head = reader.ReadUInt32();
            var face = reader.ReadUInt32();
            var spawnFx = reader.ReadInt32();
            var mount = reader.ReadInt32();
            var pose = reader.ReadInt32();
            var title = reader.ReadInt32();
            var shuttleId = reader.ReadInt64();
            var guildLogo = reader.ReadInt32();
            var exarch = reader.ReadBoolean();
            var gm = reader.ReadBoolean();
            var gmInv= reader.ReadBoolean();
            var weapModel= reader.ReadUInt32();
            var bodyModel= reader.ReadUInt32();
            var handModel= reader.ReadUInt32();
            var feetModel= reader.ReadUInt32();
            var weapDye= reader.ReadUInt32();
            var bodyDye= reader.ReadUInt32();
            var handDye= reader.ReadUInt32();
            var feetDye= reader.ReadUInt32();
            var underwearDye= reader.ReadUInt32();
            var backDye= reader.ReadUInt32();
            var headDye= reader.ReadUInt32();
            var faceDye= reader.ReadUInt32();
            var weapEnch= reader.ReadUInt32();
            var woldEventTarget= reader.ReadBoolean();
            var pkEnabled= reader.ReadBoolean();
            Level = reader.ReadInt32();


            //reader.Skip(4 + 4 + 4 + 4 + 4 + 4 + 4 + 8 + 4 + 1 + 1 + 1 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4);

            reader.RepositionAt(nameOffset);
            Name = reader.ReadTeraString();
            reader.RepositionAt(guildNameOffset);
            GuildName = reader.ReadTeraString();
            reader.RepositionAt(guildRankOffset);
            GuildRank = reader.ReadTeraString();

            //System.Console.WriteLine("[S_SPAWN_USER] {0} {1}", EntityId, Name);
        }

    }
}