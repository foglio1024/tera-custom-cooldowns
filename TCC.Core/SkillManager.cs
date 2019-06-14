using System;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC
{
    public static class SkillManager
    {
        public static event Action SkillStarted;

        public const int LongSkillTreshold = 40000;

        public static void AddSkill(uint id, ulong cd)
        {
            if (!Session.DB.SkillsDatabase.TryGetSkill(id, Session.Me.Class, out var skill)) return;
            if (!Pass(skill)) return;
            RouteSkill(new Cooldown(skill, cd));
        }
        public static void AddItemSkill(uint id, uint cd)
        {
            if (Session.DB.ItemsDatabase.TryGetItemSkill(id, out var brooch))
            {
                try
                {
                    RouteSkill(new Cooldown(brooch, cd, CooldownType.Item));

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }
        public static void AddPassivitySkill(uint abId, uint cd)
        {
            if (PassivityDatabase.TryGetPassivitySkill(abId, out var skill))
            {
                RouteSkill(new Cooldown(skill, cd * 1000, CooldownType.Passive));
            }

        }
        public static void AddSkillDirectly(Skill sk, uint cd, CooldownType type = CooldownType.Skill, CooldownMode mode = CooldownMode.Normal)
        {
            RouteSkill(new Cooldown(sk, cd, type, mode));
        }

        public static void ChangeSkillCooldown(uint id, uint cd)
        {
            if (Session.DB.SkillsDatabase.TryGetSkill(id, Session.Me.Class, out var skill))
            {
                if (!Pass(skill)) return;
                WindowManager.ViewModels.Cooldowns.Change(skill, cd);
            }

        }
        public static void ResetSkill(uint id)
        {
            if (Session.DB.SkillsDatabase.TryGetSkill(id, Session.Me.Class, out var skill))
            {
                if (!Pass(skill)) return;
                WindowManager.ViewModels.Cooldowns.ResetSkill(skill);
            }
        }
        //public static void Clear()
        //{
        //    WindowManager.ViewModels.Cooldowns.ClearSkills();
        //}

        private static void RouteSkill(Cooldown skillCooldown)
        {

            if (skillCooldown.Duration == 0)
            {
                skillCooldown.Dispose();
                WindowManager.ViewModels.Cooldowns.Remove(skillCooldown.Skill);
            }
            else
            {
                WindowManager.ViewModels.Cooldowns.AddOrRefresh(skillCooldown);
            }
            App.BaseDispatcher.BeginInvoke(new Action(() => SkillStarted?.Invoke()));
        }
        private static bool Pass(Skill sk)
        {
            if (sk.Detail == "off") return false;
            return sk.Class != Class.Common && sk.Class != Class.None;
        }
    }
}
