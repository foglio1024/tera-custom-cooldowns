using Nostrum.WPF.ThreadSafe;

namespace TCC.UI.Controls.NPCs;

public class EnragePeriodItem : ThreadSafeObservableObject
{
    public double Start { get; }
    public double End { get; private set; }
    public double Factor => Duration * .01;
    public double StartFactor => End * .01;

    public double Duration => Start - End;
    public EnragePeriodItem(double start)
    {
        Start = start;

    }
    public void SetEnd(double end)
    {
        End = end;
        Refresh();
    }

    void Refresh()
    {
        N(nameof(Factor));
        N(nameof(StartFactor));
    }
}