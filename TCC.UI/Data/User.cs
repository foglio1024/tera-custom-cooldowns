using System.Windows.Threading;

namespace TCC.Data
{
    public class User : TSPropertyChanged
    {
        private ulong entityId;
        public ulong EntityId
        {
            get { return entityId; }
            set
            {
                if (entityId == value) return;
                entityId = value;
                NotifyPropertyChanged("EntityId");
            }
        }

        private uint level;
        public uint Level
        {
            get { return level; }
            set
            {
                if (level == value) return;
                level = value;
                NotifyPropertyChanged("Level");
            }
        }

        private Class userClass;
        public Class UserClass
        {
            get { return userClass; }
            set
            {
                if (userClass == value) return;
                userClass = value;
                NotifyPropertyChanged("UserClass");
            }
        }

        private bool online;
        public bool Online
        {
            get
            {
                System.Console.WriteLine("Get {0} online status ({1})", name, online);

                return online;
            }
            set
            {
                if (online == value) return;
                online = value;
                NotifyPropertyChanged("Online");
                System.Console.WriteLine("{0} online status = {1}", name, online);
            }
        }

        private uint serverId;
        public uint ServerId
        {
            get { return serverId; }
            set
            {
                if (serverId == value) return;
                serverId = value;
                NotifyPropertyChanged("ServerId");
            }
        }

        private uint playerId;
        public uint PlayerId
        {
            get { return playerId; }
            set
            {
                if (playerId == value) return;
                playerId = value;
                NotifyPropertyChanged("PlayerId");
            }
        }

        private int order;
        public int Order
        {
            get { return order; }
            set
            {
                if (order == value) return;
                order = value;
                NotifyPropertyChanged("Order");
            }
        }

        private bool canInvite;
        public bool CanInvite
        {
            get { return canInvite; }
            set
            {
                if (canInvite == value) return;
                canInvite = value;
                NotifyPropertyChanged("CanInvite");
            }
        }

        private Laurel laurel;
        public Laurel Laurel
        {
            get { return laurel; }
            set
            {
                if (laurel == value) return;
                laurel = value;
                NotifyPropertyChanged("Laurel");
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name == value) return;
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private int currentHP;
        public int CurrentHP
        {
            get { return currentHP; }
            set
            {
                if (currentHP == value) return;
                currentHP = value;
                NotifyPropertyChanged("CurrentHP");
                NotifyPropertyChanged("HpFactor");
            }
        }

        private int maxHP;
        public int MaxHP
        {
            get { return maxHP; }
            set
            {
                if (maxHP == value) return;
                maxHP = value;
                NotifyPropertyChanged("MaxHP");
                NotifyPropertyChanged("HpFactor");
            }
        }

        private int currentMP;
        public int CurrentMP
        {
            get { return currentMP; }
            set
            {
                if (currentMP == value) return;
                currentMP = value;
                NotifyPropertyChanged("CurrentMP");
                NotifyPropertyChanged("MpFactor");
            }
        }

        private int maxMP;
        public int MaxMP
        {
            get { return maxMP; }
            set
            {
                if (maxMP == value) return;
                maxMP = value;
                NotifyPropertyChanged("MaxMP");
                NotifyPropertyChanged("MpFactor");
            }
        }

        private double hpFactor;
        public double HpFactor
        {
            get
            {
                if (maxHP > 0)
                {
                    hpFactor = (double)currentHP / (double)maxHP > 1 ? 1 : (double)currentHP / (double)maxHP;
                    return hpFactor;
                }
                else
                {
                    hpFactor = 1;
                    return hpFactor;
                }
            }
            set
            {
                if (hpFactor == value) return;
                hpFactor = value;
                NotifyPropertyChanged("HpFactor");
            }
        }


        private double mpFactor;
        public double MpFactor
        {
            get
            {
                if (maxMP > 0)
                {
                    mpFactor = (double)currentMP / (double)maxMP > 1 ? 1 : (double)currentMP / (double)maxMP;
                    return mpFactor;
                }
                else
                {
                    mpFactor = 1;
                    return mpFactor;
                }
            }
            set
            {
                if (mpFactor == value) return;
                mpFactor = value;
                NotifyPropertyChanged("MpFactor");
            }
        }

        private bool alive;
        public bool Alive
        {
            get { return alive; }
            set
            {
                if (alive == value) return;
                alive = value;
                NotifyPropertyChanged("Alive");
            }
        }

        public User(Dispatcher d)
        {
            _dispatcher = d;
        }
    }
}