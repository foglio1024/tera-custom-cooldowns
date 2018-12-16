using System;
using System.Windows.Threading;
using TCC.Data.NPCs;

namespace TCC.Controls.NPCs
{
    public class NpcViewModel : TSPropertyChanged
    {
        public const uint Delay = 5000;
        protected readonly DispatcherTimer _deleteTimer;

        public event Action Disposed;
        public event Action HpFactorChanged;

        public NPC NPC { get; set; }

        public NpcViewModel(NPC npc) 
        {
            NPC = npc;

            _deleteTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Delay) };
            _deleteTimer.Tick += (s, ev) =>
            {
                _deleteTimer.Stop();
                InvokeDisposed();
            };
        }

        protected void InvokeDisposed() => Disposed?.Invoke();
        protected void InvokeHpChanged() => HpFactorChanged?.Invoke();
    }
}
