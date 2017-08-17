using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC;

namespace TCC.Parsing.Messages
{
    class C_CHECK_VERSION_CUSTOM
    {
        public C_CHECK_VERSION_CUSTOM(CustomReader reader)
        {
            var count = reader.ReadUInt16();
            var offset = reader.ReadUInt16();
            for (var i = 1; i <= count; i++)
            {
                reader.BaseStream.Position = offset - 4;
                var pointer = reader.ReadUInt16();
                var nextOffset = reader.ReadUInt16();
                var VersionKey = reader.ReadUInt32();
                var VersionValue = reader.ReadUInt32();
                Versions.Add(VersionKey, VersionValue);
                offset = nextOffset;
            }
        }

        public Dictionary<uint, uint> Versions { get; } = new Dictionary<uint, uint>();
    }
}
