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
        private int _vanguardDailiesDone;
        private int _vanguardWeekliesDone;
        private int _vanguardCredits;
        private int _guardianCredits;
        private bool _isLoggedIn;
        private bool _isSelected;
        private int _claimedGuardianQuests;
        private int _maxGuardianQuests = 40;
        private uint _elleonMarks;
        private int _clearedGuardianQuests;
        private uint _dragonwingScales;
        private uint _piecesOfDragonScroll;
        private int _itemLevel;
        private int _level;

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
        public int Level
        {
            get => _level;
            set
            {
                if (_level == value) return;
                _level = value;
                NPC();
            }
        }
        public int VanguardDailiesDone
        {
            get => _vanguardDailiesDone;
            set
            {
                if (_vanguardDailiesDone == value) return;
                _vanguardDailiesDone = value;
                NPC(nameof(VanguardDailiesDone));
                NPC(nameof(VanguardDailyCompletion));
            }
        }
        public string GuildName { get; set; } = "";
        public void UpdateDungeons(Dictionary<uint, short> dungeonCooldowns)
        {
            Dungeons.ToSyncArray().ToList().ForEach(dung =>
            {
                if (dungeonCooldowns.ContainsKey(dung.Dungeon.Id))
                    dung.Entries = dungeonCooldowns[dung.Dungeon.Id];
            });
        }
        public void SetDungeonClears(uint dgId, int runs)
        {
            var dg = Dungeons.ToSyncArray().FirstOrDefault(d => d.Dungeon.Id == dgId);
            if (dg != null) dg.Clears = runs;
        }
        public int VanguardWeekliesDone
        {
            get => _vanguardWeekliesDone;
            set
            {
                if (_vanguardWeekliesDone == value) return;
                _vanguardWeekliesDone = value;
                NPC(nameof(VanguardWeekliesDone));
                NPC(nameof(VanguardWeeklyCompletion));

            }
        }
        public int VanguardCredits
        {
            get => _vanguardCredits;
            set
            {
                if (_vanguardCredits == value) return;
                _vanguardCredits = value;
                NPC(nameof(VanguardCredits));
                NPC(nameof(VanguardCreditsFactor));
            }
        }
        public int GuardianCredits
        {
            get => _guardianCredits;
            set
            {
                if (_guardianCredits == value) return;
                _guardianCredits = value;
                NPC(nameof(GuardianCredits));
                NPC(nameof(GuardianCreditsFactor));
            }
        }
        public double VanguardCreditsFactor => VanguardCredits / 9000.0d;
        public double GuardianCreditsFactor => GuardianCredits / 100000.0d;
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
        public double VanguardWeeklyCompletion => VanguardWeekliesDone / (double)SessionManager.MaxWeekly;
        public double VanguardDailyCompletion => VanguardDailiesDone / (double)SessionManager.MaxDaily;
        public double GuardianCompletion => ClaimedGuardianQuests / (double)MaxGuardianQuests;

        public int ItemLevel  // 412 431 - 439 446 453 456
        {
            get => _itemLevel;
            set
            {
                if (_itemLevel == value) return;
                _itemLevel = value;
                NPC();
                NPC(nameof(ItemLevelTier));
            }
        }
        public ItemLevelTier ItemLevelTier
        {
            get
            {
                var tiers = Enum.GetValues(typeof(ItemLevelTier)).Cast<ItemLevelTier>().ToList();
                var ret = ItemLevelTier.Tier0;
                foreach (var t in tiers)
                {
                    if (_itemLevel >= (int)t) ret = t;
                }

                return ret;
            }
        }


        public SynchronizedObservableCollection<DungeonCooldown> Dungeons { get; set; }
        public List<DungeonCooldown> VisibleDungeons => Dungeons.Where(x => x.Dungeon.Show).ToList();
        public ICollectionViewLiveShaping VisibleDungeonsView { get; set; }
        public SynchronizedObservableCollection<GearItem> Gear { get; set; }

        public GearItem Weapon => Gear.ToSyncArray().FirstOrDefault(x => x.Piece == GearPiece.Weapon) ?? new GearItem(0, GearTier.Low, GearPiece.Weapon, 0, 0);
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
                NPC();
                NPC(nameof(ElleonMarksFactor));
            }
        }

        public float ElleonMarksFactor => ElleonMarks / 1000f;
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

        public uint DragonwingScales
        {
            get => _dragonwingScales;
            set
            {
                if (_dragonwingScales == value) return;
                _dragonwingScales = value;
                NPC();
                NPC(nameof(DragonwingScalesFactor));
            }
        }
        public uint PiecesOfDragonScroll
        {
            get => _piecesOfDragonScroll;
            set
            {
                if (_piecesOfDragonScroll == value) return;
                _piecesOfDragonScroll = value;
                NPC();
                NPC(nameof(PiecesOfDragonScrollFactor));
            }
        }
        public float DragonwingScalesFactor => DragonwingScales > 10 ? 1 : DragonwingScales / 10f;
        public float PiecesOfDragonScrollFactor => PiecesOfDragonScroll > 40 ? 1 : PiecesOfDragonScroll / 40f;


        public Character()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Dungeons = new SynchronizedObservableCollection<DungeonCooldown>(Dispatcher);
            Gear = new SynchronizedObservableCollection<GearItem>(Dispatcher);
            VanguardDailiesDone = 0;
            VanguardWeekliesDone = 0;
            Laurel = Laurel.None;
            MaxGuardianQuests = SessionManager.MaxGuardianQuests;
            foreach (var dg in SessionManager.DungeonDatabase.Dungeons.Values)
            {
                Dungeons.Add(new DungeonCooldown(dg, Dispatcher));
            }
            //VisibleDungeonsView = new CollectionViewSource() { Source = Dungeons }.View;
            //VisibleDungeonsView.Filter = dc => ((DungeonCooldown) dc).Dungeon.Show;
            //VisibleDungeonsView.SortDescriptions.Add(new SortDescription(nameof(DungeonCooldown.RequiredIlvl), ListSortDirection.Ascending));
            VisibleDungeonsView = Utils.InitLiveView(d => ((DungeonCooldown)d).Dungeon.Show, Dungeons, new string[] { },
                new[]
                    {new SortDescription(nameof(Dungeon.Index), ListSortDirection.Ascending)});
            Jewels = new CollectionViewSource() { Source = Gear }.View;
            Jewels.Filter = g => ((GearItem)g).IsJewel && ((GearItem)g).Piece < GearPiece.Circlet;
            Jewels.SortDescriptions.Add(new SortDescription("Piece", ListSortDirection.Ascending));


        }
        public Character(string name, Class c, uint id, int pos) : this()
        {
            Name = name;
            Class = c;
            Id = id;
            Position = pos;
        }

        public int CompareTo(object obj)
        {
            var ch = (Character)obj;
            return Position.CompareTo(ch.Position);
        }

        internal void EngageDungeon(uint dgId)
        {
            var dg = Dungeons.FirstOrDefault(x => x.Dungeon.Id == dgId);
            if (dg != null)
            {
                dg.Entries = dg.Entries == 0
                    ? dg.Runs - 1
                    : dg.Entries - 1;
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
