using System;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC
{
    public static class SkillManager
    {
        public const int LongSkillTreshold = 40000;

        public static void AddSkill(uint id, ulong cd)
        {
            if (!Game.DB.SkillsDatabase.TryGetSkill(id, Game.Me.Class, out var skill)) return;
            if (!Pass(skill)) return;
            RouteSkill(new Cooldown(skill, cd));
        }
        public static void AddItemSkill(uint id, uint cd)
        {
            if (Game.DB.ItemsDatabase.TryGetItemSkill(id, out var brooch))
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
            if (!PassivityDatabase.TryGetPassivitySkill(abId, out var skill)) return;
            RouteSkill(new Cooldown(skill, cd * 1000, CooldownType.Passive));

        }
        public static void AddSkillDirectly(Skill sk, uint cd, CooldownType type = CooldownType.Skill, CooldownMode mode = CooldownMode.Normal)
        {
            RouteSkill(new Cooldown(sk, cd, type, mode));
        }

        public static void ChangeSkillCooldown(uint id, uint cd)
        {
            if (!Game.DB.SkillsDatabase.TryGetSkill(id, Game.Me.Class, out var skill)) return;
            if (!Pass(skill)) return;
            WindowManager.ViewModels.CooldownsVM.Change(skill, cd);

        }
        public static void ResetSkill(uint id)
        {
            if (!Game.DB.SkillsDatabase.TryGetSkill(id, Game.Me.Class, out var skill)) return;
            if (!Pass(skill)) return;
            WindowManager.ViewModels.CooldownsVM.ResetSkill(skill);
        }

        private static void RouteSkill(Cooldown skillCooldown)
        {
            if (skillCooldown.Duration == 0)
            {
                skillCooldown.Dispose();
                WindowManager.ViewModels.CooldownsVM.Remove(skillCooldown.Skill);
            }
            else
            {
                WindowManager.ViewModels.CooldownsVM.AddOrRefresh(skillCooldown);
            }
        }

        private static bool Pass(Skill sk)
        {
            if (sk.Detail == "off") return false;
            return sk.Class != Class.Common && sk.Class != Class.None;
        }
    }
}
