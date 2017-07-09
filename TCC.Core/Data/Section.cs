using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Data
{
    public class Section
    {
        public uint Id { get; }
        public uint NameId { get; }

        public Section(uint sId, uint sNameId)
        {
            Id = sId;
            NameId = sNameId;
        }
    }
}
