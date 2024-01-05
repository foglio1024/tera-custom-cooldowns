#pragma warning disable 659
using System.Windows.Forms;

namespace TCC.Data;

public struct HotKey
{
    public Keys Key { get; set; }
    public ModifierKeys Modifier { get; set; }

    public HotKey(Keys key, ModifierKeys modifier) : this()
    {
        Key = key;
        Modifier = modifier;
    }

    public override readonly bool Equals(object? obj)
    {
        if (obj is not HotKey other) return false;
        return other.Key == Key && other.Modifier == Modifier;
    }

    public override readonly string ToString()
    {
        var control = (Modifier & ModifierKeys.Control) != 0;
        var shift = (Modifier & ModifierKeys.Shift) != 0;
        var alt = (Modifier & ModifierKeys.Alt) != 0;

        return $"{(control ? "Ctrl + " : "")}{(shift ? "Shift + " : "")}{(alt ? "Alt + " : "")}{Key}";
    }
}
#pragma warning restore 659