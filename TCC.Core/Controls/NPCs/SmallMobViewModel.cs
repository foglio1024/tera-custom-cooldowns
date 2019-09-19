using System.ComponentModel;
using TCC.Data.NPCs;

namespace TCC.Controls.NPCs
{
    public class SmallMobViewModel : NpcViewModel
    {

        public bool Compact => WindowManager.ViewModels.NPC.IsCompact;


        public SmallMobViewModel(NPC npc) : base(npc)
        {
            NPC = npc;

            WindowManager.ViewModels.NPC.NpcListChanged += () => N(nameof(Compact));

            NPC.PropertyChanged += OnPropertyChanged;
            NPC.DeleteEvent += () =>
            {
                WindowManager.ViewModels.NPC.RemoveNpc(NPC, Delay);
                DeleteTimer.Start();
            };


        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NPC.CurrentHP):
                    InvokeHpChanged();
                    break;
            }
        }

    }
}
