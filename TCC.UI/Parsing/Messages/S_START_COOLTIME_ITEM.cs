using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_START_COOLTIME_ITEM : ParsedMessage
    {
        /// <summary>
        /// Item's ID
        /// </summary>
        public uint ItemId { get; set; }

        /// <summary>
        /// Item's cooldown in seconds
        /// </summary>
        public uint Cooldown { get; set; }

        public S_START_COOLTIME_ITEM(TeraMessageReader reader) : base(reader)
        {
            ItemId = reader.ReadUInt32(); //- 0x04000000;
            Cooldown = reader.ReadUInt32();
        }
    }
}
