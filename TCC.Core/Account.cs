using TCC.Data.Pc;

namespace TCC
{
    public class Account
    {
        public bool IsElite { get; set; }
        public SynchronizedObservableCollection<Character> Characters { get; set; }

        public Account()
        {
            Characters = new SynchronizedObservableCollection<Character>();
        }
    }
}