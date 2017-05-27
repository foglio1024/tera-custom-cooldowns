using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TCC.Data;

namespace TCC.ViewModels
{
    class SorcererBarManager : ClassManager
    {
        private static SorcererBarManager _instance;
        public static SorcererBarManager Instance => _instance ?? (_instance = new SorcererBarManager());

        public SorcererBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSkills("sorcerer-skills.xml", Class.Sorcerer);
        }
        protected override void LoadSkills(string filename, Class c)
        {
            //User defined skills
            if (!File.Exists("resources/config/" + filename))
            {
                //create default file
                XElement skills = new XElement("Skills",
                    new XElement("Skill", new XAttribute("id", 270400), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 40900), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 60700), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 111140), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 300100), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 120600), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 310100), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 340200), new XAttribute("row", 1))
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
        }
    }
}
