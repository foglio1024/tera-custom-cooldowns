using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Windows;

namespace TCC.Data.Databases
{
    /// <summary>
    /// Class which holds all databases and exposes some methods that output results from different databases.
    /// </summary>
    public class TccDatabase
    {
        public AccountBenefitDatabase AccountBenefitDatabase { get; private set; }
        public MonsterDatabase MonsterDatabase { get; private set; }
        public ItemsDatabase ItemsDatabase { get; private set; }
        public ItemExpDatabase ItemExpDatabase { get; private set; }
        public SkillsDatabase SkillsDatabase { get; private set; }
        public SystemMessagesDatabase SystemMessagesDatabase { get; private set; }
        public GuildQuestDatabase GuildQuestDatabase { get; private set; }
        public AchievementDatabase AchievementDatabase { get; private set; }
        public AchievementGradeDatabase AchievementGradeDatabase { get; private set; }
        public MapDatabase MapDatabase { get; private set; }
        public RegionsDatabase RegionsDatabase { get; private set; }
        public QuestDatabase QuestDatabase { get; private set; }
        public AbnormalityDatabase AbnormalityDatabase { get; private set; }
        public DungeonDatabase DungeonDatabase { get; private set; }
        public SocialDatabase SocialDatabase { get; private set; }

        /// <summary>
        /// True if all database files are found.
        /// </summary>
        public bool Exists => MonsterDatabase.Exists &&
                              AccountBenefitDatabase.Exists &&
                              ItemsDatabase.Exists &&
                              ItemExpDatabase.Exists &&
                              SkillsDatabase.Exists &&
                              AbnormalityDatabase.Exists &&
                              DungeonDatabase.Exists &&
                              SocialDatabase.Exists &&
                              SystemMessagesDatabase.Exists &&
                              GuildQuestDatabase.Exists &&
                              AchievementDatabase.Exists &&
                              AchievementGradeDatabase.Exists &&
                              MapDatabase.Exists &&
                              RegionsDatabase.Exists &&
                              QuestDatabase.Exists;

        public TccDatabase(string lang)
        {
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
            AccountBenefitDatabase.Load();
            MonsterDatabase.Load();
            ItemsDatabase.Load();
            ItemExpDatabase.Load();
            SkillsDatabase.Load();
            SystemMessagesDatabase.Load();
            GuildQuestDatabase.Load();
            AchievementDatabase.Load();
            AchievementGradeDatabase.Load();
            MapDatabase.Load();
            RegionsDatabase.Load();
            QuestDatabase.Load();
            AbnormalityDatabase.Load();
            DungeonDatabase.Load();
            SocialDatabase.Load();
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

            if (!openWorld.Guards.ContainsKey(guard.Id)) return ret;

            var nameId = openWorld.Guards[guard.Id].NameId;

            if (RegionsDatabase.Names.ContainsKey(nameId)) ret = RegionsDatabase.Names[nameId];
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
                    if (!w.Value.Guards.ContainsKey(guardId)) return;
                    var g = w.Value.Guards[guardId];
                    if (!g.Sections.ContainsKey(sectionId)) return;
                    var name = g.Sections[sectionId].NameId;
                    ret = RegionsDatabase.Names[name];
                });
            }
            catch
            {
                // ignored
            }
            return ret;
        }
        /// <summary>
        /// Gets max exp amount for the specified item at the specified enchantment level.
        /// </summary>
        /// <param name="id">id of the item</param>
        /// <param name="enchant">enchantment level</param>
        /// <returns>max exp amount</returns>
        public int GetItemMaxExp(uint id, int enchant)
        {
            if (!ItemsDatabase.Items.ContainsKey(id)) return 0;
            if (ItemsDatabase.Items[id].ExpId == 0) return 0;
            return ItemExpDatabase.ExpData[ItemsDatabase.Items[id].ExpId][enchant];
        }
        /// <summary>
        /// Tries to convert a continent id to a guard name or dungeon name.
        /// </summary>
        /// <param name="continentId">id of the continent</param>
        /// <param name="name">resulting name, "Unknown" if not found</param>
        /// <returns>true if value was successfully found</returns>
        public bool TryGetGuardOrDungeonNameFromContinentId(uint continentId, out string name)
        {
            if (DungeonDatabase.Dungeons.ContainsKey(continentId))
            {
                name = DungeonDatabase.Dungeons[continentId].Name;
                return true;
            }
            var guard = MapDatabase.Worlds[1].Guards.FirstOrDefault(x => x.Value.ContinentId == continentId);
            if (guard.Value == null)
            {
                name = "Unknown";
                return false;
            }
            name = RegionsDatabase.Names[guard.Value.NameId];
            return true;

        }
    }
}
