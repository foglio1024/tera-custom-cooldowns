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
        worldElem.Attributes().ToList().ForEach(a =>
        {
            if (a.Name == "id") worldId = uint.Parse(a.Value);
        });

        var world = new World(worldId);
        worldElem.Descendants().Where(x => x.Name == "Guard").ToList().ForEach(guardElem =>
        {
            var guard = Guard.FromXElement(guardElem);
            world.Guards[guard.Id] = guard;
        });

        return world;
    }
}