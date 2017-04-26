using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;


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
        public static event UpdateStatWithIdEventHandler BossHPChanged;
        public static event UpdateStatWithIdEventHandler EnragedChanged;

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
                    if(Name == "Terradrax")
                    {
                        Console.WriteLine("Terradrax enrage changed ({0})", value);
                    }
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
                    Buffs.Remove(Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id));
                }
                catch (Exception)
                {
                    Console.WriteLine("Cannot remove {0}", ab.Name);
                }
            });
        }

        public Boss(ulong eId, uint zId, uint tId, float curHP, float maxHP, Visibility visible)
        {
            EntityId = eId;
            Name = MonsterDatabase.GetName(tId, zId);
            MaxHP = maxHP;
            CurrentHP = curHP;
            Buffs = new ObservableCollection<AbnormalityDuration>();
            Visible = visible;
        }

        public Boss(ulong eId, uint zId, uint tId, Visibility visible)
        {
            EntityId = eId;
            Name = MonsterDatabase.GetName(tId, zId);
            MaxHP = MonsterDatabase.GetMaxHP(tId, zId);
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
