using System;
using System.Linq;
using TCC.ClassSpecific;
using TCC.Data;
using TCC.Data.Databases;
using TCC.ViewModels;

namespace TCC
{
    public static class AbnormalityManager
    {
        public static ClassAbnormalityTracker CurrentAbnormalityTracker { get; set; }

        public static void SetAbnormalityTracker(Class c)
        {
            switch (c)
            {
                case Class.Warrior:
                    CurrentAbnormalityTracker = new WarriorAbnormalityTracker();
                    break;
                case Class.Lancer:
                    CurrentAbnormalityTracker = new LancerAbnormalityTracker();
                    break;
                case Class.Slayer:
                    CurrentAbnormalityTracker = new SlayerAbnormalityTracker();
                    break;
                case Class.Berserker:
                    CurrentAbnormalityTracker = new BerserkerAbnormalityTracker();
                    break;
                case Class.Sorcerer:
                    CurrentAbnormalityTracker = new SorcererAbnormalityTracker();
                    break;
                case Class.Archer:
                    CurrentAbnormalityTracker = new ArcherAbnormalityTracker();
                    break;
                case Class.Priest:
                    CurrentAbnormalityTracker = new PriestAbnormalityTracker();
                    break;
                case Class.Mystic:
                    CurrentAbnormalityTracker = new MysticAbnormalityTracker();
                    break;
                case Class.Reaper:
                    CurrentAbnormalityTracker = new ReaperAbnormalityTracker();
                    break;
                case Class.Gunner:
                    CurrentAbnormalityTracker = new GunnerAbnormalityTracker();
                    break;
                case Class.Brawler:
                    CurrentAbnormalityTracker = new BrawlerAbnormalityTracker();
                    break;
                case Class.Ninja:
                    CurrentAbnormalityTracker = new NinjaAbnormalityTracker();
                    break;
                case Class.Valkyrie:
                    CurrentAbnormalityTracker = new ValkyrieAbnormalityTracker();
                    break;
                default:
                    break;
            }
        }
        public static bool BeginAbnormality(uint id, ulong target, uint duration, int stacks)
        {
            if (!SessionManager.AbnormalityDatabase.Abnormalities.TryGetValue(id, out var ab)) return false;
            if (!Filter(ab)) return false;
            if (duration == int.MaxValue) ab.Infinity = true;
            if (target.IsMe())
            {
                BeginPlayerAbnormality(ab, stacks, duration);
                if (!Settings.DisablePartyAbnormals)
                {
                    GroupWindowViewModel.Instance.BeginOrRefreshAbnormality(
                        ab,
                        stacks,
                        duration,
                        SessionManager.CurrentPlayer.PlayerId,
                        SessionManager.CurrentPlayer.ServerId
                    );
                }
            }
            else
            {
                BeginNpcAbnormality(ab, stacks, duration, target);
            }

            return true;
        }
        public static bool EndAbnormality(ulong target, uint id)
        {
            if (!SessionManager.AbnormalityDatabase.Abnormalities.TryGetValue(id, out var ab)) return false;
            if (target.IsMe()) EndPlayerAbnormality(ab);
            else BossGageWindowViewModel.Instance.EndNpcAbnormality(target, ab);

            return true;
        }

        private static void BeginPlayerAbnormality(Abnormality ab, int stacks, uint duration)
        {
            Log.CW($"[BeginPlayerAbnormality] {ab.Name} ({ab.Id})");
            if (ab.Type == AbnormalityType.Buff)
            {
                if (ab.Infinity)
                {
                    SessionManager.CurrentPlayer.AddOrRefreshInfBuff(ab, duration, stacks);
                }
                else
                {
                    SessionManager.CurrentPlayer.AddOrRefreshBuff(ab, duration, stacks);
                    if (ab.IsShield) SessionManager.SetPlayerMaxShield(ab.ShieldSize);
                }
            }
            else
            {
                SessionManager.CurrentPlayer.AddOrRefreshDebuff(ab, duration, stacks);
                SessionManager.CurrentPlayer.AddToDebuffList(ab);
            }
            CheckPassivity(ab, duration);
        }

        private static void CheckPassivity(Abnormality ab, uint duration)
        {
            if (PassivityDatabase.Passivities.Contains(ab.Id))
            {
                SkillManager.AddPassivitySkill(ab.Id, 60);
            }
            else if (CooldownWindowViewModel.Instance.MainSkills.Any(m => m.CooldownType == CooldownType.Passive && ab.Id == m.Skill.Id) ||
                CooldownWindowViewModel.Instance.SecondarySkills.Any(m => m.CooldownType == CooldownType.Passive && ab.Id == m.Skill.Id))

            {
                //TODO: can't do this correctly since we don't know passivity cooldown from database so we just add duration
                SkillManager.AddPassivitySkill(ab.Id, duration / 1000);
            }
        }

        private static void EndPlayerAbnormality(Abnormality ab)
        {
            GroupWindowViewModel.Instance.EndAbnormality(ab, SessionManager.CurrentPlayer.PlayerId, SessionManager.CurrentPlayer.ServerId);

            if (ab.Type == AbnormalityType.Buff)
            {
                if (ab.Infinity)
                {
                    SessionManager.CurrentPlayer.RemoveInfBuff(ab);
                }
                else
                {
                    SessionManager.CurrentPlayer.RemoveBuff(ab);
                    if (ab.IsShield)
                    {
                        SessionManager.SetPlayerShield(0);
                        SessionManager.SetPlayerMaxShield(0);
                    }
                }
            }
            else
            {
                SessionManager.CurrentPlayer.RemoveDebuff(ab);
                SessionManager.CurrentPlayer.RemoveFromDebuffList(ab);
            }
        }

        private static void BeginNpcAbnormality(Abnormality ab, int stacks, uint duration, ulong target)
        {
            //if (EntitiesViewModel.TryGetBossById(target, out Npc b))
            //{
            //    b.AddorRefresh(ab, duration, stacks, BOSS_AB_SIZE, BOSS_AB_LEFT_MARGIN);
            //}
            BossGageWindowViewModel.Instance.AddOrRefreshNpcAbnormality(ab, stacks, duration, target);
        }

        private static bool Filter(Abnormality ab)
        {
            return ab.IsShow &&
                   !ab.Name.Contains("BTS") && 
                   !ab.ToolTip.Contains("BTS") && 
                   (
                   !ab.Name.Contains("(Hidden)") && 
                   !ab.Name.Equals("Unknown") && 
                   !ab.Name.Equals(string.Empty)
                   );
        }

        public static void BeginOrRefreshPartyMemberAbnormality(uint playerId, uint serverId, uint id, uint duration, int stacks)
        {
            if (!SessionManager.AbnormalityDatabase.Abnormalities.TryGetValue(id, out var ab)) return;
            if (!Filter(ab)) return;
            GroupWindowViewModel.Instance.BeginOrRefreshAbnormality(ab, stacks, duration, playerId, serverId);
        }

        internal static void EndPartyMemberAbnormality(uint playerId, uint serverId, uint id)
        {
            if (!SessionManager.AbnormalityDatabase.Abnormalities.TryGetValue(id, out var ab)) return;
            if (!Filter(ab)) return;
            GroupWindowViewModel.Instance.EndAbnormality(ab, playerId, serverId);
        }
    }
}
