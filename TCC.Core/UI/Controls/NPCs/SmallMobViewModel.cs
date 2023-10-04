using System;
using System.ComponentModel;
using TCC.Data.Npc;

namespace TCC.UI.Controls.NPCs;

public class SmallMobViewModel : NpcViewModel
{

    public bool Compact => WindowManager.ViewModels.NpcVM.IsCompact || NPC.CurrentHP == 0;


    public SmallMobViewModel(NPC npc) : base(npc)
    {
        NPC = npc;

        WindowManager.ViewModels.NpcVM.NpcListChanged += OnNpcVMOnNpcListChanged;
        NPC.PropertyChanged += OnPropertyChanged;
    }

    protected override void OnDeleteTimerTick(object? s, EventArgs ev)
    {
        WindowManager.ViewModels.NpcVM.NpcListChanged -= OnNpcVMOnNpcListChanged;
        NPC.PropertyChanged -= OnPropertyChanged;
        base.OnDeleteTimerTick(s, ev);
    }

    protected override void OnNpcDelete()
    {
        WindowManager.ViewModels.NpcVM.RemoveNPC(NPC, Delay);
        DeleteTimer.Start();
    }

    void OnNpcVMOnNpcListChanged()
    {
        N(nameof(Compact));
    }

    void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(NPC.CurrentHP):
                InvokeHpChanged();
                break;
        }
    }

}