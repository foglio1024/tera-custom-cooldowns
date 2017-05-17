using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
        public double Scale;
    }
    public static class SettingsManager
    {
        static Rectangle _screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

        public static WindowSettings GroupWindowSettings = new WindowSettings()
        {
            X = 10,
            Y = 10,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1
        };
        public static WindowSettings CooldownWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 3,
            Y = _screen.Height / 1.5,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1

        };
        public static WindowSettings BossGaugeWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 2 - 200,
            Y = 20,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1

        };
        public static WindowSettings BuffBarWindowSettings = new WindowSettings()
        {
            X = _screen.Width - 1000,
            Y = _screen.Height / 1.5,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1

        };
        public static WindowSettings CharacterWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 2 - 200,
            Y = _screen.Height - 120,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1

        };

        public static bool IgnoreMeInGroupWindow { get; set; }
        public static bool IgnoreMyBuffsInGroupWindow { get; set; }
        public static bool IgnoreGroupBuffs { get; set; }
        public static bool IgnoreAllBuffsInGroupWindow { get; set; }
        public static bool IgnoreRaidAbnormalitiesInGroupWindow { get; set; }
        public static FlowDirection BuffsDirection { get; set; } = FlowDirection.RightToLeft;
        public static bool ClassWindowOn { get; set; } = true;
        public static void LoadSettings()
        {
            if (File.Exists(Environment.CurrentDirectory + @"/tcc-config.xml"))
            {
                var settingsDoc = XDocument.Load(Environment.CurrentDirectory + @"/tcc-config.xml");
                foreach (var ws in settingsDoc.Descendants().Where(x => x.Name == "WindowSetting"))
                {
                    if (ws.Attribute("Name").Value == "BossWindow")
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
                try
                {
                    IgnoreMeInGroupWindow = Boolean.Parse(b.Attribute("IgnoreMeInGroupWindow").Value);
                }
                catch (Exception) { }
                try
                {
                    IgnoreMyBuffsInGroupWindow = Boolean.Parse(b.Attribute("IgnoreMyBuffsInGroupWindow").Value);
                }
                catch (Exception) { }
                try
                {
                    IgnoreGroupBuffs = Boolean.Parse(b.Attribute("IgnoreGroupBuffs").Value);
                }
                catch (Exception) { }
                try
                {
                    IgnoreAllBuffsInGroupWindow = Boolean.Parse(b.Attribute("IgnoreAllBuffsInGroupWindow").Value);
                }
                catch (Exception) { }
                try
                {
                    IgnoreRaidAbnormalitiesInGroupWindow = Boolean.Parse(b.Attribute("IgnoreRaidAbnormalitiesInGroupWindow").Value);
                }
                catch (Exception) { }
                try
                {
                    BuffsDirection = (FlowDirection)Enum.Parse(typeof(FlowDirection), b.Attribute("BuffsDirection").Value);
                }
                catch (Exception) { }
            }
        }

        private static void ParseWindowSettings(WindowSettings w, XElement ws)
        {
            try
            {
                w.X = Double.Parse(ws.Attribute("X").Value);
            }
            catch (Exception) { }
            try
            {
                w.Y = Double.Parse(ws.Attribute("Y").Value);
            }
            catch (Exception) { }

            try
            {
                w.ClickThru = Boolean.Parse(ws.Attribute("ClickThru").Value);
            }
            catch (Exception) { }

            try
            {
                w.Visibility = (Visibility)Enum.Parse(typeof(Visibility), ws.Attribute("Visibility").Value);
            }
            catch (Exception) { }

            try
            {
                w.Scale = Double.Parse(ws.Attribute("Scale").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }

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
                        new XAttribute("Visibility", BossGaugeWindowSettings.Visibility),
                        new XAttribute("Scale", BossGaugeWindowSettings.Scale)
                        ),
                    new XElement("WindowSetting",
                        new XAttribute("Name", "BuffWindow"),
                        new XAttribute("X", BuffBarWindowSettings.X),
                        new XAttribute("Y", BuffBarWindowSettings.Y),
                        new XAttribute("ClickThru", BuffBarWindowSettings.ClickThru),
                        new XAttribute("Visibility", BuffBarWindowSettings.Visibility),
                        new XAttribute("Scale", BuffBarWindowSettings.Scale)
                        ),
                    new XElement("WindowSetting",
                        new XAttribute("Name", "CharacterWindow"),
                        new XAttribute("X", CharacterWindowSettings.X),
                        new XAttribute("Y", CharacterWindowSettings.Y),
                        new XAttribute("ClickThru", CharacterWindowSettings.ClickThru),
                        new XAttribute("Visibility", CharacterWindowSettings.Visibility),
                        new XAttribute("Scale", CharacterWindowSettings.Scale)
                        ),
                    new XElement("WindowSetting",
                        new XAttribute("Name", "CooldownWindow"),
                        new XAttribute("X", CooldownWindowSettings.X),
                        new XAttribute("Y", CooldownWindowSettings.Y),
                        new XAttribute("ClickThru", CooldownWindowSettings.ClickThru),
                        new XAttribute("Visibility", CooldownWindowSettings.Visibility),
                        new XAttribute("Scale", CooldownWindowSettings.Scale)
                        ),
                    new XElement("WindowSetting",
                        new XAttribute("Name", "GroupWindow"),
                        new XAttribute("X", GroupWindowSettings.X),
                        new XAttribute("Y", GroupWindowSettings.Y),
                        new XAttribute("ClickThru", GroupWindowSettings.ClickThru),
                        new XAttribute("Visibility", GroupWindowSettings.Visibility),
                        new XAttribute("Scale", GroupWindowSettings.Scale)
                        )
                    ),
                new XElement("OtherSettings",
                new XAttribute("IgnoreMeInGroupWindow", IgnoreMeInGroupWindow),
                new XAttribute("IgnoreMyBuffsInGroupWindow", IgnoreMyBuffsInGroupWindow),
                new XAttribute("IgnoreGroupBuffs", IgnoreGroupBuffs),
                new XAttribute("IgnoreAllBuffsInGroupWindow", IgnoreAllBuffsInGroupWindow),
                new XAttribute("IgnoreRaidAbnormalitiesInGroupWindow", IgnoreRaidAbnormalitiesInGroupWindow),
                new XAttribute("BuffsDirection", BuffsDirection)
                )
            );
            xSettings.Save(Environment.CurrentDirectory + @"/tcc-config.xml");
        }


    }
}
