using System.Collections.Generic;
using System.Linq;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.Data.Databases
{
    /// <summary>
    /// Class which holds all databases and exposes some methods that output results from different databases.
    /// </summary>
    public class TccDatabase
    {
        public AccountBenefitDatabase AccountBenefitDatabase { get; }
        public MonsterDatabase MonsterDatabase { get; }
        public ItemsDatabase ItemsDatabase { get; }
        public ItemExpDatabase ItemExpDatabase { get; }
        public SkillsDatabase SkillsDatabase { get; }
        public SystemMessagesDatabase SystemMessagesDatabase { get; }
        public GuildQuestDatabase GuildQuestDatabase { get; }
        public AchievementDatabase AchievementDatabase { get; }
        public AchievementGradeDatabase AchievementGradeDatabase { get; }
        public MapDatabase MapDatabase { get; }
        public RegionsDatabase RegionsDatabase { get; }
        public QuestDatabase QuestDatabase { get; }
        public AbnormalityDatabase AbnormalityDatabase { get; }
        public DungeonDatabase DungeonDatabase { get; }
        public SocialDatabase SocialDatabase { get; }

        /// <summary>
        /// True if all database files are found.
        /// </summary>
        public bool Exists => Databases.All(db => db.Exists);

        public bool IsUpToDate => Databases.All(db => db.IsUpToDate);

        public string Language { get; }

        public TccDatabase(string lang)
        {
            Language = lang;
            MonsterDatabase = new MonsterDatabase(lang);
            AccountBenefitDatabase = new AccountBenefitDatabase(lang);
            ItemsDatabase = new ItemsDatabase(lang);
            ItemExpDatabase = new ItemExpDatabase(lang);
            SkillsDatabase = new SkillsDatabase(lang);
            AbnormalityDatabase = new AbnormalityDatabase(lang);
            DungeonDatabase = new DungeonDatabase(lang);
            SocialDatabase = new SocialDatabase(lang);
            SystemMessagesDatabase = new SystemMessagesDatabase(lang);
            GuildQuestDatabase = new GuildQuestDatabase(lang);
            AchievementDatabase = new AchievementDatabase(lang);
            AchievementGradeDatabase = new AchievementGradeDatabase(lang);
            MapDatabase = new MapDatabase(lang);
            RegionsDatabase = new RegionsDatabase(lang);
            QuestDatabase = new QuestDatabase(lang);
        }

        /// <summary>
        /// Calls Load() on all databases.
        /// </summary>
        public void Load()
        {
            Databases.ForEach(db => db.Load());
        }

        /// <summary>
        /// Returns the guard name for the specified dungeon.
        /// </summary>
        /// <param name="dungeonId">id of the dungeon</param>
        /// <returns>guard name of the dungeon if found, else "Unknown"</returns>
        public string GetDungeonGuardName(uint dungeonId)
        {
            var ret = "Unknown";
            var dungWorld = MapDatabase.Worlds[9999];
            var guardList = dungWorld.Guards.Values.ToList();
            var guard = guardList.FirstOrDefault(x => x.Sections.ContainsKey(dungeonId));
            if (guard == null) return ret;

            var openWorld = MapDatabase.Worlds[1];

            if (!openWorld.Guards.TryGetValue(guard.Id, out var grd)) return ret;

            ret = RegionsDatabase.GetZoneName(grd.NameId);
            return ret;
        }
        /// <summary>
        /// Gets the section name starting from guard and section ids.
        /// </summary>
        /// <param name="guardId">id of the guard</param>
        /// <param name="sectionId">id of the section</param>
        /// <returns>name of the section if found, else "Unknown"</returns>
        public string GetSectionName(uint guardId, uint sectionId)
        {
            var ret = "Unknown";
            try
            {
                MapDatabase.Worlds.ToList().ForEach(w =>
                {
                    if (!w.Value.Guards.TryGetValue(guardId, out var g)) return;
                    if (!g.Sections.TryGetValue(sectionId, out var s)) return;
                    var nameId = s.NameId;
                    ret = RegionsDatabase.GetZoneName(nameId);
                });
            }
            catch
            {
                // ignored
            }
            return ret;
        }

        public bool GetSkillFromId(uint id, Class c, CooldownType t, out Skill sk)
        {
            sk = new Skill(0, Class.None, "", "");
            switch (t)
            {
                case CooldownType.Skill:
                    if (!Game.DB!.SkillsDatabase.TryGetSkill(id, c, out sk)) return false;
                    break;
                case CooldownType.Item:
                    if (!Game.DB!.ItemsDatabase.TryGetItemSkill(id, out sk)) return false;
                    break;
                case CooldownType.Passive:
                    if (!Game.DB!.AbnormalityDatabase.TryGetPassiveSkill(id, out sk)) return false;
                    break;
            }

            return true;
        }

        public void DownloadOutdatedDatabases()
        {
            foreach (var outdated in Databases.Where(db => !db.IsUpToDate))
            {
                outdated.Update();
            }
        }

        /// <summary>
        /// Gets max exp amount for the specified item at the specified enchantment level.
        /// </summary>
        /// <param name="id">id of the item</param>
        /// <param name="enchant">enchantment level</param>
        /// <returns>max exp amount</returns>
        public int GetItemMaxExp(uint id, int enchant)
        {
            if (!ItemsDatabase.Items.TryGetValue(id, out var item)) return 0;
            if (item.ExpId == 0) return 0;
            return ItemExpDatabase.ExpData[item.ExpId][enchant];
        }

        private List<DatabaseBase> Databases
        {
            get
            {
                var type = GetType();
                var props = type.GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(DatabaseBase)));
                var dbs = props.Select(prop => (DatabaseBase)prop.GetValue(this)!).ToList();
                return dbs;
            }
        }
        public void CheckVersion()
        {
            Databases.ForEach(db => db.CheckVersion());
        }

        /// <summary>
        /// Tries to convert a continent id to a guard name or dungeon name.
        /// </summary>
        /// <param name="continentId">id of the continent</param>
        /// <param name="name">resulting name, "Unknown" if not found</param>
        /// <returns>true if value was successfully found</returns>
        public bool TryGetGuardOrDungeonNameFromContinentId(uint continentId, out string name)
        {
            if (DungeonDatabase.Dungeons.TryGetValue(continentId, out var dung))
            {
                name = dung.Name;
                return true;
            }
            var (_, guard) = MapDatabase.Worlds[1].Guards.FirstOrDefault(x => x.Value.ContinentId == continentId);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (guard == null)
            {
                name = "Unknown";
                return false;
            }
            name = RegionsDatabase.GetZoneName(guard.NameId);
            return true;

        }
    }
}
