using FoglioUtils;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using TCC.Controls;
using TCC.Data.Abnormalities;
using TCC.Utilities;
using TCC.ViewModels.Widgets;
using TeraDataLite;

namespace TCC.Data.NPCs
{
    public class NPC : TSPropertyChanged, IDisposable
    {
        public bool HasGage { get; set; }
        public ICommand Override { get; }
        public ulong EntityId { get; }
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    N(nameof(Name));
                }
            }
        }

        public bool IsBoss
        {
            get => _isBoss;
            set
            {
                if (_isBoss == value) return;
                _isBoss = value;
                N();
            }
        }

        private TSObservableCollection<AbnormalityDuration> _buffs;
        public TSObservableCollection<AbnormalityDuration> Buffs
        {
            get => _buffs;
            set
            {
                if (_buffs == value) return;
                _buffs = value;
                N();
            }
        }

        private bool _enraged;
        public bool Enraged
        {
            get => _enraged;
            set
            {
                if (_enraged == value) return;
                _enraged = value;
                N();
            }
        }

        private float _maxHP;
        public float MaxHP
        {
            get => _maxHP;
            set
            {
                if (_maxHP != value)
                {
                    _maxHP = value;
                    EnragePattern?.Update(value);
                    N();
                }
            }
        }

        private float _currentHP;
        public float CurrentHP
        {
            get => _currentHP;
            set
            {
                if (_currentHP != value)
                {
                    _currentHP = value;
                    N(nameof(CurrentHP));
                    N(nameof(CurrentPercentage));
                    N(nameof(HPFactor));
                }
            }
        }
        private uint _maxShield;
        public uint MaxShield
        {
            get => _maxShield;
            set
            {
                if (_maxShield != value)
                {
                    _maxShield = value;
                    N(nameof(MaxShield));
                    N(nameof(ShieldFactor));
                }
            }
        }
        private float _currentShield;
        public float CurrentShield
        {
            get => _currentShield;
            set
            {
                if (_currentShield == value) return;
                _currentShield = value;
                N(nameof(CurrentShield));
                N(nameof(ShieldFactor));
            }
        }

        public double ShieldFactor => MaxShield > 0 ? CurrentShield / MaxShield : 0;

        public float HPFactor => (float)MathUtils.FactorCalc(_currentHP, _maxHP); //_maxHP == 0 ? 0 : (_currentHP / _maxHP);
        public float CurrentPercentage => HPFactor * 100;
        private bool _visible;
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

        private ulong _target;
        public ulong Target
        {
            get => _target;
            set
            {
                if (_target == value) return;
                _target = value;
                N();
            }
        }

        private AggroCircle _currentAggroType = AggroCircle.None;
        public AggroCircle CurrentAggroType
        {
            get => _currentAggroType;
            set
            {
                if (_currentAggroType == value) return;
                _currentAggroType = value;
                N();
            }
        }

        public uint ZoneId { get; }
        public uint TemplateId { get; }

        public EnragePattern EnragePattern { get; set; }
        public TimerPattern TimerPattern { get; set; }

        public void AddorRefresh(Abnormality ab, uint duration, int stacks)
        {
            var existing = Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, _target, Dispatcher, true);
                if (ab.Infinity) Buffs.Insert(0, newAb);
                else Buffs.Add(newAb);
                if (ab.IsShield)
                {
                    MaxShield = ab.ShieldSize;
                    CurrentShield = ab.ShieldSize;
                }
                return;
            }
            existing.Duration = duration;
            existing.DurationLeft = duration;
            existing.Stacks = stacks;
            existing.Refresh();

        }
        public void EndBuff(Abnormality ab)
        {
            try
            {
                var buff = Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
                if (buff == null) return;

                Buffs.Remove(buff);
                buff.Dispose();

                if (!ab.IsShield) return;
                CurrentShield = 0;
                MaxShield = 0;
            }
            catch
            {
                // ignored
            }
        }

        public bool IsTower => TccUtils.IsGuildTower(ZoneId, TemplateId);
        public bool IsPhase1Dragon => TccUtils.IsPhase1Dragon(ZoneId, TemplateId);

        public uint GuildId
        {
            get
            {
                WindowManager.ViewModels.NpcVM.GuildIds.TryGetValue(EntityId, out var val);
                return val;
            }
        }

        //public NPC(ulong eId, uint zId, uint tId, float curHP, float maxHP, Visibility visible)
        //{
        //    _dispatcher = WindowManager.ViewModels.NPC.GetDispatcher();
        //    EntityId = eId;
        //    Name = EntityManager.MonsterDatabase.GetName(tId, zId);
        //    ZoneId = zId;
        //    TemplateId = tId;
        //    MaxHP = maxHP;
        //    CurrentHP = curHP;
        //    _buffs = new SynchronizedObservableCollection<AbnormalityDuration>(_dispatcher);
        //    Visible = visible;

        //    IsShieldPhase = false;
        //    IsSelected = false;
        //    if (WindowManager.ViewModels.NPC.CurrentHHphase == HarrowholdPhase.Phase1)
        //    {
        //        ShieldDuration = new DispatcherTimer();
        //        ShieldDuration.Interval = TimeSpan.FromSeconds(13);
        //        ShieldDuration.Tick += ShieldFailed;
        //    }

        //}
        public NPC(ulong eId, uint zId, uint tId, bool boss, bool visible, EnragePattern ep = null, TimerPattern tp = null)
        {
            Dispatcher = WindowManager.ViewModels.NpcVM.GetDispatcher();
            _buffs = new TSObservableCollection<AbnormalityDuration>(Dispatcher);
            Game.DB.MonsterDatabase.TryGetMonster(tId, zId, out var monster);
            Name = monster.Name;
            MaxHP = monster.MaxHP;
            EntityId = eId;
            ZoneId = zId;
            TemplateId = tId;
            IsBoss = boss;
            Visible = visible;
            CurrentHP = MaxHP;
            EnragePattern = ep ?? new EnragePattern(10, 36);
            TimerPattern = tp;
            TimerPattern?.SetTarget(this);
            if (IsPhase1Dragon)
            {
                _shieldDuration = new Timer { Interval = NpcWindowViewModel.Ph1ShieldDuration * 1000 };
                _shieldDuration.Elapsed += ShieldFailed;
            }
            Override = new RelayCommand(ex =>
            {
                Game.DB.MonsterDatabase.ToggleOverride(ZoneId, TemplateId, !IsBoss);

            }, ce => true);
        }

        public override string ToString()
        {
            return $"{EntityId} - {Name}";
        }

        public void Dispose()
        {
            foreach (var buff in _buffs) buff.Dispose();
            if (_shieldDuration != null) _shieldDuration.Elapsed -= ShieldFailed;

            _shieldDuration?.Dispose();
            TimerPattern?.Dispose();
            DeleteEvent?.Invoke();
        }

        ///////////////////TIMER////////////////////////


        //////////////////SHIELD////////////////////////
        //TODO: make this a separate class
        private readonly Timer _shieldDuration;

        private ShieldStatus _shield = ShieldStatus.Off;
        public ShieldStatus Shield
        {
            get => _shield;
            set
            {
                if (_shield == value) return;
                _shield = value;
                N(nameof(Shield));
            }
        }

        private bool _isSelected = true;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                N();
            }
        }

        private int _remainingEnrageTime;
        private bool _isBoss;

        public int RemainingEnrageTime
        {
            get => _remainingEnrageTime;
            set
            {
                if (_remainingEnrageTime == value) return;
                _remainingEnrageTime = value;
                N();
            }
        }

        private void ShieldFailed(object sender, EventArgs e)
        {
            _shieldDuration.Stop();
            Shield = ShieldStatus.Failed;
        }
        public void BreakShield()
        {
            _shieldDuration.Stop();
            Shield = ShieldStatus.Broken;
            Task.Delay(5000).ContinueWith(t =>
            {
                Shield = ShieldStatus.Off;
            });
        }
        public void StartShield()
        {
            _shieldDuration.Start();
            Shield = ShieldStatus.On;
        }

        public event Action DeleteEvent;
        public void Delete()
        {
            foreach (var buff in _buffs) buff.Dispose();

            DeleteEvent?.Invoke();
        }

        public void SetTimerPattern()
        {
            if (App.Settings.EthicalMode) return;

            // vergos ph4
            if (TemplateId == 4000 && ZoneId == 950) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            // nightmare kylos
            if (TemplateId == 3000 && ZoneId == 982) TimerPattern = new HpTriggeredTimerPattern(9 * 60, .8f);
            // nightmare antaroth
            if (TemplateId == 3000 && ZoneId == 920) TimerPattern = new HpTriggeredTimerPattern(5 * 60, .5f);
            // bahaar
            if (TemplateId == 2000 && ZoneId == 444) TimerPattern = new HpTriggeredTimerPattern(5 * 60, .3f);
            // dreadspire
            if (TemplateId == 1000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (TemplateId == 2000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (TemplateId == 3000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (TemplateId == 4000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (TemplateId == 5000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (TemplateId == 6000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (TemplateId == 7000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (TemplateId == 8000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (TemplateId == 9000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (TemplateId == 10000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);

            TimerPattern?.SetTarget(this);
        }

        public void SetEnragePattern()
        {
            if (App.Settings.EthicalMode)
            {
                EnragePattern = new EnragePattern(0, 0);
                return;
            }
            if (IsPhase1Dragon) EnragePattern = new EnragePattern(14, 50);
            if (ZoneId == 950 && !IsPhase1Dragon) EnragePattern = new EnragePattern(0, 0);
            if (ZoneId == 450 && TemplateId == 1003) EnragePattern = new EnragePattern((long)MaxHP, 600000000, 72);

            //ghilli
            if (TemplateId == 81301 && ZoneId == 713) EnragePattern = new EnragePattern(100 - 65, Int32.MaxValue) { StaysEnraged = true };
            if (TemplateId == 81312 && ZoneId == 713) EnragePattern = new EnragePattern(0, 0);
            if (TemplateId == 81398 && ZoneId == 713) EnragePattern = new EnragePattern(100 - 25, Int32.MaxValue) { StaysEnraged = true };
            if (TemplateId == 81399 && ZoneId == 713) EnragePattern = new EnragePattern(100 - 25, Int32.MaxValue) { StaysEnraged = true };


            if (ZoneId == 620 && TemplateId == 1000) EnragePattern = new EnragePattern((long)MaxHP, 420000000, 36);
            if (ZoneId == 622 && TemplateId == 1000) EnragePattern = new EnragePattern((long)MaxHP, 480000000, 36);
            if (ZoneId == 628)
            {
                if (TemplateId == 1000) EnragePattern = new EnragePattern(0, 0);
                if (TemplateId == 3000 || TemplateId == 3001) EnragePattern = new EnragePattern(10, 36);
            }

            if (TccUtils.IsFieldBoss(ZoneId, TemplateId)) EnragePattern = new EnragePattern(0, 0);
        }
    }
}
