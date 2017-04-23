using DamageMeter.Sniffing;
using Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TCC.Data;
using TCC.Messages;
using Tera.Game;

namespace TCC
{
    public static class SessionManager
    {
        public static bool Logged;

        public static Player CurrentPlayer = new Player();

        public static ObservableCollection<Boss> CurrentBosses = new ObservableCollection<Boss>();
        public static bool TryGetBossById(ulong id, out Boss b)
        {
            b = CurrentBosses.FirstOrDefault(x => x.EntityId == id);
            if(b == null)
            {
                b = new Boss(0, 0, 0, Visibility.Collapsed);
                return false;
            }
            else
            {
                return true;
            }
        }

    }
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

        public bool Enraged { get; set; }
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
    public class Player :INotifyPropertyChanged
    {
        string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if(name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        ulong entityId;
        public ulong EntityId
        {
            get
            {
                return entityId;
            }
            set
            {
                if(entityId != value)
                {
                    entityId = value;
                    NotifyPropertyChanged("EntityId");
                }
            }
        }
        Class playerclass;
        public Class Class
        {
            get
            {
                return playerclass;
            }
            set
            {
                if(playerclass != value)
                {
                    playerclass = value;
                    NotifyPropertyChanged("Class");
                }
            }
        }
        Laurel laurel;
        public Laurel Laurel
        {
            get
            {
                return laurel;
            }
            set
            {
                if(laurel != value)
                {
                    laurel = value;
                    NotifyPropertyChanged("Laurel");
                }
            }
        }
        int level;
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if(level != value)
                {
                    level = value;
                    NotifyPropertyChanged("Level");
                }
            }
        }
        private int itemLevel;
        public int ItemLevel
        {
            get => itemLevel;
            set
            {
                if (value != itemLevel)
                {
                    itemLevel = value;
                    NotifyPropertyChanged("ItemLevel");
                }
            }
        }

        private int maxHP;
        public int MaxHP
        {
            get { return maxHP; }
            set {
                if(maxHP != value)
                {
                    maxHP = value;
                    NotifyPropertyChanged("MaxHP");
                }

                }
        }
        private int maxMP;
        public int MaxMP
        {
            get { return maxMP; }
            set
            {
                if (maxMP != value)
                {
                    maxMP = value;
                    NotifyPropertyChanged("MaxMP");
                }

            }
        }
        private int maxST;
        public int MaxST
        {
            get { return maxST; }
            set
            {
                if (maxST != value)
                {
                    maxST = value;
                    NotifyPropertyChanged("MaxST");
                }

            }
        }

        private float currentHP;
        public float CurrentHP
        {
            get
            {
                return currentHP;
            }
            set
            {
                if (currentHP != value)
                {
                    currentHP = value;
                    NotifyPropertyChanged("CurrentHP");
                    HPUpdated?.Invoke(value);
                }
            }
        }
        private float currentMP;
        public float CurrentMP
        {
            get
            {
                return currentMP;
            }
            set
            {
                if (currentMP != value)
                {
                    currentMP = value;
                    NotifyPropertyChanged("CurrentMP");
                    MPUpdated?.Invoke(value);
                }
            }
        }
        private float currentST;
        public float CurrentST
        {
            get
            {
                return currentST;
            }
            set
            {
                if (currentST != value)
                {
                    currentST = value;
                    NotifyPropertyChanged("CurrentST");
                    STUpdated?.Invoke(value);
                }
            }
        }

        private float flightEnergy;
        public float FlightEnergy
        {
            get { return flightEnergy; }
            set {
                if (flightEnergy != value)
                {
                    flightEnergy = value;
                    NotifyPropertyChanged("FlightEnergy");
                    FlightEnergyUpdated?.Invoke(value);
                }
            }
        }

        public double PercentageHP
        {
            get
            {
                if(MaxHP > 0)
                {
                    return CurrentHP/MaxHP;
                }
                else
                {
                    return 0;
                }
            }
        }

        bool isInCombat;
        public  bool IsInCombat
        {
            get { return isInCombat; }
            set
            {
                if (value != isInCombat)
                {
                    isInCombat = value;
                    if (!isInCombat)
                    {
                        OutOfCombat?.Invoke();
                    }
                    else
                    {
                        InCombat?.Invoke();
                    }
                    NotifyPropertyChanged("IsInCombat");
                }
            }
        }

        public ObservableCollection<AbnormalityDuration> Buffs = new ObservableCollection<AbnormalityDuration>();
        public ObservableCollection<AbnormalityDuration> Debuffs = new ObservableCollection<AbnormalityDuration>();
        public ObservableCollection<AbnormalityDuration> InfBuffs = new ObservableCollection<AbnormalityDuration>();

        public event Action OutOfCombat;
        public event Action InCombat;
        public event Action<float> HPUpdated;
        public event Action<float> MPUpdated;
        public event Action<float> STUpdated;

        public event Action<float> FlightEnergyUpdated;


        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
