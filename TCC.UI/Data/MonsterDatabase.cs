using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TCC.Data
{
    public class MonsterDatabase
    {
        XDocument MonstersDoc;
        List<Zone> Zones;

        public MonsterDatabase(string lang)
        {
            Zones = new List<Zone>();

            LoadDoc(lang);
            ParseDoc();
            MonstersDoc = null;
        }
        public void Populate(string region)
        {

        }
        void LoadDoc(string region)
        {
            MonstersDoc = XDocument.Load(Environment.CurrentDirectory + @"/resources/data/monsters/monsters-"+region+".xml");
        }
        void ParseDoc()
        {
            foreach (var zone in MonstersDoc.Descendants().Where(x => x.Name == "Zone"))
            {
                var zoneId = Convert.ToInt32(zone.Attribute("id").Value);
                Zone z = new Zone(zoneId);

                foreach (var monster in zone.Descendants().Where(x => x.Name == "Monster"))
                {
                    var id = Convert.ToInt32(monster.Attribute("id").Value);
                    var name = monster.Attribute("name").Value;
                    bool isBoss = false;
                    if(monster.Attribute("isBoss").Value == "True")
                    {
                        isBoss = true;
                    }
                    var maxHP = Convert.ToInt32(monster.Attribute("hp").Value);

                    Monster m = new Monster(id, name, maxHP, isBoss);
                    z.AddMonster(m);
                }
                Zones.Add(z);
            }
        }

        public bool TryGetMonster(uint templateId, uint zoneId, out Monster m)
        {
            if(Zones.Where(x => x.Id == zoneId).Count() > 0)
            {
                //found zone
                if(Zones.Where(x => x.Id == zoneId).Single().Monsters.Where(x => x.Id == templateId).Count() > 0)
                {
                    //found monster
                    m = Zones.Where(x => x.Id == zoneId).Single().Monsters.Where(x => x.Id == templateId).FirstOrDefault();
                    return true;
                }
                else
                {
                    m = new Monster(0, "Unknown", 0, false);
                    return false;
                }
            }
            else
            {
                m = new Monster(0, "Unknown", 0, false);
                return false;
            }
        }

        public string GetName(uint templateId, uint zoneId)
        {
            if (TryGetMonster(templateId, zoneId, out Monster m))
            {
                return m.Name;
            }
            else return "Unknown";
        }
        public int GetMaxHP(uint templateId, uint zoneId)
        {
            if (TryGetMonster(templateId, zoneId, out Monster m))
            {
                return m.MaxHP;
            }
            else return 1;
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
    public class Monster
    {
        public int Id { get; private set; } //npc
        public string Name { get; private set; }
        public int MaxHP { get; private set; }
        public bool IsBoss { get; private set; }

        public Monster(int npc, string name, int maxHp, bool isBoss)
        {
            Id = npc;
            Name = name;
            MaxHP = maxHp;
            IsBoss = isBoss;
        }
    }
}
