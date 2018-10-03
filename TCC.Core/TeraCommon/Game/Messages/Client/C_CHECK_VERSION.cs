using System.Collections.Generic;
using System.Diagnostics;
using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Client
{
    public class C_CHECK_VERSION : ParsedMessage
    {
        internal C_CHECK_VERSION(TeraMessageReader reader) : base(reader)
        {
            var count = reader.ReadUInt16();
            var offset = reader.ReadUInt16();
            for (var i = 1; i <= count; i++)
            {
                reader.BaseStream.Position = offset-4;
                var pointer = reader.ReadUInt16();
                Debug.Assert(pointer==offset);//should be the same
                var nextOffset = reader.ReadUInt16();
                var versionKey = reader.ReadUInt32();
                var versionValue = reader.ReadUInt32();
                Versions.Add(versionKey,versionValue);
                offset = nextOffset;
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once CollectionNeverQueried.Global
        public Dictionary<uint,uint> Versions { get; } = new Dictionary<uint, uint>();
    }
}