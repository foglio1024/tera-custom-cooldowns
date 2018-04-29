using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TCC.Data.Databases
{
    public class MonsterDatabase
    {
        XDocument MonstersDoc;
        XDocument OverrideDoc;
        Dictionary<uint, Zone> Zones;

        public MonsterDatabase(string lang)
        {
            Zones = new Dictionary<uint, Zone>();

            LoadDoc(lang);
            ParseDoc();
            MonstersDoc = null;
        }

        void LoadDoc(string region)
        {
            MonstersDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"/resources/data/monsters/monsters-" + region + ".xml");
            OverrideDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"/resources/data/monsters/monsters-override.xml");
        }
        void ParseDoc()
        {
            foreach (var zone in MonstersDoc.Descendants().Where(x => x.Name == "Zone"))
            {
                var zoneId = Convert.ToUInt32(zone.Attribute("id").Value);
                var zoneName = zone.Attribute("name").Value;
                var z = new Zone(zoneId, zoneName);

                foreach (var monster in zone.Descendants().Where(x => x.Name == "Monster"))
                {
                    var id = Convert.ToUInt32(monster.Attribute("id").Value);
                    var name = monster.Attribute("name").Value;
                    var isBoss = false;
                    if (monster.Attribute("isBoss").Value == "True")
                    {
                        isBoss = true;
                    }
                    var maxHP = Convert.ToUInt64(monster.Attribute("hp").Value);

                    var m = new Monster(id, name, maxHP, isBoss);
                    z.AddMonster(m);
                }
                Zones.Add(zoneId, z);
            }

            foreach (var zone in OverrideDoc.Descendants().Where(x => x.Name == "Zone"))
            {
                var zoneId = Convert.ToUInt32(zone.Attribute("id").Value);

                foreach (var monst in zone.Descendants().Where(x => x.Name == "Monster"))
                {
                    var mId = Convert.ToUInt32(monst.Attribute("id").Value);
                    if (Zones.TryGetValue(zoneId, out var z))
                    {
                        if (z.Monsters.TryGetValue(mId, out var m))
                        {
                            if (monst.Attribute("isBoss") != null)
                            {
                                m.IsBoss = bool.Parse(monst.Attribute("isBoss").Value);
                            }
                            if (monst.Attribute("name") != null)
                            {
                                m.Name = monst.Attribute("name").Value;
                            }
                        }
                        else
                        {
                            var name = monst.Attribute("name").Value;
                            var isBoss = bool.Parse(monst.Attribute("isBoss").Value);
                            var maxHp = ulong.Parse(monst.Attribute("hp").Value);
                            z.Monsters.Add(mId, new Monster(mId, name, maxHp, isBoss));
                        }
                    }
                }
            }
        }

        public bool TryGetMonster(uint templateId, uint zoneId, out Monster m)
        {
            if (Zones.TryGetValue(zoneId, out var z))
            {
                if (z.Monsters.TryGetValue(templateId, out m))
                {
                    return true;
                }
            }
            m = new Monster(0, "Unknown", 0, false);
            return false;
        }
        public string GetZoneName(uint zoneId)
        {
            Zones.TryGetValue(zoneId, out var z);
            if (z != null) return z.Name;

            return "Unkown zone";

        }
        public string GetName(uint templateId, uint zoneId)
        {
            if (TryGetMonster(templateId, zoneId, out var m))
            {
                return m.Name;
            }
            else return "Unknown";
        }
        public ulong GetMaxHP(uint templateId, uint zoneId)
        {
            if (TryGetMonster(templateId, zoneId, out var m))
            {
                return m.MaxHP;
            }
            else return 1;
        }
    }

    class Zone
    {
        public uint Id { get; private set; } //templateId / type
        public string Name { get; private set; }
        public Dictionary<uint, Monster> Monsters { get; private set; }

        public void AddMonster(Monster m)
        {
            Monsters.Add(m.Id, m);
        }

        public Zone(uint id, string name)
        {
            Monsters = new Dictionary<uint, Monster>();
            Id = id;
            Name = name;
        }
    }
    public class Monster
    {
        public uint Id { get; private set; } //npc
        public string Name { get; set; }
        public ulong MaxHP { get; set; }
        public bool IsBoss { get; set; }

        public Monster(uint npc, string name, ulong maxHp, bool isBoss)
        {
            Id = npc;
            Name = name;
            MaxHP = maxHp;
            IsBoss = isBoss;
        }
    }
}
