using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using TCC.Data;
using TCC.Data.Databases;
using TCC.ViewModels;

namespace TCC
{
    public class SkillConfigParser
    {
        public List<FixedSkillCooldown> Main;
        public List<FixedSkillCooldown> Secondary;
        public List<FixedSkillCooldown> Hidden;
        void ParseSkillConfig(string filename, Class c)
        {
            XDocument skillsDoc = XDocument.Load("resources/config/skills/" + filename);
            foreach (XElement skillElement in skillsDoc.Descendants("Skills").Descendants())
            {
                var type = CooldownType.Skill;
                if (skillElement.Name == "Item") type = CooldownType.Item;

                var skillId = Convert.ToUInt32(skillElement.Attribute("id").Value);
                var row = Convert.ToInt32(skillElement.Attribute("row").Value);
                if (type == CooldownType.Skill)
                {
                    if (SkillsDatabase.TryGetSkill(skillId, c, out var sk))
                    {
                        switch (row)
                        {
                            case 1:
                                Main.Add(new FixedSkillCooldown(sk, type, CooldownWindowViewModel.Instance.GetDispatcher(), false));
                                break;
                            case 2:
                                Secondary.Add(new FixedSkillCooldown(sk, type, CooldownWindowViewModel.Instance.GetDispatcher(), false));
                                break;
                            case 3:
                                Hidden.Add(new FixedSkillCooldown(sk, type, CooldownWindowViewModel.Instance.GetDispatcher(), false));
                                break;
                        }
                    }
                }
                else if (type == CooldownType.Item)
                {
                    if (ItemSkillsDatabase.TryGetItemSkill(skillId, out var sk))
                    {
                        switch (row)
                        {
                            case 1:
                                Main.Add(new FixedSkillCooldown(sk, type, CooldownWindowViewModel.Instance.GetDispatcher(), false));
                                break;
                            case 2:
                                Secondary.Add(new FixedSkillCooldown(sk, type, CooldownWindowViewModel.Instance.GetDispatcher(), false));
                                break;
                            case 3:
                                Hidden.Add(new FixedSkillCooldown(sk, type, CooldownWindowViewModel.Instance.GetDispatcher(), false));
                                break;
                        }
                    }
                }
            }
        }

        public SkillConfigParser(string filename, Class c)
        {
            Main = new List<FixedSkillCooldown>();
            Secondary = new List<FixedSkillCooldown>();
            Hidden = new List<FixedSkillCooldown>();
            ParseSkillConfig(filename, c);
        }
    }
    public static class SkillUtils
    {
        private static void SaveSkillFile(XElement xe, string filename)
        {
            if (!Directory.Exists("resources/config/skills")) Directory.CreateDirectory("resources/config/skills");
            xe.Save("resources/config/skills/" + filename);
        }
        public static void BuildDefaultSkillConfig(string filename, Class c)
        {
            switch (c)
            {
                case Class.Warrior:
                    BuildDefaultWarriorSkillConfig("warrior-skills.xml");
                    break;
                case Class.Lancer:
                    BuildDefaultLancerSkillConfig("lancer-skills.xml");
                    break;
                case Class.Slayer:
                    BuildDefaultSlayerSkillConfig("slayer-skills.xml");
                    break;
                case Class.Berserker:
                    BuildDefaultBerserkerSkillConfig("berserker-skills.xml");
                    break;
                case Class.Sorcerer:
                    BuildDefaultSorcererSkillConfig("sorcerer-skills.xml");
                    break;
                case Class.Archer:
                    BuildDefaultArcherSkillConfig("archer-skills.xml");
                    break;
                case Class.Priest:
                    BuildDefaultPriestSkillConfig("priest-skills.xml");
                    break;
                case Class.Elementalist:
                    BuildDefaultMysticSkillConfig("mystic-skills.xml");
                    break;
                case Class.Soulless:
                    BuildDefaultReaperSkillConfig("reaper-skills.xml");
                    break;
                case Class.Engineer:
                    BuildDefaultGunnerSkillConfig("gunner-skills.xml");
                    break;
                case Class.Fighter:
                    BuildDefaultBrawlerSkillConfig("brawler-skills.xml");
                    break;
                case Class.Assassin:
                    BuildDefaultNinjaSkillConfig("ninja-skills.xml");
                    break;
                case Class.Glaiver:
                    BuildDefaultValkyrieSkillConfig("valkyrie-skills.xml");
                    break;
                default:
                    break;
            }
        }

