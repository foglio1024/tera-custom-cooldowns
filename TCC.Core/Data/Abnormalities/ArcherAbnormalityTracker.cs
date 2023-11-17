using System.Linq;
using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class ArcherAbnormalityTracker : AbnormalityTracker
{
    const uint FocusId = 601400;
    const uint FocusXId = 601450;
    static readonly uint[] WindsongIds = [602101, 602221 /*, 602107, 602108, 602227*/];

    static readonly uint[] WindWalkIds = [602102, 602103];
    //private const string WindsongIconName = "icon_skills.breeze_tex";

    public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;  
        CheckFocus(p);
        CheckFocusX(p);
        CheckWindsong(p);
        CheckGaleSteps(p);
    }
    public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        CheckFocus(p);
        CheckFocusX(p);
        CheckWindsong(p);
        CheckWindWalk(p);
    }
    public override void CheckAbnormality(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        CheckFocus(p);
        CheckFocusX(p);
        CheckWindsong(p);
        CheckGaleSteps(p);
    }

    static void CheckWindsong(S_ABNORMALITY_BEGIN p)
    {
        if (!WindsongIds.Contains(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<ArcherLayoutViewModel>(out var vm)) return;
        vm.Windsong.StartEffect(p.Duration);
    }

    static void CheckWindsong(S_ABNORMALITY_REFRESH p)
    {
        if (!WindsongIds.Contains(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<ArcherLayoutViewModel>(out var vm)) return;

        vm.Windsong.RefreshEffect(p.Duration);
    }

    static void CheckWindsong(S_ABNORMALITY_END p)
    {
        if (!WindsongIds.Contains(p.AbnormalityId)) return;
        //if (!CheckByIconName(p.AbnormalityId, WindsongIconName)) return; //TODO: temporary
        if (!IsViewModelAvailable<ArcherLayoutViewModel>(out var vm)) return;

        vm.Windsong.StopEffect();
    }

    static void CheckGaleSteps(S_ABNORMALITY_BEGIN p)
    {
        if (!WindWalkIds.Contains(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<ArcherLayoutViewModel>(out var vm)) return;

        vm.WindWalk.Start(p.Duration);
        vm.WindWalkProc = true;
    }

    static void CheckWindWalk(S_ABNORMALITY_REFRESH p)
    {
        if (!WindWalkIds.Contains(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<ArcherLayoutViewModel>(out var vm)) return;

        vm.WindWalk.Refresh(p.Duration, CooldownMode.Normal);
        vm.WindWalkProc = true;
    }

    static void CheckGaleSteps(S_ABNORMALITY_END p)
    {
        if (!WindWalkIds.Contains(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<ArcherLayoutViewModel>(out var vm)) return;

        vm.WindWalk.Stop();
        vm.WindWalkProc = false;
    }

    static void CheckFocus(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != FocusId) return;
        if (!IsViewModelAvailable<ArcherLayoutViewModel>(out var vm)) return;
        vm.Focus.StartFocus(p.Duration);
    }

    static void CheckFocus(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != FocusId) return;
        if (!IsViewModelAvailable<ArcherLayoutViewModel>(out var vm)) return;
        vm.Focus.SetFocusStacks(p.Stacks, p.Duration);
    }

    static void CheckFocus(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != FocusId) return;
        if (!IsViewModelAvailable<ArcherLayoutViewModel>(out var vm)) return;
        vm.Focus.StopFocus();
    }

    static void CheckFocusX(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != FocusXId) return;
        if (!IsViewModelAvailable<ArcherLayoutViewModel>(out var vm)) return;
        vm.Focus.StartFocusX(p.Duration);
    }

    static void CheckFocusX(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != FocusXId) return;
        if (!IsViewModelAvailable<ArcherLayoutViewModel>(out var vm)) return;
        vm.Focus.StartFocusX(p.Duration);
    }

    static void CheckFocusX(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != FocusXId) return;
        if (!IsViewModelAvailable<ArcherLayoutViewModel>(out var vm)) return;
        vm.Focus.StopFocusX();
    }
}