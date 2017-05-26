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
    public class SlayerBarManager : ClassManager
    {
        private static SlayerBarManager _instance;
        public static SlayerBarManager Instance => _instance ?? (_instance = new SlayerBarManager());

        public SlayerBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSkills("slayer-skills.xml", Class.Slayer);
        }

        protected override void LoadSkills(string filename, Class c)
        {
            //User defined skills
            if (!File.Exists("resources/config/" + filename))
            {
                //create default file
                XElement skills = new XElement("Skills",
                    new XElement("Skill", new XAttribute("id", 21100), new XAttribute("row", 1)),  
                    new XElement("Skill", new XAttribute("id", 80930), new XAttribute("row", 1)), 
                    new XElement("Skill", new XAttribute("id", 120500), new XAttribute("row", 1)),  
                    new XElement("Skill", new XAttribute("id", 230200), new XAttribute("row", 1)),  
                    new XElement("Skill", new XAttribute("id", 240100), new XAttribute("row", 1))
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
