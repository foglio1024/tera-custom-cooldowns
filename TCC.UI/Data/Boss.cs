using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace TCC
{
    public delegate void UpdateBossEnrageEventHandler(ulong id, bool enraged);
    public delegate void UpdateBossHPEventHandler(ulong id, float hp);

}
namespace TCC.Data
{
    public class Boss : INotifyPropertyChanged
    {
        public ulong EntityId { get; set; }
        string name;
        public string Name
        { get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                }
            }
        }

        public ObservableCollection<AbnormalityDuration> Buffs;

        public event PropertyChangedEventHandler PropertyChanged;
        public static event UpdateBossHPEventHandler BossHPChanged;
        public static event UpdateBossEnrageEventHandler EnragedChanged;

        bool enraged;
        public bool Enraged
        {
            get => enraged;
            set
            {
                if (enraged != value)
                {
                    enraged = value;
                    NotifyPropertyChanged("Enraged");
                    EnragedChanged?.Invoke(EntityId, value);
                }
            }
        }
        float maxHP;
        public float MaxHP
        {
            get => maxHP;
            set
            {
                if (maxHP != value)
                {
                    maxHP = value;
                    NotifyPropertyChanged("MaxHP");
                }
            }
        }
        float currentHP;
        public float CurrentHP
        {
            get => currentHP;
            set
            {
                if (currentHP != value)
                {
                    currentHP = value;
                    NotifyPropertyChanged("CurrentHP");
                    BossHPChanged?.Invoke(EntityId, value);
                }
            }
        }
        Visibility visible;
        public Visibility Visible { get { return visible; }  set {
                if(visible != value)
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
            set {
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


        public void NotifyPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }
        public bool HasBuff(Abnormality ab)
        {
            if(Buffs.Any(x => x.Abnormality.Id == ab.Id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void EndBuff(Abnormality ab)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id).Dispose();
                    Buffs.Remove(Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id));
                }
                catch (Exception)
                {
                    //Console.WriteLine("Cannot remove {0}", ab.Name);
                }
            });
        }

        public Boss(ulong eId, uint zId, uint tId, float curHP, float maxHP, Visibility visible)
        {
            EntityId = eId;
            Name = EntitiesManager.CurrentDatabase.GetName(tId, zId);
            MaxHP = maxHP;
            CurrentHP = curHP;
            Buffs = new ObservableCollection<AbnormalityDuration>();
            Visible = visible;
        }

        public Boss(ulong eId, uint zId, uint tId, Visibility visible)
        {
            EntityId = eId;
            Name = EntitiesManager.CurrentDatabase.GetName(tId, zId);
            MaxHP = EntitiesManager.CurrentDatabase.GetMaxHP(tId, zId);
            CurrentHP = MaxHP;
            Buffs = new ObservableCollection<AbnormalityDuration>();
            Visible = visible;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", EntityId, Name);
        }
    }
}
