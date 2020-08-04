using System.Windows.Threading;
using Nostrum;
using TCC.Analysis;
using TCC.Settings.WindowSettings;

namespace TCC.ViewModels
{
    public class TccWindowViewModel : TSPropertyChanged
    {
        public WindowSettingsBase Settings { get; }
        
        /// <summary>
        /// Called from <see cref="OnEnabledChanged"/> or when <see cref="PacketAnalyzer.ProcessorReady"/> is raised.
        /// </summary>
        protected virtual void InstallHooks() { }
        /// <summary>
        /// Called from <see cref="OnEnabledChanged"/>.
        /// </summary>
        protected virtual void RemoveHooks() { }
        /// <summary>
        /// Called when <see cref="WindowSettingsBase.Enabled"/> changes.
        /// </summary>
        /// <param name="enabled"></param>
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
