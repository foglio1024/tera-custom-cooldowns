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
        public bool AutoDim;
        public double DimOpacity;
        public bool ShowAlways;
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
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false
        };
        public static WindowSettings CooldownWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 3,
            Y = _screen.Height / 1.5,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false

        };
        public static WindowSettings BossGaugeWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 2 - 200,
            Y = 20,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false

        };
        public static WindowSettings BuffBarWindowSettings = new WindowSettings()
        {
            X = _screen.Width - 1000,
            Y = _screen.Height / 1.5,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false

        };
        public static WindowSettings CharacterWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 2 - 200,
            Y = _screen.Height - 120,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false

        };
        public static WindowSettings ClassWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 3,
            Y = _screen.Height - 200,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false

        };
        public static WindowSettings ChatWindowSettings = new WindowSettings()
        {
            X = 0,
            Y = _screen.Height * (2 / 3),
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = false,
            DimOpacity = 1,
            ShowAlways = false

        };

        public static bool IgnoreMeInGroupWindow { get; set; }
        public static bool IgnoreMyBuffsInGroupWindow { get; set; }
        public static bool IgnoreGroupBuffs { get; set; }
        public static bool IgnoreAllBuffsInGroupWindow { get; set; }
        public static bool IgnoreRaidAbnormalitiesInGroupWindow { get; set; }
        public static FlowDirection BuffsDirection { get; set; } = FlowDirection.RightToLeft;
        public static bool ClassWindowOn { get; set; } = false;
        public static bool ClickThruWhenDim { get; set; } = true;
        public static int MaxMessages { get; set; } = 500;
        public static int SpamThreshold { get; set; } = 2;
        public static bool ShowChannel { get; set; } = true;
        public static bool ShowTimestamp { get; set; } = true;
        public static void LoadWindowSettings()
        {
            if (File.Exists(Environment.CurrentDirectory + @"/tcc-config.xml"))
            {
                SettingsDoc = XDocument.Load(Environment.CurrentDirectory + @"/tcc-config.xml");

                foreach (var ws in SettingsDoc.Descendants().Where(x => x.Name == "WindowSetting"))
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
                    else if (ws.Attribute("Name").Value == "ClassWindow")
                    {
                        ParseWindowSettings(ClassWindowSettings, ws);
                    }
                    else if (ws.Attribute("Name").Value == "ChatWindow")
                    {
                        ParseWindowSettings(ChatWindowSettings, ws);
                    }
                    //add window here
                }
            }
        }
        public static XDocument SettingsDoc;
        public static void LoadSettings()
        {
            if (File.Exists(Environment.CurrentDirectory + @"/tcc-config.xml"))
            {
                SettingsDoc = XDocument.Load(Environment.CurrentDirectory + @"/tcc-config.xml");

                var b = SettingsDoc.Descendants("OtherSettings").FirstOrDefault();
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
                try
                {
                    ClassWindowOn = Boolean.Parse(b.Attribute("ClassWindowOn").Value);
                }
                catch (Exception) { }
                try
                {
                    ClickThruWhenDim = Boolean.Parse(b.Attribute("ClickThruWhenDim").Value);
                }
                catch (Exception) { }
                try
                {
                    MaxMessages = Int32.Parse(b.Attribute(nameof(MaxMessages)).Value);
                }
                catch (Exception) { }
                try
                {
                    SpamThreshold = Int32.Parse(b.Attribute(nameof(SpamThreshold)).Value);
                }
                catch (Exception) { }
                try
                {
                    ShowChannel = Boolean.Parse(b.Attribute(nameof(ShowChannel)).Value);
                }
                catch (Exception) { }
                try
                {
                    ShowTimestamp = Boolean.Parse(b.Attribute(nameof(ShowTimestamp)).Value);
                }
                catch (Exception) { }
                //add settings here
            }
        }

        private static void ParseWindowSettings(WindowSettings w, XElement ws)
        {
            try
            {
                w.X = Double.Parse(ws.Attribute("X").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                w.Y = Double.Parse(ws.Attribute("Y").Value, CultureInfo.InvariantCulture);
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
            try
            {
                w.AutoDim = Boolean.Parse(ws.Attribute("AutoDim").Value);
            }
            catch (Exception) { }
            try
            {
                w.DimOpacity = Double.Parse(ws.Attribute("DimOpacity").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                w.ShowAlways = Boolean.Parse(ws.Attribute("ShowAlways").Value);
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
                        new XAttribute("Scale", BossGaugeWindowSettings.Scale),
                        new XAttribute("AutoDim", BossGaugeWindowSettings.AutoDim),
                        new XAttribute("DimOpacity", BossGaugeWindowSettings.DimOpacity)
                        ),
                    new XElement("WindowSetting",
                        new XAttribute("Name", "BuffWindow"),
                        new XAttribute("X", BuffBarWindowSettings.X),
                        new XAttribute("Y", BuffBarWindowSettings.Y),
                        new XAttribute("ClickThru", BuffBarWindowSettings.ClickThru),
                        new XAttribute("Visibility", BuffBarWindowSettings.Visibility),
                        new XAttribute("Scale", BuffBarWindowSettings.Scale),
                        new XAttribute("AutoDim", BuffBarWindowSettings.AutoDim),
                        new XAttribute("DimOpacity", BuffBarWindowSettings.DimOpacity)
                        ),
                    new XElement("WindowSetting",
                        new XAttribute("Name", "CharacterWindow"),
                        new XAttribute("X", CharacterWindowSettings.X),
                        new XAttribute("Y", CharacterWindowSettings.Y),
                        new XAttribute("ClickThru", CharacterWindowSettings.ClickThru),
                        new XAttribute("Visibility", CharacterWindowSettings.Visibility),
                        new XAttribute("Scale", CharacterWindowSettings.Scale),
                        new XAttribute("AutoDim", CharacterWindowSettings.AutoDim),
                        new XAttribute("DimOpacity", CharacterWindowSettings.DimOpacity)
                        ),
                    new XElement("WindowSetting",
                        new XAttribute("Name", "CooldownWindow"),
                        new XAttribute("X", CooldownWindowSettings.X),
                        new XAttribute("Y", CooldownWindowSettings.Y),
                        new XAttribute("ClickThru", CooldownWindowSettings.ClickThru),
                        new XAttribute("Visibility", CooldownWindowSettings.Visibility),
                        new XAttribute("Scale", CooldownWindowSettings.Scale),
                        new XAttribute("AutoDim", CooldownWindowSettings.AutoDim),
                        new XAttribute("DimOpacity", CooldownWindowSettings.DimOpacity)
                        ),
                    new XElement("WindowSetting",
                        new XAttribute("Name", "GroupWindow"),
                        new XAttribute("X", GroupWindowSettings.X),
                        new XAttribute("Y", GroupWindowSettings.Y),
                        new XAttribute("ClickThru", GroupWindowSettings.ClickThru),
                        new XAttribute("Visibility", GroupWindowSettings.Visibility),
                        new XAttribute("Scale", GroupWindowSettings.Scale),
                        new XAttribute("AutoDim", GroupWindowSettings.AutoDim),
                        new XAttribute("DimOpacity", GroupWindowSettings.DimOpacity)
                        ),
                    new XElement("WindowSetting",
                        new XAttribute("Name", "ClassWindow"),
                        new XAttribute("X", ClassWindowSettings.X),
                        new XAttribute("Y", ClassWindowSettings.Y),
                        new XAttribute("ClickThru", ClassWindowSettings.ClickThru),
                        new XAttribute("Visibility", ClassWindowSettings.Visibility),
                        new XAttribute("Scale", ClassWindowSettings.Scale),
                        new XAttribute("AutoDim", ClassWindowSettings.AutoDim),
                        new XAttribute("DimOpacity", ClassWindowSettings.DimOpacity)
                        ),
                        new XElement("WindowSetting",
                        new XAttribute("Name", "ChatWindow"),
                        new XAttribute("X", ChatWindowSettings.X),
                        new XAttribute("Y", ChatWindowSettings.Y),
                        new XAttribute("ClickThru", ChatWindowSettings.ClickThru),
                        new XAttribute("Visibility", ChatWindowSettings.Visibility),
                        new XAttribute("Scale", ChatWindowSettings.Scale),
                        new XAttribute("AutoDim", ChatWindowSettings.AutoDim),
                        new XAttribute("DimOpacity", ChatWindowSettings.DimOpacity)
                        )
                    //add window here
                    ),

                new XElement("OtherSettings",
                new XAttribute("IgnoreMeInGroupWindow", IgnoreMeInGroupWindow),
                new XAttribute("IgnoreMyBuffsInGroupWindow", IgnoreMyBuffsInGroupWindow),
                new XAttribute("IgnoreGroupBuffs", IgnoreGroupBuffs),
                new XAttribute("IgnoreAllBuffsInGroupWindow", IgnoreAllBuffsInGroupWindow),
                new XAttribute("IgnoreRaidAbnormalitiesInGroupWindow", IgnoreRaidAbnormalitiesInGroupWindow),
                new XAttribute("BuffsDirection", BuffsDirection),
                new XAttribute("ClassWindowOn", ClassWindowOn),
                new XAttribute("ClickThruWhenDim", ClickThruWhenDim),
                new XAttribute("MaxMessages", MaxMessages),
                new XAttribute("SpamThreshold", SpamThreshold),
                new XAttribute(nameof(ShowChannel), ShowChannel),
                new XAttribute(nameof(ShowTimestamp), ShowTimestamp)
                //add setting here
                )
            );
            xSettings.Save(Environment.CurrentDirectory + @"/tcc-config.xml");
        }


    }
}
