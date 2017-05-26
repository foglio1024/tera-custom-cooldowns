using System;
using System.IO;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Data;

namespace TCC.ViewModels
{
    internal class MysticBarManager : ClassManager
    {
        private static MysticBarManager _instance;
        public static MysticBarManager Instance => _instance ?? (_instance = new MysticBarManager());

        public AurasTracker Auras { get; private set; }

        public MysticBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSkills("mystic-skills.xml", Class.Elementalist);
            Auras = new AurasTracker();
        }
        protected override void LoadSkills(string filename, Class c)
        {
            //User defined skills
            if (!File.Exists("resources/config/" + filename))
            {
                //create default file
                XElement skills = new XElement("Skills",
                    new XElement("Skill", new XAttribute("id", 50900), new XAttribute("row", 1)), //heal
                    new XElement("Skill", new XAttribute("id", 90100), new XAttribute("row", 1)), //cleanse
                    new XElement("Skill", new XAttribute("id", 420100), new XAttribute("row", 1)), //boomerang pulse
                    new XElement("Skill", new XAttribute("id", 370200), new XAttribute("row", 1)), //totem
                    new XElement("Skill", new XAttribute("id", 241010), new XAttribute("row", 1)), //voc
                    new XElement("Skill", new XAttribute("id", 410100), new XAttribute("row", 1)), //contagion
                    new XElement("Skill", new XAttribute("id", 120100), new XAttribute("row", 1)) //vow
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