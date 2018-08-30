using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TCC.Data.Databases
{
    public class SkillsDatabase
    {
        public Dictionary<Class, Dictionary<uint, Skill>> Skills { get; }
        private List<SkillConnection> SkillConnections { get; }

        private class SkillConnection
        {
            public Class Class;
            public int Id;
            public List<int> ConnectedSkills;

            public SkillConnection(int id, Class c)
            {
                ConnectedSkills = new List<int>();
                Id = id;
                Class = c;
            }
            public void AddConnectedSkill(int id)
            {
                ConnectedSkills.Add(id);
            }
        }

        public SkillsDatabase(string lang)
        {
            var f = File.OpenText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"resources/data/skills/skills-{lang}.tsv"));

            SkillConnections = new List<SkillConnection>();
            Skills = new Dictionary<Class, Dictionary<uint, Skill>>();
            for (var i = 0; i <= 12; i++)
            {
                Skills.Add((Class)i, new Dictionary<uint, Skill>());
            }
            Skills.Add(Class.Common, new Dictionary<uint, Skill>());
            Skills.Add(Class.None, new Dictionary<uint, Skill>());


            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;
                var s = line.Split('\t');
                var id = Convert.ToUInt32(s[0]);
                var cString = s[3];
                /*if (!*/
                Enum.TryParse(s[3], out Class c);/*)*/
                //{
                    //if (cString == "Mystic") cString = "Mystic";
                    //if (cString == "Reaper") cString = "Reaper";
                    //if (cString == "Brawler") cString = "Brawler";
                    //if (cString == "Ninja") cString = "Ninja";
                    //if (cString == "Gunner") cString = "Gunner";
                    //if (cString == "Valkyrie") cString = "Valkyrie";
                //}
                //Enum.TryParse(cString, out c);
                var name = s[4];
                //var tooltip = s[3];
                var detail = s[6];
                var iconName = s[7];

                var sk = new Skill(id, c, name, "");
                sk.IconName = iconName;
                sk.Detail = detail.ToLowerInvariant();
                if (Skills[c].ContainsKey(id)) continue;
                Skills[c].Add(id, sk);


                //var skc = new SkillConnection((int)id, c);
                //for (int i = 5; i < s.Count(); i++)
                //{
                //    skc.AddConnectedSkill(Convert.ToInt32(s[i]));
                //}
                //if(skc.ConnectedSkills.Count > 0)
                //{
                //    SkillConnections.Add(skc);
                //}
            }

        }

        private string FindSkillNameByIdClass(uint id, Class c)
        {
            if (Skills[c].TryGetValue(id, out var sk))
            {
                return sk.Name;
            }
            else return "Not found";

        }

        private int GetSkillIdByConnectedId(uint id, Class c)
        {
            foreach (var skillConnection in SkillConnections.Where(x => x.Class == c))
            {
                foreach (var connectedSkill in skillConnection.ConnectedSkills)
                {
                    if ((int)id == connectedSkill)
                    {
                        return skillConnection.Id;
                    }
                }
            }
            return -1;
        }
        public string SkillIdToName(uint id, Class c)
        {
            var name = FindSkillNameByIdClass(id, c);
            var connSkill = GetSkillIdByConnectedId(id, c);

            if (name != "Not found") //found skill
            {
                return name;
            }
            else if (connSkill != -1) //skill found in connected skills
            {
                name = FindSkillNameByIdClass(id, c);
            }
            return name;
        }
        public bool TryGetSkill(uint id, Class c, out Skill sk)
        {
            var result = false;
            sk = new Skill(0, Class.None, String.Empty, String.Empty);
            if (Skills[c].TryGetValue(id, out sk))
            {
                result = true;
            }
            return result;

        }
        public bool TryGetSkillByName(string name, Class c, out Skill sk)
        {
            var classSkills = Skills[c];
            sk = classSkills.FirstOrDefault(x => x.Value.Name.Contains(name) || x.Value.Name.Equals(name)).Value;
            if (sk != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //TODO do this better one day
        public static IEnumerable<Skill> SkillsForClass
        {
            get
            {
                var list = new SynchronizedObservableCollection<Skill>();
                var c = SessionManager.CurrentPlayer.Class;
                var skillsForClass = SessionManager.SkillsDatabase.Skills[c];
                foreach (var skill in skillsForClass.Values)
                {
                    if (list.All(x => x.IconName != skill.IconName) && !IsIgnoredSkill(skill))
                    {
                        list.Add(skill);
                    }
                }
                return list;
            }

        }

        public static bool IsIgnoredSkill(Skill skill)
        {
            return IgnoredSkills[skill.Class].Any(x => x == skill.IconName);
        }

        public static readonly Dictionary<Class, List<string>> IgnoredSkills = new Dictionary<Class, List<string>>()
        {
            {
                Class.Archer, new List<string>()
                {
                    "icon_skills.arrowshot_tex",
                    "icon_skills.webtrap_tex",
                    "icon_skills.focusstance_moveslow_tex"
                }
            },
            {
                Class.Berserker, new List<string>()
                {
                    "icon_skills.comboattack_tex",
                    "icon_skills.weapondefence_tex",
                }
            },
            {
                Class.Brawler, new List<string>()
                {
                    "icon_skills.comboattack01_tex",
                    "icon_skills.comboattack02_tex",
                    "icon_skills.comboattack03_tex",
                    "icon_skills.comboattack04_tex",
                    "icon_skills.smashattack01_tex",
                    "icon_skills.smashattack02_tex",
                    "icon_skills.smashattack03_tex",
                    "icon_skills.smashattack04_tex",
                    "icon_skills.pet_mushroom_tex",
                    "icon_skills.rampage_tex",
                }
            },
            {
                Class.Gunner, new List<string>()
                {
                    "icon_skills.cannonshot_tex",
                    "icon_skills.gatlingshot_tex",
                    "icon_skills.superrocketjump_tex",
                    "icon_skills.command_electricballshot_tex",
                }
            },
            {
                Class.Lancer, new List<string>()
                {
                    "icon_skills.comboattack_tex",
                    "icon_skills.defence_tex",
                    "icon_skills.backstep_tex",
                }
            },
            {
                Class.Mystic, new List<string>()
                {
                    "icon_skills.elementalshot_tex",
                    "icon_skills.mpsupplycharge_tex",
                    "icon_skills.energiesofrestriction_tex",
                    "icon_skills.energiesofquickness_tex",
                    "icon_skills.energiesofwillpower_tex"
                }
            },
             {
                Class.Ninja, new List<string>()
                {
                    "icon_skills.c12_meleecombo",
                }
            },
            {
                Class.Priest, new List<string>()
                {
                    "icon_skills.magicshot_tex",
                    "icon_skills.adventgoddess_tex"
                }
            },
            {
                Class.Reaper, new List<string>()
                {
                    "icon_skills.comboattack2_tex",
                    "icon_skills.shieldattack_tex",
                    "icon_skills.tornadoprison_tex"
                }
            },
            {
                Class.Slayer, new List<string>()
                {
                    "icon_skills.comboattack_tex",
                }
            },
            {
                Class.Sorcerer, new List<string>()
                {
                    "icon_skills.fireball_tex",
                    "icon_skills.tornadoprison_tex",
                    "icon_skills.contractofquickness_tex"
                }
            },
            {
                Class.Valkyrie, new List<string>()
                {
                    "icon_skills.combo_tex",

                }
            },
            {
                Class.Warrior, new List<string>()
                {
                    "icon_skills.comboattack_tex",
                    "icon_skills.twinswordsdefence_tex",
                }
            },
        };

        public bool TryGetSkillByIconName(string iconName, Class c, out Skill sk)
        {
            var result = false;
            sk = Skills[c].Values.ToList().FirstOrDefault(x => x.IconName == iconName);
            if (sk != null) result = true;
            return result;
        }
    }
}
