using System.Linq;
using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class SlayerAbnormalityTracker : AbnormalityTracker
{
    static readonly uint[] IcbIds = [300800, 300801, 300805];

    public override void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckInColdBloodBegin(p);
    }

    public override void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckInColdBloodRefresh(p);
    }

    public override void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckInColdBloodEnd(p);
    }

    static void CheckInColdBloodBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!IcbIds.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<SlayerLayoutViewModel>(out var vm)) return;

        vm.InColdBlood.StartEffect(p.Duration);
    }

    static void CheckInColdBloodRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!IcbIds.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<SlayerLayoutViewModel>(out var vm)) return;

        vm.InColdBlood.StartEffect(p.Duration);
    }

    static void CheckInColdBloodEnd(S_ABNORMALITY_END p)
    {
        if (!IcbIds.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<SlayerLayoutViewModel>(out var vm)) return;

        vm.InColdBlood.StopEffect();
    }
}