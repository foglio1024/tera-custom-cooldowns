using System.Windows.Forms;
using TCC.Tera.Data;

namespace TCC
{
    public struct HotKey
    {
        public HotKey(Keys k, HotkeysData.ModifierKeys m) : this()
        {
            Key = k;
            Modifier = m;
        }

        public Keys Key { get; }
        public HotkeysData.ModifierKeys Modifier { get; }
    }
}