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
        public static List<Skill> Skills;
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
                    Skills.Add(skill);
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
                if(Skills.Where(x => x.Id == id).Where(x => x.Class == c).Count() > 0)
                {
                    Skills.Where(x => x.Id == id).Where(x => x.Class == c).First().SetSkillIcon(iconName);              
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
                    skc.AddConnectedSkill(Convert.ToInt32(sk.Attribute("id").Value));
                }
                SkillConnections.Add(skc);
            }
        }
        static string FindSkillNameByIdClass(int id, Class c)
        {
            if (Skills.Where(x => x.Id == id).Where(x => x.Class == c).Count() > 0)
            {
                return Skills.Where(x => x.Id == id).Where(x => x.Class == c).First().Name;
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
            Skills = new List<Skill>();
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
            Skills.Add(s);

            ParseConnectedSkills();

            SkillIconData.Clear();
            StrSheet_UserSkillsDocs.Clear();
            
        }
        public static string SkillIdToName(uint id, Class c)
        {
            var name = FindSkillNameByIdClass((int)id, c);
            var connSkill = GetSkillIdByConnectedId(id, c);

            if (name != "Not found") //found skill
            {
                return name;
            }
            else if (connSkill != -1) //skill found in connected skills
            {
                name = FindSkillNameByIdClass((int)id, c);
            }
            return name;
        }
        public static Skill GetSkill(uint id, Class c)
        {
            if (Skills.Where(x => x.Id == id).Where(x => x.Class == c).Count() > 0)
            {
                return Skills.Where(x => x.Id == id).Where(x => x.Class == c).First();
            }
            else return new Skill(0, Class.None, string.Empty, string.Empty);

        }

        public static bool TryGetSkill(uint id, Class c, out Skill sk)
        {
            bool result = false;
            var connSkills = GetSkillIdByConnectedId(id, c);
            sk = new Skill(0, Class.None, string.Empty, string.Empty);
            if (Skills.Where(x => x.Id == id).Where(x => x.Class == c).Count() > 0)
            {
                sk = Skills.Where(x => x.Id == id).Where(x => x.Class == c).First();
                result = true;
            }
            else if (connSkills != -1)
            {
                if (Skills.Where(x => x.Id == connSkills).Where(x => x.Class == c).Count() > 0)
                {
                    sk = Skills.Where(x => x.Id == connSkills).Where(x => x.Class == c).First();
                    result = true;
                }

            }
            else
            {
                sk = new Skill(0, Class.None, string.Empty, string.Empty);
                result = false;
            }
            return result;

        }

        public static BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {

                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                memory.Close();
                return bitmapimage;
            }
        }

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
