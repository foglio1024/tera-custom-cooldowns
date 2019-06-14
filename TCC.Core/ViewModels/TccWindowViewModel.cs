using TCC.Parsing;

namespace TCC.ViewModels
{
    public class TccWindowViewModel : TSPropertyChanged
    {
        protected virtual void InstallHooks() { }

        protected TccWindowViewModel()
        {
            PacketAnalyzer.ProcessorReady += InstallHooks;
        }
    }
}
