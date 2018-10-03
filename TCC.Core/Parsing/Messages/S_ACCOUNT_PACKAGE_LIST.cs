using System.Collections.Generic;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    internal class S_ACCOUNT_PACKAGE_LIST : ParsedMessage
    {
        public bool IsElite { get; }

        public S_ACCOUNT_PACKAGE_LIST(TeraMessageReader reader) : base(reader)
        {
            var packageIDs = new List<uint>();

            var count = reader.ReadUInt16();
            reader.Skip(2);
            for (var i = 0; i < count; i++)
            {
                reader.Skip(4);
                packageIDs.Add(reader.ReadUInt32());
                reader.Skip(4);
            }

            if (packageIDs.Contains(433) || packageIDs.Contains(333) || packageIDs.Contains(533))
            {
                IsElite = true;
            }

        }
    }
}
