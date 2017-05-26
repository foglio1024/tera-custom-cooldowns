using System;
using System.IO;
using System.Xml.Linq;
using TCC.Data;

namespace TCC.ViewModels
{
    public class PriestBarManager : ClassManager
    {
        private static PriestBarManager _instance;
        public static PriestBarManager Instance => _instance ?? (_instance = new PriestBarManager());

        public DurationCooldownIndicator EnergyStars { get; private set; }

        public PriestBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSkills("priest-skills.xml", Class.Priest);
        }
        protected override void LoadSkills(string filename, Class c)
        {
            //User defined skills
            if (!File.Exists("resources/config/" + filename))
            {
                //create default file
                XElement skills = new XElement("Skills",
                    new XElement("Skill", new XAttribute("id", 280100), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 291100), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 390100), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 120100), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 181200), new XAttribute("row", 1))
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

            //Energy Stars
            EnergyStars = new DurationCooldownIndicator();
            SkillsDatabase.TryGetSkill(350410, Class.Priest, out Skill es);
            EnergyStars.Cooldown = new FixedSkillCooldown(es, CooldownType.Skill, Dispatcher);
            EnergyStars.Buff = new FixedSkillCooldown(es, CooldownType.Skill, Dispatcher);
        }

        protected override bool StartSpecialSkill(SkillCooldown sk)
        {
            if(sk.Skill.IconName == EnergyStars.Cooldown.Skill.IconName)
            {
                EnergyStars.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}