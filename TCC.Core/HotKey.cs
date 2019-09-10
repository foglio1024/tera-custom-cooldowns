using System.Windows.Forms;
using TCC.Data;

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

        public override bool Equals(object obj)
        {
            if (!(obj is HotKey other)) return false;
            return other.Key == Key && other.Modifier == Modifier;
        }
    }
}