using System.Windows.Forms;
using TCC.Parsing;

namespace TCC
{
    public struct HotKey
    {
        public HotKey(Keys k, ModifierKeys m) : this()
        {
            Key = k;
            Modifier = m;
        }

        public Keys Key { get; }
        public ModifierKeys Modifier { get; }
    }
}