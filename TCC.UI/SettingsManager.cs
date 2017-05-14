using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace TCC
{
    public class WindowSettings
    {
        public double X;
        public double Y;
        public Visibility Visibility;
        public bool ClickThru;
    }
    public static class SettingsManager
    {
        static Rectangle _screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

        public static WindowSettings GroupWindowSettings = new WindowSettings()
        {
            X = 10,
            Y = 10,
            Visibility = Visibility.Visible,
            ClickThru = false
        };
        public static WindowSettings CooldownWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 3,
            Y = _screen.Height / 1.5,
            Visibility = Visibility.Visible,
            ClickThru = false
        };
        public static WindowSettings BossGaugeWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 2 - 200,
            Y = 20,
            Visibility = Visibility.Visible,
            ClickThru = false
        };
        public static WindowSettings BuffBarWindowSettings = new WindowSettings()
        {
            X = _screen.Width - 1000,
            Y = _screen.Height / 1.5,
            Visibility = Visibility.Visible,
            ClickThru = false
        };
        public static WindowSettings CharacterWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 2 - 200,
            Y = _screen.Height - 120,
            Visibility = Visibility.Visible,
            ClickThru = false
        };

        public static bool IgnoreMeInGroupWindow { get; set; }
        public static bool IgnoreMyBuffsInGroupWindow { get; set; }
        public static bool IgnoreGroupBuffs { get; set; }
        public static bool IgnoreAllBuffsInGroupWindow { get; set; }
        public static bool IgnoreRaidAbnormalitiesInGroupWindow { get; set; }

        public static void LoadSettingsOld()
        {
            if (File.Exists(Environment.CurrentDirectory + @"/settings.csv"))
            {
                var sr = File.OpenText(Environment.CurrentDirectory + @"/settings.csv");

                SetWindowParametersOld(SettingsManager.BossGaugeWindowSettings, sr); //0
                SetWindowParametersOld(SettingsManager.BuffBarWindowSettings, sr); //1
                SetWindowParametersOld(SettingsManager.CharacterWindowSettings, sr); //2
                SetWindowParametersOld(SettingsManager.CooldownWindowSettings, sr); //4
                SetWindowParametersOld(SettingsManager.GroupWindowSettings, sr);

                sr.Close();
            }
        }
        public static void LoadSettings()
        {
            if(File.Exists(Environment.CurrentDirectory + @"/tcc-config.xml"))
            {
                var settingsDoc = XDocument.Load(Environment.CurrentDirectory + @"/tcc-config.xml");
                foreach (var ws in settingsDoc.Descendants().Where(x => x.Name == "WindowSetting"))
                {
                    if(ws.Attribute("Name").Value == "BossWindow")
                    {
                        ParseWindowSettings(BossGaugeWindowSettings, ws);
                    }
                    else if (ws.Attribute("Name").Value == "CharacterWindow")
                    {
                        ParseWindowSettings(CharacterWindowSettings, ws);
                    }
                    else if (ws.Attribute("Name").Value == "CooldownWindow")
                    {
                        ParseWindowSettings(CooldownWindowSettings, ws);
                    }
                    else if (ws.Attribute("Name").Value == "BuffWindow")
                    {
                        ParseWindowSettings(BuffBarWindowSettings, ws);
                    }
                    else if (ws.Attribute("Name").Value == "GroupWindow")
                    {
                        ParseWindowSettings(GroupWindowSettings, ws);
                    }
                }

                var b = settingsDoc.Descendants("OtherSettings").FirstOrDefault();
                if (b == null) return;
                IgnoreMeInGroupWindow = Boolean.Parse(b.Attribute("IgnoreMeInGroupWindow").Value);
                IgnoreMyBuffsInGroupWindow = Boolean.Parse(b.Attribute("IgnoreMyBuffsInGroupWindow").Value);
                IgnoreGroupBuffs = Boolean.Parse(b.Attribute("IgnoreGroupBuffs").Value);
                IgnoreAllBuffsInGroupWindow = Boolean.Parse(b.Attribute("IgnoreAllBuffsInGroupWindow").Value);
                IgnoreRaidAbnormalitiesInGroupWindow = Boolean.Parse(b.Attribute("IgnoreRaidAbnormalitiesInGroupWindow").Value);
                
            }
        }

        private static void ParseWindowSettings(WindowSettings w, XElement ws)
        {
            w.X = Double.Parse(ws.Attribute("X").Value);
            w.Y = Double.Parse(ws.Attribute("Y").Value);
            w.ClickThru = Boolean.Parse(ws.Attribute("ClickThru").Value);
            w.Visibility = (Visibility)Enum.Parse(typeof(Visibility), ws.Attribute("Visibility").Value);
        }

        private static void SetWindowParametersOld(WindowSettings ws, StreamReader sr)
        {
            var line = sr.ReadLine();
            var vals = line.Split(',');
            try
            {
                ws.Y = Convert.ToDouble(vals[0]);
                ws.X = Convert.ToDouble(vals[1]);
                if (Enum.TryParse(vals[2], out Visibility v))
                {
                    ws.Visibility = v;
                }
                if (Boolean.TryParse(vals[3], out bool ct))
                {
                    ws.ClickThru = ct;
                }

            }
            catch (Exception)
            {

            }
        }
        public static void SaveSettingsOld()
        {
            string[] vals = new string[5];
            AddSetting(BossGaugeWindowSettings, vals, 0);
            AddSetting(BuffBarWindowSettings, vals, 1);
            AddSetting(CharacterWindowSettings, vals, 2);
            AddSetting(CooldownWindowSettings, vals, 3);
            AddSetting(GroupWindowSettings, vals, 4);

            File.WriteAllLines(Environment.CurrentDirectory + @"/settings.csv", vals);
        }
        public static void SaveSettings()
        {
            var xSettings = new XElement("Settings",
                new XElement("WindowSettings",
                    new XElement("WindowSetting",
                        new XAttribute("Name", "BossWindow"),
                        new XAttribute("X", BossGaugeWindowSettings.X),
                        new XAttribute("Y", BossGaugeWindowSettings.Y),
                        new XAttribute("ClickThru", BossGaugeWindowSettings.ClickThru),
                        new XAttribute("Visibility", BossGaugeWindowSettings.Visibility)),
                    new XElement("WindowSetting",
                        new XAttribute("Name", "BuffWindow"),
                        new XAttribute("X", BuffBarWindowSettings.X),
                        new XAttribute("Y", BuffBarWindowSettings.Y),
                        new XAttribute("ClickThru", BuffBarWindowSettings.ClickThru),
                        new XAttribute("Visibility", BuffBarWindowSettings.Visibility)),
                    new XElement("WindowSetting",
                        new XAttribute("Name", "CharacterWindow"),
                        new XAttribute("X", CharacterWindowSettings.X),
                        new XAttribute("Y", CharacterWindowSettings.Y),
                        new XAttribute("ClickThru", CharacterWindowSettings.ClickThru),
                        new XAttribute("Visibility", CharacterWindowSettings.Visibility)),
                    new XElement("WindowSetting",
                        new XAttribute("Name", "CooldownWindow"),
                        new XAttribute("X", CooldownWindowSettings.X),
                        new XAttribute("Y", CooldownWindowSettings.Y),
                        new XAttribute("ClickThru", CooldownWindowSettings.ClickThru),
                        new XAttribute("Visibility", CooldownWindowSettings.Visibility)),
                    new XElement("WindowSetting",
                        new XAttribute("Name", "GroupWindow"),
                        new XAttribute("X", GroupWindowSettings.X),
                        new XAttribute("Y", GroupWindowSettings.Y),
                        new XAttribute("ClickThru", GroupWindowSettings.ClickThru),
                        new XAttribute("Visibility", GroupWindowSettings.Visibility))),
                new XElement("OtherSettings",
                new XAttribute("IgnoreMeInGroupWindow", IgnoreMeInGroupWindow),
                new XAttribute("IgnoreMyBuffsInGroupWindow", IgnoreMyBuffsInGroupWindow),
                new XAttribute("IgnoreGroupBuffs", IgnoreGroupBuffs),
                new XAttribute("IgnoreAllBuffsInGroupWindow", IgnoreAllBuffsInGroupWindow),
                new XAttribute("IgnoreRaidAbnormalitiesInGroupWindow", IgnoreRaidAbnormalitiesInGroupWindow)
                ));
            xSettings.Save(Environment.CurrentDirectory + @"/tcc-config.xml");
        }

        private static void AddSetting(WindowSettings ws, string[] vals, int i)
        {
            vals[i] = String.Format("{0},{1},{2},{3}", ws.Y, ws.X, ws.Visibility.ToString(), ws.ClickThru.ToString());
        }

    }
}
