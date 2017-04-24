using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TCC.Data;

namespace TCC
{
    public static class SessionManager
    {
        public static bool Logged;

        public static Player CurrentPlayer = new Player();
        public static Dragon CurrentDragon = Dragon.None;
        public static ObservableCollection<Boss> CurrentBosses = new ObservableCollection<Boss>();
        public static bool TryGetBossById(ulong id, out Boss b)
        {
            b = CurrentBosses.FirstOrDefault(x => x.EntityId == id);
            if(b == null)
            {
                b = new Boss(0, 0, 0, Visibility.Collapsed);
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
