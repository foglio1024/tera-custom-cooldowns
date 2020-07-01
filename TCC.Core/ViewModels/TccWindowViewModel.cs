using System.Windows.Threading;
using Nostrum;
using TCC.Analysis;
using TCC.Settings.WindowSettings;

namespace TCC.ViewModels
{
    public class TccWindowViewModel : TSPropertyChanged
    {
        public WindowSettingsBase Settings { get; }
        protected virtual void InstallHooks() { }
        protected virtual void RemoveHooks() { }

        protected virtual void OnEnabledChanged(bool enabled)
        {
            if (enabled) InstallHooks();
            else RemoveHooks();
        }

        protected TccWindowViewModel(WindowSettingsBase settings)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            Settings = settings;
            if (settings != null)
            {
                settings.EnabledChanged += OnEnabledChanged;
                if (!settings.Enabled) return;
            }
            PacketAnalyzer.ProcessorReady += InstallHooks;
        }
    }
}
