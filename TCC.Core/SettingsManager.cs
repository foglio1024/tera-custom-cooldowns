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
        public double W;
        public double H;
        public Visibility Visibility;
        public bool ClickThru;
        public double Scale;
        public bool AutoDim;
        public double DimOpacity;
        public bool ShowAlways;
        public bool AllowTransparency;
        public bool Enabled;

        public XElement ToXElement(string name)
        {
            var xe = new XElement("WindowSetting");
            xe.Add(new XAttribute("Name", name));
            xe.Add(new XAttribute(nameof(X), X));
            xe.Add(new XAttribute(nameof(Y), Y));
            xe.Add(new XAttribute(nameof(W), W));
            xe.Add(new XAttribute(nameof(H), H));
            xe.Add(new XAttribute(nameof(Visibility), Visibility));
            xe.Add(new XAttribute(nameof(ClickThru), ClickThru));
            xe.Add(new XAttribute(nameof(Scale), Scale));
            xe.Add(new XAttribute(nameof(AutoDim), AutoDim));
            xe.Add(new XAttribute(nameof(DimOpacity), DimOpacity));
            xe.Add(new XAttribute(nameof(ShowAlways), ShowAlways));
            xe.Add(new XAttribute(nameof(AllowTransparency), AllowTransparency));
            xe.Add(new XAttribute(nameof(Enabled), Enabled));
            return xe;
        }

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
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true
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
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true

        };
        public static WindowSettings BossWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 2 - 200,
            Y = 20,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true

        };
        public static WindowSettings BuffWindowSettings = new WindowSettings()
        {
            X = _screen.Width - 1000,
            Y = _screen.Height / 1.5,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true

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
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true

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
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true

        };
        public static WindowSettings ChatWindowSettings = new WindowSettings()
        {
            X = 0,
            Y = _screen.Height * (2 / 3),
            W = 600,
            H = 200,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = false,
            DimOpacity = 1,
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true

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
        public static bool ShowOnlyBosses { get; set; } = false;
        public static bool DisablePartyMP { get; set; } = false;
        public static bool DisablePartyHP { get; set; } = false;
        public static bool ShowOnlyAggroStacks { get; set; } = true;
        public static bool DisablePartyAbnormals { get; set; } = false;
        public static bool LfgOn { get; set; } = true;

        public static void LoadWindowSettings()
        {
            if (File.Exists(Environment.CurrentDirectory + @"/tcc-config.xml"))
            {
                SettingsDoc = XDocument.Load(Environment.CurrentDirectory + @"/tcc-config.xml");

                foreach (var ws in SettingsDoc.Descendants().Where(x => x.Name == "WindowSetting"))
                {
                    if (ws.Attribute("Name").Value == "BossWindow")
                    {
                        ParseWindowSettings(BossWindowSettings, ws);
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
                        ParseWindowSettings(BuffWindowSettings, ws);
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
                try
                {
                    ShowOnlyBosses = Boolean.Parse(b.Attribute(nameof(ShowOnlyBosses)).Value);
                }
                catch (Exception) { }
                try
                {
                    DisablePartyMP = Boolean.Parse(b.Attribute(nameof(DisablePartyMP)).Value);
                }
                catch (Exception) { }
                try
                {
                    DisablePartyHP = Boolean.Parse(b.Attribute(nameof(DisablePartyHP)).Value);
                }
                catch (Exception) { }
                try
                {
                    ShowOnlyAggroStacks = Boolean.Parse(b.Attribute(nameof(ShowOnlyAggroStacks)).Value);
                }
                catch (Exception) { }
                try
                {
                    DisablePartyAbnormals = Boolean.Parse(b.Attribute(nameof(DisablePartyAbnormals)).Value);
                }
                catch (Exception) { }
                try
                {
                    LfgOn = Boolean.Parse(b.Attribute(nameof(LfgOn)).Value);
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
                w.W = Double.Parse(ws.Attribute("W").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                w.H = Double.Parse(ws.Attribute("H").Value, CultureInfo.InvariantCulture);
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
            try
            {
                w.AllowTransparency = Boolean.Parse(ws.Attribute("AllowTransparency").Value);
            }
            catch (Exception) { }
            try
            {
                w.Enabled = Boolean.Parse(ws.Attribute("Enabled").Value);
            }
            catch (Exception) { }
        }
        public static void SaveSettings()
        {
            var xSettings = new XElement("Settings",
                new XElement("WindowSettings",
                    BossWindowSettings.ToXElement("BossWindow"),
                    BuffWindowSettings.ToXElement("BuffWindow"),
                    CharacterWindowSettings.ToXElement("CharacterWindow"),
                    CooldownWindowSettings.ToXElement("CooldownWindow"),
                    GroupWindowSettings.ToXElement("GroupWindow"),
                    ClassWindowSettings.ToXElement("ClassWindow"),
                    ChatWindowSettings.ToXElement("ChatWindow")
                    //add window here
                    ),
                new XElement("OtherSettings",
                new XAttribute(nameof(IgnoreMeInGroupWindow), IgnoreMeInGroupWindow),
                new XAttribute(nameof(IgnoreMyBuffsInGroupWindow), IgnoreMyBuffsInGroupWindow),
                new XAttribute(nameof(IgnoreGroupBuffs), IgnoreGroupBuffs),
                new XAttribute(nameof(IgnoreAllBuffsInGroupWindow), IgnoreAllBuffsInGroupWindow),
                new XAttribute(nameof(IgnoreRaidAbnormalitiesInGroupWindow), IgnoreRaidAbnormalitiesInGroupWindow),
                new XAttribute(nameof(BuffsDirection), BuffsDirection),
                new XAttribute(nameof(ClassWindowOn), ClassWindowOn),
                new XAttribute(nameof(ClickThruWhenDim), ClickThruWhenDim),
                new XAttribute(nameof(MaxMessages), MaxMessages),
                new XAttribute(nameof(SpamThreshold), SpamThreshold),
                new XAttribute(nameof(ShowChannel), ShowChannel),
                new XAttribute(nameof(ShowTimestamp), ShowTimestamp),
                new XAttribute(nameof(ShowOnlyBosses), ShowOnlyBosses),
                new XAttribute(nameof(DisablePartyMP), DisablePartyMP),
                new XAttribute(nameof(DisablePartyHP), DisablePartyHP),
                new XAttribute(nameof(DisablePartyAbnormals), DisablePartyAbnormals),
                new XAttribute(nameof(ShowOnlyAggroStacks), ShowOnlyAggroStacks),
                new XAttribute(nameof(LfgOn), LfgOn)
                //add setting here
                )
            );
            xSettings.Save(Environment.CurrentDirectory + @"/tcc-config.xml");
        }


    }
}
