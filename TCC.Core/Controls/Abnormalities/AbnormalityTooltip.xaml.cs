using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace TCC.Controls.Abnormalities
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
        public static readonly DependencyProperty AbnormalityNameProperty = DependencyProperty.Register("AbnormalityName", typeof(string), typeof(AbnormalityToolTipControl));

        public string AbnormalityToolTip
        {
            get => (string)GetValue(AbnormalityToolTipProperty);
            set => SetValue(AbnormalityToolTipProperty, value);
        }
        public static readonly DependencyProperty AbnormalityToolTipProperty = DependencyProperty.Register("AbnormalityToolTip", typeof(string), typeof(AbnormalityToolTipControl));

        public uint Id
        {
            get => (uint)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(uint), typeof(AbnormalityToolTipControl));


        private const string GoodStart = "$H_W_GOOD";
        private const string BadStart = "$H_W_BAD";
        private const string End = "$COLOR_END";
        private const string Cr = "$BR";
        private const string Cr2 = "<br>";

        private void ParseToolTip(string t)
        {
            t = t.Replace(Cr, "\n");
            t = t.Replace(Cr2, "\n");
            var s = t.Split(new[] { End }, StringSplitOptions.None);

            for (var i = 0; i < s.Length; i++)
            {
                if(i != s.Length - 1)
                {
                    if (s[i].Contains(GoodStart))
                    {
                        var d = s[i].Split(new[] { GoodStart }, StringSplitOptions.None);
                        ToolTipTb.Inlines.Add(d[0]);
                        var r = new Run(d[1]) {Foreground = new SolidColorBrush(Color.FromRgb(0x3f, 0x9f, 0xff))};
                        ToolTipTb.Inlines.Add(r);
                    }
                    else if (s[i].Contains(BadStart))
                    {
                        var d = s[i].Split(new[] { BadStart }, StringSplitOptions.None);
                        ToolTipTb.Inlines.Add(d[0]);
                        var r = new Run(d[1]) {Foreground = new SolidColorBrush(Colors.OrangeRed)};
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
