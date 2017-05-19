using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCC.Data;

namespace TCC.Controls
{
    public partial class AbnormalityToolTipControl : UserControl
    {
        public AbnormalityToolTipControl()
        {
            InitializeComponent();
        }

        public string AbnormalityName
        {
            get { return (string)GetValue(AbnormalityNameProperty); }
            set { SetValue(AbnormalityNameProperty, value); }
        }
        public static readonly DependencyProperty AbnormalityNameProperty = DependencyProperty.Register("AbnormalityName", typeof(string), typeof(AbnormalityToolTipControl));

        public string AbnormalityToolTip
        {
            get { return (string)GetValue(AbnormalityToolTipProperty); }
            set { SetValue(AbnormalityToolTipProperty, value); }
        }
        public static readonly DependencyProperty AbnormalityToolTipProperty = DependencyProperty.Register("AbnormalityToolTip", typeof(string), typeof(AbnormalityToolTipControl));

        string goodStart = "$H_W_GOOD";
        string badStart = "$H_W_BAD";
        string end = "$COLOR_END";
        string cr = "$BR";
        string cr2 = "<br>";

        void ParseToolTip(string t)
        {
            t = t.Replace(cr, "\n");
            t = t.Replace(cr2, "\n");
            var s = t.Split(new string[] { end }, StringSplitOptions.None);

            for (int i = 0; i < s.Length; i++)
            {
                if(i != s.Length - 1)
                {
                    if (s[i].Contains(goodStart))
                    {
                        var d = s[i].Split(new string[] { goodStart }, StringSplitOptions.None);
                        toolTipTB.Inlines.Add(d[0]);
                        Run r = new Run(d[1]);
                        r.Foreground = new SolidColorBrush(Color.FromRgb(0x3f,0x9f,0xff));
                        toolTipTB.Inlines.Add(r);
                    }
                    else if (s[i].Contains(badStart))
                    {
                        var d = s[i].Split(new string[] { badStart }, StringSplitOptions.None);
                        toolTipTB.Inlines.Add(d[0]);
                        Run r = new Run(d[1]);
                        r.Foreground = new SolidColorBrush(Colors.OrangeRed);
                        toolTipTB.Inlines.Add(r);
                    }
                }
                else
                {
                    toolTipTB.Inlines.Add(s[i]);
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            toolTipTB.Text = "";
            toolTipTB.FontSize = 11;
            try
            {
                ParseToolTip(AbnormalityToolTip);

            }
            catch (Exception)
            {

                
            }
            //toolTipTB.Text = ParseToolTip(AbnormalityToolTip);
        }
    }
}
