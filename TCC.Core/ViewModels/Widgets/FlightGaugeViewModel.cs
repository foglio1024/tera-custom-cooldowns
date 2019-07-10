using System;
using TCC.Data;
using TCC.Parsing;
using TCC.Settings;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets
{

    [TccModule]
    public class FlightGaugeViewModel : TccWindowViewModel
    {
        public event Action<double> EnergyChanged;

        public FlightStackType Type => FlyingGuardianDataProvider.StackType;
        public double FlightGaugeRotation => App.Settings.FlightGaugeWindowSettings.Rotation;
        public bool FlipFlightGauge => App.Settings.FlightGaugeWindowSettings.Flip;
        public bool FlyingMissionInProgress => FlyingGuardianDataProvider.IsInProgress;

        public FlightGaugeViewModel(WindowSettings settings) : base(settings)
        {
            FlyingGuardianDataProvider.StackTypeChanged += () => N(nameof(Type));
            FlyingGuardianDataProvider.IsInProgressChanged += () => N(nameof(FlyingMissionInProgress));
        }

        protected override void InstallHooks()
        {
            PacketAnalyzer.NewProcessor.Hook<S_PLAYER_CHANGE_FLIGHT_ENERGY>(OnPlayerChangeFlightEnergy);
        }

        protected override void RemoveHooks()
        {
            PacketAnalyzer.NewProcessor.Unhook<S_PLAYER_CHANGE_FLIGHT_ENERGY>(OnPlayerChangeFlightEnergy);
        }

        private void OnPlayerChangeFlightEnergy(S_PLAYER_CHANGE_FLIGHT_ENERGY m)
        {
            EnergyChanged?.Invoke(m.Energy);
        }
    }
}