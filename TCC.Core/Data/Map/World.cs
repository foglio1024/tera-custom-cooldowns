using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TCC.Data.Map;

public readonly record struct World(uint Id)
{
    public Dictionary<uint, Guard> Guards { get; } = [];

    public static World FromXElement(XElement worldElem)
    {
        var worldId = 0U;
        foreach (var a in worldElem.Attributes()
            .Where(a => a.Name == "id"))
        {
            worldId = uint.Parse(a.Value);
        }

        var world = new World(worldId);
        foreach (var guard in worldElem.Descendants()
            .Where(x => x.Name == "Guard")
            .Select(Guard.FromXElement))
        {
            world.Guards[guard.Id] = guard;
        }

        return world;
    }
}