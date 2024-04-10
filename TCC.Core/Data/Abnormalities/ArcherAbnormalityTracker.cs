using System.Linq;
using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class ArcherAbnormalityTracker : AbnormalityTracker
{
    private const uint FocusId = 601400;
    private const uint FocusXId = 601450;
    private static readonly uint[] WindsongIds = [602101, 602221 /*, 602107, 602108, 602227*/];

    private static readonly uint[] WindWalkIds = [602102, 602103];
    //private const string WindsongIconName = "icon_skills.breeze_tex";

    public override void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckFocusBegin(p);
        CheckFocusXBegin(p);
        CheckWindsongBegin(p);
        CheckGaleStepsBegin(p);
    }

    public override void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckFocusRefresh(p);
        CheckFocusXRefresh(p);
        CheckWindsongRefresh(p);
        CheckWindWalkRefresh(p);
    }

    public override void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckFocusRefresh(p);
        CheckFocusXRefresh(p);
        CheckWindsongEnd(p);
        CheckGaleStepsEnd(p);
    }

    private static void CheckWindsongBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!WindsongIds.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<ArcherLayoutViewModel>(out var vm)) return;

        vm.Windsong.StartEffect(p.Duration);
    }

    private static void CheckWindsongRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!WindsongIds.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<ArcherLayoutViewModel>(out var vm)) return;

        vm.Windsong.RefreshEffect(p.Duration);
    }

    private static void CheckWindsongEnd(S_ABNORMALITY_END p)
    {
        if (!WindsongIds.Contains(p.AbnormalityId)) return;
        //if (!CheckByIconName(p.AbnormalityId, WindsongIconName)) return; //TODO: temporary
        if (!TryGetClassViewModel<ArcherLayoutViewModel>(out var vm)) return;

        vm.Windsong.StopEffect();
    }

    private static void CheckGaleStepsBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!WindWalkIds.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<ArcherLayoutViewModel>(out var vm)) return;

        vm.WindWalk.Start(p.Duration);
        vm.WindWalkProc = true;
    }

    private static void CheckWindWalkRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!WindWalkIds.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<ArcherLayoutViewModel>(out var vm)) return;

        vm.WindWalk.Refresh(p.Duration, CooldownMode.Normal);
        vm.WindWalkProc = true;
    }

    private static void CheckGaleStepsEnd(S_ABNORMALITY_END p)
    {
        if (!WindWalkIds.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<ArcherLayoutViewModel>(out var vm)) return;

        vm.WindWalk.Stop();
        vm.WindWalkProc = false;
    }

    private static void CheckFocusBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != FocusId) return;
        if (!TryGetClassViewModel<ArcherLayoutViewModel>(out var vm)) return;

        vm.Focus.StartFocus(p.Duration);
    }

    private static void CheckFocusRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != FocusId) return;
        if (!TryGetClassViewModel<ArcherLayoutViewModel>(out var vm)) return;

        vm.Focus.SetFocusStacks(p.Stacks, p.Duration);
    }

    private static void CheckFocusRefresh(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != FocusId) return;
        if (!TryGetClassViewModel<ArcherLayoutViewModel>(out var vm)) return;

        vm.Focus.StopFocus();
    }

    private static void CheckFocusXBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != FocusXId) return;
        if (!TryGetClassViewModel<ArcherLayoutViewModel>(out var vm)) return;

        vm.Focus.StartFocusX(p.Duration);
    }

    private static void CheckFocusXRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != FocusXId) return;
        if (!TryGetClassViewModel<ArcherLayoutViewModel>(out var vm)) return;

        vm.Focus.StartFocusX(p.Duration);
    }

    private static void CheckFocusXRefresh(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != FocusXId) return;
        if (!TryGetClassViewModel<ArcherLayoutViewModel>(out var vm)) return;

        vm.Focus.StopFocusX();
    }
}