using Nostrum;
using TCC.Data.Pc;

namespace TCC.UI.Controls.Dashboard
{
    public class DungeonCooldownViewModel : TSPropertyChanged
    {
        public DungeonCooldownData Cooldown { get; set; }
        public Character Owner { get; set; }

        public DungeonCooldownViewModel(DungeonCooldownData cooldown, Character owner)
        {
            Cooldown = cooldown;
            Owner = owner;
        }
    }
}