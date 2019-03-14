using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TCC.Data.Map;

namespace TCC.Data.Databases
{
    public class MapDatabase : DatabaseBase
    {
        public Dictionary<uint, World> Worlds { get; }

        protected override string FolderName => "world_map";
        protected override string Extension => "xml";


        public MapDatabase(string lang) : base(lang)
        {
            Worlds = new Dictionary<uint, World>();
        }

        public bool IsDungeon(Location loc)
        {
            return loc.World == 9999 || Worlds[loc.World].Guards[loc.Guard].Sections[loc.Section].IsDungeon;
        }

        public string GetMapId(uint w, uint g, uint s)
        {
            return Worlds[w].Guards[g].Sections[s].MapId;
        }

        private static World WorldFromXElement(XElement worldElem)
        {
            var worldId = 0U;
            var worldNameId = 0U;
            worldElem.Attributes().ToList().ForEach(a =>
            {
                if (a.Name == "id") worldId = uint.Parse(a.Value);
                if (a.Name == "nameId") worldNameId = uint.Parse(a.Value);
            });

            var world = new World(worldId, worldNameId);
            ParseGuards();
            return world;

            //----------------------------------------------
            void ParseGuards()
            {
                worldElem.Descendants().Where(x => x.Name == "Guard").ToList().ForEach(guardElem =>
                {
                    var guard = GuardFromXElement(guardElem);
                    world.Guards[guard.Id] = guard;
                });

            }
        }

        private static Guard GuardFromXElement(XElement guardElem)
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

            ParseSections();

            return guard;
            // ------------------------------
            void ParseSections()
            {
                guardElem.Descendants().Where(x => x.Name == "Section").ToList().ForEach(sectionElem =>
                {
                    var section = SectionFromXElement(sectionElem);
                    guard.Sections[section.Id] = section;
                });
            }
        }

        private static Section SectionFromXElement(XElement sectionElem)
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

        public override void Load()
        {
            Worlds.Clear();
            var xdoc = XDocument.Load(FullPath);

            xdoc.Descendants().Where(x => x.Name == "World").ToList().ForEach(worldElem =>
            {
                var world = WorldFromXElement(worldElem);
                Worlds[world.Id] = world;
            });

        }
    }
}
