using System;
using System.Windows.Threading;
using FoglioUtils;
using TCC.Data.NPCs;
using TCC.Utilities;

namespace TCC.Controls.NPCs
{
    public class NpcViewModel : TSPropertyChanged
    {
        protected const uint Delay = 5000;
        protected readonly DispatcherTimer DeleteTimer;
        protected readonly DispatcherTimer ShowMenuButtonTimer;

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
                    ShowMenuButtonTimer.Start();
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
            ShowMenuButtonTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            ShowMenuButtonTimer.Tick += OnShowMenuButtonTimerTick;

            DeleteTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Delay) };
            DeleteTimer.Tick += OnDeleteTimerTick;

            NPC.DeleteEvent += OnNpcDelete;
        }

        protected virtual void OnNpcDelete()
        {
            NPC.DeleteEvent -= OnNpcDelete;
            DeleteTimer.Start();
        }

        private void OnShowMenuButtonTimerTick(object _, EventArgs __)
        {
            N(nameof(ShowOverrideBtn));
            ShowMenuButtonTimer.Stop();
        }

        protected virtual void OnDeleteTimerTick(object s, EventArgs ev)
        {
            DeleteTimer.Stop();
            InvokeDisposed();
            ShowMenuButtonTimer.Tick -= OnShowMenuButtonTimerTick;
            DeleteTimer.Tick -= OnDeleteTimerTick;
        }

        protected void InvokeDisposed() => Disposed?.Invoke();
        protected void InvokeHpChanged() => HpFactorChanged?.Invoke();
    }
}
