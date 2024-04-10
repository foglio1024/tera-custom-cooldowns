using System.Linq;
using System.Xml.Linq;

namespace TCC.Data.Map;

public readonly record struct Section
{
    public uint Id { get; }
    public uint NameId { get; }
    public string MapId { get; }
    public bool IsDungeon { get; }

    private Section(uint sId, uint sNameId, string mapId, bool dg)
    {
        Id = sId;
        NameId = sNameId;
        MapId = mapId;
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
}