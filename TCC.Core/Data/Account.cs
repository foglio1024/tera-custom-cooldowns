using TCC.Data.Pc;

namespace TCC.Data
{
    public class Account
    {
        public bool IsElite { get; set; }
        public SynchronizedObservableCollection<Character> Characters { get; }

        public Account()
        {
            Characters = new SynchronizedObservableCollection<Character>();
        }
    }
}