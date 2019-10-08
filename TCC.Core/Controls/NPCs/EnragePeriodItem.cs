using System.Windows.Threading;
using FoglioUtils;

namespace TCC.Controls.NPCs
{
    public class EnragePeriodItem : TSPropertyChanged
    {
        public double Start { get; }
        public double End { get; private set; }
        public double Factor => Duration * .01;
        public double StartFactor => End * .01;

        public double Duration => Start - End;
        public EnragePeriodItem(double start)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Start = start;

        }
        public void SetEnd(double end)
        {
            End = end;
            Refresh();
        }

        private void Refresh()
        {
            N(nameof(Factor));
            N(nameof(StartFactor));
        }
    }
}