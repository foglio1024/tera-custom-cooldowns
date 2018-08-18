using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Berserker
    {
        private static readonly int BloodlustId = 400701;
        private static readonly int FieryRageId = 400105;

        private static readonly int UnleashId = 401705;
        private static readonly int Sinister = 401707;
        private static readonly int Dexter = 401709;
        private static readonly int Rampage = 401710;

        public static void CheckBuff(S_ABNORMALITY_BEGIN p)
        {
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                if (p.AbnormalityId == BloodlustId)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).Bloodlust.Buff.Start(p.Duration);
                }
                if (p.AbnormalityId == FieryRageId)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).FieryRage.Buff.Start(p.Duration);
                }
                if (p.AbnormalityId == UnleashId)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).Unleash.Buff.Start(p.Duration);
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).IsUnleashOn = true;
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).IsUnleashOff = false;
                }
            }
        }
        public static void CheckBuff(S_ABNORMALITY_REFRESH p)
        {
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                if (p.AbnormalityId == BloodlustId)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).Bloodlust.Buff.Refresh(p.Duration);
                }
                if (p.AbnormalityId == FieryRageId)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).FieryRage.Buff.Refresh(p.Duration);
                }
                if (p.AbnormalityId == UnleashId)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).Unleash.Buff.Refresh(p.Duration);
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).IsUnleashOn = true;
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).IsUnleashOff = false;
                }
            }
        }
        public static void CheckBuffEnd(S_ABNORMALITY_END p)
        {
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                if (p.AbnormalityId == BloodlustId)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).Bloodlust.Buff.Refresh(0);
                }
                if (p.AbnormalityId == FieryRageId)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).FieryRage.Buff.Refresh(0);
                }
                if (p.AbnormalityId == UnleashId)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).Unleash.Buff.Refresh(0);
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).IsUnleashOn = false;
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).IsUnleashOff = true;
                }
            }
        }

        public static void CheckUnleashAbnormals(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId == Sinister && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).SinisterTracker.Val = p.Stacks;
            }
            if (p.AbnormalityId == Dexter && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).DexterTracker.Val = p.Stacks;
            }
            if (p.AbnormalityId == Rampage && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).RampageTracker.Val = p.Stacks;
            }
        }
        public static void CheckUnleashAbnormals(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId == Sinister && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).SinisterTracker.Val = p.Stacks;
            }
            if (p.AbnormalityId == Dexter && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).DexterTracker.Val = p.Stacks;
            }
            if (p.AbnormalityId == Rampage && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).RampageTracker.Val = p.Stacks;
            }
        }
        public static void CheckUnleashAbnormals(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId == Sinister && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).SinisterTracker.Val = 0;
            }
            if (p.AbnormalityId == Dexter && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).DexterTracker.Val = 0;
            }
            if (p.AbnormalityId == Rampage && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).RampageTracker.Val = 0;
            }
        }

    }
}
