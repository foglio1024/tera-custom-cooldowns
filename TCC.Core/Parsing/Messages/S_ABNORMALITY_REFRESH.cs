using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_ABNORMALITY_REFRESH : ParsedMessage
    {
        ulong target;
        uint id;
        uint duration;
        int stacks;

        public ulong TargetId { get => target; }
        public uint AbnormalityId { get => id; }
        public uint Duration { get => duration; }
        public int Stacks { get => stacks; }

        public S_ABNORMALITY_REFRESH(TeraMessageReader reader) : base(reader)
        {
            target = reader.ReadUInt64();
            id = reader.ReadUInt32();
            duration = reader.ReadUInt32();
            reader.Skip(4);
            stacks = reader.ReadInt32();
        }
    }
}
