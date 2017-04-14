using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Tera.Game.Messages
{
    public class S_CREST_MESSAGE : ParsedMessage
    {
        internal S_CREST_MESSAGE(TeraMessageReader reader) : base(reader)
        {
            
            //PrintRaw();
            var unknow = reader.ReadUInt32();

            //Type? 6 = reset? 
            var typeId = reader.ReadUInt32();
           // Debug.WriteLine("Crest type id:" + typeId);
            Type = (CrestType)typeId;
            SkillId = reader.ReadInt32() & 0x3FFFFFF;
          //  Debug.WriteLine("Crest :" + Type + ";Skill Id "+SkillId);

        }

        public CrestType Type { get; set; }

        public int SkillId { get; set; }
    }

    public enum CrestType
    {
        Reset = 6
    }
}
