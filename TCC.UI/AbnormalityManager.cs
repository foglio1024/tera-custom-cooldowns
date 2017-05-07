using System.Linq;
using TCC.Data;
using TCC.ViewModels;

namespace TCC
{
    public delegate void UpdateAbnormalityEventHandler(ulong target, Abnormality ab, int duration, int stacks);

    public static class AbnormalityManager
    {

        public static void BeginAbnormality(uint id, ulong target, int duration, int stacks)
        {
            if (AbnormalityDatabase.Abnormalities.TryGetValue(id, out Abnormality ab))
            {
                if(!Filter(ab)) return;
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
                if(target == SessionManager.CurrentPlayer.EntityId)
                {
                    EndPlayerAbnormality(ab);
                }
                else if (EntitiesManager.TryGetBossById(target, out Boss b))
                {
                    b.EndBuff(ab);
                }
            }

        }
        static void BeginPlayerAbnormality(Abnormality ab, int stacks, int duration, ulong target)
        {
            var newAb = new AbnormalityDuration(ab, duration, stacks, target, BuffBarWindowManager.Instance.Dispatcher);
            if (ab.Infinity)
            {
                newAb.Duration = -1;
                SessionManager.CurrentPlayer.AddOrRefreshInfBuff(newAb);
                BuffBarWindowManager.Instance.Player.AddOrRefreshInfBuff(newAb);
            }
            else
            {
                if(ab.Type == AbnormalityType.Buff)
                {
                    SessionManager.CurrentPlayer.AddOrRefreshBuff(newAb);
                    BuffBarWindowManager.Instance.Player.AddOrRefreshBuff(newAb);

                }
                else
                {
                    SessionManager.CurrentPlayer.AddOrRefreshDebuff(newAb);
                    BuffBarWindowManager.Instance.Player.AddOrRefreshDebuff(newAb);

                }
            }

            //App.Current.Dispatcher.Invoke(() =>
            //{
            //    if (ab.Type == AbnormalityType.Buff)
            //    {
            //        if (ab.Infinity)
            //        {
            //            if (SessionManager.CurrentPlayer.InfBuffs.Any(x => x.Abnormality == ab))
            //            {
            //                PlayerAbnormalityUpdated?.Invoke(target, ab, -1, stacks);
            //            }
            //            else
            //            {
            //                SessionManager.CurrentPlayer.InfBuffs.Add(new AbnormalityDuration(ab, -1, stacks, target));
            //                if (!ab.IsBuff)
            //                {
            //                    SessionManager.SetDebuffedStatus(true);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (SessionManager.CurrentPlayer.Buffs.Any(x => x.Abnormality == ab))
            //            {
            //                PlayerAbnormalityUpdated?.Invoke(target, ab, duration, stacks);
            //            }
            //            else
            //            {
            //                SessionManager.CurrentPlayer.Buffs.Add(new AbnormalityDuration(ab, duration, stacks, target));
            //                if (!ab.IsBuff)
            //                {
            //                    SessionManager.SetDebuffedStatus(true);
            //                }

            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (ab.Infinity)
            //        {
            //            if (SessionManager.CurrentPlayer.Debuffs.Any(x => x.Abnormality == ab))
            //            {
            //                PlayerAbnormalityUpdated?.Invoke(target, ab, -1, stacks);
            //            }
            //            else
            //            {
            //                SessionManager.CurrentPlayer.Debuffs.Insert(0, new AbnormalityDuration(ab, -1, stacks, target));
            //                if (!ab.IsBuff)
            //                {
            //                    SessionManager.SetDebuffedStatus(true);
            //                }

            //            }
            //        }
            //        else
            //        {
            //            if (SessionManager.CurrentPlayer.Debuffs.Any(x => x.Abnormality == ab))
            //            {
            //                PlayerAbnormalityUpdated?.Invoke(target, ab, duration, stacks);
            //            }
            //            else
            //            {
            //                SessionManager.CurrentPlayer.Debuffs.Add(new AbnormalityDuration(ab, duration, stacks, target));
            //                if (!ab.IsBuff)
            //                {
            //                    SessionManager.SetDebuffedStatus(true);
            //                }
            //            }
            //        }
            //    }
            //});

        }
        static void EndPlayerAbnormality(Abnormality ab)
        {
            if (ab.Infinity)
            {
                SessionManager.CurrentPlayer.RemoveInfBuff(ab);
                BuffBarWindowManager.Instance.Player.RemoveInfBuff(ab);
            }
            else
            {
                if(ab.Type == AbnormalityType.Buff)
                {
                    SessionManager.CurrentPlayer.RemoveBuff(ab);
                    BuffBarWindowManager.Instance.Player.RemoveBuff(ab);

                }
                else
                {
                    SessionManager.CurrentPlayer.RemoveDebuff(ab);
                    BuffBarWindowManager.Instance.Player.RemoveDebuff(ab);
                }
            }
            //if (CurrentPlayer.Buffs.Any(x => x.Abnormality.Id == ab.Id))
            //{
            //    CurrentPlayer.Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id).Dispose();
            //    CurrentPlayer.Buffs.Remove(CurrentPlayer.Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id));
            //}
            //else if (CurrentPlayer.Debuffs.Any(x => x.Abnormality.Id == ab.Id))
            //{
            //    CurrentPlayer.Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id).Dispose();
            //    CurrentPlayer.Debuffs.Remove(CurrentPlayer.Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id));
            //}
            //else if (CurrentPlayer.InfBuffs.Any(x => x.Abnormality.Id == ab.Id))
            //{
            //    CurrentPlayer.InfBuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id).Dispose();
            //    CurrentPlayer.InfBuffs.Remove(CurrentPlayer.InfBuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id));
            //}
            //if (!ab.IsBuff)
            //{
            //    SetDebuffedStatus(false);
            //}

        }

        static void BeginNPCAbnormality(Abnormality ab, int stacks, int duration, ulong target)
        {
            if (EntitiesManager.TryGetBossById(target, out Boss b))
            {
                b.AddorRefresh(new AbnormalityDuration(ab, duration, stacks, target, BossGageWindowManager.Instance.Dispatcher));
                //App.Current.Dispatcher.Invoke(() =>
                //{
                //    if (b.HasBuff(ab))
                //    {
                //        NPCAbnormalityUpdated?.Invoke(b.EntityId, ab, duration, stacks);
                //    }
                //    else
                //    {
                //        if (!ab.Infinity)
                //        {
                //            EntitiesManager.CurrentBosses.Where(x => x.EntityId == target).First().Buffs.Add(new AbnormalityDuration(ab, duration, stacks, target));
                //        }
                //        else
                //        {
                //            EntitiesManager.CurrentBosses.Where(x => x.EntityId == target).First().Buffs.Insert(0, new AbnormalityDuration(ab, -1, stacks, target));
                //        }

                //    }
                //});

            }

        }
        static bool Filter(Abnormality ab)
        {
            if (ab.Name.Contains("BTS") || ab.ToolTip.Contains("BTS") || !ab.IsShow) return false;
            if (ab.Name.Contains("(Hidden)") || ab.Name.Equals("Unknown") || ab.Name.Equals(string.Empty)) return false;
            return true;
        }
    }
}
