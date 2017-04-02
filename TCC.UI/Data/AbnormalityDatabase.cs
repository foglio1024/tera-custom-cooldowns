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
        static List<XDocument> AbnormalityDataDocs;

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

            foreach (var f in Directory.EnumerateFiles(Environment.CurrentDirectory + @"/resources/database/Abnormality"))
            {
                var d = XDocument.Load(f);
                AbnormalityDataDocs.Add(d);
            }


        }
        static void ParseStrSheetAbnormalityDoc(XDocument doc)
        {
            foreach (XElement abn in doc.Descendants().Where(x => x.Name == "String"))
            {
                var id = Convert.ToUInt32(abn.Attribute("id").Value);
                string name = String.Empty;
                string toolTip = String.Empty;
                if(abn.Attributes().Any(x => x.Name == "name"))
                {
                    name = abn.Attribute("name").Value;
                }
                if (abn.Attributes().Any(x => x.Name == "tooltip"))
                {
                    toolTip = abn.Attribute("tooltip").Value;
                }

                //Abnormality ab = new Abnormality(id, name, toolTip);
                //Abnormalities.Add(id, ab);
                if (Abnormalities.TryGetValue(id, out Abnormality a))
                {
                    a.SetInfo(name, toolTip);
                }

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
        static void ParseAbnormalityDataDoc(XDocument doc)
        {
            foreach (var a in doc.Descendants().Where(x => x.Name == "Abnormal"))
            {
                uint id = Convert.ToUInt32(a.Attribute("id").Value);
                bool isBuff = false;
                if(a.Attribute("isBuff").Value == "True")
                {
                    isBuff = true;
                }
                bool isShow = true;
                if (a.Attribute("isShow").Value == "False")
                {
                    isShow = false;
                }
                bool infinity = false;
                if (a.Attribute("infinity").Value == "True")
                {
                    infinity = true;
                }
                int prop = Convert.ToInt32(a.Attribute("property").Value);

                Abnormality ab = new Abnormality(id, isBuff, isShow, infinity, prop);
                Abnormalities.Add(ab.Id, ab);



            }
        }
        public static void Populate()
        {
            Abnormalities = new Dictionary<uint, Abnormality>();
            StrSheet_AbnormalityDocs = new List<XDocument>();
            AbnormalityIconDataDocs = new List<XDocument>();
            AbnormalityDataDocs = new List<XDocument>();

            LoadFiles();

            foreach (var doc in AbnormalityDataDocs)
            {
                ParseAbnormalityDataDoc(doc);
            }
            foreach (var  doc in StrSheet_AbnormalityDocs)
            {
                ParseStrSheetAbnormalityDoc(doc);
            }
            foreach (var doc in AbnormalityIconDataDocs)
            {
                ParseAbnormalityIconDoc(doc);
            }

            StrSheet_AbnormalityDocs.Clear();
            AbnormalityIconDataDocs.Clear();
            AbnormalityDataDocs.Clear();
            var toBeRemovedList = new List<Abnormality>();
            foreach (var item in Abnormalities)
            {
                if(item.Value.Name == null)
                {
                    toBeRemovedList.Add(item.Value);
                }
            }
            foreach (var item in toBeRemovedList)
            {
                Abnormalities.Remove(item.Id);
            }
        }

    }
}
