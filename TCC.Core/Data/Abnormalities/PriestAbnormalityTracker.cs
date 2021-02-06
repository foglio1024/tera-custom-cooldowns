using System.Linq;
using TCC.UI;
using TCC.ViewModels;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities
{
    public class PriestAbnormalityTracker : AbnormalityTracker
    {
        private static readonly uint[] EnergyStarsIDs = { 801500, 801501, 801502, 801503, 98000107 };
        private const int GraceId = 801700;
        private const int TripleNemesisId = 28090;
        private const int DivineId = 805713;
        private static readonly uint[] EdictIDs = { 805800 };

        private static void CheckTripleNemesis(S_ABNORMALITY_BEGIN p)
        {
            if (TripleNemesisId != p.AbnormalityId) return;
            if (!WindowManager.ViewModels.NpcVM.TryFindNPC(p.TargetId, out _)) return;
            if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
            InvokeMarkingRefreshed(p.Duration);
        }
        private static void CheckTripleNemesis(S_ABNORMALITY_REFRESH p)
        {
            if (TripleNemesisId != p.AbnormalityId) return;
            if (!WindowManager.ViewModels.NpcVM.TryFindNPC(p.TargetId, out _)) return;
            if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
            InvokeMarkingRefreshed(p.Duration);
        }
        private static void CheckTripleNemesis(S_ABNORMALITY_END p)
        {
            if (TripleNemesisId != p.AbnormalityId) return;
            if (MarkedTargets.Contains(p.TargetId)) MarkedTargets.Remove(p.TargetId);
            if (MarkedTargets.Count == 0) InvokeMarkingExpired();
        }

        private static void CheckEnergyStars(S_ABNORMALITY_BEGIN p)
        {
            if (!EnergyStarsIDs.Contains(p.AbnormalityId)) return;
            if (!IsViewModelAvailable<PriestLayoutVM>(out var vm)) return;

            vm!.EnergyStars.StartEffect(p.Duration);
        }
        private static void CheckEnergyStars(S_ABNORMALITY_REFRESH p)
        {
            if (!EnergyStarsIDs.Contains(p.AbnormalityId)) return;
            if (!IsViewModelAvailable<PriestLayoutVM>(out var vm)) return;

            vm!.EnergyStars.RefreshEffect(p.Duration);

        }
        private static void CheckEnergyStars(S_ABNORMALITY_END p)
        {
            if (!EnergyStarsIDs.Contains(p.AbnormalityId)) return;
            if (!IsViewModelAvailable<PriestLayoutVM>(out var vm)) return;

            vm!.EnergyStars.StopEffect();
        }

        private static void CheckGrace(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != GraceId) return;
            if (!IsViewModelAvailable<PriestLayoutVM>(out var vm)) return;

            vm!.Grace.StartEffect(p.Duration);
        }
        private static void CheckGrace(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != GraceId) return;
            if (!IsViewModelAvailable<PriestLayoutVM>(out var vm)) return;

            vm!.Grace.RefreshEffect(p.Duration);
        }
        private static void CheckGrace(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != GraceId) return;
            if (!IsViewModelAvailable<PriestLayoutVM>(out var vm)) return;

            vm!.Grace.StopEffect();
        }

        private static void CheckEdict(S_ABNORMALITY_BEGIN p)
        {
            if (!EdictIDs.Contains(p.AbnormalityId)) return;
            if (!IsViewModelAvailable<PriestLayoutVM>(out var vm)) return;

            vm!.EdictOfJudgment.StartEffect(p.Duration);
        }
        private static void CheckEdict(S_ABNORMALITY_REFRESH p)
        {
            if (!EdictIDs.Contains(p.AbnormalityId)) return;
            if (!IsViewModelAvailable<PriestLayoutVM>(out var vm)) return;

            vm!.EdictOfJudgment.RefreshEffect(p.Duration);
        }
        private static void CheckEdict(S_ABNORMALITY_END p)
        {
            if (!EdictIDs.Contains(p.AbnormalityId)) return;
            if (!IsViewModelAvailable<PriestLayoutVM>(out var vm)) return;

            vm!.EdictOfJudgment.StopEffect();
        }

        private static void CheckDivine(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != DivineId) return;
            if (!IsViewModelAvailable<PriestLayoutVM>(out var vm)) return;

            vm!.DivineCharge.StartEffect(p.Duration);
        }
        private static void CheckDivine(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != DivineId) return;
            if (!IsViewModelAvailable<PriestLayoutVM>(out var vm)) return;

            vm!.DivineCharge.RefreshEffect(p.Duration);
        }
        private static void CheckDivine(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != DivineId) return;
            if (!IsViewModelAvailable<PriestLayoutVM>(out var vm)) return;

            vm!.DivineCharge.StopEffect();
        }

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            CheckTripleNemesis(p);
            CheckEnergyStars(p);
            CheckDivine(p);
            if (!Game.IsMe(p.TargetId)) return;
            CheckGrace(p);
            CheckEdict(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            CheckTripleNemesis(p);
            CheckEnergyStars(p);
            CheckDivine(p);
            if (!Game.IsMe(p.TargetId)) return;
            CheckGrace(p);
            CheckEdict(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            CheckTripleNemesis(p);
            CheckEnergyStars(p);
            CheckDivine(p);
            if (!Game.IsMe(p.TargetId)) return;
            CheckGrace(p);
            CheckEdict(p);
        }

    }
}
