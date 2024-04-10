using System.Linq;
using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class LancerAbnormalityTracker : AbnormalityTracker
{
    public static readonly uint[] ARushIDs = [200700, 200701, 200731];
    private static readonly uint[] GShoutIDs = [200200, 200201, 200202];
    private const uint LineHeldId = 201701;
    public const string AdrenalineRushIconName = "icon_skills.fightingwill_tex";

    public override void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckArushBegin(p);
        CheckGshoutBegin(p);
        CheckLineHeldBegin(p);
    }

    public override void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckArushRefresh(p);
        CheckGshoutRefresh(p);
        CheckLineHeldRefresh(p);
    }

    public override void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckArushEnd(p);
        CheckGshoutEnd(p);
        CheckLineHeldEnd(p);
    }

    private static void CheckArushBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!CheckByIconName(p.AbnormalityId, AdrenalineRushIconName)) return; //temporary
        if (!TryGetClassViewModel<LancerLayoutViewModel>(out var vm)) return;

        vm.AdrenalineRush.StartEffect(p.Duration);
    }

    private static void CheckArushRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!CheckByIconName(p.AbnormalityId, AdrenalineRushIconName)) return; //temporary
        if (!TryGetClassViewModel<LancerLayoutViewModel>(out var vm)) return;

        vm.AdrenalineRush.StartEffect(p.Duration);
    }

    private static void CheckArushEnd(S_ABNORMALITY_END p)
    {
        if (!CheckByIconName(p.AbnormalityId, AdrenalineRushIconName)) return; //temporary
        if (!TryGetClassViewModel<LancerLayoutViewModel>(out var vm)) return;

        vm.AdrenalineRush.StopEffect();
    }

    private static void CheckGshoutBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!GShoutIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<LancerLayoutViewModel>(out var vm)) return;

        vm.GuardianShout.StartEffect(p.Duration);
    }

    private static void CheckGshoutRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!GShoutIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<LancerLayoutViewModel>(out var vm)) return;

        vm.GuardianShout.StartEffect(p.Duration);
    }

    private static void CheckGshoutEnd(S_ABNORMALITY_END p)
    {
        if (!GShoutIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<LancerLayoutViewModel>(out var vm)) return;

        vm.GuardianShout.StopEffect();
    }

    private static void CheckLineHeldBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != LineHeldId) return;
        if (!TryGetClassViewModel<LancerLayoutViewModel>(out var vm)) return;

        vm.LH.StartBaseBuff(p.Duration);
    }

    private static void CheckLineHeldRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != LineHeldId) return;
        if (!TryGetClassViewModel<LancerLayoutViewModel>(out var vm)) return;

        vm.LH.RefreshBaseBuff(p.Stacks, p.Duration);
    }

    private static void CheckLineHeldEnd(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != LineHeldId) return;
        if (!TryGetClassViewModel<LancerLayoutViewModel>(out var vm)) return;

        vm.LH.Stop();
    }
}