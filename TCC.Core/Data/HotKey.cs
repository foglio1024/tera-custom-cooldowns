#pragma warning disable 659
using System.Windows.Forms;

namespace TCC.Data;

public struct HotKey
{
    public HotKey(Keys k, ModifierKeys m) : this()
    {
        Key = k;
        Modifier = m;
    }

    public Keys Key { get; set; }
    public ModifierKeys Modifier { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not HotKey other) return false;
        return other.Key == Key && other.Modifier == Modifier;
    }

    public override string ToString()
    {
        var control = (Modifier & ModifierKeys.Control) != 0;
        var shift = (Modifier & ModifierKeys.Shift) != 0;
        var alt = (Modifier & ModifierKeys.Alt) != 0;

        return $"{(control ? "Ctrl + " : "")}{(shift ? "Shift + " : "")}{(alt ? "Alt + " : "")}{Key.ToString()}";
    }
}
#pragma warning restore 659