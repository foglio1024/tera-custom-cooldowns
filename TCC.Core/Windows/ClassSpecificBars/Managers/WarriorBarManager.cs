using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using TCC.Data;

namespace TCC.ViewModels
{
    public class WarriorBarManager : DependencyObject
    {
        private static WarriorBarManager _instance;
        public static WarriorBarManager Instance => _instance ?? (_instance = new WarriorBarManager());

        public FixedSkillCooldown DeadlyGamble { get; set; }
        public FixedSkillCooldown DeadlyGambleBuff { get; set; }

        private SynchronizedObservableCollection<FixedSkillCooldown> mainSkills;
        public SynchronizedObservableCollection<FixedSkillCooldown> MainSkills
        {
            get
            {
                return mainSkills; ;
            }
            set
            {
                if (mainSkills == value) return;
                mainSkills = value;
            }
        }

        private SynchronizedObservableCollection<FixedSkillCooldown> secondarySkills;
        public SynchronizedObservableCollection<FixedSkillCooldown> SecondarySkills
        {
            get
            {
                return secondarySkills; ;
            }
            set
            {
                if (secondarySkills == value) return;
                secondarySkills = value;
            }
        }

        private SynchronizedObservableCollection<SkillCooldown> otherSkills;
        public SynchronizedObservableCollection<SkillCooldown> OtherSkills
        {
            get
            {
                return otherSkills;
            }
            set
            {
                if (otherSkills == value) return;
                otherSkills = value;
            }
        }

        public WarriorBarManager()
        {
            LoadSkills();
        }

        void LoadSkills()
        {
            //User defined skills

            XDocument warriorSkillsDoc = XDocument.Load("config/warrior-skills.xml");
            foreach (XElement skillElement in warriorSkillsDoc.Descendants("Skill"))
            {
                uint skillId = Convert.ToUInt32(skillElement.Attribute("id").Value);
                int row = Convert.ToInt32(skillElement.Attribute("row").Value);

                if (SkillsDatabase.TryGetSkill(skillId, Class.Warrior, out Skill sk))
                {
                    if (row == 1)
                    {
                        MainSkills.Add(new FixedSkillCooldown(sk, CooldownType.Skill, Dispatcher));
                    }
                    else if (row == 2)
                    {
                        SecondarySkills.Add(new FixedSkillCooldown(sk, CooldownType.Skill, Dispatcher));
                    }
                }
            }

            //Deadly gamble
            SkillsDatabase.TryGetSkill(200200, Class.Warrior, out Skill dg);
            DeadlyGamble = new FixedSkillCooldown(dg, CooldownType.Skill, Dispatcher);
            DeadlyGambleBuff = new FixedSkillCooldown(dg, CooldownType.Skill, Dispatcher);
        }

        internal void ResetCooldown(SkillCooldown s)
        {
            s.SetDispatcher(this.Dispatcher);
            var skill = mainSkills.FirstOrDefault(x => x.Skill.IconName == s.Skill.IconName);
            if (skill != null)
            {
                skill.Refresh(0);
                return;
            }
            skill = secondarySkills.FirstOrDefault(x => x.Skill.IconName == s.Skill.IconName);
            if (skill != null)
            {
                skill.Refresh(0);
                return;
            }
            try
            {
                var otherSkill = otherSkills.FirstOrDefault(x => x.Skill.Name == s.Skill.Name);
                if (otherSkill != null)
                {

                    otherSkills.Remove(otherSkill);
                    otherSkill.Dispose();
                }
            }
            catch { }
        }

        internal void RemoveSkill(Skill skill)
        {
            try
            {

                var otherSkill = otherSkills.FirstOrDefault(x => x.Skill.Name == skill.Name);
                if (otherSkill != null)
                {

                    otherSkills.Remove(otherSkill);
                    otherSkill.Dispose();
                }
            }
            catch
            {

            }

        }

        internal void StartCooldown(SkillCooldown sk)
        {
            var skill = mainSkills.FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (skill != null)
            {
                skill.Start(sk.Cooldown);

                return;
            }
            skill = secondarySkills.FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (skill != null)
            {
                skill.Start(sk.Cooldown);
                return;
            }

            if (sk.Skill.IconName == DeadlyGamble.Skill.IconName)
            {
                DeadlyGamble.Start(sk.Cooldown);
                return;
            }
            AddOrRefreshSkill(sk);



        }
        void AddOrRefreshSkill(SkillCooldown sk)
        {
            sk.SetDispatcher(this.Dispatcher);

            try
            {
                var existing = otherSkills.FirstOrDefault(x => x.Skill.Name == sk.Skill.Name);
                if (existing == null)
                {
                    otherSkills.Add(sk);
                    return;
                }
                existing.Refresh(sk.Cooldown);
            }
            catch
            {

            }
        }

    }
}
