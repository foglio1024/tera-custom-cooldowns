using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TCC.Data
{
    public static class AbnormalityDatabase
    {
        public static Dictionary<uint ,Abnormality> Abnormalities;

        static List<XDocument> StrSheet_AbnormalityDocs;
        static List<XDocument> AbnormalityIconDataDocs;

        static void LoadFiles()
        {
            foreach (var f in Directory.EnumerateFiles(Environment.CurrentDirectory + @"/resources/database/StrSheet_Abnormality"))
            {
                var d = XDocument.Load(f);
                StrSheet_AbnormalityDocs.Add(d);
            }

            foreach (var f in Directory.EnumerateFiles(Environment.CurrentDirectory + @"/resources/database/AbnormalityIconData"))
            {
                var d = XDocument.Load(f);
                AbnormalityIconDataDocs.Add(d);
            }
        }
        static void ParseAbnormalityDoc(XDocument doc)
        {
            foreach (XElement abn in doc.Descendants().Where(x => x.Name == "String"))
            {
                var id = Convert.ToUInt32(abn.Attribute("id").Value);
                string name = String.Empty;
                string toolTip = String.Empty;
                if(abn.Attributes().Where(x => x.Name == "name").Count() > 0)
                {
                    name = abn.Attribute("name").Value;
                }
                if (abn.Attributes().Where(x => x.Name == "tooltip").Count() > 0)
                {
                    toolTip = abn.Attribute("tooltip").Value;
                }

                Abnormality ab = new Abnormality(id, name, toolTip);
                Abnormalities.Add(id, ab);
            }
        }
        static void ParseAbnormalityIconDoc(XDocument doc)
        {
            foreach (var s in doc.Descendants().Where(x => x.Name == "Icon"))
            {
                var id = Convert.ToUInt32(s.Attribute("abnormalityId").Value);
                string iconName = string.Empty;
                if(s.Attributes().Any(x => x.Name == "iconName"))
                {
                    iconName = s.Attribute("iconName").Value;
                }

                if (Abnormalities.TryGetValue(id, out Abnormality a))
                {
                    a.SetIcon(iconName);
                }
            }
        }
        public static void Populate()
        {
            Abnormalities = new Dictionary<uint, Abnormality>();
            StrSheet_AbnormalityDocs = new List<XDocument>();
            AbnormalityIconDataDocs = new List<XDocument>();

            LoadFiles();

            foreach (var  doc in StrSheet_AbnormalityDocs)
            {
                ParseAbnormalityDoc(doc);
            }
            foreach (var doc in AbnormalityIconDataDocs)
            {
                ParseAbnormalityIconDoc(doc);
            }

            StrSheet_AbnormalityDocs.Clear();
            AbnormalityIconDataDocs.Clear();
        }
        //public static bool TryGetAbnormality(uint id, out Abnormality ab)
        //{
        //    bool result = false;
        //    ab = new Abnormality(0, string.Empty, string.Empty);
        //    if (Abnormalities.TryGetValue(id, out ab))
        //    {
        //        result = true;
        //    }
        //    else
        //    {
        //        ab = new Abnormality(0, "Unknown", string.Empty);
        //        result = false;
        //    }
        //    return result;

        //}

    }
}
