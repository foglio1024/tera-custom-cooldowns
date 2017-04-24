using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TCC.Data
{
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
