using System.Threading;

namespace TCC.Test
{
    public class ThreadInfo
    {
        private double _totalTime;
        public string Name;
        public int Id;

        public double TotalTime
        {
            get => _totalTime;
            set
            {
                var old = _totalTime;
                _totalTime = value;
                DiffTime = value - old;
            }
        }

        public double DiffTime { get; private set; }
        public ThreadPriority Priority;
    }
}
