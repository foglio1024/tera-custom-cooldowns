using TCC.Data;

namespace TCC.ViewModels
{
    public class FlightGaugeViewModel : TSPropertyChanged
    {
        public FlightStackType Type => FlyingGuardianDataProvider.StackType;
        public double FlightGaugeRotation => App.Settings.FlightGaugeWindowSettings.Rotation;
        public bool FlipFlightGauge => App.Settings.FlightGaugeWindowSettings.Flip;
        public bool FlyingMissionInProgress => FlyingGuardianDataProvider.IsInProgress;

        public FlightGaugeViewModel()
        {
            FlyingGuardianDataProvider.StackTypeChanged += () => N(nameof(Type));
            FlyingGuardianDataProvider.IsInProgressChanged += () => N(nameof(FlyingMissionInProgress));
        }
    }
}