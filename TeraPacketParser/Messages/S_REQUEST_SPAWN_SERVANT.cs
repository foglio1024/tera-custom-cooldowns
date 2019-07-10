namespace TeraPacketParser.Messages
{
    public class S_REQUEST_SPAWN_SERVANT : ParsedMessage
    {
        public ulong Owner { get; }
        public ulong GameId { get; set; }

        public uint FellowShip { get; set; }
        public uint Id { get; set; }

        public S_REQUEST_SPAWN_SERVANT(TeraMessageReader reader) : base(reader)
        {
            var giftedSkillsCount = reader.ReadUInt16();
            var giftedSkillsOffset = reader.ReadUInt16();
            var abilitiesCount = reader.ReadUInt16();
            var abilitiesOffset = reader.ReadUInt16();
            var nameOffset = reader.ReadUInt16();
            GameId = reader.ReadUInt64();
            var dbid = reader.ReadUInt64();
            var loc = reader.ReadVector3f();
            var h = reader.ReadUInt16();
            var type = reader.ReadUInt32();
            Id = reader.ReadUInt32();
            var linkedNpcTemplateId = reader.ReadUInt32();
            var linkedNpcZoneId = reader.ReadUInt16();
            var walkSpeed = reader.ReadUInt16();
            var runSpeed = reader.ReadUInt16();

            //reader.Skip(8+8+12+2+4+4+4+2+2+2);
            Owner = reader.ReadUInt64();
            var energy = reader.ReadUInt32();
            var spawnType = reader.ReadUInt32();
            var level = reader.ReadUInt32();
            FellowShip = reader.ReadUInt32();
            //PetAbilities = new List<uint>();
            //if (giftedSkillsOffset != 0)
            //{

            //reader.RepositionAt(giftedSkillsOffset);
            //for (int i = 0; i < giftedSkillsCount; i++)
            //{
            //    var curr = reader.ReadUInt16();
            //    var next = reader.ReadUInt16();

            //    var ability = reader.ReadUInt32();
            //    PetAbilities.Add(ability);

            //    if (next == 0) break;
            //    reader.RepositionAt(next);
            //}
            //}

            //if (abilitiesOffset != 0)
            //{
            //reader.RepositionAt(abilitiesOffset);

            //for (int i = 0; i < abilitiesCount; i++)
            //{
            //    var curr = reader.ReadUInt16();
            //    var next = reader.ReadUInt16();

            //    var ability = reader.ReadUInt32();
            //    PetAbilities.Add(ability);

            //    if (next == 0) break;
            //    reader.RepositionAt(next);
            //}
            //}

            //try
            //{
            //    reader.RepositionAt(nameOffset);
            //    var name = reader.ReadTeraString();
            //}
            //catch
            //{

            //}
        }

    }
}
