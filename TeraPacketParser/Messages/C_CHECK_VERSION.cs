using System.Collections.Generic;

namespace TeraPacketParser.Messages
{
    public class C_CHECK_VERSION : ParsedMessage
    {
        public Dictionary<uint, uint> Versions { get; } = new();

        internal C_CHECK_VERSION(TeraMessageReader reader) : base(reader)
        {
            var count = reader.ReadUInt16();
            var offset = reader.ReadUInt16();
            for (var i = 1; i <= count; i++)
            {
                reader.BaseStream.Position = offset - 4;
                reader.Skip(2);
                var nextOffset = reader.ReadUInt16();
                var versionKey = reader.ReadUInt32();
                var versionValue = reader.ReadUInt32();
                Versions.Add(versionKey, versionValue);
                offset = nextOffset;
            }
        }
    }
}