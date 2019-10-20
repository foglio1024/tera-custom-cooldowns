using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using TCC.Utils;
using TeraDataLite;

namespace TCC.Data.Databases
{
    public class DatabaseQueryMeasure
    {
        private Stopwatch _sw;
        private static int _totalCount;
        private static int _hitCount;
        private static int _missCount;
        private static long _totTotalTime;
        private static long _totHitTime;
        private static long _totMissTime;
        private static double _avgTotalTime => _totalCount == 0 ? 0 : _totTotalTime / (double)_totalCount;
        private static double _avgHitTime => _hitCount == 0 ? 0 : _totHitTime / (double)_hitCount;
        private static double _avgMissTime => _missCount == 0 ? 0 : _totMissTime / (double)_missCount;


        public DatabaseQueryMeasure()
        {
            _sw = new Stopwatch();
        }
        public void StartQuery()
        {
            _totalCount++;
            _sw.Restart();
        }

        public void RegisterHit()
        {
            _sw.Stop();
            _hitCount++;
            _totHitTime += _sw.ElapsedMilliseconds;
            _totTotalTime += _sw.ElapsedMilliseconds;
            Log.CW($"Last query took: {_sw.ElapsedTicks}ticks [hit]");

        }

        public void RegisterMiss()
        {
            _sw.Stop();
            _missCount++;
            _totMissTime += _sw.ElapsedTicks;
            _totTotalTime += _sw.ElapsedTicks;
            Log.CW($"Last query took: {_sw.ElapsedTicks}ticks [miss]");
        }

        public static void PrintInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Total queries: {_totalCount} ({_hitCount} hit - {_missCount} miss)");
            sb.AppendLine($"Avg time: {_avgTotalTime:N3}ticks ({_avgHitTime:N3}ticks hit - {_avgMissTime:N3}ticks miss)");

            Log.CW(sb.ToString());
        }
    }
    public class MonsterDatabase : DatabaseBase
    {
        public static event Action<uint, uint, bool> OverrideChangedEvent;
        public static event Action<uint, uint, bool> BlacklistChangedEvent;

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
            //var measure = new DatabaseQueryMeasure();
            //measure.StartQuery();
            if (_zones.TryGetValue(zoneId, out var z) && z.Monsters.TryGetValue(templateId, out m))
            {
                //measure.RegisterHit();
                return !m.IsHidden;
            }
            m = new Monster(0, 0, "Unknown", 0, false, false, Species.Unknown);
            //measure.RegisterMiss();

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
                    var species = (Species)int.Parse(monster.Attribute("speciesId")?.Value ?? "0");

                    var m = new Monster(id, zoneId, name, maxHP, isBoss, false, species);
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
                        if (monst.Attribute("isHidden") != null) m.IsHidden = bool.Parse(monst.Attribute("isHidden")?.Value ?? "false");
                        if (monst.Attribute("name") != null) m.Name = monst.Attribute("name")?.Value;
                    }
                    else
                    {
                        var name = monst.Attribute("name")?.Value;
                        var isBoss = bool.Parse(monst.Attribute("isBoss")?.Value ?? "false");
                        var isHidden = bool.Parse(monst.Attribute("isHidden")?.Value ?? "false");
                        var maxHp = ulong.Parse(monst.Attribute("hp")?.Value ?? "0");
                        var species = int.Parse(monst.Attribute("speciesId")?.Value ?? "0");
                        z.Monsters.Add(mId, new Monster(mId, zoneId, name, maxHp, isBoss, isHidden, (Species)species));
                    }
                }
            }
        }

        public override void Update(string custom = null)
        {
            base.Update(custom);
            if (!File.Exists(OverrideFileFullPath)) base.Update(OverrideFileRelativePath);
        }

        public void ToggleOverride(uint zoneId, uint templateId, bool b)
        {
            if (TryGetMonster(templateId, zoneId, out var m)) m.IsBoss = b;
            var overrideDoc = XDocument.Load(OverrideFileFullPath);
            var zone = overrideDoc.Descendants("Zone").FirstOrDefault(x => uint.Parse(x.Attribute("id")?.Value) == zoneId);
            if (zone != null)
            {
                var monster = zone.Descendants("Monster").FirstOrDefault(x => uint.Parse(x.Attribute("id").Value) == templateId);
                if (monster != null)
                {
                    if (_zones[zoneId].Monsters[templateId].IsBoss == b)
                    {
                        if (monster.Attribute("isHidden") == null) monster.Remove();
                        else monster.Attribute("isBoss")?.Remove();

                        if (!zone.Descendants().Any()) zone.Remove();
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
        public void Blacklist(uint zoneId, uint templateId, bool b)
        {
            if (TryGetMonster(templateId, zoneId, out var m)) m.IsHidden = b;
            var overrideDoc = XDocument.Load(OverrideFileFullPath);
            var zone = overrideDoc.Descendants("Zone").FirstOrDefault(x => uint.Parse(x.Attribute("id")?.Value) == zoneId);
            if (zone != null)
            {
                var monster = zone.Descendants("Monster").FirstOrDefault(x => uint.Parse(x.Attribute("id").Value) == templateId);
                if (monster != null)
                {
                    if (!b)
                    {
                        if (monster.Attribute("isBoss") == null) monster.Remove();
                        else monster.Attribute("isHidden")?.Remove();
                        if (!zone.Descendants().Any()) zone.Remove();
                    }
                    else
                    {
                        monster.Attribute("isHidden").Value = b.ToString();
                    }
                }
            }
            else
            {
                overrideDoc.Descendants("Zones").First().Add(
                    new XElement("Zone", new XAttribute("id", zoneId),
                        new XElement("Monster", new XAttribute("id", templateId),
                            new XAttribute("isHidden", b.ToString()))));
            }

            overrideDoc.Save(OverrideFileFullPath);

            BlacklistChangedEvent?.Invoke(zoneId, templateId, b);

        }
        public void Blacklist(Monster target, bool b)
        {
            Blacklist(target.ZoneId, target.TemplateId, b);
        }

        public List<Monster> GetBlacklistedMonsters()
        {
            var ret = new List<Monster>();
            foreach (var zone in _zones.Values)
            {
                ret.AddRange(zone.Monsters.Values.Where(m => m.IsHidden));
            }
            return ret;
        }
    }

    internal class Zone
    {
        public string Name { get; }
        public Dictionary<uint, Monster> Monsters { get; }

        public void AddMonster(Monster m)
        {
            Monsters.Add(m.TemplateId, m);
        }

        public Zone(string name)
        {
            Monsters = new Dictionary<uint, Monster>();
            Name = name;
        }
    }
    public class Monster
    {
        public uint TemplateId { get; }
        public uint ZoneId { get; }
        public string Name { get; set; }
        public ulong MaxHP { get; }
        public bool IsBoss { get; set; }
        public bool IsHidden { get; set; }
        public Species Species { get; set; }

        public Monster(uint npc, uint zoneId, string name, ulong maxHp, bool isBoss, bool isHidden, Species sp)
        {
            TemplateId = npc;
            Name = name;
            MaxHP = maxHp;
            IsBoss = isBoss;
            IsHidden = isHidden;
            ZoneId = zoneId;
            Species = sp;
        }
    }
}
