using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TCC.Data
{
    public static class MonsterDatabase
    {
        static XDocument MonstersDoc;
        static List<Zone> Zones;

        public static void Populate()
        {
            Zones = new List<Zone>();

            LoadDoc();
            ParseDoc();
            //foreach (var item in Zones)
            //{
            //    Console.WriteLine("Zone: {0}", item.Id);
            //    foreach (var m in item.Monsters)
            //    {
            //        Console.WriteLine("Monster: {0} ({1})", m.Name, m.Id);
            //    }
            //}
        }
        static void LoadDoc()
        {
            MonstersDoc = XDocument.Load(Environment.CurrentDirectory + @"/resources/database/monsters-EU-EN.xml");
        }
        static void ParseDoc()
        {
            foreach (var zone in MonstersDoc.Descendants().Where(x => x.Name == "Zone"))
            {
                var zoneId = Convert.ToInt32(zone.Attribute("id").Value);
                Zone z = new Zone(zoneId);

                foreach (var monster in zone.Descendants().Where(x => x.Name == "Monster"))
                {
                    var id = Convert.ToInt32(monster.Attribute("id").Value);
                    var name = monster.Attribute("name").Value;
                    var isBoss = monster.Attribute("isBoss").Value;

                    if(isBoss == "True")
                    {
                        Monster m = new Monster(id, name);
                        z.AddMonster(m);
                    }
                }
                Zones.Add(z);
            }
        }

        public static string GetName(int npc, int type)
        {
            if(Zones.Where(x => x.Id == type).Count() > 0)
            {
                if (Zones.Where(x => x.Id == type).Single().Monsters.Where(x => x.Id == npc).Count() > 0)
                {
                    return Zones.Where(x => x.Id == type).Single().Monsters.Where(x => x.Id == npc).Single().Name;
                }
                else return "Unknown";
            }
            else return "Unknown";
        }
    }

    class Zone
    {
        public int Id { get; private set; } //templateId / type
        public List<Monster> Monsters { get; private set; }

        public void AddMonster(Monster m)
        {
            Monsters.Add(m);
        }

        public Zone(int id)
        {
            Monsters = new List<Monster>();
            Id = id;
        }
    }
    class Monster
    {
        public int Id { get; private set; } //npc
        public string Name { get; private set; }
        public Monster(int npc, string name)
        {
            Id = npc;
            Name = name;
        }
    }
}
