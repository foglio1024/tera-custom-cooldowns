using TeraPacketParser.Data;

namespace TeraPacketParser.Messages
{
    public class S_EACH_SKILL_RESULT : ParsedMessage
    {
        public ulong Source { get; private set; }
        public ulong Target { get; private set; }
        public ulong Damage { get; private set; }
        public ushort Type { get; private set; }
        public int Skill { get; private set; }
        public S_EACH_SKILL_RESULT(TeraMessageReader reader) : base(reader)
        {
            reader.BaseStream.Position = 0;
            reader.Skip(4);
            Source = reader.ReadUInt64();
            var owner = reader.ReadUInt64();
            if (owner != 0) Source = owner;
            Target = reader.ReadUInt64();
            // ReSharper disable UnusedVariable
            var template = reader.ReadInt32();
            Skill = new SkillId(reader).Id;
            var stage = reader.ReadInt32();
            var targeting = reader.ReadInt16();
            var area = reader.ReadInt16();
            var id = reader.ReadUInt32();
            var time = reader.ReadInt32();
            // ReSharper restore UnusedVariable
            Damage = reader.ReadUInt64();
            Type = reader.ReadUInt16();
        }
    }
}