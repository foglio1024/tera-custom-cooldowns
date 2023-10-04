using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Nostrum.WPF;
using TCC.Debugging;
using TCC.Settings.WindowSettings;
using TCC.Utils;

namespace TCC.ViewModels.Widgets;

[TccModule]
public class PerfMonitorViewModel : TccWindowViewModel
{
    double _memory;
    double _cpu;
    PerformanceCounter _cpuCounter = null!;
    PerformanceCounter _ramCounter = null!;
    Process _process = null!;

    public double Memory
    {
        get => _memory;
        private set
        {
            if (_memory == value) return;
            _memory = value;
            N();
            N(nameof(MemoryCritical));
            N(nameof(MemoryWarning));
            N(nameof(MemoryAAAAAAAAAAAAAAA));
        }
    }
    public double CPU
    {
        get => _cpu;
        private set
        {
            if (_cpu == value) return;
            _cpu = value;
            N();
        }
    }

    public bool MemoryWarning => _memory > 500;
    public bool MemoryCritical => _memory > 1000;
    public bool MemoryAAAAAAAAAAAAAAA => _memory > 3000;

    public ICommand DumpThreadAllocationCommand { get; }
    object _lock = new();
    bool _showDumpButton;

    public bool ShowDumpButton
    {
        get => _showDumpButton;
        set
        {
            if (_showDumpButton == value) return;
            _showDumpButton = value;
            N();
        }
    }

    public PerfMonitorViewModel(PerfMonitorSettings settings) : base(settings)
    {
        DumpThreadAllocationCommand = new RelayCommand(_ =>
        {
            var sb = "Thread allocation\n";
            var count = 0;

            foreach (var (_, dispatcher) in App.RunningDispatchers)
            {
                if (dispatcher == _dispatcher) continue;
                dispatcher.InvokeAsync(() =>
                {
                    lock (_lock)
                    {

                        sb += BuildThreadString(dispatcher, ref count);
                    }
                });
            }
            App.BaseDispatcher.InvokeAsync(() =>
            {
                lock (_lock)
                {
                    sb += BuildThreadString(App.BaseDispatcher, ref count);
                }
            });
            var max = App.RunningDispatchers.Count;
            while (count < max)
            {
                Thread.Sleep(1);
            }

            Log.F(sb, "threads_mem.log");

            ObjectTracker.DumpToFile();
        });

        Task.Run(OnTick);
    }

    string BuildThreadString(Dispatcher dispatcher, ref int count)
    {
        var ret = "";
        var name = $"- {dispatcher.Thread.Name} ";
        while (name.Length < 33)
        {
            name += "-";
        }

        ret += ($"{name} {GC.GetAllocatedBytesForCurrentThread() / (1024 * 1024D):N0} MB\n");
        Interlocked.Increment(ref count);
        System.Diagnostics.Debug.WriteLine($"{dispatcher.Thread.Name} {count}");

        return ret;
    }

    void OnTick()
    {
        _process = Process.GetCurrentProcess();
        var name = string.Empty;

        foreach (var instance in new PerformanceCounterCategory("Process").GetInstanceNames())
        {
            if (!instance.StartsWith(_process.ProcessName)) continue;
            using var processId = new PerformanceCounter("Process", "ID Process", instance, true);
            if (_process.Id != (int)processId.RawValue) continue;
            name = instance;
            break;
        }

        _cpuCounter = new PerformanceCounter("Process", "% Processor Time", name, true);
        _ramCounter = new PerformanceCounter("Process", "Private Bytes", name, true);

        while (true)
        {
            var cpu = (_cpuCounter.NextValue() / Environment.ProcessorCount);
            var mem = (_ramCounter.NextValue()) / (1024 * 1024D);

            _dispatcher.InvokeAsync(() =>
            {
                CPU = cpu;
                Memory = mem;
            });

            Thread.Sleep(1000);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}