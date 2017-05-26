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
    public class NinjaBarManager : ClassManager
    {
        private static NinjaBarManager _instance;
        public static NinjaBarManager Instance => _instance ?? (_instance = new NinjaBarManager());

        public NinjaBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSkills("ninja-skills.xml", Class.Assassin);
        }
        protected override void LoadSkills(string filename, Class c)
        {
            //User defined skills
            if (!File.Exists("resources/config/" + filename))
            {
                //create default file
                XElement skills = new XElement("Skills",
                    new XElement("Skill", new XAttribute("id", 141100), new XAttribute("row", 1)), //DC
                    new XElement("Skill", new XAttribute("id", 121100), new XAttribute("row", 1)), //SF
                    new XElement("Skill", new XAttribute("id", 131000), new XAttribute("row", 1)), //CoS
                    new XElement("Skill", new XAttribute("id", 71200), new XAttribute("row", 1)),  //DJ
                    new XElement("Skill", new XAttribute("id", 41100), new XAttribute("row", 1)),  //JP
                    new XElement("Skill", new XAttribute("id", 61000), new XAttribute("row", 1))  //1kCuts
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
