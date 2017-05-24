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
    public class ArcherBarManager : ClassManager
    {
        private static ArcherBarManager _instance;
        public static ArcherBarManager Instance => _instance ?? (_instance = new ArcherBarManager());

        public ArcherFocusTracker Focus { get; set; }
        public StanceTracker<ArcherStance> Stance { get; set; }
        public ArcherBarManager()
        {
            _instance = this;
            Focus = new ArcherFocusTracker();
            Stance = new StanceTracker<ArcherStance>();
            CurrentClassManager = this;
            LoadSkills("archer-skills.xml", Class.Archer);
        }

        protected override void LoadSkills(string filename, Class c)
        {
            //User defined skills
            if (!File.Exists("resources/config/" + filename))
            {
                //create default file
                XElement skills = new XElement("Skills",
                    new XElement("Skill", new XAttribute("id", 30900), new XAttribute("row", 1)),   //RA
                    new XElement("Skill", new XAttribute("id", 41200), new XAttribute("row", 1)),   //PA
                    new XElement("Skill", new XAttribute("id", 50200), new XAttribute("row", 1)),   //RoA
                    new XElement("Skill", new XAttribute("id", 80900), new XAttribute("row", 1)),   //RF
                    new XElement("Skill", new XAttribute("id", 290100), new XAttribute("row", 1)),  //TB
                    new XElement("Skill", new XAttribute("id", 250200), new XAttribute("row", 1)),  //IT
                    new XElement("Skill", new XAttribute("id", 220800), new XAttribute("row", 1))   //SF
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
