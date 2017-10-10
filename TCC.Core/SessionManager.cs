using System;
using System.Linq;
using TCC.Data;
using TCC.Data.Databases;
using TCC.ViewModels;

namespace TCC
{
    public delegate void HarrowholdModeEventHandler(bool val);

    public static class SessionManager
    {
        public static readonly int MAX_WEEKLY = 15;
        public static readonly int MAX_DAILY = 8;
        private static bool logged = false || !App.Debug;
        public static bool Logged
        {
            get => logged;
            set
            {
                if (logged != value)
                {
                    logged = value;
                    WindowManager.NotifyVisibilityChanged();
                }
            }
        }
        private static bool loadingScreen = true || !App.Debug;
        public static bool LoadingScreen
        {
            get => loadingScreen;
            set
            {
                if (loadingScreen != value)
                {
                    loadingScreen = value;
                    WindowManager.NotifyVisibilityChanged();
                }
            }
        }
        
        private static bool encounter;
        public static bool Encounter
        {
            get => encounter;
            set
            {
                if (encounter == value) return;
                encounter = value;
                if (!encounter)
                {
                    WindowManager.SkillsEnded = true;
                }
                WindowManager.NotifyDimChanged();
            }
        }

        public static bool IsElite { get; set; }

        public static Player CurrentPlayer = new Player();

        public static ItemsDatabase ItemsDatabase;

        public static void SetCombatStatus(ulong target, bool combat)
        {
            if (target == CurrentPlayer.EntityId)
            {
                if (combat)
                {
                    CurrentPlayer.IsInCombat = true;
                    CharacterWindowViewModel.Instance.Player.IsInCombat = true;
                }
                else
                {
                    CurrentPlayer.IsInCombat = false;
                    CharacterWindowViewModel.Instance.Player.IsInCombat = false;
                }
            }


        }
        public static void SetPlayerHP(ulong target, float hp)
        {
            CurrentPlayer.CurrentHP = hp;
            CharacterWindowViewModel.Instance.Player.CurrentHP = hp;
            ClassManager.SetHP(Convert.ToInt32(hp));


        }
        public static void SetPlayerMP(ulong target, float mp)
        {
            if (target == CurrentPlayer.EntityId)
            {
                CurrentPlayer.CurrentMP = mp;
                CharacterWindowViewModel.Instance.Player.CurrentMP = mp;
                ClassManager.SetMP(Convert.ToInt32(mp));
            }
        }
        public static void SetPlayerST(ulong target, float st)
        {
            if (target == CurrentPlayer.EntityId)
            {
                CurrentPlayer.CurrentST = st;
                CharacterWindowViewModel.Instance.Player.CurrentST = st;
                ClassManager.SetST(Convert.ToInt32(st));
            }
        }
        public static void SetPlayerFE(float en)
        {
            CurrentPlayer.FlightEnergy = en;
            CharacterWindowViewModel.Instance.Player.FlightEnergy = en;
        }
        public static void SetPlayerLaurel(Player p)
        {
            try
            {
                p.Laurel = InfoWindowViewModel.Instance.Characters.First(x => x.Name == p.Name).Laurel;
            }
            catch
            {
                p.Laurel = Laurel.None;
            }
        }

        public static void SetPlayerMaxHP(ulong target, int maxHP)
        {
            if (target == CurrentPlayer.EntityId)
            {
                CurrentPlayer.MaxHP = maxHP;
                CharacterWindowViewModel.Instance.Player.MaxHP = maxHP;
                ClassManager.SetMaxHP(Convert.ToInt32(maxHP));
            }
        }
        public static void SetPlayerMaxMP(ulong target, int maxMP)
        {
            if (target == CurrentPlayer.EntityId)
            {
                CurrentPlayer.MaxMP = maxMP;
                CharacterWindowViewModel.Instance.Player.MaxMP = maxMP;
                ClassManager.SetMaxMP(Convert.ToInt32(maxMP));
            }
        }
        public static void SetPlayerMaxST(ulong target, int maxST)
        {
            if (target == CurrentPlayer.EntityId)
            {
                CurrentPlayer.MaxST = maxST;
                CharacterWindowViewModel.Instance.Player.MaxST = maxST;
                ClassManager.SetMaxST(Convert.ToInt32(maxST));
            }
        }
    }

}
