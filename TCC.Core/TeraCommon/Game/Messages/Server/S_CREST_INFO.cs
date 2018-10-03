using System.Collections.Generic;
using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class S_CREST_INFO : ParsedMessage
    {
        internal S_CREST_INFO(TeraMessageReader reader) : base(reader)
        {
            var count = reader.ReadUInt16();
            reader.ReadUInt16();
            PointsMax = reader.ReadUInt32();
            PointsUsed = reader.ReadUInt32();
            for (var i = 1; i <= count; i++)
            {
                reader.Skip(4); //offset pointer & next member offset
                var id = reader.ReadUInt32();
                var enabled = (reader.ReadByte() & 1) != 0;
                Glyphs[id]=enabled;
            }

            //foreach (var Glyph in Glyphs)
            //{
            //    Debug.WriteLine($"Glyphid:{Glyph.Key} : "+ (Glyph.Value?"Enabled":"Disabled"));
            //}
        }

        public uint PointsMax { get; }
        public uint PointsUsed { get; }
        public Dictionary<uint,bool> Glyphs { get; } = new Dictionary<uint,bool>();
    }
}