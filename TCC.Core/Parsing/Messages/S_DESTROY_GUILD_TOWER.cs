using TCC.Annotations;
using TCC.TeraCommon.Game;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_DESTROY_GUILD_TOWER : ParsedMessage
    {
        public Vector3f Location { [UsedImplicitly] get; }
        public uint SourceGuildId { get; }
        public uint TargetGuildId { [UsedImplicitly] get; }
        public string SourceGuildName { [UsedImplicitly] get; }
        public string PlayerName { [UsedImplicitly] get; }
        public string TargetGuildName { [UsedImplicitly] get; }

        public S_DESTROY_GUILD_TOWER(TeraMessageReader reader) : base(reader)
        {
            try
            {
                reader.Skip(2 + 2 + 2);
                Location = reader.ReadVector3f();
                SourceGuildId = reader.ReadUInt32();
                TargetGuildId = reader.ReadUInt32();
                SourceGuildName = reader.ReadTeraString();
                PlayerName = reader.ReadTeraString();
                TargetGuildName = reader.ReadTeraString();
            }
            catch
            {
                // ignored
            }
        }
    }
}
