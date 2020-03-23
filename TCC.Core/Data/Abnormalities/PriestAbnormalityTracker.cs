using System.Linq;
using TCC.UI;
using TCC.Utilities;
using TCC.ViewModels;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities
{
    public class PriestAbnormalityTracker : AbnormalityTracker
    {
        private static readonly uint[] EnergyStarsIDs = { 801500, 801501, 801502, 801503, 98000107 };
        private const int GraceId = 801700;
        private const int TripleNemesisId = 28090;
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
            TccUtils.CurrentClassVM<PriestLayoutVM>().EnergyStars.Buff.Start(p.Duration);
        }
        private static void CheckEnergyStars(S_ABNORMALITY_REFRESH p)
        {
            if (!EnergyStarsIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<PriestLayoutVM>().EnergyStars.Buff.Refresh(p.Duration, CooldownMode.Normal);

        }
        private static void CheckEnergyStars(S_ABNORMALITY_END p)
        {
            if (!EnergyStarsIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<PriestLayoutVM>().EnergyStars.Buff.Refresh(0, CooldownMode.Normal);
        }

        private static void CheckGrace(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != GraceId) return;
            TccUtils.CurrentClassVM<PriestLayoutVM>().Grace.Buff.Start(p.Duration);
        }
        private static void CheckGrace(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != GraceId) return;
            TccUtils.CurrentClassVM<PriestLayoutVM>().Grace.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }
        private static void CheckGrace(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != GraceId) return;
            TccUtils.CurrentClassVM<PriestLayoutVM>().Grace.Buff.Refresh(0, CooldownMode.Normal);
        }

        private static void CheckEdict(S_ABNORMALITY_BEGIN p)
        {
            if (!EdictIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<PriestLayoutVM>().EdictOfJudgment.Buff.Start(p.Duration);
        }
        private static void CheckEdict(S_ABNORMALITY_REFRESH p)
        {
            if (!EdictIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<PriestLayoutVM>().EdictOfJudgment.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }
        private static void CheckEdict(S_ABNORMALITY_END p)
        {
            if (!EdictIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<PriestLayoutVM>().EdictOfJudgment.Buff.Refresh(0, CooldownMode.Normal);
        }

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            CheckTripleNemesis(p);
            if (!Game.IsMe(p.TargetId)) return;
            CheckEnergyStars(p);
            CheckGrace(p);
            CheckEdict(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            CheckTripleNemesis(p);
            if (!Game.IsMe(p.TargetId)) return;
            CheckEnergyStars(p);
            CheckGrace(p);
            CheckEdict(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            CheckTripleNemesis(p);
            if (!Game.IsMe(p.TargetId)) return;
            CheckEnergyStars(p);
            CheckGrace(p);
            CheckEdict(p);
        }

    }
}
