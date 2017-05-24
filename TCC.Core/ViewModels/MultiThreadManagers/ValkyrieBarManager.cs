using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.ViewModels
{
    public class ValkyrieBarManager : ClassManager
    {
        private static ValkyrieBarManager _instance;
        public static ValkyrieBarManager Instance => _instance ?? (_instance = new ValkyrieBarManager());
        public Counter RunemarksCounter { get; set; }
        public DurationCooldownIndicator Ragnarok { get; private set; }

        public ValkyrieBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            RunemarksCounter = new Counter(7, false);
            LoadSkills("valkyrie-skills.xml", Class.Glaiver);
        }

        protected override void LoadSkills(string filename, Class c)
        {
            //User defined skills
            if (!File.Exists("resources/config/"+filename))
            {
                //create default file
                XElement skills = new XElement("Skills",
                    new XElement("Skill", new XAttribute("id", 136100), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 66230), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 35930), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 96130), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 156130), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 55530), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 75930), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 205800), new XAttribute("row", 1)),
                    new XElement("Skill", new XAttribute("id", 115431), new XAttribute("row", 1))
                    );
                skills.Save("resources/config/"+filename);
            }

            XDocument skillsDoc = XDocument.Load("resources/config/"+filename);
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


            //Ragnarok
            Ragnarok = new DurationCooldownIndicator();
            SkillsDatabase.TryGetSkill(120100, Class.Glaiver, out Skill rag);
            Ragnarok.Cooldown = new FixedSkillCooldown(rag, CooldownType.Skill, Dispatcher);
            Ragnarok.Buff = new FixedSkillCooldown(rag, CooldownType.Skill, Dispatcher);
        }
        protected override bool StartSpecialSkill(SkillCooldown sk)
        {

            if (sk.Skill.IconName == Ragnarok.Cooldown.Skill.IconName)
            {
                Ragnarok.Cooldown.Start(sk.Cooldown);
                return true;
            }
            return false;
        }
    }
}