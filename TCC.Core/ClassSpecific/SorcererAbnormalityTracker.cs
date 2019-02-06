using TCC.Data;
using TCC.Data.Skills;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class SorcererAbnormalityTracker : ClassAbnormalityTracker
    {
        private const int ManaBoostId = 500150;
        private const int FlameFusionIncreaseId = 502070;   // Equipoise-Flame
        private const int FrostFusionIncreaseId = 502071;   // Equipoise-Frost
        private const int ArcaneFusionIncreaseId = 502072;  // Equipoise-Arcane

        private const int FireIceFusionId = 502021;
        //private const int FireArcaneFusionId = 502030;
        //private const int IceArcaneFusionId = 502040;

        private static Skill _fireIceFusion;
        //private static Skill _fireArcaneFusion;
        //private static Skill _iceArcaneFusion;

        public SorcererAbnormalityTracker()
        {
            var fireIceFusionAb = SessionManager.CurrentDatabase.AbnormalityDatabase.Abnormalities[FireIceFusionId];
            //var fireArcaneFusionAb = SessionManager.CurrentDatabase.AbnormalityDatabase.Abnormalities[FireArcaneFusionId];
            //var iceArcaneFusionAb = SessionManager.CurrentDatabase.AbnormalityDatabase.Abnormalities[IceArcaneFusionId];

            _fireIceFusion = new Skill(fireIceFusionAb, Class.Sorcerer);
            //_fireArcaneFusion = new Skill(fireArcaneFusionAb, Class.Sorcerer);
            //_iceArcaneFusion = new Skill(iceArcaneFusionAb, Class.Sorcerer);
        }
        private static void CheckManaBoost(S_ABNORMALITY_BEGIN p)
        {
            if (ManaBoostId != p.AbnormalityId) return;
            ((SorcererBarManager)WindowManager.ClassWindow.VM.CurrentManager).ManaBoost.Buff.Start(p.Duration);

        }
        private static void CheckManaBoost(S_ABNORMALITY_REFRESH p)
        {
            if (ManaBoostId != p.AbnormalityId) return;
            ((SorcererBarManager)WindowManager.ClassWindow.VM.CurrentManager).ManaBoost.Buff.Refresh(p.Duration, CooldownMode.Normal);

        }
        private static void CheckManaBoost(S_ABNORMALITY_END p)
        {
            if (ManaBoostId != p.AbnormalityId) return;
            ((SorcererBarManager)WindowManager.ClassWindow.VM.CurrentManager).ManaBoost.Buff.Refresh(0, CooldownMode.Normal);
        }

        private static void CheckFusionBoost(S_ABNORMALITY_BEGIN p)
        {
            if (FlameFusionIncreaseId == p.AbnormalityId)
            {
                SessionManager.SetSorcererElementsBoost(true, false, false);
            }
            else if (FrostFusionIncreaseId == p.AbnormalityId)
            {
                SessionManager.SetSorcererElementsBoost(false, true, false);
            }
            else if (ArcaneFusionIncreaseId == p.AbnormalityId)
            {
                SessionManager.SetSorcererElementsBoost(false, false, true);
            }
        }
        private static void CheckFusionBoost(S_ABNORMALITY_REFRESH p)
        {
            if (FlameFusionIncreaseId == p.AbnormalityId)
            {
                SessionManager.SetSorcererElementsBoost(true, false, false);
            }
            else if (FrostFusionIncreaseId == p.AbnormalityId)
            {
                SessionManager.SetSorcererElementsBoost(false, true, false);
            }
            else if (ArcaneFusionIncreaseId == p.AbnormalityId)
            {
                SessionManager.SetSorcererElementsBoost(false, false, true);
            }
        }
        private static void CheckFusionBoost(S_ABNORMALITY_END p)
        {
            if (FlameFusionIncreaseId == p.AbnormalityId || FrostFusionIncreaseId == p.AbnormalityId || ArcaneFusionIncreaseId == p.AbnormalityId)
            {
                SessionManager.SetSorcererElementsBoost(false, false, false);
            }
        }
        private static void CheckFusions(S_ABNORMALITY_BEGIN p)
        {
            if (FireIceFusionId == p.AbnormalityId)
            {
                StartPrecooldown(_fireIceFusion, p.Duration);
            }
            //else if (FireArcaneFusionId == p.AbnormalityId)
            //{
            //    StartPrecooldown(_fireArcaneFusion, p.Duration);
            //}
            //else if (IceArcaneFusionId == p.AbnormalityId)
            //{
            //    StartPrecooldown(_iceArcaneFusion, p.Duration);
            //}
        }
        private static void CheckFusions(S_ABNORMALITY_END p)
        {
            if (FireIceFusionId == p.AbnormalityId)
            {
                ((SorcererBarManager)WindowManager.ClassWindow.VM.CurrentManager).EndFireIcePre();
            }
            //else if (FireArcaneFusionId == p.AbnormalityId)
            //{
            //    StartPrecooldown(_fireArcaneFusion, p.Duration);
            //}
            //else if (IceArcaneFusionId == p.AbnormalityId)
            //{
            //    StartPrecooldown(_iceArcaneFusion, p.Duration);
            //}
        }

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckFusions(p);
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
            CheckFusions(p);
        }


    }
}
