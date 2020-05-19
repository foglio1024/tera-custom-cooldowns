using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Nostrum;
using TCC.Utils;

namespace TCC.UI.Controls.Abnormalities
{
    public partial class AbnormalityToolTipControl
    {
        public AbnormalityToolTipControl()
        {
            InitializeComponent();
        }

        public string AbnormalityName
        {
            get => (string)GetValue(AbnormalityNameProperty);
            set => SetValue(AbnormalityNameProperty, value);
        }
        public static readonly DependencyProperty AbnormalityNameProperty = DependencyProperty.Register("AbnormalityName", typeof(string), typeof(AbnormalityToolTipControl), new PropertyMetadata("Abnormality Name"));

        public string AbnormalityToolTip
        {
            get => (string)GetValue(AbnormalityToolTipProperty);
            set => SetValue(AbnormalityToolTipProperty, value);
        }
        public static readonly DependencyProperty AbnormalityToolTipProperty = DependencyProperty.Register("AbnormalityToolTip", typeof(string), typeof(AbnormalityToolTipControl), new PropertyMetadata("Abnormality tooltip."));

        public uint Id
        {
            get => (uint)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(uint), typeof(AbnormalityToolTipControl), new PropertyMetadata(0U));

        private const string Start = "$H_W_";
        private const string GoodStart = "$H_W_GOOD";
        private const string BadStart = "$H_W_BAD";
        private const string End = "$COLOR_END";
        private const string Cr = "$BR";
        private const string Cr2 = "<br>";

        private void ParseToolTip(string t)
        {
            const string CustomStart = "$H_W_CSTM";

            t = t.Replace(Cr, "\n")
                 .Replace(Cr2, "\n")
                 .Replace("color = ", "color=")
                 .Replace("=\"", "='")
                 .Replace("\">", "'>");

            var correctionSplit = t.Split('$');
            var swapped = false;
            for (var i = 0; i < correctionSplit.Length - 1; i++)
            {
                if (correctionSplit[i].StartsWith("COLOR_END") && !(correctionSplit[i - 1].StartsWith("H_W_GOOD") || correctionSplit[i - 1].StartsWith("H_W_BAD")))
                {
                    if (correctionSplit[i + 1].StartsWith("H_W_GOOD"))
                    {
                        correctionSplit[i] = correctionSplit[i].Replace("COLOR_END", "$H_W_GOOD");
                        correctionSplit[i + 1] = correctionSplit[i + 1].Replace("H_W_GOOD", "$COLOR_END");
                        swapped = true;
                    }
                    else if (correctionSplit[i + 1].StartsWith("H_W_BAD"))
                    {
                        correctionSplit[i] = correctionSplit[i].Replace("COLOR_END", "$H_W_BAD");
                        correctionSplit[i + 1] = correctionSplit[i + 1].Replace("H_W_BAD", "$COLOR_END");
                        swapped = true;
                    }
                }
            }

            if (swapped)
            {
                t = "";
                foreach (var s1 in correctionSplit)
                {
                    t += s1;
                }
            }

            while (t.Contains("<font"))
            {
                t = t.Replace("<font color='", CustomStart)
                     .Replace("'>", "")
                     .Replace("</font>", End);
            }

            var s = t.Split(new[] { End }, StringSplitOptions.None);

            for (var i = 0; i < s.Length; i++)
            {
                if (i != s.Length - 1)
                {
                    if (s[i].Contains(GoodStart))
                    {
                        var d = s[i].Split(new[] { GoodStart }, StringSplitOptions.None);
                        ToolTipTb.Inlines.Add(d[0]);

                        var r = new Run(d[1]) { Foreground = new SolidColorBrush(Color.FromRgb(0x3f, 0x9f, 0xff)) };
                        ToolTipTb.Inlines.Add(r);
                    }
                    else if (s[i].Contains(BadStart))
                    {
                        var d = s[i].Split(new[] { BadStart }, StringSplitOptions.None);
                        ToolTipTb.Inlines.Add(d[0]);

                        var r = new Run(d[1]) { Foreground = new SolidColorBrush(Colors.OrangeRed) };
                        ToolTipTb.Inlines.Add(r);
                    }
                    else if (s[i].Contains(CustomStart))
                    {
                        var d = s[i].Split(new[] { CustomStart }, StringSplitOptions.None);
                        ToolTipTb.Inlines.Add(d[0]);


                        var txt = d[1].Substring(7);
                        var col = d[1].Substring(0, 7);
                        var r = new Run(txt) { Foreground = new SolidColorBrush(MiscUtils.ParseColor(col)) };
                        ToolTipTb.Inlines.Add(r);
                    }
                }
                else
                {
                    ToolTipTb.Inlines.Add(s[i]);
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ToolTipTb.Text = "";
            ToolTipTb.FontSize = 11;
            try
            {
                ParseToolTip(AbnormalityToolTip);
                if (ToolTipTb.Text != "") return;
                ToolTipTb.Text = Id.ToString();
                Debug.WriteLine("Unknown abnoramlity: {0}", Id.ToString());
            }
            catch
            {
                // ignored
            }
        }
    }
}
