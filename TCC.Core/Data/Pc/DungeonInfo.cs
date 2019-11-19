using FoglioUtils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TCC.Data.Pc
{
    public class DungeonInfo
    {
        [JsonIgnore]
        public ICollectionViewLiveShaping VisibleDungeonsView { get; }
        public List<DungeonCooldownData> DungeonList { get; }

        public DungeonInfo()
        {
            DungeonList = Game.DB.DungeonDatabase.Dungeons.Values.Where(d => d.HasDef).Select(d => new DungeonCooldownData(d.Id)).ToList();
            VisibleDungeonsView = CollectionViewUtils.InitLiveView(DungeonList,
                sortFilters: new[] { new SortDescription($"{nameof(Dungeon)}.{nameof(Dungeon.Index)}", ListSortDirection.Ascending) });

        }

        public void Engage(uint dgId)
        {
            var dg = DungeonList.FirstOrDefault(x => x.Dungeon.Id == dgId);
            if (dg == null) return;
            dg.Entries = dg.Entries == 0
                ? dg.Dungeon.MaxEntries - 1
                : dg.Entries - 1;
        }
        public void ResetAll(ResetMode mode)
        {
            DungeonList.Where(d => d.Dungeon.ResetMode == mode).ToList().ForEach(dg => dg.Reset());
        }
        public void UpdateEntries(Dictionary<uint, short> dungeonCooldowns)
        {
            (from keyValuePair in dungeonCooldowns
             let dg = DungeonList.FirstOrDefault(x => x.Id == keyValuePair.Key)
             where dg == null
             select new { Id = keyValuePair.Key, Entries = keyValuePair.Value }).ToList()
            .ForEach(x =>
             {
                 DungeonList.Add(new DungeonCooldownData(x.Id) { Entries = x.Entries });
             });

            DungeonList.ForEach(dung =>
            {
                if (dungeonCooldowns.TryGetValue(dung.Dungeon.Id, out var entries)) dung.Entries = entries;
                else dung.Reset();
            });
        }
        public void UpdateClears(uint dgId, int runs)
        {
            var dg = DungeonList.FirstOrDefault(d => d.Dungeon.Id == dgId);
            if (dg != null) dg.Clears = runs;
            var dgd = DungeonList.FirstOrDefault(d => d.Id == dgId); //todo: what is this?
            if (dgd != null) dgd.Clears = runs;
        }
        public void UpdateAvailableEntries(uint coins, uint maxCoins)
        {
            DungeonList.ForEach(x => x.UpdateAvailableEntries(coins, maxCoins));
        }
    }
}