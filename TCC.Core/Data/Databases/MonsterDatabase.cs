using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TCC.Data.Databases
{
    public class MonsterDatabase
    {
        private XDocument _monstersDoc;
        private XDocument _overrideDoc;
        private readonly Dictionary<uint, Zone> _zones;

        public MonsterDatabase(string lang)
        {
            _zones = new Dictionary<uint, Zone>();

            LoadDoc(lang);
            ParseDoc();
            _monstersDoc = null;
        }

        private void LoadDoc(string region)
        {
            _monstersDoc = XDocument.Load(Path.Combine(App.DataPath,$"monsters/monsters-{region}.xml"));
            _overrideDoc = XDocument.Load(Path.Combine(App.DataPath,$"monsters/monsters-override.xml"));
        }

        private void ParseDoc()
        {
            foreach (var zone in _monstersDoc.Descendants().Where(x => x.Name == "Zone"))
            {
                var zoneId = Convert.ToUInt32(zone.Attribute("id")?.Value);
                var zoneName = zone.Attribute("name")?.Value;
                var z = new Zone(zoneName);

                foreach (var monster in zone.Descendants().Where(x => x.Name == "Monster"))
                {
                    var id = Convert.ToUInt32(monster.Attribute("id")?.Value);
                    var name = monster.Attribute("name")?.Value;
                    var isBoss = monster.Attribute("isBoss")?.Value == "True";
                    var maxHP = Convert.ToUInt64(monster.Attribute("hp")?.Value);

                    var m = new Monster(id, name, maxHP, isBoss);
                    z.AddMonster(m);
                }
                _zones.Add(zoneId, z);
            }

            foreach (var zone in _overrideDoc.Descendants().Where(x => x.Name == "Zone"))
            {
                var zoneId = Convert.ToUInt32(zone.Attribute("id")?.Value);

                foreach (var monst in zone.Descendants().Where(x => x.Name == "Monster"))
                {
                    var mId = Convert.ToUInt32(monst.Attribute("id")?.Value);
                    if (!_zones.TryGetValue(zoneId, out var z)) continue;
                    if (z.Monsters.TryGetValue(mId, out var m))
                    {
                        if (monst.Attribute("isBoss") != null) m.IsBoss = bool.Parse(monst.Attribute("isBoss")?.Value ?? "false");
                        if (monst.Attribute("name") != null) m.Name = monst.Attribute("name")?.Value;
                    }
                    else
                    {
                        var name = monst.Attribute("name")?.Value;
                        var isBoss = bool.Parse(monst.Attribute("isBoss")?.Value ?? "false");
                        var maxHp = ulong.Parse(monst.Attribute("hp")?.Value ?? "0");
                        z.Monsters.Add(mId, new Monster(mId, name, maxHp, isBoss));
                    }
                }
            }
        }

        public bool TryGetMonster(uint templateId, uint zoneId, out Monster m)
        {
            if (_zones.TryGetValue(zoneId, out var z))
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
            _zones.TryGetValue(zoneId, out var z);
            return z != null ? z.Name : "Unkown zone";
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

    internal class Zone
    {
        public string Name { get; }
        public Dictionary<uint, Monster> Monsters { get; }

        public void AddMonster(Monster m)
        {
            Monsters.Add(m.Id, m);
        }

        public Zone(string name)
        {
            Monsters = new Dictionary<uint, Monster>();
            Name = name;
        }
    }
    public class Monster
    {
        public uint Id { get; } //npc
        public string Name { get; set; }
        public ulong MaxHP { get; }
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
