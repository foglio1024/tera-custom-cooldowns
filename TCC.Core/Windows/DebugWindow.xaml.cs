using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TCC.Annotations;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window, INotifyPropertyChanged
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        private int last;
        private int sum;
        private int max;

        public int Last
        {
            get { return last; }
            set { last = value; NPC(); }
        }
        public int Max
        {
            get { return max; }
            set { max = value; NPC(); }
        }
        public int Sum
        {
            get { return sum; }
            set { sum = value; NPC();}
        }
        public double Avg => count == 0 ? 0 : Sum / count;

        private int count = 0;
        public void SetQueuedPackets(int val)
        {
            Last = val;
            if (val > Max) Max = val;
            NPC(nameof(Avg));
            if (int.MaxValue - Sum < val)
            {
                Sum = 0;
                count = 0;
            }
            else Sum += val;

            count++;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NPC([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
