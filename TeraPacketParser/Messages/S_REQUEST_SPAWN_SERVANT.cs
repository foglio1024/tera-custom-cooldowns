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
            // ReSharper disable UnusedVariable
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

            Owner = reader.ReadUInt64();
            var energy = reader.ReadUInt32();
            var spawnType = reader.ReadUInt32();
            var level = reader.ReadUInt32();
            FellowShip = reader.ReadUInt32();
            // ReSharper restore UnusedVariable
        }

    }
}
