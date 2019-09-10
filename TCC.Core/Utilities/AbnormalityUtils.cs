using System.Linq;
using TCC.ClassSpecific;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Databases;
using TeraDataLite;

namespace TCC.Utilities
{
    public static class AbnormalityUtils
    {
        public static ClassAbnormalityTracker CurrentAbnormalityTracker { get; set; }

        public static bool Pass(Abnormality ab)
        {
            return ab.IsShow
                   && !ab.ToolTip.Contains("BTS")
                   && !ab.Name.Contains("BTS")
                   && !ab.Name.Contains("(Hidden)")
                   && !ab.Name.Equals("Unknown")
                   && !ab.Name.Equals(string.Empty);
        }
        public static bool Exists(uint id, out Abnormality ab)
        {
            return Game.DB.AbnormalityDatabase.Abnormalities.TryGetValue(id, out ab);
        }

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
            }
        }

        // can still be used
        #region Deprecated 
        public static void BeginAbnormality(uint id, ulong target, ulong source, uint duration, int stacks)
        {
            if (!Exists(id, out var ab) || !Pass(ab)) return;
            if (duration == int.MaxValue) ab.Infinity = true;
            if (Game.IsMe(target))
            {
                BeginPlayerAbnormality(ab, stacks, duration);
                if (WindowManager.ViewModels.Group.Size <= App.Settings.GroupWindowSettings.DisableAbnormalitiesThreshold)
                {
                    WindowManager.ViewModels.Group.BeginOrRefreshAbnormality(ab, stacks, duration, Game.Me.PlayerId, Game.Me.ServerId);
                }
            }
            else
            {
                BeginNpcAbnormality(ab, stacks, duration, target);
            }
            if (Game.IsMe(source) || Game.IsMe(target)) CheckPassivity(ab, duration);
        }
        public static void EndAbnormality(ulong target, uint id)
        {
            if (!Exists(id, out var ab) || !Pass(ab)) return;
            if (Game.IsMe(target)) EndPlayerAbnormality(ab);
            else WindowManager.ViewModels.NPC.EndAbnormality(target, ab);
        }
        public static void CheckPassivity(Abnormality ab, uint duration)
        {
            if (App.Settings.EthicalMode) return;
            if (PassivityDatabase.Passivities.TryGetValue(ab.Id, out var cdFromDb))
            {
                SkillManager.AddPassivitySkill(ab.Id, cdFromDb);
            }
            else if (WindowManager.ViewModels.Cooldowns.MainSkills.Any(m => m.CooldownType == CooldownType.Passive && ab.Id == m.Skill.Id)
                  || WindowManager.ViewModels.Cooldowns.SecondarySkills.Any(m => m.CooldownType == CooldownType.Passive && ab.Id == m.Skill.Id))
            {
                //note: can't do this correctly since we don't know passivity cooldown from database so we just add duration
                SkillManager.AddPassivitySkill(ab.Id, duration / 1000);
            }
        }
        private static void BeginPlayerAbnormality(Abnormality ab, int stacks, uint duration)
        {
            if (ab.Type == AbnormalityType.Buff || ab.Type == AbnormalityType.Special)
            {
                if (ab.Infinity)
                {
                    Game.Me.AddOrRefreshInfBuff(ab, duration, stacks);
                }
                else
                {
                    Game.Me.AddOrRefreshBuff(ab, duration, stacks);
                }
            }
            else
            {
                Game.Me.AddOrRefreshDebuff(ab, duration, stacks);
                Game.Me.AddToDebuffList(ab);
            }
        }
        private static void EndPlayerAbnormality(Abnormality ab)
        {
            WindowManager.ViewModels.Group.EndAbnormality(ab, Game.Me.PlayerId, Game.Me.ServerId);

            if (ab.Type == AbnormalityType.Buff || ab.Type == AbnormalityType.Special)
            {
                if (ab.Infinity)
                {
                    Game.Me.RemoveInfBuff(ab);
                }
                else
                {
                    Game.Me.RemoveBuff(ab);

                }
            }
            else
            {
                Game.Me.RemoveDebuff(ab);
                Game.Me.RemoveFromDebuffList(ab);
            }
        }
        private static void BeginNpcAbnormality(Abnormality ab, int stacks, uint duration, ulong target)
        {
            WindowManager.ViewModels.NPC.UpdateAbnormality(ab, stacks, duration, target);
        }
        #endregion

    }
}
