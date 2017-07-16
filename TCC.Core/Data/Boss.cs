using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using TCC.ViewModels;

namespace TCC
{
    public delegate void UpdateBossEnrageEventHandler(ulong id, bool enraged);
    public delegate void UpdateBossHPEventHandler(ulong id, float hp);

}
namespace TCC.Data
{
    public class Boss : TSPropertyChanged, IDisposable
    {
        public ulong EntityId { get; set; }
        protected string name;
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                }
            }
        }

        public bool IsBoss { get; set; }
        protected SynchronizedObservableCollection<AbnormalityDuration> _buffs;
        public SynchronizedObservableCollection<AbnormalityDuration> Buffs
        {
            get { return _buffs; }
            set
            {
                if (_buffs == value) return;
                _buffs = value;
                NotifyPropertyChanged("Buffs");
            }
        }

        protected bool enraged;
        public bool Enraged
        {
            get => enraged;
            set
            {
                if (enraged != value)
                {
                    enraged = value;
                    NotifyPropertyChanged("Enraged");
                }
            }
        }
        protected float _maxHP;
        public float MaxHP
        {
            get => _maxHP;
            set
            {
                if (_maxHP != value)
                {
                    _maxHP = value;
                    NotifyPropertyChanged("MaxHP");
                }
            }
        }
        protected float _currentHP;
        public float CurrentHP
        {
            get => _currentHP;
            set
            {
                if (_currentHP != value)
                {
                    _currentHP = value;
                    NotifyPropertyChanged(nameof(CurrentHP));
                    NotifyPropertyChanged(nameof(CurrentPercentage));
                    NotifyPropertyChanged(nameof(CurrentFactor));
                }
            }
        }

        public float CurrentFactor => _maxHP == 0 ? 0 : (_currentHP / _maxHP);
        public float CurrentPercentage => CurrentFactor * 100;
        protected Visibility visible;
        public Visibility Visible
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    visible = value;
                    NotifyPropertyChanged("Visible");
                }
            }
        }

        protected ulong target;
        public ulong Target
        {
            get { return target; }
            set
            {
                if (target != value)
                {
                    target = value;
                    NotifyPropertyChanged("Target");
                }
            }
        }

        protected AggroCircle currentAggroType = AggroCircle.None;
        public AggroCircle CurrentAggroType
        {
            get { return currentAggroType; }
            set
            {
                if (currentAggroType != value)
                {
                    currentAggroType = value;
                    NotifyPropertyChanged("CurrentAggroType");
                }
            }
        }

        public uint ZoneId { get; protected set; }
        public uint TemplateId { get; protected set; }

        public EnragePattern EnragePattern { get; set; }
        public void AddorRefresh(Abnormality ab, uint duration, int stacks, double size, double margin)
        {
            var existing = Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, target, _dispatcher, true, size * .9, size, new System.Windows.Thickness(margin));
                if (ab.Infinity) Buffs.Insert(0, newAb);
                else Buffs.Add(newAb);
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
            }
            catch (Exception)
            {
            }
        }

        //public Boss(ulong eId, uint zId, uint tId, float curHP, float maxHP, Visibility visible)
        //{
        //    _dispatcher = BossGageWindowViewModel.Instance.GetDispatcher();
        //    EntityId = eId;
        //    Name = EntitiesManager.CurrentDatabase.GetName(tId, zId);
        //    ZoneId = zId;
        //    TemplateId = tId;
        //    MaxHP = maxHP;
        //    CurrentHP = curHP;
        //    _buffs = new SynchronizedObservableCollection<AbnormalityDuration>(_dispatcher);
        //    Visible = visible;

        //    IsShieldPhase = false;
        //    IsSelected = false;
        //    if (BossGageWindowViewModel.Instance.CurrentHHphase == HarrowholdPhase.Phase1)
        //    {
        //        ShieldDuration = new DispatcherTimer();
        //        ShieldDuration.Interval = TimeSpan.FromSeconds(13);
        //        ShieldDuration.Tick += ShieldFailed;
        //    }

        //}
        public Boss(ulong eId, uint zId, uint tId, bool boss, Visibility visible)
        {
            _dispatcher = BossGageWindowViewModel.Instance.GetDispatcher();
            EntityId = eId;
            Name = EntitiesManager.CurrentDatabase.GetName(tId, zId);
            MaxHP = EntitiesManager.CurrentDatabase.GetMaxHP(tId, zId);
            ZoneId = zId;
            IsBoss = boss;
            TemplateId = tId;
            CurrentHP = MaxHP;
            _buffs = new SynchronizedObservableCollection<AbnormalityDuration>(_dispatcher);
            Visible = visible;
            Shield = ShieldStatus.Off;
            IsSelected = true;
            EnragePattern = new EnragePattern(10, 36);
            if (IsPhase1Dragon)
            {
                ShieldDuration = new Timer();
                ShieldDuration.Interval = 15000;
                ShieldDuration.Elapsed += ShieldFailed;

                EnragePattern.Duration = 50;
                EnragePattern.Percentage = 14;
            }
        }
        public override string ToString()
        {
            return String.Format("{0} - {1}", EntityId, Name);
        }

        public void Dispose()
        {
            foreach (var buff in _buffs) buff.Dispose();
            ShieldDuration?.Dispose();
        }
        public bool IsPhase1Dragon
        {
            get
            {
                return Utils.IsPhase1Dragon(ZoneId, TemplateId);
            }
        }
        ///////////////////////////////////////////
        Timer ShieldDuration;

        private ShieldStatus _shield;
        public ShieldStatus Shield
        {
            get => _shield;
            set
            {
                if (_shield == value) return;
                _shield = value;
                NotifyPropertyChanged(nameof(Shield));
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected == value) return;
                isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }
        private void ShieldFailed(object sender, EventArgs e)
        {
            Shield = ShieldStatus.Failed;
        }
        public void BreakShield()
        {
            ShieldDuration.Stop();
            Shield = ShieldStatus.Broken;
            Task.Delay(5000).ContinueWith(t =>
            {
                Shield = ShieldStatus.Off;
            });
        }
        public void StartShield()
        {
            ShieldDuration.Start();
            Shield = ShieldStatus.On;
        }



    }
}
