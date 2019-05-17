using TCC.Data;

namespace TCC.ViewModels
{
    public class FlightGaugeViewModel : TSPropertyChanged
    {
        public FlightStackType Type => FlyingGuardianDataProvider.StackType;
        public double FlightGaugeRotation => Settings.SettingsHolder.FlightGaugeRotation;
        public bool FlipFlightGauge => Settings.SettingsHolder.FlipFlightGauge;
        public bool FlyingMissionInProgress => FlyingGuardianDataProvider.IsInProgress;

        public FlightGaugeViewModel()
        {
            FlyingGuardianDataProvider.StackTypeChanged += () => N(nameof(Type));
            FlyingGuardianDataProvider.IsInProgressChanged += () => N(nameof(FlyingMissionInProgress));
        }
    }
}