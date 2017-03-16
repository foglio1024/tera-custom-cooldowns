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
        static bool combat;
        public static bool Combat {
            get { return combat; }
            set
            {
                if(value != combat)
                {
                    combat = value;
                    if (!combat)
                    {
                        OutOfCombat?.Invoke();
                    }
                    else
                    {
                        InCombat?.Invoke();
                    }
                }
            }
        }

        public static string CurrentCharName;
        public static ulong CurrentCharId;
        public static Class CurrentClass;
        public static Laurel CurrentLaurel;
        public static int CurrentLevel;
        public static ObservableCollection<Boss> CurrentBosses = new ObservableCollection<Boss>();
        public static bool TryGetBossById(ulong id, out Boss b)
        {
            if(CurrentBosses.Where(x => x.EntityId == id).Count() > 0)
            {
                b = CurrentBosses.Where(x => x.EntityId == id).FirstOrDefault();
                return true;
            }
            else
            {
                b = new Boss(0, 0, 0, Visibility.Collapsed);
                return false;
            }
        }

        public static event Action OutOfCombat;
        public static event Action InCombat;
    }
    public class Boss : INotifyPropertyChanged
    {
        public ulong EntityId { get; set; }
        public float MaxHP { get; set; }
        string name;
        public string Name
        { get => name;
            set
            {
                if(name != value)
                {
                    name = value;
                }
            }
        }
        public ObservableCollection<BuffDuration> Buffs;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Enraged { get; set; }
        public float CurrentHP { get; set; }
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
            if(Buffs.Where(x => x.Buff.Id == ab.Id).Count() > 0)
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
                    Buffs.Remove(Buffs.Where(x => x.Buff.Id == ab.Id).FirstOrDefault());
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
            Buffs = new ObservableCollection<BuffDuration>();
            Visible = visible;
        }

        public Boss(ulong eId, uint zId, uint tId, Visibility visible)
        {
            EntityId = eId;
            Name = MonsterDatabase.GetName(tId, zId);
            MaxHP = MonsterDatabase.GetMaxHP(tId, zId);
            CurrentHP = 0;
            Buffs = new ObservableCollection<BuffDuration>();
            Visible = visible;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", EntityId, Name);
        }
    }
}
