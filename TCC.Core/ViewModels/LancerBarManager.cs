using System;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Data;

namespace TCC.ViewModels
{
    public class DurationCooldownIndicator : TSPropertyChanged
    {
        public FixedSkillCooldown Cooldown { get; set; }
        public FixedSkillCooldown Buff { get; set; }

        public DurationCooldownIndicator()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Cooldown = new FixedSkillCooldown(_dispatcher);
            Buff = new FixedSkillCooldown(_dispatcher);
        }
    }
    internal class LancerBarManager : ClassManager
    {
        private static LancerBarManager _instance;
        public static LancerBarManager Instance => _instance ?? (_instance = new LancerBarManager());
        public LancerBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSkills("lancer-skills.xml", Class.Lancer);
            LH = new StatTracker()
            {
                Max = 10
            };
        }

        //public FixedSkillCooldown AdrenalineRush { get; set; }
        //public FixedSkillCooldown AdrenalineRushBuff { get; set; }
        //public FixedSkillCooldown GuardianShout { get; set; }
        //public FixedSkillCooldown GuardianShoutBuff { get; set; }

        public DurationCooldownIndicator AdrenalineRush { get; set; }
        public DurationCooldownIndicator GuardianShout { get; set; }
        public DurationCooldownIndicator Infuriate { get; set; }
        public StatTracker LH { get; set; }

        protected override bool StartSpecialSkill(SkillCooldown sk)
        {
            if (sk.Skill.IconName == GuardianShout.Cooldown.Skill.IconName)
            {
                GuardianShout.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if (sk.Skill.IconName == AdrenalineRush.Cooldown.Skill.IconName)
            {
                AdrenalineRush.Cooldown.Start(sk.Cooldown);
                return true;
            }
            if(sk.Skill.IconName == Infuriate.Cooldown.Skill.IconName)
            {
                Infuriate.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }

        protected override void LoadSkills(string filename, Class c)
        {
            //User defined skills
            if (!File.Exists("resources/config/" + filename))
            {
                //create default file
                XElement skills = new XElement("Skills",
                    new XElement("Skill", new XAttribute("id", 50100), new XAttribute("row", 1)),   //shield bash
                    new XElement("Skill", new XAttribute("id", 30800), new XAttribute("row", 1)),   //onslaught
                    new XElement("Skill", new XAttribute("id", 181100), new XAttribute("row", 1)),  //shield barrage
                    new XElement("Skill", new XAttribute("id", 131100), new XAttribute("row", 1)),  //spring attack
                    new XElement("Skill", new XAttribute("id", 100300), new XAttribute("row", 1)),  //debilitate
                    new XElement("Skill", new XAttribute("id", 250730), new XAttribute("row", 1))   //wallop
                    );
                skills.Save("resources/config/" + filename);
            }

            XDocument skillsDoc = XDocument.Load("resources/config/" + filename);
            foreach (XElement skillElement in skillsDoc.Descendants("Skill"))
            {
                uint skillId = Convert.ToUInt32(skillElement.Attribute("id").Value);
                int row = Convert.ToInt32(skillElement.Attribute("row").Value);

                if (SkillsDatabase.TryGetSkill(skillId, c, out Skill sk))
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

            SkillsDatabase.TryGetSkill(70300, c, out Skill gshout);
            SkillsDatabase.TryGetSkill(170200, c, out Skill arush);
            SkillsDatabase.TryGetSkill(120100, c, out Skill infu);

            GuardianShout = new DurationCooldownIndicator();
            AdrenalineRush = new DurationCooldownIndicator();
            Infuriate = new DurationCooldownIndicator();

            GuardianShout.Cooldown = new FixedSkillCooldown(gshout, CooldownType.Skill, Dispatcher);
            GuardianShout.Buff = new FixedSkillCooldown(gshout, CooldownType.Skill, Dispatcher);
            AdrenalineRush.Cooldown = new FixedSkillCooldown(arush, CooldownType.Skill, Dispatcher);
            AdrenalineRush.Buff = new FixedSkillCooldown(arush, CooldownType.Skill, Dispatcher);
            Infuriate.Cooldown = new FixedSkillCooldown(infu, CooldownType.Skill, Dispatcher);
        }
    }
}