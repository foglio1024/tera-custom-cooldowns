using System;
using System.Windows.Threading;
using Nostrum.WPF.ThreadSafe;
using TCC.Data.Npc;
using TCC.Utilities;

namespace TCC.UI.Controls.NPCs;

public class NpcViewModel : ThreadSafeObservableObject
{
    public event Action? Disposed;
    public event Action? HpFactorChanged;

    protected const uint Delay = 5000;
    protected readonly DispatcherTimer DeleteTimer;
    protected readonly DispatcherTimer ShowMenuButtonTimer;

    bool _showOverrideBtn;



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

    public Npc NPC { get; protected set; }

    public NpcViewModel(Npc npc)
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

    void OnShowMenuButtonTimerTick(object? _, EventArgs __)
    {
        InvokePropertyChanged(nameof(ShowOverrideBtn));
        ShowMenuButtonTimer.Stop();
    }
    protected virtual void OnDeleteTimerTick(object? s, EventArgs ev)
    {
        DeleteTimer.Stop();
        InvokeDisposed();
        ShowMenuButtonTimer.Tick -= OnShowMenuButtonTimerTick;
        DeleteTimer.Tick -= OnDeleteTimerTick;
    }

    protected void InvokeDisposed() => Disposed?.Invoke();
    protected void InvokeHpChanged() => HpFactorChanged?.Invoke();
}