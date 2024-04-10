using System.Diagnostics;
using System.Text;
using TCC.Utils;

namespace TCC.Data.Databases;

public class DatabaseQueryMeasure
{
    private readonly Stopwatch _sw = new();
    private static int _totalCount;
    private static int _hitCount;
    private static int _missCount;
    private static long _totTotalTime;
    private static long _totHitTime;
    private static long _totMissTime;
    private static double _avgTotalTime => _totalCount == 0 ? 0 : _totTotalTime / (double)_totalCount;
    private static double _avgHitTime => _hitCount == 0 ? 0 : _totHitTime / (double)_hitCount;
    private static double _avgMissTime => _missCount == 0 ? 0 : _totMissTime / (double)_missCount;

    public void StartQuery()
    {
        _totalCount++;
        _sw.Restart();
    }

    public void RegisterHit()
    {
        _sw.Stop();
        _hitCount++;
        _totHitTime += _sw.ElapsedMilliseconds;
        _totTotalTime += _sw.ElapsedMilliseconds;
        //Log.CW($"Last query took: {_sw.ElapsedTicks}ticks [hit]");
    }

    public void RegisterMiss()
    {
        _sw.Stop();
        _missCount++;
        _totMissTime += _sw.ElapsedTicks;
        _totTotalTime += _sw.ElapsedTicks;
        //Log.CW($"Last query took: {_sw.ElapsedTicks}ticks [miss]");
    }

    public static void PrintInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Total queries: {_totalCount} ({_hitCount} hit - {_missCount} miss)");
        sb.AppendLine(
            $"Avg time: {_avgTotalTime:N3}ticks ({_avgHitTime:N3}ticks hit - {_avgMissTime:N3}ticks miss)");

        Log.CW(sb.ToString());
    }
}