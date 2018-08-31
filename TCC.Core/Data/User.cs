using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace TCC.Data
{
    public class User : TSPropertyChanged
    {


        private ulong _entityId;
        private uint _level;
        private Class _userClass;
        private Role _role;
        private bool _online;
        private uint _serverId;
        private uint _playerId;
        private int _order;
        private bool _canInvite;
        private Laurel _laurel;
        private string _name;
        private long _currentHp = 1;
        private int _currentMp = 1;
        private long _maxHp = 1;
        private int _maxMp = 1;
        private ReadyStatus _ready = ReadyStatus.None;
        private bool _alive = true;
        private int _rollResult;
        private bool _isRolling;
        private bool _isWinning;
        private bool _isLeader;
        private bool _hasAggro;
        private string _location;
        private readonly List<uint> _debuffList = new List<uint>();
        private GearItem _weapon;
        private GearItem _armor;
        private GearItem _gloves;
        private GearItem _boots;

        public ulong EntityId
        {
            get => _entityId;
            set
            {
                if (_entityId == value) return;
                _entityId = value;
                NPC(nameof(EntityId));
            }
        }
        public uint Level
        {
            get => _level;
            set
            {
                if (_level == value) return;
                _level = value;
                NPC(nameof(Level));
            }
        }
        public Class UserClass
        {
            get => _userClass;
            set
            {
                if (_userClass == value) return;
                _userClass = value;
                switch (value)
                {
                    case Class.Lancer:
                        Role = Role.Tank;
                        break;
                    case Class.Priest:
                        Role = Role.Healer;
                        break;
                    case Class.Mystic:
                        Role = Role.Healer;
                        break;
                    case Class.Brawler:
                        Role = Role.Tank;
                        break;
                    default:
                        Role = Role.Dps;
                        break;
                }
                NPC(nameof(UserClass));
            }
        }
        public Role Role
        {
            get => _role;
            set
            {
                if (_role == value) return;
                _role = value;
                NPC(nameof(Role));
            }
        }
        public bool Online
        {
            get => _online;
            set
            {
                if (_online == value) return;
                _online = value;
                if (!_online) { CurrentHp = 0; CurrentMp = 0; }
                NPC(nameof(Online));
            }
        }
        public uint ServerId
        {
            get => _serverId;
            set
            {
                if (_serverId == value) return;
                _serverId = value;
                NPC(nameof(ServerId));
            }
        }
        public uint PlayerId
        {
            get => _playerId;
            set
            {
                if (_playerId == value) return;
                _playerId = value;
                NPC(nameof(PlayerId));
            }
        }
        public int Order
        {
            get => _order;
            set
            {
                if (_order == value) return;
                _order = value;
                NPC(nameof(Order));
            }
        }
        public bool CanInvite
        {
            get => _canInvite;
            set
            {
                if (_canInvite == value) return;
                _canInvite = value;
                NPC(nameof(CanInvite));
            }
        }
        public Laurel Laurel
        {
            get => _laurel;
            set
            {
                if (_laurel == value) return;
                _laurel = value;
                NPC(nameof(Laurel));
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                NPC(nameof(Name));
            }
        }
        public long CurrentHp
        {
            get => _currentHp;
            set
            {
                if (_currentHp == value) return;
                _currentHp = value;
                NPC(nameof(CurrentHp));
                NPC(nameof(HpFactor));
            }
        }
        public int CurrentMp
        {
            get => _currentMp;
            set
            {
                if (_currentMp == value) return;
                _currentMp = value;
                NPC(nameof(CurrentMp));
                NPC(nameof(MpFactor));
            }
        }
        public long MaxHp
        {
            get => _maxHp;
            set
            {
                if (_maxHp == value) return;
                _maxHp = value;
                NPC(nameof(MaxHp));
                NPC(nameof(HpFactor));
            }
        }
        public int MaxMp
        {
            get => _maxMp;
            set
            {
                if (_maxMp == value) return;
                _maxMp = value;
                NPC(nameof(MaxMp));
                NPC(nameof(MpFactor));
            }
        }
        public double HpFactor => Utils.FactorCalc(CurrentHp, MaxHp);
        public double MpFactor => Utils.FactorCalc(CurrentMp, MaxMp);
        public ReadyStatus Ready
        {
            get => _ready;
            set
            {
                if (_ready == value) return;
                _ready = value;
                NPC(nameof(Ready));
            }
        }
        public bool Alive
        {
            get => _alive;
            set
            {
                if (_alive == value) return;
                _alive = value;
                NPC(nameof(Alive));
            }
        }
        public int RollResult
        {
            get => _rollResult;
            set
            {
                if (_rollResult == value) return;
                _rollResult = value;
                if (_rollResult == -1) IsRolling = false;
                NPC(nameof(RollResult));
            }
        }
        public bool IsRolling
        {
            get => _isRolling;
            set
            {
                if (_isRolling == value) return;
                _isRolling = value;
                NPC(nameof(IsRolling));
            }
        }
        public bool IsWinning
        {
            get => _isWinning;
            set
            {
                if (_isWinning == value) return;
                _isWinning = value;
                NPC(nameof(IsWinning));
            }
        }
        public bool IsLeader
        {
            get => _isLeader;
            set
            {
                if (_isLeader == value) return;
                _isLeader = value;
                NPC(nameof(IsLeader));
            }
        }
        public bool IsPlayer => Name == SessionManager.CurrentPlayer.Name;
        public bool IsDebuffed => _debuffList.Count != 0;
        public bool HasAggro
        {
            get => _hasAggro; set
            {
                if (_hasAggro == value) return;
                _hasAggro = value;
                NPC(nameof(HasAggro));
            }
        }
        public string Location
        {
            get => _location;
            set
            {
                if (_location == value) return;
                _location = value;
                NPC(nameof(Location));
            }
        }
        public GearItem Weapon
        {
            get => _weapon;
            set
            {
                if (_weapon == value) return;
                _weapon = value;
                NPC(nameof(Weapon));
            }
        }
        public GearItem Armor
        {
            get => _armor;
            set
            {
                if (_armor == value) return;
                _armor = value;
                NPC(nameof(Armor));
            }
        }
        public GearItem Gloves
        {
            get => _gloves;
            set
            {
                if (_gloves == value) return;
                _gloves = value;
                NPC(nameof(Gloves));
            }
        }
        public GearItem Boots
        {
            get => _boots;
            set
            {
                if (_boots == value) return;
                _boots = value;
                NPC(nameof(Boots));
            }
        }
        public SynchronizedObservableCollection<AbnormalityDuration> Buffs { get; }
        public SynchronizedObservableCollection<AbnormalityDuration> Debuffs { get; }
        public bool Awakened { get; set; }

        public void AddOrRefreshBuff(Abnormality ab, uint duration, int stacks)
        {
            if (!Settings.ShowAllGroupAbnormalities)
            {
                if (Settings.GroupAbnormals.ContainsKey(Class.Common))
                {
                    if (!Settings.GroupAbnormals[Class.Common].Contains(ab.Id))
                    {
                        if (Settings.GroupAbnormals.ContainsKey(SessionManager.CurrentPlayer.Class))
                        {
                            if (!Settings.GroupAbnormals[SessionManager.CurrentPlayer.Class].Contains(ab.Id)) return;
                        }
                        else return;
                    }
                }
                else return;
            }

            var existing = Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {

                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, _dispatcher, false);
                if (ab.Infinity) Buffs.Insert(0, newAb); else Buffs.Add(newAb);
                return;
            }
            existing.Duration = duration;
            existing.DurationLeft = duration;
            existing.Stacks = stacks;
            existing.Refresh();

        }
        public void AddOrRefreshDebuff(Abnormality ab, uint duration, int stacks)
        {
            if (!ab.IsBuff && !_debuffList.Contains(ab.Id))
            {
                _debuffList.Add(ab.Id);
                NPC(nameof(IsDebuffed));
            }

            var existing = Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, _dispatcher, false/*, size * .9, size, new Thickness(margin, 1, 1, 1)*/);

                Debuffs.Add(newAb);
                return;
            }
            existing.Duration = duration;
            existing.DurationLeft = duration;
            existing.Stacks = stacks;
            existing.Refresh();
        }
        public void RemoveBuff(Abnormality ab)
        {
            var buff = Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (buff == null) return;
            Buffs.Remove(buff);
            buff.Dispose();
        }
        public void RemoveDebuff(Abnormality ab)
        {
            if (!ab.IsBuff)
            {
                _debuffList.Remove(ab.Id);
                NPC("IsDebuffed");
            }
            var buff = Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (buff == null) return;
            Debuffs.Remove(buff);
            buff.Dispose();
        }
        public void ClearAbnormalities()
        {
            foreach (var item in Buffs)
            {
                item.Dispose();
            }
            foreach (var item in Debuffs)
            {
                item.Dispose();
            }
            Buffs.Clear();
            Debuffs.Clear();
            _debuffList.Clear();
        }

        public User(Dispatcher d)
        {
            _dispatcher = d;
            Debuffs = new SynchronizedObservableCollection<AbnormalityDuration>(d);
            Buffs = new SynchronizedObservableCollection<AbnormalityDuration>(d);
        }
    }
}