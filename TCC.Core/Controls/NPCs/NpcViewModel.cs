using System;
using System.Windows.Threading;
using TCC.Data.NPCs;
using TCC.Utilities;

namespace TCC.Controls.NPCs
{
    public class NpcViewModel : TSPropertyChanged
    {
        protected const uint Delay = 5000;
        protected readonly DispatcherTimer DeleteTimer;

        private bool _showOverrideBtn;

        public event Action Disposed;
        public event Action HpFactorChanged;


        public bool ShowHP => !TccUtils.IsFieldBoss(NPC.ZoneId, NPC.TemplateId);

        public bool ShowOverrideBtn
        {
            get => _showOverrideBtn;
            set
            {
                if (_showOverrideBtn == value) return;
                _showOverrideBtn = value;
                if (!_showOverrideBtn)
                {
                    var t = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
                    t.Tick += (_, __) =>
                    {
                        N();
                        t.Stop();
                    };
                    t.Start();
                }
                else
                {
                    N();
                }
            }
        }

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
