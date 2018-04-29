using System.Collections.Generic;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    class S_ACCOUNT_PACKAGE_LIST : ParsedMessage
    {
        public bool IsElite { get; }
        List<uint> packageIDs;
        public S_ACCOUNT_PACKAGE_LIST(TeraMessageReader reader) : base(reader)
        {
            packageIDs = new List<uint>();

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
