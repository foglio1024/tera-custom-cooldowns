using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;

namespace TCC.Data.Pc
{
    //TODO: remove INPC from properties where it's not needed
    public class Character : TSPropertyChanged, IComparable
    {
        private string _name;
        private Class _class;
        private Laurel _laurel;
        private int _dailiesDone;
        private int _weekliesDone;
        private int _credits;
        private bool _isLoggedIn;
        private bool _isSelected;
        private int _claimedGuardianQuests;
        private int _maxGuardianQuests = 40;
        private uint _elleonMarks;
        private int _clearedGuardianQuests;

        public uint Id { get; set; }
        public int Position { get; set; }
        public string Name
        {
            get => _name; set
            {
                if (_name == value) return;
                _name = value;
                NPC(nameof(Name));
            }
        }
        public Class Class
        {
            get => _class; set
            {
                if (_class == value) return;
                _class = value;
                NPC(nameof(Class));
            }
        }
        public Laurel Laurel
        {
            get => _laurel; set
            {
                if (_laurel == value) return;
                _laurel = value;
                NPC(nameof(Laurel));
            }
        }
        public int DailiesDone
        {
            get => _dailiesDone;
            set
            {
                if (_dailiesDone == value) return;
                _dailiesDone = value;
                NPC(nameof(DailiesDone));
                NPC(nameof(VanguardDailyCompletion));
            }
        }

