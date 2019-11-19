using FoglioUtils;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
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
        private Laurel _laurel = Laurel.None;
        private bool _isLoggedIn;
        private bool _isSelected;
        private int _elleonMarks;
        private int _dragonwingScales;
        private int _piecesOfDragonScroll;
        private float _itemLevel;
        private int _level;
        private Location _lastLocation;
        private long _lastOnline;
        private string _serverName = "";
        private bool _hidden;
        private uint _coins;
        private uint _maxCoins;

        public uint Id { get; set; }
        public int Position { get; set; }
        public string GuildName { get; set; } = "";
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
        public float ItemLevel
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
        public uint Coins
        {
            get => _coins;
            set
            {
                if (_coins == value) return;
                _coins = value;
                N();
                DungeonInfo.UpdateAvailableEntries(_coins, _maxCoins);
            }
        }
        public uint MaxCoins
        {
            get => _maxCoins;
            set
            {
                if (_maxCoins == value) return;
                _maxCoins = value;
                N();
                DungeonInfo.UpdateAvailableEntries(_coins, _maxCoins);
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
        public bool Hidden
        {
            get => _hidden; set
            {
                if (_hidden == value) return;
                _hidden = value;
                N();
            }
        }

        public GuardianInfo GuardianInfo { get; }
        public VanguardInfo VanguardInfo { get; }
        public DungeonInfo DungeonInfo { get; }

        public TSObservableCollection<AbnormalityData> Buffs { get; }
        public TSObservableCollection<InventoryItem> Inventory { get; }

        [JsonIgnore]
        public ICommand UnhideCommand { get; }

        //[JsonIgnore]
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
        [JsonIgnore]
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
        [JsonIgnore]
        public bool IsSelected
        {
            get => _isSelected; set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                N(nameof(IsSelected));
            }
        }
        [JsonIgnore]
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
        [JsonIgnore] public float ElleonMarksFactor => (float)MathUtils.FactorCalc(ElleonMarks, 1000);
        [JsonIgnore] public float DragonwingScalesFactor => (float)MathUtils.FactorCalc(DragonwingScales, 10);
        [JsonIgnore] public float PiecesOfDragonScrollFactor => (float)MathUtils.FactorCalc(PiecesOfDragonScroll, 40);

        public Character()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Buffs = new TSObservableCollection<AbnormalityData>(Dispatcher);
            Inventory = new TSObservableCollection<InventoryItem>(Dispatcher);
            GuardianInfo = new GuardianInfo();
            VanguardInfo = new VanguardInfo();
            DungeonInfo = new DungeonInfo();
            UnhideCommand = new RelayCommand(_ => Hidden = false);
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

        public void ResetDailyData()
        {
            VanguardInfo.DailiesDone = 0;
            GuardianInfo.Claimed = 0;
            DungeonInfo.ResetAll(ResetMode.Daily);
        }

        int IComparable.CompareTo(object obj)
        {
            var ch = (Character)obj;
            return Position.CompareTo(ch.Position);
        }
    }
}
