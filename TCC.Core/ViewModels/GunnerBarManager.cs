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
    public class GunnerBarManager : ClassManager
    {
        private static GunnerBarManager _instance;
        public static GunnerBarManager Instance => _instance ?? (_instance = new GunnerBarManager());

        public GunnerBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSkills("gunner-skills.xml", Class.Engineer);
        }
        protected override void LoadSkills(string filename, Class c)
        {
            //User defined skills
            if (!File.Exists("resources/config/" + filename))
            {
                //create default file
                XElement skills = new XElement("Skills",
                    new XElement("Skill", new XAttribute("id", 70800), new XAttribute("row", 1)), 
                    new XElement("Skill", new XAttribute("id", 31100), new XAttribute("row", 1)), 
                    new XElement("Skill", new XAttribute("id", 150900), new XAttribute("row", 1)), 
                    new XElement("Skill", new XAttribute("id", 91000), new XAttribute("row", 1)), 
                    new XElement("Skill", new XAttribute("id", 20700), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 130200), new XAttribute("row", 1))
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
