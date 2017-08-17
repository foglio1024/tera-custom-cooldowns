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
        public string MapId { get; }
        public double Top { get; }
        public double Left { get; }
        double Width { get; }
        public bool IsDungeon { get; }
        public double Scale
        {
            get
            {
                return Width / (Double)App.Current.FindResource("MapWidth");
            }
        }
        public Section(uint sId, uint sNameId, string mapId, double top, double left, double width, bool dg)
        {
            Id = sId;
            NameId = sNameId;
            MapId = mapId;
            Top = top;
            Left = left;
            Width = width;
            IsDungeon = dg;
        }
    }
}
