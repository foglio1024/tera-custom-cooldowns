using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TCC.Data.Map;

public class World
{
    public Dictionary<uint, Guard> Guards { get; set; }
    public uint Id { get; }
    public uint NameId { get; }

    public World(uint wId, uint name)
    {
        Guards = new Dictionary<uint, Guard>();
        Id = wId;
        NameId = name;
    }



    public static World FromXElement(XElement worldElem)
    {
        var worldId = 0U;
        var worldNameId = 0U;
        worldElem.Attributes().ToList().ForEach(a =>
        {
            if (a.Name == "id") worldId = uint.Parse(a.Value);
            if (a.Name == "nameId") worldNameId = uint.Parse(a.Value);
        });

        var world = new World(worldId, worldNameId);
        worldElem.Descendants().Where(x => x.Name == "Guard").ToList().ForEach(guardElem =>
        {
            var guard = Guard.FromXElement(guardElem);
            world.Guards[guard.Id] = guard;
        });
        return world;

        //----------------------------------------------

    }


}