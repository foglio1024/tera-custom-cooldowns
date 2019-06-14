using FoglioUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Controls;
using TCC.Data.Abnormalities;
using TCC.Data.Map;
using TeraDataLite;

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
        private int _clearedGuardianQuests;
        private int _elleonMarks;
        private int _dragonwingScales;
        private int _piecesOfDragonScroll;
        private int _itemLevel;
        private int _level;
        private Location _lastLocation;
        private long _lastOnline;
        private string _serverName = "";
        private bool _hidden;

        private uint _coins;
        public uint Coins
        {
            get { return _coins; }
            set
            {
                if (_coins == value) return;
                _coins = value;
                N();
            }
        }
        private uint _maxCoins;
        public uint MaxCoins
        {
            get { return _maxCoins; }
            set
            {
                if (_maxCoins == value) return;
                _maxCoins = value;
                N();

            }
        }


        public uint Id { get; set; }
        public int Position { get; set; }
        public string Name
        {
            get => _name; set
            {
                if (_name == value) return;
                _name = value;
                N(nameof(Name));
            }
        }
        public Class Class
        {
            get => _class; set
            {
                if (_class == value) return;
                _class = value;
                N(nameof(Class));
            }
        }
        public Laurel Laurel
        {
            get => _laurel; set
            {
                if (_laurel == value) return;
                _laurel = value;
                N(nameof(Laurel));
            }
        }
        public int Level
        {
            get => _level;
            set
            {
                if (_level == value) return;
                _level = value;
                N();
            }
        }
        public int VanguardDailiesDone
        {
            get => _vanguardDailiesDone;
            set
            {
                if (_vanguardDailiesDone == value) return;
                _vanguardDailiesDone = value;
                N(nameof(VanguardDailiesDone));
                N(nameof(VanguardDailyCompletion));
            }
        }
        public string GuildName { get; set; } = "";
        public void UpdateDungeons(Dictionary<uint, short> dungeonCooldowns)
        {
            Dungeons.ToSyncList().ForEach(dung =>
            {
                if (dungeonCooldowns.TryGetValue(dung.Dungeon.Id, out var entries)) dung.Entries = entries;
                else dung.Reset();
            });
        }
        public void SetDungeonClears(uint dgId, int runs)
        {
            var dg = Dungeons.ToSyncList().FirstOrDefault(d => d.Dungeon.Id == dgId);
            if (dg != null) dg.Clears = runs;
        }
        public int VanguardWeekliesDone
        {
            get => _vanguardWeekliesDone;
            set
            {
                if (_vanguardWeekliesDone == value) return;
                _vanguardWeekliesDone = value;
                N(nameof(VanguardWeekliesDone));
                N(nameof(VanguardWeeklyCompletion));

            }
        }
        public int VanguardCredits
        {
            get => _vanguardCredits;
            set
            {
                if (_vanguardCredits == value) return;
                _vanguardCredits = value;
                N(nameof(VanguardCredits));
                N(nameof(VanguardCreditsFactor));
            }
        }
        public int GuardianCredits
        {
            get => _guardianCredits;
            set
            {
                if (_guardianCredits == value) return;
                _guardianCredits = value;
                N(nameof(GuardianCredits));
                N(nameof(GuardianCreditsFactor));
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
                N(nameof(IsLoggedIn));
            }
        }
        public bool IsSelected
        {
            get => _isSelected; set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                N(nameof(IsSelected));
            }
        }
        public double VanguardWeeklyCompletion => VanguardWeekliesDone / (double)Session.MaxWeekly;
        public double VanguardDailyCompletion => VanguardDailiesDone / (double)Session.MaxDaily;
        public double ClaimedGuardianCompletion => ClaimedGuardianQuests / (double)MaxGuardianQuests;
        public double ClearedGuardianCompletion => ClearedGuardianQuests / (double)MaxGuardianQuests;

        public int ItemLevel  // 412 431 - 439 446 453 456
        {
            get => _itemLevel;
            set
            {
                if (_itemLevel == value) return;
                _itemLevel = value;
                N();
                N(nameof(ItemLevelTier));
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

        public GearItem Weapon => Gear.ToSyncList().FirstOrDefault(x => x.Piece == GearPiece.Weapon) ?? new GearItem(0, GearTier.Low, GearPiece.Weapon, 0, 0);
        public GearItem Chest => Gear.ToSyncList().FirstOrDefault(x => x.Piece == GearPiece.Armor) ?? new GearItem(0, GearTier.Low, GearPiece.Armor, 0, 0);
        public GearItem Hands => Gear.ToSyncList().FirstOrDefault(x => x.Piece == GearPiece.Hands) ?? new GearItem(0, GearTier.Low, GearPiece.Hands, 0, 0);
        public GearItem Feet => Gear.ToSyncList().FirstOrDefault(x => x.Piece == GearPiece.Feet) ?? new GearItem(0, GearTier.Low, GearPiece.Feet, 0, 0);
        public GearItem Belt => Gear.ToSyncList().FirstOrDefault(x => x.Piece == GearPiece.Belt) ?? new GearItem(0, GearTier.Low, GearPiece.Belt, 0, 0);
        public GearItem Circlet => Gear.ToSyncList().FirstOrDefault(x => x.Piece == GearPiece.Circlet) ?? new GearItem(0, GearTier.Low, GearPiece.Circlet, 0, 0);
        public ICollectionView Jewels { get; set; }

        public int ClaimedGuardianQuests
        {
            get => _claimedGuardianQuests;
            set
            {
                if (_claimedGuardianQuests == value) return;
                _claimedGuardianQuests = value;
                N();
                N(nameof(ClaimedGuardianCompletion));
            }
        }
        public int MaxGuardianQuests
        {
            get => _maxGuardianQuests; set
            {
                if (_maxGuardianQuests == value) return;
                _maxGuardianQuests = value;
                N();
                N(nameof(ClaimedGuardianCompletion));
            }
        }

        public int ElleonMarks
        {
            get => _elleonMarks; set
            {
                if (_elleonMarks == value) return;
                _elleonMarks = value;
                N();
                N(nameof(ElleonMarksFactor));
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
                N();
                N(nameof(ClearedGuardianCompletion));
            }

        }

        public int DragonwingScales
        {
            get => _dragonwingScales;
            set
            {
                if (_dragonwingScales == value) return;
                _dragonwingScales = value;
                N();
                N(nameof(DragonwingScalesFactor));
            }
        }
        public int PiecesOfDragonScroll
        {
            get => _piecesOfDragonScroll;
            set
            {
                if (_piecesOfDragonScroll == value) return;
                _piecesOfDragonScroll = value;
                N();
                N(nameof(PiecesOfDragonScrollFactor));
            }
        }
        public float DragonwingScalesFactor => DragonwingScales > 10 ? 1 : DragonwingScales / 10f;
        public float PiecesOfDragonScrollFactor => PiecesOfDragonScroll > 40 ? 1 : PiecesOfDragonScroll / 40f;

        public long LastOnline
        {
            get => _lastOnline;
            set
            {
                if (_lastOnline == value) return;
                _lastOnline = value;
                N();
            }
        }

        public Location LastLocation
        {
            get => _lastLocation;
            set
            {
                if (_lastLocation == value) return;
                _lastLocation = value;
                N();
            }
        }

        public SynchronizedObservableCollection<AbnormalityData> Buffs { get; set; }
        public SynchronizedObservableCollection<InventoryItem> Inventory { get; }

        public string ServerName
        {
            get => _serverName;
            set
            {
                if (_serverName == value) return;
                _serverName = value;
                N();
            }
        }

        public bool Hidden
        {
            get => _hidden; set
            {
                if (_hidden == value) return;
                _hidden = value;
                N();
            }
        }

        public Character()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Dungeons = new SynchronizedObservableCollection<DungeonCooldown>(Dispatcher);
            Gear = new SynchronizedObservableCollection<GearItem>(Dispatcher);
            Buffs = new SynchronizedObservableCollection<AbnormalityData>(Dispatcher);
            Inventory = new SynchronizedObservableCollection<InventoryItem>(Dispatcher);
            VanguardDailiesDone = 0;
            VanguardWeekliesDone = 0;
            Laurel = Laurel.None;
            MaxGuardianQuests = Session.MaxGuardianQuests;
            foreach (var dg in Session.DB.DungeonDatabase.Dungeons.Values)
            {
                Dungeons.Add(new DungeonCooldown(dg, Dispatcher, this));
            }
            VisibleDungeonsView = CollectionViewUtils.InitLiveView(d => ((DungeonCooldown)d).Dungeon.Show, Dungeons, new string[] { },
                new[]
                    {new SortDescription(nameof(Dungeon.Index), ListSortDirection.Ascending)});
            Jewels = new CollectionViewSource() { Source = Gear }.View;
            Jewels.Filter = g => ((GearItem)g).IsJewel && ((GearItem)g).Piece < GearPiece.Circlet;
            Jewels.SortDescriptions.Add(new SortDescription("Piece", ListSortDirection.Ascending));
            UnhideCommand = new RelayCommand((o) => Dispatcher.Invoke(() => Hidden = false));
        }
        public Character(string name, Class c, uint id, int pos) : this()
        {
            Name = name;
            Class = c;
            Id = id;
            Position = pos;
        }

        public Character(CharacterData item) : this()
        {
            Id = item.Id;
            Class = item.CharClass;
            Level = item.Level;
            LastLocation = new Location(item.LastWorldId, item.LastGuardId, item.LastSectionId);
            LastOnline = item.LastOnline;
            Laurel = item.Laurel;
            Position = item.Position;
            Name = item.Name;
            GuildName = item.GuildName;
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

        public RelayCommand UnhideCommand { get; }

        public void ResetWeeklyDungeons()
        {
            Dungeons.Where(d => d.Dungeon.ResetMode == ResetMode.Weekly).ToList().ForEach(dg => dg.Reset());
        }

        public void ResetDailyData()
        {
            VanguardDailiesDone = 0;
            ClaimedGuardianQuests = 0;
            Dungeons.Where(d => d.Dungeon.ResetMode == ResetMode.Daily).ToList().ForEach(dg => dg.Reset());
        }
    }

    public class InventoryItem : TSPropertyChanged
    {
        private int _amount;
        private readonly uint _id;

        public Item Item => Session.DB.ItemsDatabase.Items.TryGetValue(_id, out var item)
                            ? item
                            : new Item(0, "", RareGrade.Common, 0, 0, "");
        public uint Slot { get; }
        public int Amount
        {
            get => _amount;
            set
            {
                if (_amount == value) return;
                _amount = value;
                N();
            }
        }

        public InventoryItem(uint slot, uint id, int amount)
        {
            _id = id;
            Amount = amount;
            Slot = slot;
        }
    }
}