        private static void BuildDefaultValkyrieSkillConfig(string filename)
        {
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
            SaveSkillFile(skills, filename);
        }
        private static void BuildDefaultNinjaSkillConfig(string filename)
        {
            XElement skills = new XElement("Skills",
                new XElement("Skill", new XAttribute("id", 141100), new XAttribute("row", 1)), //DC
                new XElement("Skill", new XAttribute("id", 121100), new XAttribute("row", 1)), //SF
                new XElement("Skill", new XAttribute("id", 131000), new XAttribute("row", 1)), //CoS
                new XElement("Skill", new XAttribute("id", 71200), new XAttribute("row", 1)),  //DJ
                new XElement("Skill", new XAttribute("id", 41100), new XAttribute("row", 1)),  //JP
                new XElement("Skill", new XAttribute("id", 61000), new XAttribute("row", 1))  //1kCuts
                );
            SaveSkillFile(skills, filename);
        }
        private static void BuildDefaultBrawlerSkillConfig(string filename)
        {
            XElement skills = new XElement("Skills",
                new XElement("Skill", new XAttribute("id", 71230), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 91130), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 81130), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 61131), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 41000), new XAttribute("row", 1))
                );
            SaveSkillFile(skills, filename);
        }
        private static void BuildDefaultGunnerSkillConfig(string filename)
        {
            XElement skills = new XElement("Skills",
                new XElement("Skill", new XAttribute("id", 70800), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 31100), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 150900), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 91000), new XAttribute("row", 1))
                //new XElement("Skill", new XAttribute("id", 20700), new XAttribute("row", 1)),
                //new XElement("Skill", new XAttribute("id", 130200), new XAttribute("row", 1))
                );
            SaveSkillFile(skills, filename);
        }
        private static void BuildDefaultReaperSkillConfig(string filename)
        {
            XElement skills = new XElement("Skills",
                new XElement("Skill", new XAttribute("id", 60231), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 30300), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 50330), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 40330), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 200100), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 120200), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 100230), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 80230), new XAttribute("row", 1))
                );
            SaveSkillFile(skills, filename);
        }
        private static void BuildDefaultMysticSkillConfig(string filename)
        {
            XElement skills = new XElement("Skills",
                new XElement("Skill", new XAttribute("id", 420100), new XAttribute("row", 1)), //boomerang pulse
                new XElement("Skill", new XAttribute("id", 370200), new XAttribute("row", 1)), //totem
                new XElement("Skill", new XAttribute("id", 241010), new XAttribute("row", 1)) //voc
                                                                                              //new XElement("Skill", new XAttribute("id", 410100), new XAttribute("row", 1)), //contagion
                                                                                              //new XElement("Skill", new XAttribute("id", 120100), new XAttribute("row", 1)) //vow
                );
            SaveSkillFile(skills, filename);
        }
        private static void BuildDefaultPriestSkillConfig(string filename)
        {
            XElement skills = new XElement("Skills",
                new XElement("Skill", new XAttribute("id", 280100), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 291100), new XAttribute("row", 1)),
                //new XElement("Skill", new XAttribute("id", 390100), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 120100), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 181200), new XAttribute("row", 1))
                );
            SaveSkillFile(skills, filename);
        }
        private static void BuildDefaultArcherSkillConfig(string filename)
        {
            XElement skills = new XElement("Skills",
                new XElement("Skill", new XAttribute("id", 30900), new XAttribute("row", 1)),   //RA
                new XElement("Skill", new XAttribute("id", 41200), new XAttribute("row", 1)),   //PA
                new XElement("Skill", new XAttribute("id", 50200), new XAttribute("row", 1)),   //RoA
                new XElement("Skill", new XAttribute("id", 80900), new XAttribute("row", 1)),   //RF
                                                                                                //new XElement("Skill", new XAttribute("id", 290100), new XAttribute("row", 1)),  //TB
                new XElement("Skill", new XAttribute("id", 250200), new XAttribute("row", 1)),  //IT
                new XElement("Skill", new XAttribute("id", 220800), new XAttribute("row", 1))   //SF
                );
            SaveSkillFile(skills, filename);
        }
        private static void BuildDefaultSorcererSkillConfig(string filename)
        {
            XElement skills = new XElement("Skills",
                new XElement("Skill", new XAttribute("id", 270400), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 40900), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 60700), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 111140), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 300100), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 120600), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 310100), new XAttribute("row", 1))
                //new XElement("Skill", new XAttribute("id", 340200), new XAttribute("row", 1))
                );
            SaveSkillFile(skills, filename);
        }
        private static void BuildDefaultBerserkerSkillConfig(string filename)
        {
            XElement skills = new XElement("Skills",
                new XElement("Skill", new XAttribute("id", 31000), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 101100), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 150900), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 240200), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 250100), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 260100), new XAttribute("row", 1))
                );
            SaveSkillFile(skills, filename);
        }
        private static void BuildDefaultSlayerSkillConfig(string filename)
        {
            XElement skills = new XElement("Skills",
                new XElement("Skill", new XAttribute("id", 21100), new XAttribute("row", 1)),
                //new XElement("Skill", new XAttribute("id", 80930), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 120500), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 230200), new XAttribute("row", 1)),
                new XElement("Skill", new XAttribute("id", 240100), new XAttribute("row", 1))
                );
            SaveSkillFile(skills, filename);
        }
        private static void BuildDefaultWarriorSkillConfig(string filename)
        {
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
            SaveSkillFile(skills, filename);
        }
        private static void BuildDefaultLancerSkillConfig(string filename)
        {
            XElement skills = new XElement("Skills",
                new XElement("Skill", new XAttribute("id", 50100), new XAttribute("row", 1)),   //shield bash
                new XElement("Skill", new XAttribute("id", 30800), new XAttribute("row", 1)),   //onslaught
                new XElement("Skill", new XAttribute("id", 181100), new XAttribute("row", 1)),  //shield barrage
                new XElement("Skill", new XAttribute("id", 131100), new XAttribute("row", 1)),  //spring attack
                new XElement("Skill", new XAttribute("id", 100300), new XAttribute("row", 1)),  //debilitate
                new XElement("Skill", new XAttribute("id", 250730), new XAttribute("row", 1))   //wallop
                );
            SaveSkillFile(skills, filename);
        }
    }
}
