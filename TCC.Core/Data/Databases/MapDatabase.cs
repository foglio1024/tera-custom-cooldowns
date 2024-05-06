using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TCC.Data.Map;

namespace TCC.Data.Databases;

public class MapDatabase : DatabaseBase
{
    protected override string FolderName => "world_map";
    protected override string Extension => "xml";

    public Dictionary<uint, World> Worlds { get; } = [];

    public MapDatabase(string lang) : base(lang)
    {
    }

    public bool IsDungeon(int zoneId)
    {
        return Worlds.Values.Any(world =>
            world.Guards.Values.Any(guard =>
                guard.Sections.Values.Any(section
                    => section.Id == zoneId && section.IsDungeon)));
    }

    public bool IsDungeon(Location loc)
    {
        return loc.World == 9999 || Worlds[loc.World].Guards[loc.Guard].Sections[loc.Section].IsDungeon;
    }

    public string GetMapId(uint w, uint g, uint s)
    {
        return Worlds[w].Guards[g].Sections[s].MapId;
    }

    public override void Load()
    {
        Worlds.Clear();
        var xdoc = XDocument.Load(FullPath);
        foreach (var worldElem in xdoc.Descendants().Where(x => x.Name == "World"))
        {
            var world = World.FromXElement(worldElem);
            Worlds[world.Id] = world;
        }
    }
}