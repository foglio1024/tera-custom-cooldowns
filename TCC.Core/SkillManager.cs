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
        public const int Ending = 1; //TODO: this stuff should be removed btw



        public static void AddSkill(uint id, ulong cd)
        {
            if (SessionManager.DB.SkillsDatabase.TryGetSkill(id, SessionManager.CurrentPlayer.Class, out var skill))
            {
                if (!Pass(skill)) return;
                RouteSkill(new Cooldown(skill, cd));
                //WindowManager.SkillsEnded = false;
            }
        }
        public static void AddItemSkill(uint id, uint cd)
        {
            if (SessionManager.DB.ItemsDatabase.TryGetItemSkill(id, out var brooch))
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
            if (SessionManager.DB.SkillsDatabase.TryGetSkill(id, SessionManager.CurrentPlayer.Class, out var skill))
            {
                if (!Pass(skill)) return;
                WindowManager.CooldownWindow.VM.Change(skill, cd);
            }

        }
        public static void ResetSkill(uint id)
        {
            if (SessionManager.DB.SkillsDatabase.TryGetSkill(id, SessionManager.CurrentPlayer.Class, out var skill))
            {
                if (!Pass(skill)) return;
                WindowManager.CooldownWindow.VM.ResetSkill(skill);
            }
        }
        public static void Clear()
        {
            WindowManager.CooldownWindow.VM.ClearSkills();
        }

        private static void RouteSkill(Cooldown skillCooldown)
        {

            if (skillCooldown.Duration == 0)
            {
                skillCooldown.Dispose();
                WindowManager.CooldownWindow.VM.Remove(skillCooldown.Skill);
            }
            else
            {
                WindowManager.CooldownWindow.VM.AddOrRefresh(skillCooldown);
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
