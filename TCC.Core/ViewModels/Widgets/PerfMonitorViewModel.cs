using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Nostrum;
using TCC.Settings.WindowSettings;
using TCC.Utils;

namespace TCC.ViewModels.Widgets
{
    [TccModule]
    public class PerfMonitorViewModel : TccWindowViewModel
    {
        private double _memory;
        private double _cpu;
        private PerformanceCounter _cpuCounter = null!;
        private PerformanceCounter _ramCounter = null!;
        private Process _process = null!;

        public double Memory
        {
            get => _memory;
            set
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
            set
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
        private bool _showDumpButton;

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

                foreach (var (key, dispatcher) in App.RunningDispatchers)
                {
                    if (dispatcher == Dispatcher) continue;
                    dispatcher.InvokeAsync(() =>
                    {
                        lock (DumpThreadAllocationCommand)
                        {
                            var name = $"- {dispatcher.Thread.Name} ";
                            while (name.Length < 33)
                            {
                                name += "-";
                            }

                            sb += ($"{name} {GC.GetAllocatedBytesForCurrentThread() / (1024 * 1024D):N0} MB\n");
                            Interlocked.Increment(ref count);
                            Debug.WriteLine($"{dispatcher.Thread.Name} {count}");
                        }
                    });
                }
                App.BaseDispatcher.InvokeAsync(() =>
                {
                    lock (DumpThreadAllocationCommand)
                    {
                        var name = $"- {App.BaseDispatcher.Thread.Name} ";
                        while (name.Length < 33)
                        {
                            name += "-";
                        }


                        sb += ($"{name} {GC.GetAllocatedBytesForCurrentThread() / (1024 * 1024D):N0} MB\n");
                        Interlocked.Increment(ref count);
                        Debug.WriteLine($"{App.BaseDispatcher.Thread.Name} {count}");
                    }
                });
                var max = App.RunningDispatchers.Count;
                while (count < max)
                {
                    Thread.Sleep(1);
                }

                Log.F(sb, "threads_mem.log");

            });

            Task.Run(OnTick);
        }

        private void OnTick()
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

                Dispatcher.InvokeAsync(() =>
                {
                    CPU = cpu;
                    Memory = mem;
                });

                Thread.Sleep(1000);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
