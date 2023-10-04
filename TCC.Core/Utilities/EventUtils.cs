namespace TCC.Utilities;

public static class EventUtils
{
    public static bool EndsToday(double start, double ed, bool d)
    {
        return d ? start + ed <= 24 : start <= ed;
    }
}