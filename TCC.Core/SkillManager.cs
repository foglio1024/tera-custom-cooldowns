using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TCC.Data.Databases;
using TCC.Parsing.Messages;
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
            RouteSkill(new SkillCooldown(sk, cd, CooldownType.Skill, CooldownWindowManager.Instance.Dispatcher));
        }

        static void RouteSkill(SkillCooldown skillCooldown)
        {
            if (skillCooldown.Cooldown == 0)
            {
                CooldownWindowManager.Instance.RemoveSkill(skillCooldown.Skill);
            }
            else
            {
                CooldownWindowManager.Instance.AddOrRefreshSkill(skillCooldown);
            }
        }



        public static void AddSkill(uint id, uint cd)
        {
            if (SkillsDatabase.TryGetSkill(id, SessionManager.CurrentPlayer.Class, out Skill skill))
            {
                if (!Filter(skill.Name))
                {
                    return;
                }
                RouteSkill(new SkillCooldown(skill, cd, CooldownType.Skill, CooldownWindowManager.Instance.Dispatcher));
                //WindowManager.SkillsEnded = false;
            }
        }
        public static void AddBrooch(uint id, uint cd)
        {
            if (BroochesDatabase.TryGetBrooch(id, out Skill brooch))
            {
                RouteSkill(new SkillCooldown(brooch, cd, CooldownType.Item, CooldownWindowManager.Instance.Dispatcher));
            }

        }

        public static void ChangeSkillCooldown(uint id, uint cd)
        {
            if (SkillsDatabase.TryGetSkill(id, SessionManager.CurrentPlayer.Class, out Skill skill))
            {
                CooldownWindowManager.Instance.RefreshSkill(skill, cd);
            }

        }

        public static void Clear()
        {
            CooldownWindowManager.Instance.ShortSkills.Clear();
            CooldownWindowManager.Instance.LongSkills.Clear();

            SessionManager.CurrentPlayer.Class = Class.None;
            SessionManager.CurrentPlayer.EntityId = 0;
            SessionManager.Logged = false;
        }
    }
}
