using FoglioUtils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TCC.Data.Pc
{
    public class DungeonInfo
    {
        //private List<DungeonCooldownData> _dungeonData;
        //[JsonIgnore] public SynchronizedObservableCollection<DungeonCooldown> Dungeons { get; }
        //[JsonIgnore] public List<DungeonCooldown> VisibleDungeons => Dungeons.Where(x => x.Dungeon.Show).ToList();
        //public List<DungeonCooldownData> DungeonData
        //{
        //    get => _dungeonData;
        //    set
        //    {
        //        if (value == null) return;
        //        _dungeonData = value;
        //        Dungeons.ToSyncList().ForEach(dung =>
        //        {
        //            var dg = value.FirstOrDefault(dd => dd.Id == dung.Dungeon.Id);
        //            if (dg == null) return;
        //            dung.Entries = dg.Entries;
        //            dung.Clears = dg.Clears;
        //        });
        //    }
        //}
        [JsonIgnore] public ICollectionViewLiveShaping VisibleDungeonsView { get; }

        public List<DungeonCooldownData> DungeonList { get; }

        public void Engage(uint dgId)
        {
            var dg = DungeonList.FirstOrDefault(x => x.Dungeon.Id == dgId);
            if (dg != null)
            {
                dg.Entries = dg.Entries == 0
                    ? dg.Dungeon.MaxEntries - 1
                    : dg.Entries - 1;
            }
        }
        public void ResetAll(ResetMode mode)
        {
            DungeonList.Where(d => d.Dungeon.ResetMode == mode).ToList().ForEach(dg => dg.Reset());
        }
        public void UpdateEntries(Dictionary<uint, short> dungeonCooldowns)
        {
            foreach (var keyValuePair in dungeonCooldowns)
            {
                var dg = DungeonList.FirstOrDefault(x => x.Id == keyValuePair.Key);
                if (dg == null) DungeonList.Add(new DungeonCooldownData(keyValuePair.Key) { Entries = keyValuePair.Value });
            }

            DungeonList.Where(x => x.Dungeon.HasDef).ToList().ForEach(dung =>
            {
                if (dungeonCooldowns.TryGetValue(dung.Dungeon.Id, out var entries)) dung.Entries = entries;
                else dung.Reset();
                //var dgd = DungeonList.FirstOrDefault(d => d.Id == dung.Id);
                //if (dgd != null) dgd.Entries = dung.Entries;
            });
        }
        public void UpdateClears(uint dgId, int runs)
        {
            var dg = DungeonList.FirstOrDefault(d => d.Dungeon.Id == dgId);
            if (dg != null) dg.Clears = runs;
            var dgd = DungeonList.FirstOrDefault(d => d.Id == dgId);
            if (dgd != null) dgd.Clears = runs;
        }

        public DungeonInfo()
        {
            DungeonList = Game.DB.DungeonDatabase.Dungeons.Values.Where(d => d.HasDef)
                         .Select(d => new DungeonCooldownData(d.Id)).ToList();
            //DungeonList = new List<DungeonCooldownData>();
            VisibleDungeonsView = CollectionViewUtils.InitLiveView(DungeonList, null,
                new string[] { }, 
                new[] { new SortDescription($"{nameof(Dungeon)}.{nameof(Dungeon.Index)}", ListSortDirection.Ascending) });

        }

        public void UpdateAvailableEntries(uint coins, uint maxCoins)
        {
            DungeonList.ForEach(x => x.UpdateAvailableEntries(coins, maxCoins));
        }
    }
}