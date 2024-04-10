using Nostrum.WPF.ThreadSafe;
using TCC.Data.Pc;

namespace TCC.UI.Controls.Dashboard;

public class CharacterViewModel : ThreadSafeObservableObject
{
    private bool _hilight;

    public Character Character { get; }
    public bool Hilight
    {
        get => _hilight;
        set => RaiseAndSetIfChanged(value, ref _hilight);
    }

    public CharacterViewModel(Character c)
    {
        Character = c;
    }
}