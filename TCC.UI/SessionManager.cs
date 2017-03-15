using DamageMeter.Sniffing;
using Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public static event Action OutOfCombat;
        public static event Action InCombat;
    }
    public class Boss 
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

        public bool Enraged { get; set; }
        public float CurrentHP { get; set; }

        public Boss(ulong eId, int zId, int tId, float curHP, float maxHP)
        {
            EntityId = eId;
            Name = MonsterDatabase.GetName(tId, zId);
            MaxHP = maxHP;
            CurrentHP = curHP;
            Buffs = new ObservableCollection<BuffDuration>();
        }
    }
}
