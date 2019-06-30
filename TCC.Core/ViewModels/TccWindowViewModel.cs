using System.Windows.Threading;
using TCC.Parsing;

namespace TCC.ViewModels
{
    public class TccWindowViewModel : TSPropertyChanged
    {
        protected virtual void InstallHooks() { }

        protected TccWindowViewModel()
        {
            App.BaseDispatcher.Invoke(() =>
            {
                PacketAnalyzer.ProcessorReady += InstallHooks;
            });
        }
    }
}
