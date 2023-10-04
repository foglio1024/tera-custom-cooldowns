using System.Diagnostics;
using System.Text;
using TCC.Utils;

namespace TCC.Data.Databases;

public class DatabaseQueryMeasure
{
    readonly Stopwatch _sw;
    static int _totalCount;
    static int _hitCount;
    static int _missCount;
    static long _totTotalTime;
    static long _totHitTime;
    static long _totMissTime;
    static double _avgTotalTime => _totalCount == 0 ? 0 : _totTotalTime / (double) _totalCount;
    static double _avgHitTime => _hitCount == 0 ? 0 : _totHitTime / (double) _hitCount;
    static double _avgMissTime => _missCount == 0 ? 0 : _totMissTime / (double) _missCount;


    public DatabaseQueryMeasure()
    {
        _sw = new Stopwatch();
    }

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