using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using TCC.Annotations;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per DebugWindow.xaml
    /// </summary>
    public sealed partial class DebugWindow : INotifyPropertyChanged
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        private int _last;
        private int _sum;
        private int _max;

        public int Last
        {
            get => _last;
            private set { _last = value; NPC(); }
        }
        public int Max
        {
            get => _max;
            private set { _max = value; NPC(); }
        }

        private int Sum
        {
            get => _sum;
            set { _sum = value; NPC();}
        }
        public double Avg => _count == 0 ? 0 : Sum / _count;

        private int _count;
        public void SetQueuedPackets(int val)
        {
            Last = val;
            if (val > Max) Max = val;
            NPC(nameof(Avg));
            if (int.MaxValue - Sum < val)
            {
                Sum = 0;
                _count = 0;
            }
            else Sum += val;

            _count++;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void NPC([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SwitchClass(object sender, RoutedEventArgs e)
        {
            SessionManager.CurrentPlayer.Class = (Class)Enum.Parse(typeof(Class), (sender as Button).Content.ToString());
            ClassWindowViewModel.Instance.CurrentClass = SessionManager.CurrentPlayer.Class;
        }
    }
}
