using System;
using System.Windows.Threading;
using TCC.Data.NPCs;

namespace TCC.Controls.NPCs
{
    public class NpcViewModel : TSPropertyChanged
    {
        protected const uint Delay = 5000;
        protected readonly DispatcherTimer DeleteTimer;

        public event Action Disposed;
        public event Action HpFactorChanged;

        public NPC NPC { get; protected set; }

        public NpcViewModel(NPC npc) 
        {
            NPC = npc;

            DeleteTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Delay) };
            DeleteTimer.Tick += (s, ev) =>
            {
                DeleteTimer.Stop();
                InvokeDisposed();
            };
        }

        protected void InvokeDisposed() => Disposed?.Invoke();
        protected void InvokeHpChanged() => HpFactorChanged?.Invoke();
    }
}
