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

namespace TCC.UI
{
    public static class SkillsDatabase
    {
        public static List<Skill> Skills;

        static List<XDocument> StrSheet_UserSkillsDocs;
        static List<XDocument> SkillIconData;

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
                if(s.Attribute("class").Value != "Common")
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

            var s = new Skill(60010, Class.Elementalist, "Hurricane", "");
            s.SetSkillIcon("Icon_Skills.Armorbreak_Tex");
            Skills.Add(s);

            SkillIconData.Clear();
            StrSheet_UserSkillsDocs.Clear();
        }
        public static event Action<double> Progress;
        public static string SkillIdToName(uint id, Class c)
        {
            if (Skills.Where(x => x.Id == id).Where(x => x.Class == c).Count() > 0)
            {
                return Skills.Where(x => x.Id == id).Where(x => x.Class == c).First().Name;
            }
            else return "Unknown";
        }

        //public static BitmapImage SkillIdToIcon(uint id, Class c)
        //{
        //    if (Skills.Where(x => x.Id == id).Where(x => x.Class == c).Count() > 0)
        //    {
        //        return Skills.Where(x => x.Id == id).Where(x => x.Class == c).First().Icon;
        //    }
        //    else return null;
        //}

        public static Skill GetSkill(uint id, Class c)
        {
            if (Skills.Where(x => x.Id == id).Where(x => x.Class == c).Count() > 0)
            {
                return Skills.Where(x => x.Id == id).Where(x => x.Class == c).First();
            }
            else return null;

        }
    }
}
