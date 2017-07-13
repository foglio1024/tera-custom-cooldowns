using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        private string name;
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

        private SynchronizedObservableCollection<AbnormalityDuration> _buffs;
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

        private bool enraged;
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
        private float _maxHP;
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
        private float _currentHP;
        public float CurrentHP
        {
            get => _currentHP;
            set
            {
                if (_currentHP != value)
                {
                    _currentHP = value;
                    NotifyPropertyChanged("CurrentHP");
                    NotifyPropertyChanged("CurrentPercentage");
                }
            }
        }

        public float CurrentPercentage => _maxHP == 0 ? 0 : (_currentHP / _maxHP);
        private Visibility visible;
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

        private ulong target;
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

        private AggroCircle currentAggroType = AggroCircle.None;
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

        public uint ZoneId { get; private set; }
        public uint TemplateId { get; private set; }
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

        public Boss(ulong eId, uint zId, uint tId, float curHP, float maxHP, Visibility visible)
        {
            _dispatcher = BossGageWindowViewModel.Instance.GetDispatcher();
            EntityId = eId;
            Name = EntitiesManager.CurrentDatabase.GetName(tId, zId);
            ZoneId = zId;
            TemplateId = tId;
            MaxHP = maxHP;
            CurrentHP = curHP;
            _buffs = new SynchronizedObservableCollection<AbnormalityDuration>(_dispatcher);
            Visible = visible;
        }
        public Boss(ulong eId, uint zId, uint tId, Visibility visible)
        {
            _dispatcher = BossGageWindowViewModel.Instance.GetDispatcher();
            EntityId = eId;
            Name = EntitiesManager.CurrentDatabase.GetName(tId, zId);
            MaxHP = EntitiesManager.CurrentDatabase.GetMaxHP(tId, zId);
            ZoneId = zId;
            TemplateId = tId;
            CurrentHP = MaxHP;
            _buffs = new SynchronizedObservableCollection<AbnormalityDuration>(_dispatcher);
            Visible = visible;
        }
        public override string ToString()
        {
            return String.Format("{0} - {1}", EntityId, Name);
        }

        public void Dispose()
        {
            foreach (var buff in _buffs) buff.Dispose();
        }
    }
}
