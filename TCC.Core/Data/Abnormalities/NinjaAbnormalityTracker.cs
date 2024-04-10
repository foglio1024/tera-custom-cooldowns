using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class NinjaAbnormalityTracker : AbnormalityTracker
{
    private const int FocusId = 10154030;
    private const int InnerHarmonyBuffId = 10154480;

    public override void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckFocusBegin(p);
        CheckInnerHarmonyBegin(p);
    }

    public override void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckFocusRefresh(p);
        CheckInnerHarmonyRefresh(p);
    }

    public override void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckFocusEnd(p);
        CheckInnerHarmonyEnd(p);
    }

    private static void CheckFocusBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != FocusId) return;
        if (!TryGetClassViewModel<NinjaLayoutViewModel>(out var vm)) return;

        vm.FocusOn = true;
    }

    private static void CheckFocusRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != FocusId) return;
        if (!TryGetClassViewModel<NinjaLayoutViewModel>(out var vm)) return;

        vm.FocusOn = true;
    }

    private static void CheckFocusEnd(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != FocusId) return;
        if (!TryGetClassViewModel<NinjaLayoutViewModel>(out var vm)) return;

        vm.FocusOn = false;
    }

    private static void CheckInnerHarmonyBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != InnerHarmonyBuffId) return;
        if (!TryGetClassViewModel<NinjaLayoutViewModel>(out var vm)) return;

        vm.InnerHarmony.StartEffect(p.Duration);
    }

    private static void CheckInnerHarmonyRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != InnerHarmonyBuffId) return;
        if (!TryGetClassViewModel<NinjaLayoutViewModel>(out var vm)) return;

        vm.InnerHarmony.RefreshEffect(p.Duration);
    }

    private static void CheckInnerHarmonyEnd(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != InnerHarmonyBuffId) return;
        if (!TryGetClassViewModel<NinjaLayoutViewModel>(out var vm)) return;

        vm.InnerHarmony.StopEffect();
    }
}