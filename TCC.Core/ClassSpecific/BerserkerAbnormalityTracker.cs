using TCC.Data;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class BerserkerAbnormalityTracker : ClassAbnormalityTracker
    {
        private const int BloodlustId = 400701;
        private const int FieryRageId = 400105;

        private const int UnleashId = 401705;
        private const int Sinister = 401707;
        private const int Dexter = 401709;
        private const int Rampage = 401710;

        private const int Sinister_KR = 401706; // KR patch by HQ
        private const int Dexter_KR = 401706;   // KR patch by HQ

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckUnleashAbnormals(p);
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
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckUnleashAbnormals(p);

            if (p.AbnormalityId == BloodlustId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).Bloodlust.Buff.Refresh(p.Duration, CooldownMode.Normal);
            }
            if (p.AbnormalityId == FieryRageId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).FieryRage.Buff.Refresh(p.Duration, CooldownMode.Normal);
            }
            if (p.AbnormalityId == UnleashId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).Unleash.Buff.Refresh(p.Duration, CooldownMode.Normal);
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).IsUnleashOn = true;
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).IsUnleashOff = false;
            }
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckUnleashAbnormals(p);
            if (p.AbnormalityId == BloodlustId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).Bloodlust.Buff.Refresh(0, CooldownMode.Normal);
            }
            if (p.AbnormalityId == FieryRageId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).FieryRage.Buff.Refresh(0, CooldownMode.Normal);
            }
            if (p.AbnormalityId == UnleashId)
            {
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).Unleash.Buff.Refresh(0, CooldownMode.Normal);
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).IsUnleashOn = false;
                ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).IsUnleashOff = true;
            }
        }

        private static void CheckUnleashAbnormals(S_ABNORMALITY_BEGIN p)
        {
            if(Settings.SettingsStorage.LastRegion == "KR")  // KR patch by HQ
            {
                if (p.AbnormalityId == Sinister_KR && p.TargetId.IsMe())
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).SinisterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Dexter_KR && p.TargetId.IsMe())
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).DexterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Rampage && p.TargetId.IsMe())
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).RampageTracker.Val = p.Stacks;
                }
            }
            else
            {
                if (p.AbnormalityId == Sinister && p.TargetId.IsMe())
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).SinisterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Dexter && p.TargetId.IsMe())
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).DexterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Rampage && p.TargetId.IsMe())
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).RampageTracker.Val = p.Stacks;
                }
            }
        }
        private static void CheckUnleashAbnormals(S_ABNORMALITY_REFRESH p)
        {
            if (Settings.SettingsStorage.LastRegion == "KR")  // KR patch by HQ
            {
                if (p.AbnormalityId == Sinister_KR && p.TargetId.IsMe())
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).SinisterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Dexter_KR && p.TargetId.IsMe())
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).DexterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Rampage && p.TargetId.IsMe())
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).RampageTracker.Val = p.Stacks;
                }
            }
            else
            {
                if (p.AbnormalityId == Sinister && p.TargetId.IsMe())
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).SinisterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Dexter && p.TargetId.IsMe())
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).DexterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Rampage && p.TargetId.IsMe())
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).RampageTracker.Val = p.Stacks;
                }
            }
        }
        private static void CheckUnleashAbnormals(S_ABNORMALITY_END p)
        {
            if (Settings.SettingsStorage.LastRegion == "KR")  // KR patch by HQ
            {
                if (p.AbnormalityId == Sinister_KR)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).SinisterTracker.Val = 0;
                }
                if (p.AbnormalityId == Dexter_KR)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).DexterTracker.Val = 0;
                }
                if (p.AbnormalityId == Rampage)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).RampageTracker.Val = 0;
                }
            }
            else
            { 
                if (p.AbnormalityId == Sinister)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).SinisterTracker.Val = 0;
                }
                if (p.AbnormalityId == Dexter)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).DexterTracker.Val = 0;
                }
                if (p.AbnormalityId == Rampage)
                {
                    ((BerserkerBarManager)ClassWindowViewModel.Instance.CurrentManager).RampageTracker.Val = 0;
                }
            }
        }

    }
}
