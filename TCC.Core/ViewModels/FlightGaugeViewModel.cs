using TCC.Data;

namespace TCC.ViewModels
{
    public class FlightGaugeViewModel : TSPropertyChanged
    {
        public FlightStackType Type => FlyingGuardianDataProvider.StackType;
        public double FlightGaugeRotation => App.Settings.FlightGaugeRotation;
        public bool FlipFlightGauge => App.Settings.FlipFlightGauge;
        public bool FlyingMissionInProgress => FlyingGuardianDataProvider.IsInProgress;

        public FlightGaugeViewModel()
        {
            FlyingGuardianDataProvider.StackTypeChanged += () => N(nameof(Type));
            FlyingGuardianDataProvider.IsInProgressChanged += () => N(nameof(FlyingMissionInProgress));
        }
    }
}