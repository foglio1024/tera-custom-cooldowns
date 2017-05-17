using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Data;

namespace TCC.ViewModels
{
    public class EdgeCounter : TSPropertyChanged
    {
        private int edge = 0;
        public int Edge
        {
            get => edge;
            set
            {
                if (edge == value) return;
                edge = value;
                if (edge == 10)
                {
                    NotifyPropertyChanged("Maxed");
                }
                RefreshTimer();
                NotifyPropertyChanged("Edge");
            }
        }

        DispatcherTimer _expire;
        void RefreshTimer()
        {
            _expire.Stop();
            if (edge == 0) return;
            _expire.Start();
        }
        public EdgeCounter()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _expire = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(8000) };
            _expire.Tick += (s ,ev) => Edge = 0;
        }
    }
    public class IntTracker : TSPropertyChanged
    {
        private int val = 0;
        public int Val
        {
            get { return val; }
            set {
                if (val == value) return;
                val = value;

                NotifyPropertyChanged("Val");
            }            
        }
        public IntTracker()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }
    }
    public class WarriorBarManager : ClassManager
    {
        private static WarriorBarManager _instance;
        public static WarriorBarManager Instance => _instance ?? (_instance = new WarriorBarManager());



        public FixedSkillCooldown DeadlyGamble { get; set; }
        public FixedSkillCooldown DeadlyGambleBuff { get; set; }

        public EdgeCounter EdgeCounter { get; set; }
        public int SkillCount { get; set; }

        public WarriorBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            EdgeCounter = new EdgeCounter();
            LoadSkills();
            SkillCount = Math.Max(SecondarySkills.Count, MainSkills.Count);
        }

        void LoadSkills()
        {
            //User defined skills
            XDocument skillsDoc = XDocument.Load("resources/config/warrior-skills.xml");
            foreach (XElement skillElement in skillsDoc.Descendants("Skill"))
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
            var skill = MainSkills.FirstOrDefault(x => x.Skill.IconName == s.Skill.IconName);
            if (skill != null)
            {
                skill.Refresh(0);
                return;
            }
            skill = SecondarySkills.FirstOrDefault(x => x.Skill.IconName == s.Skill.IconName);
            if (skill != null)
            {
                skill.Refresh(0);
                return;
            }
            try
            {
                var otherSkill = OtherSkills.FirstOrDefault(x => x.Skill.Name == s.Skill.Name);
                if (otherSkill != null)
                {

                    OtherSkills.Remove(otherSkill);
                    otherSkill.Dispose();
                }
            }
            catch { }
        }
        internal void RemoveSkill(Skill skill)
        {
            try
            {

                var otherSkill = OtherSkills.FirstOrDefault(x => x.Skill.Name == skill.Name);
                if (otherSkill != null)
                {

                    OtherSkills.Remove(otherSkill);
                    otherSkill.Dispose();
                }
            }
            catch
            {

            }

        }

        internal void SetRE(int currRe)
        {
            throw new NotImplementedException();
        }

        internal void StartCooldown(SkillCooldown sk)
        {
            var skill = MainSkills.FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (skill != null)
            {
                skill.Start(sk.Cooldown);

                return;
            }
            skill = SecondarySkills.FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
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
                var existing = OtherSkills.FirstOrDefault(x => x.Skill.Name == sk.Skill.Name);
                if (existing == null)
                {
                    OtherSkills.Add(sk);
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
