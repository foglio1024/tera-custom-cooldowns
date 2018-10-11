using System.Linq;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class PriestAbnormalityTracker : ClassAbnormalityTracker
    {
        private static readonly uint[] EnergyStarsIDs = { 801500, 801501, 801502, 801503, 98000107 };
        private static readonly int GraceId = 801700;
        private static readonly int TripleNemesisId = 28090;
        private static readonly uint[] EdictIDs = { 805800, 805801, 805802, 805803 };

        private static void CheckTripleNemesis(S_ABNORMALITY_BEGIN p)
        {
            if (TripleNemesisId != p.AbnormalityId) return;
            var target = BossGageWindowViewModel.Instance.NpcList.FirstOrDefault(x => x.EntityId == p.TargetId);
            if (target != null)
            {
                if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
                InvokeMarkingRefreshed(p.Duration);
            }
        }
        private static void CheckTripleNemesis(S_ABNORMALITY_REFRESH p)
        {
            if (TripleNemesisId != p.AbnormalityId) return;
            var target = BossGageWindowViewModel.Instance.NpcList.FirstOrDefault(x => x.EntityId == p.TargetId);
            if (target != null)
            {
                if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
                InvokeMarkingRefreshed(p.Duration);
            }
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
            ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EnergyStars.Buff.Start(p.Duration);
        }
        private static void CheckEnergyStars(S_ABNORMALITY_REFRESH p)
        {
            if (!EnergyStarsIDs.Contains(p.AbnormalityId)) return;
            ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EnergyStars.Buff.Refresh(p.Duration);

        }
        private static void CheckEnergyStars(S_ABNORMALITY_END p)
        {
            if (!EnergyStarsIDs.Contains(p.AbnormalityId)) return;
            ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EnergyStars.Buff.Refresh(0);
        }

        private static void CheckGrace(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != GraceId) return;
            ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).Grace.Buff.Start(p.Duration);
        }
        private static void CheckGrace(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != GraceId) return;
            ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).Grace.Buff.Refresh(p.Duration);
        }
        private static void CheckGrace(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != GraceId) return;
            ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).Grace.Buff.Refresh(0);
        }

        private static void CheckEdict(S_ABNORMALITY_BEGIN p)
        {
            if (!EdictIDs.Contains(p.AbnormalityId)) return;
            ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EdictOfJudgment.Buff.Start(p.Duration);
        }
        private static void CheckEdict(S_ABNORMALITY_REFRESH p)
        {
            if (!EdictIDs.Contains(p.AbnormalityId)) return;
            ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EdictOfJudgment.Buff.Refresh(p.Duration);
        }
        private static void CheckEdict(S_ABNORMALITY_END p)
        {
            if (!EdictIDs.Contains(p.AbnormalityId)) return;
            ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EdictOfJudgment.Buff.Refresh(0);
        }

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            CheckTripleNemesis(p);
            if (!p.TargetId.IsMe()) return;
            CheckEnergyStars(p);
            CheckGrace(p);
            CheckEdict(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            CheckTripleNemesis(p);
            if (!p.TargetId.IsMe()) return;
            CheckEnergyStars(p);
            CheckGrace(p);
            CheckEdict(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            CheckTripleNemesis(p);
            if (!p.TargetId.IsMe()) return;
            CheckEnergyStars(p);
            CheckGrace(p);
            CheckEdict(p);
        }

    }
}
