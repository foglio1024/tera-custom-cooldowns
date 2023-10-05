using System.Linq;
using System.Xml.Linq;

namespace TCC.Data.Map;

public readonly record struct Section
{
    public uint Id { get; }
    public uint NameId { get; }
    public string MapId { get; }
    //public double Top { get; }
    //public double Left { get; }
    //public double Width { get; }
    //public double Height { get; }
    public bool IsDungeon { get; }
    //public double Scale => Width / (double)Application.Current.FindResource("MapWidth");

    Section(uint sId, uint sNameId, string mapId, bool dg)
    {
        Id = sId;
        NameId = sNameId;
        MapId = mapId;
        //Top = top;
        //Left = left;
        //Width = width;
        //Height = height;
        IsDungeon = dg;
    }
    public static Section FromXElement(XElement sectionElem)
    {
        var sectionId = 0U;
        var sectionNameId = 0U;
        var sectionMapId = "";
        var isDungeon = false;

        sectionElem.Attributes().ToList().ForEach(a =>
        {
            if (a.Name == "id") sectionId = uint.Parse(a.Value);
            if (a.Name == "nameId") sectionNameId = uint.Parse(a.Value);
            if (a.Name == "mapId") sectionMapId = a.Value;
            if (a.Name == "type") isDungeon = a.Value == "dungeon";
        });
        return new Section(sectionId, sectionNameId, sectionMapId, isDungeon);
    }

    /*
            public bool ContainsPoint(float x, float y)
            {
                var matchesY = y > Left && y < Width + Left;
                var matchesX = x < Top && x > Top - Height;
                if (matchesX & matchesY)
                {
                }
                return matchesX && matchesY;
            }
    */
}