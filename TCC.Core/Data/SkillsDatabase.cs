using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace TCC
{
    public static class SkillsDatabase
    {
        public static Dictionary<Class, Dictionary<uint, Skill>> Skills;
        static List<SkillConnection> SkillConnections;

        class SkillConnection
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

        public static void CheckIcons()
        {
            foreach (var classDict in Skills)
            {
                foreach (var skill in classDict.Value)
                {

                    string filePath = Environment.CurrentDirectory + "/resources/images/" + skill.Value.IconName.Replace('.', '/') + ".png";
                    if (File.Exists(filePath))
                    {
                        Console.Write("\r[Icon Check] - Abnormality ID:{0} \t File name:{1} \t OK", skill.Value.Id, skill.Value.IconName);
                    }
                    else
                    {
                        if (skill.Value.IconName != "")
                        {
                            Console.WriteLine("[Icon Check] - File name:{1} \t Path:{0}", filePath, skill.Value.IconName);
                        }
                    }
                }
            }
        }

        public static void Load()
        {
            var f = File.OpenText(Environment.CurrentDirectory + "/resources/data/skills.tsv");

            SkillConnections = new List<SkillConnection>();
            Skills = new Dictionary<Class, Dictionary<uint, Skill>>();
            for (int i = 0; i <= 12; i++)
            {
                Skills.Add((Class)i, new Dictionary<uint, Skill>());
            }
            Skills.Add(Class.Common, new Dictionary<uint, Skill>());


            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;
                var s = line.Split('\t');
                var id = Convert.ToUInt32(s[0]);
                Enum.TryParse(s[1], out Class c);
                var name = s[2];
                var tooltip = s[3];
                var iconName = s[4];

                var sk = new Skill(id, c, name, tooltip);
                sk.SetSkillIcon(iconName);
                Skills[c].Add(id, sk);


                var skc = new SkillConnection((int)id, c);
                for (int i = 5; i < s.Count(); i++)
                {
                    skc.AddConnectedSkill(Convert.ToInt32(s[i]));
                }
                if(skc.ConnectedSkills.Count > 0)
                {
                    SkillConnections.Add(skc);
                }
            }

        }
        static string FindSkillNameByIdClass(uint id, Class c)
        {
            if (Skills[c].TryGetValue(id, out Skill sk))
            {
                return sk.Name;
            }
            else return "Not found";

        }
        static int GetSkillIdByConnectedId(uint id, Class c)
        {
            foreach (var skillConnection in SkillConnections.Where(x => x.Class == c))
            {
                foreach (var connectedSkill in skillConnection.ConnectedSkills)
                {
                    if ((int)id == connectedSkill)
                    {
                        //Console.WriteLine("GetSkillByConnectedId({0}, {1}) => {2}.", id, c, skillConnection.Id);
                        return skillConnection.Id;
                    }
                }
            }
            //Console.WriteLine("GetSkillByConnectedId({0}, {1}) => failed.", id, c);
            return -1;
        }
        public static string SkillIdToName(uint id, Class c)
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
        public static bool TryGetSkill(uint id, Class c, out Skill sk)
        {
            bool result = false;
            var connSkills = GetSkillIdByConnectedId(id, c);
            sk = new Skill(0, Class.None, string.Empty, string.Empty);
            if (Skills[c].TryGetValue(id, out sk))
            {
                //sk = Skills.Where(x => x.Id == id).Where(x => x.Class == c).First();
                result = true;
            }
            else if (connSkills != -1)
            {
                if (Skills[c].TryGetValue((uint)connSkills, out sk))
                {
                    result = true;
                }
            }
            return result;

        }


    }

    public static class SkillsDatabaseOld
    {
        public static Dictionary<Class, Dictionary<uint, Skill>> Skills;

        public static event Action<double> Progress;

        static List<XDocument> StrSheet_UserSkillsDocs;
        static List<XDocument> SkillIconData;
        static XDocument ConnectedSkillsDoc;
        static List<SkillConnection> SkillConnections;

        class SkillConnection
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

        static void LoadFiles()
        {
            foreach (var f in Directory.EnumerateFiles(Environment.CurrentDirectory + @"/resources/database/StrSheet_UserSkill"))
            {
                var d = XDocument.Load(f);
                StrSheet_UserSkillsDocs.Add(d);
            }

            foreach (var f in Directory.EnumerateFiles(Environment.CurrentDirectory + @"/resources/database/SkillIconData"))
            {
                var d = XDocument.Load(f);
                SkillIconData.Add(d);
            }

            ConnectedSkillsDoc = XDocument.Load(Environment.CurrentDirectory + @"/resources/database/ConnectedSkills.xml");
        }
        static void ParseUserSkillDoc(XDocument doc)
        {
            foreach (var s in doc.Descendants().Where(x => x.Name == "String"))
            {
                var id = Convert.ToUInt32(s.Attribute("id").Value);
                string name = string.Empty;
                if(s.Attribute("name") != null)
                {
                    name = s.Attribute("name").Value;
                }
                Enum.TryParse(s.Attribute("class").Value, out Class c);
                string toolTip = string.Empty;

                if(s.Attribute("toolTip") != null)
                {
                    toolTip = s.Attribute("toolTip").Value;
                }
                if((s.Attribute("class").Value != "Common") && (!name.Contains("Summon: ") || name == "Summon: Party"))
                {
                    var skill = new Skill(id, c, name, toolTip);
                    Skills[c].Add(id, skill);
                }
            }
        }
        static void ParseSkillIconDoc(XDocument doc)
        {
            foreach (var s in doc.Descendants().Where(x => x.Name == "Icon"))
            {
                var id = Convert.ToUInt32(s.Attribute("skillId").Value);
                var iconName = s.Attribute("iconName").Value;
                Enum.TryParse(s.Attribute("class").Value, out Class c);
                if(Skills[c].TryGetValue(id, out Skill sk))
                {
                    sk.SetSkillIcon(iconName);
                }
            }
        }
        static void ParseConnectedSkills()
        {
            SkillConnections = new List<SkillConnection>();
            foreach (var sk in ConnectedSkillsDoc.Descendants().Where(x => x.Name == "Skill"))
            {
                var id = Convert.ToInt32(sk.Attribute("id").Value);
                Enum.TryParse(sk.Attribute("class").Value, out Class c);
                var skc = new SkillConnection(id, c);
                foreach (var conn in sk.Descendants())
                {
                    skc.AddConnectedSkill(Convert.ToInt32(conn.Attribute("id").Value));
                }
                SkillConnections.Add(skc);
            }
        }
        static string FindSkillNameByIdClass(uint id, Class c)
        {
            if (Skills[c].TryGetValue(id, out Skill sk))
            {
                return sk.Name;
            }
            else return "Not found";

        }
        static int GetSkillIdByConnectedId(uint id, Class c)
        {
            foreach (var skillConnection in SkillConnections.Where(x => x.Class == c))
            {
                foreach (var connectedSkill in skillConnection.ConnectedSkills)
                {
                    if((int)id == connectedSkill)
                    {
                        return skillConnection.Id;                
                    }
                }
            }
            return -1;
        }
        //static void AddConnectedSkills()
        //{
        //    foreach (var item in SkillConnections)
        //    {
        //        SetConnectedSkills(item);
        //    }
        //}


        public static void Populate()
        {
            StrSheet_UserSkillsDocs = new List<XDocument>();
            SkillIconData = new List<XDocument>();

            Skills = new Dictionary<Class, Dictionary<uint, Skill>>();
            for (int i = 0; i <= 12; i++)
            {
                Skills.Add((Class)i, new Dictionary<uint, Skill>());
            }
            Skills.Add(Class.Common, new Dictionary<uint, Skill>());

            LoadFiles();
            var n = SkillIconData.Count;
            Progress?.Invoke(1*100 / (n - 1));
            foreach (var doc in StrSheet_UserSkillsDocs)
            {
                ParseUserSkillDoc(doc);
            }
            Progress?.Invoke(2 * 100 / (n - 1));

            foreach (var doc in SkillIconData)
            {
                ParseSkillIconDoc(doc);
                Progress?.Invoke((SkillIconData.IndexOf(doc)+2) * 100 / (n +1));
            }

            //AddConnectedSkills();

            var s = new Skill(60010, Class.Common, "Hurricane", "");
            s.SetSkillIcon("Icon_Skills.Armorbreak_Tex");
            Skills[Class.Common].Add(s.Id, s);

            ParseConnectedSkills();

            SkillIconData.Clear();
            StrSheet_UserSkillsDocs.Clear();
            
        }
        public static string SkillIdToName(uint id, Class c)
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
        public static bool TryGetSkill(uint id, Class c, out Skill sk)
        {
            bool result = false;
            var connSkills = GetSkillIdByConnectedId(id, c);
            sk = new Skill(0, Class.None, string.Empty, string.Empty);
            if (Skills[c].TryGetValue(id, out sk))
            {
                //sk = Skills.Where(x => x.Id == id).Where(x => x.Class == c).First();
                result = true;
            }
            else if (connSkills != -1)
            {
                if (Skills[c].TryGetValue((uint)connSkills, out sk))
                {
                    result = true;
                }
            }
            return result;

        }


        //public static Skill GetSkill(uint id, Class c)
        //{
        //    if (Skills.Where(x => x.Id == id).Where(x => x.Class == c).Count() > 0)
        //    {
        //        return Skills.Where(x => x.Id == id).Where(x => x.Class == c).First();
        //    }
        //    else return new Skill(0, Class.None, string.Empty, string.Empty);

        //}

        //public static BitmapImage SkillIdToIcon(uint id, Class c)
        //{
        //    if (Skills.Where(x => x.Id == id).Where(x => x.Class == c).Count() > 0)
        //    {
        //        return Skills.Where(x => x.Id == id).Where(x => x.Class == c).First().Icon;
        //    }
        //    else return null;
        //}


        //static void SetConnectedSkills(SkillConnection skc)
        //{
        //    Skills.Where(x => x.Id == skc.Id).Where(x => x.Class == skc.Class).Single().ConnectedSkills = skc.ConnectedSkills;
        //}
    }

}
