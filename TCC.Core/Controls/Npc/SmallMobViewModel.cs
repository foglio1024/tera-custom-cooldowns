using System.ComponentModel;
using TCC.Data.NPCs;
using TCC.ViewModels;

namespace TCC.Controls.NPCs
{
    public class SmallMobViewModel : NpcViewModel
    {

        public bool Compact => BossGageWindowViewModel.Instance.IsCompact;



        public SmallMobViewModel(NPC npc) : base(npc)
        {
            NPC = npc;

            BossGageWindowViewModel.Instance.NpcListChanged += () => N(nameof(Compact));

            NPC.PropertyChanged += OnPropertyChanged;
            NPC.DeleteEvent += () =>
            {
                BossGageWindowViewModel.Instance.RemoveMe(NPC, Delay);
                _deleteTimer.Start();
            };


        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NPC.CurrentHP):
                    InvokeHpChanged();
                    break;
                default:
                    break;
            }
        }

    }
}
