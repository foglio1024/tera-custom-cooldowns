using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TCC.Data;

namespace TCC
{
    public delegate void HarrowholdModeEventHandler(bool val);

    public static class SessionManager
    {
        private static bool logged = false;
        public static bool Logged
        {
            get => logged;
            set
            {
                if(logged != value)
                {
                    logged = value;
                    WindowManager.NotifyVisibilityChanged();
                }
            }
        }
        private static bool loadingScreen = true;
        public static bool LoadingScreen
        {
            get => loadingScreen;
            set
            {
                if(loadingScreen != value)
                {
                    loadingScreen = value;
                    WindowManager.NotifyVisibilityChanged();
                }
            }
        }

        static bool harrowHoldMode = false;
        public static bool HarrowholdMode
        {
            get => harrowHoldMode;
            set
            {
                if(harrowHoldMode != value)
                {
                    harrowHoldMode = value;
                    HhModeChanged?.Invoke(harrowHoldMode);
                }
            }
        }
        public static Player CurrentPlayer = new Player();
        public static List<Character> CurrentAccountCharacters;

        public static event HarrowholdModeEventHandler HhModeChanged;

        public static void SetDebuffedStatus(bool debuffed)
        {
            CurrentPlayer.IsDebuffed = debuffed;
        }

        public static void ClearPlayersAbnormalities()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                CurrentPlayer.Buffs.Clear();
            });
        }
        public static void EndPlayerAbnormality(Abnormality ab)
        {
            CurrentPlayer.Buffs.Remove(CurrentPlayer.Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id));
            CurrentPlayer.Debuffs.Remove(CurrentPlayer.Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id));
            CurrentPlayer.InfBuffs.Remove(CurrentPlayer.InfBuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id));
            if (!ab.IsBuff)
            {
                SetDebuffedStatus(false);
            }

        }
        public static void SetCombatStatus(ulong target, bool combat)
        {
            if (target == CurrentPlayer.EntityId)
            {
                if (combat)
                {
                    CurrentPlayer.IsInCombat = true;
                }
                else
                {
                    CurrentPlayer.IsInCombat = false;
                }
            }


        }
        public static void SetPlayerHP(ulong target, float hp)
        {
            if (target == CurrentPlayer.EntityId)
            {
                CurrentPlayer.CurrentHP = hp;
            }
        }
        public static void SetPlayerMP(ulong target, float mp)
        {
            if (target == CurrentPlayer.EntityId)
            {
                CurrentPlayer.CurrentMP = mp;
            }
        }
        public static void SetPlayerLaurel(string name)
        {
            try
            {
                CurrentPlayer.Laurel = CurrentAccountCharacters.First(x => x.Name == name).Laurel;
            }
            catch
            {
                CurrentPlayer.Laurel = Laurel.None;
            }
        }
    }

}
