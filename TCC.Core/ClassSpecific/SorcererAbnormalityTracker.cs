using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class SorcererAbnormalityTracker : ClassAbnormalityTracker
    {
        private const int ManaBoostId = 500150;
        private const int FlameFusionIncreaseId = 502070;   // Equipoise-Flame
        private const int FrostFusionIncreaseId = 502071;   // Equipoise-Frost
        private const int ArcaneFusionIncreaseId = 502071;  // Equipoise-Arcane


        private static void CheckManaBoost(S_ABNORMALITY_BEGIN p)
        {
            if (ManaBoostId != p.AbnormalityId) return;
            ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).ManaBoost.Buff.Start(p.Duration);

        }
        private static void CheckManaBoost(S_ABNORMALITY_REFRESH p)
        {
            if (ManaBoostId != p.AbnormalityId) return;
            ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).ManaBoost.Buff.Refresh(p.Duration);

        }
        private static void CheckManaBoost(S_ABNORMALITY_END p)
        {
            if (ManaBoostId != p.AbnormalityId) return;
            ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).ManaBoost.Buff.Refresh(0);
        }

        private static void CheckFusionBoost(S_ABNORMALITY_BEGIN p)
        {
            if (FlameFusionIncreaseId == p.AbnormalityId)
            {
                ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).IsBoostFire = true;
            }
            else if (FrostFusionIncreaseId == p.AbnormalityId)
            {
                ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).IsBoostFrost = true;
            }
            else if (ArcaneFusionIncreaseId == p.AbnormalityId)
            {
                ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).IsBoostArcane = true;
            }
        }

        private static void CheckFusionBoost(S_ABNORMALITY_REFRESH p)
        {
            if (FlameFusionIncreaseId == p.AbnormalityId)
            {
                ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).IsBoostFire = true;
            }
            else if (FrostFusionIncreaseId == p.AbnormalityId)
            {
                ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).IsBoostFrost = true;
            }
            else if (ArcaneFusionIncreaseId == p.AbnormalityId)
            {
                ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).IsBoostArcane = true;
            }
        }

        private static void CheckFusionBoost(S_ABNORMALITY_END p)
        {
            if (FlameFusionIncreaseId == p.AbnormalityId)
            {
                ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).IsBoostFire = false;
            }
            else if (FrostFusionIncreaseId == p.AbnormalityId)
            {
                ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).IsBoostFrost = false;
            }
            else if (ArcaneFusionIncreaseId == p.AbnormalityId)
            {
                ((SorcererBarManager)ClassWindowViewModel.Instance.CurrentManager).IsBoostArcane = false;
            }
        }

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckManaBoost(p);
            CheckFusionBoost(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckManaBoost(p);
            CheckFusionBoost(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckManaBoost(p);
            CheckFusionBoost(p);
        }
    }
}
