using System;
using TCC.Data;
using TCC.Parsing;
using TeraPacketParser.Messages;

namespace TCC.ViewModels
{

    [TccModule]
    public class FlightGaugeViewModel : TccWindowViewModel
    {
        public event Action<double> EnergyChanged;

        public FlightStackType Type => FlyingGuardianDataProvider.StackType;
        public double FlightGaugeRotation => App.Settings.FlightGaugeWindowSettings.Rotation;
        public bool FlipFlightGauge => App.Settings.FlightGaugeWindowSettings.Flip;
        public bool FlyingMissionInProgress => FlyingGuardianDataProvider.IsInProgress;

        public FlightGaugeViewModel()
        {
            FlyingGuardianDataProvider.StackTypeChanged += () => N(nameof(Type));
            FlyingGuardianDataProvider.IsInProgressChanged += () => N(nameof(FlyingMissionInProgress));
        }

        protected override void InstallHooks()
        {
            PacketAnalyzer.NewProcessor.Hook<S_PLAYER_CHANGE_FLIGHT_ENERGY>(m =>
            {
                EnergyChanged?.Invoke(m.Energy);
            });
        }
    }
}