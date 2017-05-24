using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using TCC.Data;

namespace TCC.ViewModels
{

    public class WarriorBarManager : ClassManager
    {
        private static WarriorBarManager _instance;
        public static WarriorBarManager Instance => _instance ?? (_instance = new WarriorBarManager());

        public DurationCooldownIndicator DeadlyGamble { get; set; }

        public Counter EdgeCounter { get; set; }
        public StanceTracker<WarriorStance> Stance { get; set; }

        public WarriorBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            EdgeCounter = new Counter(10, true);
            Stance = new StanceTracker<WarriorStance>();
            LoadSkills("warrior-skills.xml", Class.Warrior);

        }

        protected override void LoadSkills(string filename, Class c)
        {
            //User defined skills
            if (!File.Exists("resources/config/" + filename))
            {
                //create default warrior file
                XElement skills = new XElement("Skills",
                    new XElement("Skill", new XAttribute("id", 181100), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 41100), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 110800), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 280730), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 290730), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 160700), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 171100), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 191000), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 300100), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 100700), new XAttribute("row", 2)),
                    new XElement("Skill", new XAttribute("id", 220200), new XAttribute("row", 2)),
                    new XElement("Skill", new XAttribute("id", 120800), new XAttribute("row", 0)),
                    new XElement("Skill", new XAttribute("id", 230200), new XAttribute("row", 0)),
                    new XElement("Skill", new XAttribute("id", 210200), new XAttribute("row", 0)),
                    new XElement("Skill", new XAttribute("id", 270800), new XAttribute("row", 0)),
                    new XElement("Skill", new XAttribute("id", 310900), new XAttribute("row", 0)),
                    new XElement("Skill", new XAttribute("id", 30900), new XAttribute("row", 0)),
                    new XElement("Skill", new XAttribute("id", 50900), new XAttribute("row", 0)),
                    new XElement("Skill", new XAttribute("id", 240900), new XAttribute("row", 0)),
                    new XElement("Skill", new XAttribute("id", 330900), new XAttribute("row", 0)),
                    new XElement("Skill", new XAttribute("id", 340100), new XAttribute("row", 0)),
                    new XElement("Skill", new XAttribute("id", 350100), new XAttribute("row", 0))
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


            //Deadly gamble
            DeadlyGamble = new DurationCooldownIndicator();
            SkillsDatabase.TryGetSkill(200200, c, out Skill dg);
            DeadlyGamble.Buff = new FixedSkillCooldown(dg, CooldownType.Skill, Dispatcher);
            DeadlyGamble.Cooldown = new FixedSkillCooldown(dg, CooldownType.Skill, Dispatcher);
        }

        protected override bool StartSpecialSkill(SkillCooldown sk)
        {

            if (sk.Skill.IconName == DeadlyGamble.Cooldown.Skill.IconName)
            {
                DeadlyGamble.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;

        }

    }
}
