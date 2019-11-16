using System.Linq;
using FoglioUtils;
using TCC.Data.Pc;

namespace TCC.Data
{
    public class Account
    {
        public bool IsElite { get; set; }
        public Character CurrentCharacter { get; private set; }
        public TSObservableCollection<Character> Characters { get; }

        public void LoginCharacter(uint id)
        {
            CurrentCharacter = Characters.ToSyncList().FirstOrDefault(x => x.Id == id);
        }
        public Account()
        {
            Characters = new TSObservableCollection<Character>();
        }
    }
}