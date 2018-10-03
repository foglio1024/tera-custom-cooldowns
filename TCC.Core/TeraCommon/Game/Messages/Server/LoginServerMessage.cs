using TCC.Annotations;
using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class LoginServerMessage : ParsedMessage
    {
        internal LoginServerMessage(TeraMessageReader reader)
            : base(reader)
        {
            int nameOffset = reader.ReadInt16();
            reader.Skip(8);
            RaceGenderClass = new RaceGenderClass(reader.ReadInt32());
            Id = reader.ReadEntityId();
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();
            reader.Skip(27);
            Level = reader.ReadInt16();
            reader.BaseStream.Position = nameOffset - 4;
            Name = reader.ReadTeraString();
//            Debug.WriteLine(Name + ":" + BitConverter.ToString(BitConverter.GetBytes(Id.Id)) + ":" + ServerId.ToString() + " " + BitConverter.ToString(BitConverter.GetBytes(PlayerId)));
        }

        public EntityId Id { get; private set; }
        public uint ServerId { get; private set; }
        public uint PlayerId { get; private set; }
        public int Level { get; private set; }
        public string Name { get; private set; }
        public string GuildName { get; [UsedImplicitly] private set; }
        public RaceGenderClass RaceGenderClass { get; }
    }
}