        public string GuildName { get; set; } = "";
        public void UpdateDungeons(Dictionary<uint, short> dungeonCooldowns)
        {
            foreach (var keyVal in dungeonCooldowns)
            {
                if (!SessionManager.DungeonDatabase.DungeonDefs.ContainsKey(keyVal.Key)) continue;
                var dg = Dungeons.FirstOrDefault(x => x.Id == keyVal.Key);
                if (dg != null)
                {
                    dg.Entries = keyVal.Value;
                }
                //else
                //{
                //    Dungeons.Add(new DungeonCooldown(keyVal.Key, keyVal.Value, _dispatcher));
                //}
            }
        }
        public void SetDungeonTotalRuns(uint dgId, int runs)
        {
            var dg = Dungeons.ToSyncArray().FirstOrDefault(d => d.Id == dgId);
            if (dg != null) dg.Clears = runs;
        }
        public int WeekliesDone
        {
            get => _weekliesDone;
            set
            {
                if (_weekliesDone == value) return;
                _weekliesDone = value;
                NPC(nameof(WeekliesDone));
                NPC(nameof(VanguardWeeklyCompletion));

            }
        }
        public int Credits
        {
            get => _credits;
            set
            {
                if (_credits == value) return;
                _credits = value;
                NPC(nameof(Credits));
                NPC(nameof(CreditsFactor));
            }
        }
        public double CreditsFactor => Credits / 9000.0d;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                if (_isLoggedIn == value) return;
                _isLoggedIn = value;
                NPC(nameof(IsLoggedIn));
            }
        }
        public bool IsSelected
        {
            get => _isSelected; set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                NPC(nameof(IsSelected));
            }
        }
        public double VanguardWeeklyCompletion => WeekliesDone / (double)SessionManager.MaxWeekly;
        public double VanguardDailyCompletion => DailiesDone / (double)SessionManager.MaxDaily;
        public double GuardianCompletion => ClaimedGuardianQuests / (double)MaxGuardianQuests;

        public SynchronizedObservableCollection<DungeonCooldown> Dungeons { get; set; }
        public ICollectionView VisibleDungeons { get; set; }
        public SynchronizedObservableCollection<GearItem> Gear { get; set; }

        public GearItem Weapon => Gear.ToSyncArray().FirstOrDefault(x => x.Piece == GearPiece.Weapon) ?? new GearItem(0,GearTier.Low, GearPiece.Weapon,0,0);
        public GearItem Chest => Gear.ToSyncArray().FirstOrDefault(x => x.Piece == GearPiece.Armor) ?? new GearItem(0, GearTier.Low, GearPiece.Armor, 0, 0);
        public GearItem Hands => Gear.ToSyncArray().FirstOrDefault(x => x.Piece == GearPiece.Hands) ?? new GearItem(0, GearTier.Low, GearPiece.Hands, 0, 0);
        public GearItem Feet => Gear.ToSyncArray().FirstOrDefault(x => x.Piece == GearPiece.Feet) ?? new GearItem(0, GearTier.Low, GearPiece.Feet, 0, 0);
        public GearItem Belt => Gear.ToSyncArray().FirstOrDefault(x => x.Piece == GearPiece.Belt) ?? new GearItem(0, GearTier.Low, GearPiece.Belt, 0, 0);
        public GearItem Circlet => Gear.ToSyncArray().FirstOrDefault(x => x.Piece == GearPiece.Circlet) ?? new GearItem(0, GearTier.Low, GearPiece.Circlet, 0, 0);
        public ICollectionView Jewels { get; set; }

        public int ClaimedGuardianQuests
        {
            get => _claimedGuardianQuests;
            set
            {
                if (_claimedGuardianQuests == value) return;
                _claimedGuardianQuests = value;
                NPC();
                NPC(nameof(GuardianCompletion));
            }
        }
        public int MaxGuardianQuests
        {
            get => _maxGuardianQuests; set
            {
                if (_maxGuardianQuests == value) return;
                _maxGuardianQuests = value;
                NPC();
                NPC(nameof(GuardianCompletion));
            }
        }

        public uint ElleonMarks
        {
            get => _elleonMarks; set
            {
                if (_elleonMarks == value) return;
                _elleonMarks = value;
                NPC(nameof(ElleonMarks));
            }
        }

        public int ClearedGuardianQuests
        {
            get => _clearedGuardianQuests;
            set
            {
                if (_clearedGuardianQuests == value) return;
                _clearedGuardianQuests = value;
                NPC();
            }

        }

        public Character(string name, Class c, uint id, int pos, Dispatcher d, Laurel l = Laurel.None)
        {
            Dispatcher = d;
            Dungeons = new SynchronizedObservableCollection<DungeonCooldown>(Dispatcher);
            Gear = new SynchronizedObservableCollection<GearItem>(Dispatcher);
            Name = name;
            Class = c;
            Laurel = l;
            DailiesDone = 0;
            WeekliesDone = 0;
            Id = id;
            Position = pos;
            MaxGuardianQuests = SessionManager.MaxGuardianQuests;
            foreach (var dg in SessionManager.DungeonDatabase.DungeonDefs)
            {
                Dungeons.Add(new DungeonCooldown(dg.Key, Dispatcher));
            }
            VisibleDungeons = new CollectionViewSource() { Source = Dungeons }.View;
            VisibleDungeons.Filter = dc => SessionManager.DungeonDatabase.DungeonDefs.ContainsKey(((DungeonCooldown)dc).Id) &&
                                           SessionManager.DungeonDatabase.DungeonDefs[((DungeonCooldown)dc).Id].Show;
            VisibleDungeons.SortDescriptions.Add(new SortDescription("Tier", ListSortDirection.Ascending));

            Jewels = new CollectionViewSource() { Source = Gear }.View;
            Jewels.Filter = g => ((GearItem)g).IsJewel && ((GearItem)g).Piece < GearPiece.Circlet;
            Jewels.SortDescriptions.Add(new SortDescription("Piece", ListSortDirection.Ascending));

            //Utils.InitLiveView(dc => DungeonDatabase.Instance.Dungeons.ContainsKey(((DungeonCooldown)dc).Id) &&
            //DungeonDatabase.Instance.Dungeons[((DungeonCooldown)dc).Id].Show, Dungeons, new string[] { nameof(Dungeon.Show) }, new string[] { nameof(Dungeon.Tier) });
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
            Gear = new SynchronizedObservableCollection<GearItem>(Dispatcher);
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
