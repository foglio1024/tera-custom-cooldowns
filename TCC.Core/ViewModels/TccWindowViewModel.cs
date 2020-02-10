using System.Windows.Threading;
using Nostrum;
using TCC.Parsing;
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

        private TccWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }

        protected TccWindowViewModel(WindowSettingsBase settings) : this()
        {
            Settings = settings;
            //App.BaseDispatcher.Invoke(() =>
            //{
            //});
            if (settings != null)
            {
                settings.EnabledChanged += OnEnabledChanged;
                if (!settings.Enabled) return;
            }
            PacketAnalyzer.ProcessorReady += InstallHooks;
        }
    }
}
