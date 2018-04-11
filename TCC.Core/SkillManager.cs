using System;
using TCC.Data.Databases;
using TCC.ViewModels;

namespace TCC
{
    public static class SkillManager
    {
        public const int LongSkillTreshold = 40000;
        public const int Ending = 120;

        static bool Filter(string name)
        {
            if (name != "Unknown" &&
                !name.Contains("Summon:") &&
                !name.Contains("Flight:") &&
                !name.Contains("Super Rocket Jump") &&
                !name.Contains("greeting") ||
                name == "Summon: Party") return true;
            else return false;

        }
        public static void AddSkillDirectly(Skill sk, uint cd)
        {
            RouteSkill(new SkillCooldown(sk, cd, CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher()));
        }

        static void RouteSkill(SkillCooldown skillCooldown)
        {
            if (skillCooldown.Cooldown == 0)
            {
                CooldownWindowViewModel.Instance.Remove(skillCooldown.Skill);
            }
            else
            {
                CooldownWindowViewModel.Instance.AddOrRefresh(skillCooldown);
            }
        }



        public static void AddSkill(uint id, ulong cd)
        {
            if (SkillsDatabase.TryGetSkill(id, SessionManager.CurrentPlayer.Class, out Skill skill))
            {
                if (!Filter(skill.Name))
                {
                    return;
                }
                RouteSkill(new SkillCooldown(skill, cd, CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher()));
                //WindowManager.SkillsEnded = false;
            }
        }
        public static void AddItemSkill(uint id, uint cd)
        {
            if (ItemSkillsDatabase.TryGetItemSkill(id, out Skill brooch))
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
            if (SkillsDatabase.TryGetSkill(id, SessionManager.CurrentPlayer.Class, out Skill skill))
            {
                CooldownWindowViewModel.Instance.Change(skill, cd);
            }

        }

        public static void Clear()
        {
            CooldownWindowViewModel.Instance.ShortSkills.Clear();
            CooldownWindowViewModel.Instance.LongSkills.Clear();

            SessionManager.CurrentPlayer.Class = Class.None;
            SessionManager.CurrentPlayer.EntityId = 0;
            SessionManager.Logged = false;
        }

        public static void AddPassivitySkill(uint abId, uint cd)
        {
            if (PassivityDatabase.TryGetPassivitySkill(abId, out var skill))
            {
                RouteSkill(new SkillCooldown(skill, cd*1000, CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher()));
            }

        }
    }
}
