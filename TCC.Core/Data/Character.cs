using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Databases;

namespace TCC
{
    public class Character : TSPropertyChanged, IComparable
    {
        string _name;
        Class _class;
        Laurel _laurel;
        int _dailiesDone;
        int _weekliesDone;
        int _credits;
        bool _isLoggedIn;
        bool _isSelected;

        public uint Id { get; set; }
        public int Position { get; set; }
        public string Name
        {
            get => _name; set
            {
                if (_name == value) return;
                _name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }
        public Class Class
        {
            get => _class; set
            {
                if (_class == value) return;
                _class = value;
                NotifyPropertyChanged(nameof(Class));
            }
        }
        public Laurel Laurel
        {
            get => _laurel; set
            {
                if (_laurel == value) return;
                _laurel = value;
                NotifyPropertyChanged(nameof(Laurel));
            }
        }
        public int DailiesDone
        {
            get => _dailiesDone;
            set
            {
                if (_dailiesDone == value) return;
                _dailiesDone = value;
                NotifyPropertyChanged(nameof(DailiesDone));
                NotifyPropertyChanged(nameof(VanguardDailyCompletion));
            }
        }

        public void UpdateDungeons(Dictionary<uint, short> dungeonCooldowns)
        {
            foreach (var keyVal in dungeonCooldowns)
            {
                if (!DungeonDatabase.Instance.Dungeons.ContainsKey(keyVal.Key)) continue;
                var dg = Dungeons.FirstOrDefault(x => x.Id == keyVal.Key);
                if (dg != null) dg.Entries = keyVal.Value;
                //else
                //{
                //    Dungeons.Add(new DungeonCooldown(keyVal.Key, keyVal.Value, _dispatcher));
                //}
            }
        }

        public int WeekliesDone
        {
            get => _weekliesDone;
            set
            {
                if (_weekliesDone == value) return;
                _weekliesDone = value;
                NotifyPropertyChanged(nameof(WeekliesDone));
                NotifyPropertyChanged(nameof(VanguardWeeklyCompletion));

            }
        }
        public int Credits
        {
            get => _credits;
            set
            {
                if (_credits == value) return;
                _credits = value;
                NotifyPropertyChanged(nameof(Credits));
            }
        }
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                if (_isLoggedIn == value) return;
                _isLoggedIn = value;
                NotifyPropertyChanged(nameof(IsLoggedIn));
            }
        }
        public bool IsSelected
        {
            get => _isSelected; set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }
        public double VanguardWeeklyCompletion
        {
            get => (double)WeekliesDone / (double)SessionManager.MAX_WEEKLY;
        }
        public double VanguardDailyCompletion
        {
            get => (double)DailiesDone / (double)SessionManager.MAX_DAILY;
        }
        public SynchronizedObservableCollection<DungeonCooldown> Dungeons { get; set; }
        public ICollectionViewLiveShaping VisibleDungeons { get; set; }
        public SynchronizedObservableCollection<GearItem> Gear { get; set; }
        public Character(string name, Class c, uint id, int pos, Dispatcher d, Laurel l = Laurel.None)
        {
            _dispatcher = d;
            Dungeons = new SynchronizedObservableCollection<DungeonCooldown>(_dispatcher);
            Gear = new SynchronizedObservableCollection<GearItem>(_dispatcher);
            Name = name;
            Class = c;
            Laurel = l;
            DailiesDone = 0;
            WeekliesDone = 0;
            Id = id;
            Position = pos;
            foreach (var dg in DungeonDatabase.Instance.Dungeons)
            {
                Dungeons.Add(new DungeonCooldown(dg.Key, _dispatcher));
            }
            VisibleDungeons = Utils.InitLiveView(dc => DungeonDatabase.Instance.Dungeons.ContainsKey(((DungeonCooldown)dc).Id) &&
            DungeonDatabase.Instance.Dungeons[((DungeonCooldown)dc).Id].Show, Dungeons, new string[] { nameof(Dungeon.Show) }, new string[] { nameof(Dungeon.Tier) });
        }

        public int CompareTo(object obj)
        {
            var ch = (Character)obj;
            return Position.CompareTo(ch.Position);
        }

        internal void EngageDungeon(uint dgId)
        {
            var dg = Dungeons.FirstOrDefault(x => x.Id == dgId);
            if (dg != null)
            {
                dg.Entries = dg.Entries == 0 ? Convert.ToInt16(dg.GetRuns() - 1) : Convert.ToInt16(dg.Entries - 1);
            }
        }

        public void ClearGear()
        {
            Gear = new SynchronizedObservableCollection<GearItem>(_dispatcher);
        }

        public void UpdateGear(List<GearItem> gear)
        {
            foreach (var gearItem in gear)
            {
                if (!Gear.Contains(gearItem))
                {
                    Gear.Add(gearItem);
                }
            }
        }
    }
}
