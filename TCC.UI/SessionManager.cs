using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TCC.Data;

namespace TCC
{
    public static class SessionManager
    {
        public static bool Logged = false;
        public static bool LoadingScreen = true;

        public static Player CurrentPlayer = new Player();
        public static List<Character> CurrentAccountCharacters;

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

    public delegate void UpdateAbnormalityEventHandler(ulong target, Abnormality ab, int duration, int stacks);

    public static class AbnormalityManager
    {
        public static event UpdateAbnormalityEventHandler PlayerAbnormalityUpdated;
        public static event UpdateAbnormalityEventHandler NPCAbnormalityUpdated;

        public static void BeginAbnormality(uint id, ulong target, int duration, int stacks)
        {
            if (AbnormalityDatabase.Abnormalities.TryGetValue(id, out Abnormality ab))
            {
                if (ab.Name.Contains("BTS") || ab.ToolTip.Contains("BTS") || !ab.IsShow) return;
                if (ab.Name.Contains("(Hidden)") || ab.Name.Equals("Unknown") || ab.Name.Equals(string.Empty)) return;

                if (target == SessionManager.CurrentPlayer.EntityId)
                {
                    BeginPlayerAbnormality(ab, stacks, duration, target);
                }
                else
                {
                    BeginNPCAbnormality(ab, stacks, duration, target);
                }
            }

        }
        public static void EndAbnormality(ulong target, uint id)
        {
            if (AbnormalityDatabase.Abnormalities.TryGetValue(id, out Abnormality ab))
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (target == SessionManager.CurrentPlayer.EntityId)
                    {
                        SessionManager.EndPlayerAbnormality(ab);
                    }
                    else if (EntitiesManager.TryGetBossById(target, out Boss b) && b.HasBuff(ab))
                    {
                        b.EndBuff(ab);
                    }
                });
            }

        }
        static void BeginPlayerAbnormality(Abnormality ab, int stacks, int duration, ulong target)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (ab.Type == AbnormalityType.Buff)
                {
                    if (ab.Infinity)
                    {
                        if (SessionManager.CurrentPlayer.InfBuffs.Any(x => x.Abnormality == ab))
                        {
                            PlayerAbnormalityUpdated?.Invoke(target, ab, -1, stacks);
                        }
                        else
                        {
                            SessionManager.CurrentPlayer.InfBuffs.Add(new AbnormalityDuration(ab, -1, stacks, target));
                            if (!ab.IsBuff)
                            {
                                SessionManager.SetDebuffedStatus(true);
                            }
                        }
                    }
                    else
                    {
                        if (SessionManager.CurrentPlayer.Buffs.Any(x => x.Abnormality == ab))
                        {
                            PlayerAbnormalityUpdated?.Invoke(target, ab, duration, stacks);
                        }
                        else
                        {
                            SessionManager.CurrentPlayer.Buffs.Add(new AbnormalityDuration(ab, duration, stacks, target));
                            if (!ab.IsBuff)
                            {
                                SessionManager.SetDebuffedStatus(true);
                            }

                        }
                    }
                }
                else
                {
                    if (ab.Infinity)
                    {
                        if (SessionManager.CurrentPlayer.Debuffs.Any(x => x.Abnormality == ab))
                        {
                            PlayerAbnormalityUpdated?.Invoke(target, ab, -1, stacks);
                        }
                        else
                        {
                            SessionManager.CurrentPlayer.Debuffs.Insert(0, new AbnormalityDuration(ab, -1, stacks, target));
                            if (!ab.IsBuff)
                            {
                                SessionManager.SetDebuffedStatus(true);
                            }

                        }
                    }
                    else
                    {
                        if (SessionManager.CurrentPlayer.Debuffs.Any(x => x.Abnormality == ab))
                        {
                            PlayerAbnormalityUpdated?.Invoke(target, ab, duration, stacks);
                        }
                        else
                        {
                            SessionManager.CurrentPlayer.Debuffs.Add(new AbnormalityDuration(ab, duration, stacks, target));
                            if (!ab.IsBuff)
                            {
                                SessionManager.SetDebuffedStatus(true);
                            }
                        }
                    }
                }
            });

        }
        static void BeginNPCAbnormality(Abnormality ab, int stacks, int duration, ulong target)
        {
            if (EntitiesManager.TryGetBossById(target, out Boss b))
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (b.HasBuff(ab))
                    {
                        NPCAbnormalityUpdated?.Invoke(b.EntityId, ab, duration, stacks);
                    }
                    else
                    {
                        if (!ab.Infinity)
                        {
                            EntitiesManager.CurrentBosses.Where(x => x.EntityId == target).First().Buffs.Add(new AbnormalityDuration(ab, duration, stacks, target));
                        }
                        else
                        {
                            EntitiesManager.CurrentBosses.Where(x => x.EntityId == target).First().Buffs.Insert(0, new AbnormalityDuration(ab, -1, stacks, target));
                        }

                    }
                });

            }

        }
    }
}
