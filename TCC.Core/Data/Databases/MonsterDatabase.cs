using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace TCC.Data.Databases
{
    public class MonsterDatabase : DatabaseBase
    {
        public static event Action<uint, uint, bool> OverrideChangedEvent;

        private readonly Dictionary<uint, Zone> _zones;

        protected override string FolderName => "monsters";
        protected override string Extension => "xml";
        public override bool Exists => base.Exists && File.Exists(OverrideFileFullPath);
        private string OverrideFileFullPath => FullPath.Replace(Language, "override");
        private string OverrideFileRelativePath => RelativePath.Replace(Language, "override");

        public MonsterDatabase(string lang) : base(lang)
        {
            _zones = new Dictionary<uint, Zone>();
        }

        public bool TryGetMonster(uint templateId, uint zoneId, out Monster m)
        {
            if (_zones.TryGetValue(zoneId, out var z) && z.Monsters.TryGetValue(templateId, out m)) return true;
            m = new Monster(0, "Unknown", 0, false);
            return false;
        }
        public string GetZoneName(uint zoneId)
        {
            _zones.TryGetValue(zoneId, out var z);
            return z != null ? z.Name : "Unknown zone";
        }
        public string GetName(uint templateId, uint zoneId)
        {
            return TryGetMonster(templateId, zoneId, out var m) ? m.Name : "Unknown";
        }
        public ulong GetMaxHP(uint templateId, uint zoneId)
        {
            return TryGetMonster(templateId, zoneId, out var m) ? m.MaxHP : 1;
        }

        public override void Load()
        {
            _zones.Clear();
            var monstersDoc = XDocument.Load(FullPath);

            foreach (var zone in monstersDoc.Descendants().Where(x => x.Name == "Zone"))
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
            var overrideDoc = XDocument.Load(OverrideFileFullPath);
            foreach (var zone in overrideDoc.Descendants().Where(x => x.Name == "Zone"))
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

        public override void Update(string custom = null)
        {
            base.Update(custom);
            base.Update(OverrideFileRelativePath);
        }

        public void ToggleOverride(uint zoneId, uint templateId, bool b)
        {
            var overrideDoc = XDocument.Load(OverrideFileFullPath);
            var zone = overrideDoc.Descendants("Zone").FirstOrDefault(x => uint.Parse(x.Attribute("id").Value) == zoneId);
            if (zone != null)
            {
                var monster = zone.Descendants("Monster").FirstOrDefault(x => uint.Parse(x.Attribute("id").Value) == templateId);
                if (monster != null)
                {
                    if (_zones[zoneId].Monsters[templateId].IsBoss == b)
                    {
                        monster.Remove();
                        if(!zone.Descendants().Any()) zone.Remove();
                    }
                    else
                    {
                        monster.Attribute("isBoss").Value = b.ToString();
                    }
                }
            }
            else
            {
                overrideDoc.Descendants("Zones").First().Add(
                    new XElement("Zone", new XAttribute("id", zoneId),
                        new XElement("Monster", new XAttribute("id", templateId),
                                                new XAttribute("isBoss", b.ToString()))));
            }

            overrideDoc.Save(OverrideFileFullPath);

            OverrideChangedEvent?.Invoke(zoneId, templateId, b);
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
