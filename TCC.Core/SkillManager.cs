using System;
using TCC.Data;
using TCC.Data.Databases;
using TCC.ViewModels;

namespace TCC
{
    public static class SkillManager
    {
        public static event Action SkillStarted;

        public const int LongSkillTreshold = 40000;
        public const int Ending = 1; //TODO: this stuff should be removed btw

        private static bool Pass(Skill sk)
        {
            //if (sk.Class == Class.Sorcerer && sk.Id == 280150) return false; //another stupid fix
            if (sk.Detail == "off") return false;
            return sk.Class != Class.Common && sk.Class != Class.None;
        }
        public static void AddSkillDirectly(Skill sk, uint cd)
        {
            RouteSkill(new SkillCooldown(sk, cd, CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher()));
        }

        private static void RouteSkill(SkillCooldown skillCooldown)
        {
            if (skillCooldown.Cooldown == 0)
            {
                CooldownWindowViewModel.Instance.Remove(skillCooldown.Skill);
            }
            else
            {
                CooldownWindowViewModel.Instance.AddOrRefresh(skillCooldown);
            }
            App.BaseDispatcher.Invoke(() => SkillStarted?.Invoke());
        }

        public static void ResetSkill(uint id)
        {
            if (SessionManager.SkillsDatabase.TryGetSkill(id, SessionManager.CurrentPlayer.Class, out var skill))
            {
                if (!Pass(skill)) return;
                CooldownWindowViewModel.Instance.ResetSkill(skill);
            }
        }

        public static void AddSkill(uint id, ulong cd)
        {
            if (SessionManager.SkillsDatabase.TryGetSkill(id, SessionManager.CurrentPlayer.Class, out var skill))
            {
                if (!Pass(skill)) return;
                RouteSkill(new SkillCooldown(skill, cd, CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher()));
                //WindowManager.SkillsEnded = false;
            }
        }
        public static void AddItemSkill(uint id, uint cd)
        {
            if (SessionManager.ItemsDatabase.TryGetItemSkill(id, out var brooch))
            {
                try
                {
                    RouteSkill(new SkillCooldown(brooch, cd, CooldownType.Item, CooldownWindowViewModel.Instance.GetDispatcher()));

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }

        public static void ChangeSkillCooldown(uint id, uint cd)
        {
            if (SessionManager.SkillsDatabase.TryGetSkill(id, SessionManager.CurrentPlayer.Class, out var skill))
            {
                if (!Pass(skill)) return;
                CooldownWindowViewModel.Instance.Change(skill, cd);
            }

        }

        public static void Clear()
        {
            CooldownWindowViewModel.Instance.ShortSkills.Clear();
            CooldownWindowViewModel.Instance.LongSkills.Clear();

            //SessionManager.CurrentPlayer.Class = Class.None;
            SessionManager.CurrentPlayer.EntityId = 0;
            SessionManager.Logged = false;
        }

        public static void AddPassivitySkill(uint abId, uint cd)
        {
            if (PassivityDatabase.TryGetPassivitySkill(abId, out var skill))
            {
                RouteSkill(new SkillCooldown(skill, cd * 1000, CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher()));
            }

        }
    }
}
