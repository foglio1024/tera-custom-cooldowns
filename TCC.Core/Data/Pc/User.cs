using Nostrum;

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using TCC.Data.Abnormalities;
using TCC.Interop.Proxy;
using TCC.UI;
using TeraDataLite;

namespace TCC.Data.Pc
{
    //TODO: remove INPC from properties where it's not needed
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
        private string _name = "";
        private long _currentHp = 1;
        private int _currentMp = 1;
        private int _currentSt = 1;
        private long _maxHp = 1;
        private int _maxMp = 1;
        private int _maxSt = 1;
        private ReadyStatus _ready = ReadyStatus.None;
        private bool _alive = true;
        private int _rollResult;
        private bool _isRolling;
        private bool _isWinning;
        private bool _isLeader;
        private bool _hasAggro;
        private string _location = "";
        private readonly List<uint> _debuffList = new();
        private GearItem? _weapon;
        private GearItem? _armor;
        private GearItem? _gloves;
        private GearItem? _boots;
        private bool _visible = true;
        private bool _inRange;

        public ulong EntityId
        {
            get => _entityId;
            set
            {
                if (_entityId == value) return;
                _entityId = value;
                N(nameof(EntityId));
            }
        }
        public uint Level
        {
            get => _level;
            set
            {
                if (_level == value) return;
                _level = value;
                N(nameof(Level));
            }
        }
        public Class UserClass
        {
            get => _userClass;
            set
            {
                if (_userClass == value) return;
                _userClass = value;
                Role = value switch
                {
                    Class.Lancer => Role.Tank,
                    Class.Priest => Role.Healer,
                    Class.Mystic => Role.Healer,
                    Class.Brawler => Role.Tank,
                    _ => Role.Dps
                };
                N(nameof(UserClass));
            }
        }
        public Role Role
        {
            get => _role;
            set
            {
                if (_role == value) return;
                _role = value;
                N(nameof(Role));
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
                N(nameof(Online));
            }
        }
        public uint ServerId
        {
            get => _serverId;
            set
            {
                if (_serverId == value) return;
                _serverId = value;
                N(nameof(ServerId));
            }
        }
        public uint PlayerId
        {
            get => _playerId;
            set
            {
                if (_playerId == value) return;
                _playerId = value;
                N(nameof(PlayerId));
            }
        }
        public int Order
        {
            get => _order;
            set
            {
                if (_order == value) return;
                _order = value;
                N(nameof(Order));
            }
        }
        public bool CanInvite
        {
            get => _canInvite;
            set
            {
                if (_canInvite == value) return;
                _canInvite = value;
                N(nameof(CanInvite));
            }
        }
        public Laurel Laurel
        {
            get => _laurel;
            set
            {
                if (_laurel == value) return;
                _laurel = value;
                N(nameof(Laurel));
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                N(nameof(Name));
            }
        }
        public long CurrentHp
        {
            get => _currentHp;
            set
            {
                if (_currentHp == value) return;
                _currentHp = value;
                N(nameof(CurrentHp));
                N(nameof(HpFactor));
            }
        }
        public int CurrentMp
        {
            get => _currentMp;
            set
            {
                if (_currentMp == value) return;
                _currentMp = value;
                N(nameof(CurrentMp));
                N(nameof(MpFactor));
            }
        }
        public int CurrentSt
        {
            get => _currentSt;
            set
            {
                if (_currentSt == value) return;
                _currentSt = value;
                N();
                N(nameof(StFactor));
            }
        }
        public long MaxHp
        {
            get => _maxHp;
            set
            {
                if (_maxHp == value) return;
                _maxHp = value;
                N(nameof(MaxHp));
                N(nameof(HpFactor));
            }
        }
        public int MaxMp
        {
            get => _maxMp;
            set
            {
                if (_maxMp == value) return;
                _maxMp = value;
                N(nameof(MaxMp));
                N(nameof(MpFactor));
            }
        }
        public int MaxSt
        {
            get => _maxSt;
            set
            {
                if (_maxSt == value) return;
                _maxSt = value;
                N();
                N(nameof(StFactor));
            }
        }
        public double HpFactor => MathUtils.FactorCalc(CurrentHp, MaxHp);
        public double MpFactor => MathUtils.FactorCalc(CurrentMp, MaxMp);
        public double StFactor => MathUtils.FactorCalc(CurrentSt, MaxSt);
        public ReadyStatus Ready
        {
            get => _ready;
            set
            {
                if (_ready == value) return;
                _ready = value;
                N(nameof(Ready));
            }
        }
        public bool Alive
        {
            get => _alive;
            set
            {
                if (_alive == value) return;
                _alive = value;
                N(nameof(Alive));
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
                N(nameof(RollResult));
            }
        }
        public bool IsRolling
        {
            get => _isRolling;
            set
            {
                if (_isRolling == value) return;
                _isRolling = value;
                N(nameof(IsRolling));
            }
        }
        public bool IsWinning
        {
            get => _isWinning;
            set
            {
                if (_isWinning == value) return;
                _isWinning = value;
                N(nameof(IsWinning));
            }
        }
        public bool IsLeader
        {
            get => _isLeader;
            set
            {
                if (_isLeader == value) return;
                _isLeader = value;
                N(nameof(IsLeader));
            }
        }
        public bool IsPlayer => Name == Game.Me.Name;
        public bool IsDebuffed => _debuffList.Count != 0;
        public bool HasAggro
        {
            get => _hasAggro; set
            {
                if (_hasAggro == value) return;
                _hasAggro = value;
                N(nameof(HasAggro));
            }
        }
        public string Location
        {
            get => _location;
            set
            {
                if (_location == value) return;
                _location = value;
                N(nameof(Location));
            }
        }
        public GearItem? Weapon
        {
            get => _weapon;
            set
            {
                if (_weapon == value) return;
                _weapon = value;
                N(nameof(Weapon));
            }
        }
        public GearItem? Armor
        {
            get => _armor;
            set
            {
                if (_armor == value) return;
                _armor = value;
                N(nameof(Armor));
            }
        }
        public GearItem? Gloves
        {
            get => _gloves;
            set
            {
                if (_gloves == value) return;
                _gloves = value;
                N(nameof(Gloves));
            }
        }
        public GearItem? Boots
        {
            get => _boots;
            set
            {
                if (_boots == value) return;
                _boots = value;
                N(nameof(Boots));
            }
        }
        public TSObservableCollection<AbnormalityDuration> Buffs { get; }
        public TSObservableCollection<AbnormalityDuration> Debuffs { get; }
        public bool Awakened { get; set; }
        public bool InCombat { get; set; } //make npc when needed
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible == value) return;
                _visible = value;
                N();
            }
        }
        public bool InRange
        {
            get => _inRange || IsPlayer;
            set
            {
                if (_inRange == value) return;
                _inRange = value;
                N();
            }
        }

        public ICommand RequestInteractiveCommand { get; }
        public ICommand AcceptApplyCommand { get; set; }
        public ICommand DeclineApplyCommand { get; set; }
        public ICommand InspectCommand { get; set; }


        public void AddOrRefreshBuff(Abnormality ab, uint duration, int stacks)
        {
            if (!App.Settings.GroupWindowSettings.ShowAllAbnormalities)
            {
                if (App.Settings.GroupWindowSettings.GroupAbnormals.TryGetValue(Class.Common, out var commonList))
                {
                    if (!commonList.Contains(ab.Id))
                    {
                        if (App.Settings.GroupWindowSettings.GroupAbnormals.TryGetValue(Game.Me.Class, out var classList))
                        {
                            if (!classList.Contains(ab.Id)) return;
                        }
                        else return;
                    }
                }
                else return;
            }

            var existing = Buffs.ToSyncList().FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, Dispatcher, false);
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
                N(nameof(IsDebuffed));
            }

            var existing = Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, Dispatcher, false/*, size * .9, size, new Thickness(margin, 1, 1, 1)*/);

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
                N(nameof(IsDebuffed));
            }
            var buff = Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (buff == null) return;
            Debuffs.Remove(buff);
            buff.Dispose();
        }
        public void ClearAbnormalities()
        {
            Dispatcher.Invoke(() =>
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
            });
        }

        public User(Dispatcher? d)
        {
            Dispatcher = d ?? Dispatcher.CurrentDispatcher;
            Debuffs = new TSObservableCollection<AbnormalityDuration>(Dispatcher);
            Buffs = new TSObservableCollection<AbnormalityDuration>(Dispatcher);

            AcceptApplyCommand = new RelayCommand(_ =>
            {
                StubInterface.Instance.StubClient.GroupInviteUser(Name, Game.Group.IsRaid);
            });
            DeclineApplyCommand = new RelayCommand(_ =>
            {
                StubInterface.Instance.StubClient.DeclineUserGroupApply(PlayerId);
                StubInterface.Instance.StubClient.RequestListingCandidates();
            });
            RequestInteractiveCommand = new RelayCommand(_ =>
            {
                WindowManager.ViewModels.PlayerMenuVM.Open(Name, ServerId, (int)Level);
                //ProxyInterface.Instance.Stub.AskInteractive(ServerId, Name);
            });
            InspectCommand = new RelayCommand(_ =>
            {
                StubInterface.Instance.StubClient.InspectUser(Name);
            });

        }

        public User(GroupMemberData other, Dispatcher? d = null) : this(d)
        {
            PlayerId = other.PlayerId;
            UserClass = other.Class;
            Level = other.Level;
            Order = other.Order;
            Location = Game.DB!.GetSectionName(other.GuardId, other.SectionId);
            IsLeader = other.IsLeader;
            Online = other.Online;
            Name = other.Name;
            ServerId = other.ServerId == 0 ? Game.Me.ServerId : other.ServerId;
            EntityId = other.EntityId;
            CanInvite = other.CanInvite;
            Laurel = other.Laurel;
            Awakened = other.Awakened;
            Alive = other.Alive;
            CurrentHp = other.CurrentHP;
            CurrentMp = other.CurrentMP;
            MaxHp = other.MaxHP;
            MaxMp = other.MaxMP;
            CurrentSt = other.CurrentST;
            MaxSt = other.MaxST;
            InCombat = other.InCombat;

        }
    }
}