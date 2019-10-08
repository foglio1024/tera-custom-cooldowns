using FoglioUtils;
using TCC.Data.Pc;

namespace TCC.Data
{
    public class Account
    {
        public bool IsElite { get; set; }
        public TSObservableCollection<Character> Characters { get; }

        public Account()
        {
            Characters = new TSObservableCollection<Character>();
        }
    }
}