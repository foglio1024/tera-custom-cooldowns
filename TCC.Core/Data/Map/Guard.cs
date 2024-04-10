using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TCC.Data.Map;

public readonly record struct Guard
{
    public Dictionary<uint, Section> Sections { get; } = new();
    public uint Id { get; }
    public uint NameId { get; }
    public uint ContinentId { get; }

    private Guard(uint gId, uint gNameId, uint continentId)
    {
        Id = gId;
        NameId = gNameId;
        ContinentId = continentId;
    }

    public static Guard FromXElement(XElement guardElem)
    {
        var guardId = 0U;
        var guardNameId = 0U;
        var continentId = 0U;

        guardElem.Attributes().ToList().ForEach(a =>
        {
            if (a.Name == "id") guardId = uint.Parse(a.Value);
            if (a.Name == "nameId") guardNameId = uint.Parse(a.Value);
            if (a.Name == "continentId") continentId = uint.Parse(a.Value);
        });

        var guard = new Guard(guardId, guardNameId, continentId);
        guardElem.Descendants().Where(x => x.Name == "Section").ToList().ForEach(sectionElem =>
        {
            var section = Section.FromXElement(sectionElem);
            guard.Sections[section.Id] = section;
        });

        return guard;
    }
}