using System.Linq;
using TCC.UI;
using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class PriestAbnormalityTracker : AbnormalityTracker
{
    private static readonly uint[] EnergyStarsIDs = [801500, 801501, 801502, 801503, 98000107];
    private const int GraceId = 801700;
    private const int TripleNemesisId = 28090;
    private const int DivineId = 805713;
    private static readonly uint[] EdictIDs = [805800];

    public override void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        CheckTripleNemesisBegin(p);
        CheckEnergyStarsBegin(p);
        CheckDivineBegin(p);

        if (!Game.IsMe(p.TargetId)) return;
        
        CheckGraceBegin(p);
        CheckEdictBegin(p);
    }

    public override void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        CheckTripleNemesisRefresh(p);
        CheckEnergyStarsRefresh(p);
        CheckDivineRefresh(p);
        
        if (!Game.IsMe(p.TargetId)) return;
        
        CheckGraceRefresh(p);
        CheckEdictRefresh(p);
    }

    public override void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        CheckTripleNemesisEnd(p);
        CheckEnergyStarsEnd(p);
        CheckDivineEnd(p);
        
        if (!Game.IsMe(p.TargetId)) return;
        
        CheckGraceEnd(p);
        CheckEdictEnd(p);
    }

    private static void CheckTripleNemesisBegin(S_ABNORMALITY_BEGIN p)
    {
        if (TripleNemesisId != p.AbnormalityId) return;
        if (!WindowManager.ViewModels.NpcVM.TryFindNPC(p.TargetId, out _)) return;
        
        if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
        InvokeMarkingRefreshed(p.Duration);
    }

    private static void CheckTripleNemesisRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (TripleNemesisId != p.AbnormalityId) return;
        if (!WindowManager.ViewModels.NpcVM.TryFindNPC(p.TargetId, out _)) return;
        
        if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
        InvokeMarkingRefreshed(p.Duration);
    }

    private static void CheckTripleNemesisEnd(S_ABNORMALITY_END p)
    {
        if (TripleNemesisId != p.AbnormalityId) return;
        
        if (MarkedTargets.Contains(p.TargetId)) MarkedTargets.Remove(p.TargetId);
        if (MarkedTargets.Count == 0) InvokeMarkingExpired();
    }

    private static void CheckEnergyStarsBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!EnergyStarsIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<PriestLayoutViewModel>(out var vm)) return;

        vm.EnergyStars.StartEffect(p.Duration);
    }

    private static void CheckEnergyStarsRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!EnergyStarsIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<PriestLayoutViewModel>(out var vm)) return;

        vm.EnergyStars.RefreshEffect(p.Duration);
    }

    private static void CheckEnergyStarsEnd(S_ABNORMALITY_END p)
    {
        if (!EnergyStarsIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<PriestLayoutViewModel>(out var vm)) return;

        vm.EnergyStars.StopEffect();
    }

    private static void CheckGraceBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != GraceId) return;
        if (!TryGetClassViewModel<PriestLayoutViewModel>(out var vm)) return;

        vm.Grace.StartEffect(p.Duration);
    }

    private static void CheckGraceRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != GraceId) return;
        if (!TryGetClassViewModel<PriestLayoutViewModel>(out var vm)) return;

        vm.Grace.RefreshEffect(p.Duration);
    }

    private static void CheckGraceEnd(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != GraceId) return;
        if (!TryGetClassViewModel<PriestLayoutViewModel>(out var vm)) return;

        vm.Grace.StopEffect();
    }

    private static void CheckEdictBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!EdictIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<PriestLayoutViewModel>(out var vm)) return;

        vm.EdictOfJudgment.StartEffect(p.Duration);
    }

    private static void CheckEdictRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!EdictIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<PriestLayoutViewModel>(out var vm)) return;

        vm.EdictOfJudgment.RefreshEffect(p.Duration);
    }

    private static void CheckEdictEnd(S_ABNORMALITY_END p)
    {
        if (!EdictIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<PriestLayoutViewModel>(out var vm)) return;

        vm.EdictOfJudgment.StopEffect();
    }

    private static void CheckDivineBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != DivineId) return;
        if (!TryGetClassViewModel<PriestLayoutViewModel>(out var vm)) return;

        vm.DivineCharge.StartEffect(p.Duration);
    }

    private static void CheckDivineRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != DivineId) return;
        if (!TryGetClassViewModel<PriestLayoutViewModel>(out var vm)) return;

        vm.DivineCharge.RefreshEffect(p.Duration);
    }

    private static void CheckDivineEnd(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != DivineId) return;
        if (!TryGetClassViewModel<PriestLayoutViewModel>(out var vm)) return;

        vm.DivineCharge.StopEffect();
    }
}