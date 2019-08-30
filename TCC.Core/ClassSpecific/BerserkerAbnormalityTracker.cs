using TCC.Data;
using TCC.ViewModels;
using TeraPacketParser.Messages;
using TeraDataLite;

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

        private const int SinisterKR = 401706; // KR patch by HQ
        private const int DexterKR = 401706;   // KR patch by HQ

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckUnleashAbnormals(p);
            if (p.AbnormalityId == BloodlustId)
            {
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().Bloodlust.Buff.Start(p.Duration);
            }
            if (p.AbnormalityId == FieryRageId)
            {
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().FieryRage.Buff.Start(p.Duration);
            }
            if (p.AbnormalityId == UnleashId)
            {
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().Unleash.Buff.Start(p.Duration);
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOn = true;
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOff = false;
            }
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckUnleashAbnormals(p);

            if (p.AbnormalityId == BloodlustId)
            {
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().Bloodlust.Buff.Refresh(p.Duration, CooldownMode.Normal);
            }
            if (p.AbnormalityId == FieryRageId)
            {
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().FieryRage.Buff.Refresh(p.Duration, CooldownMode.Normal);
            }
            if (p.AbnormalityId == UnleashId)
            {
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().Unleash.Buff.Refresh(p.Duration, CooldownMode.Normal);
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOn = true;
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOff = false;
            }
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckUnleashAbnormals(p);
            if (p.AbnormalityId == BloodlustId)
            {
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().Bloodlust.Buff.Refresh(0, CooldownMode.Normal);
            }
            if (p.AbnormalityId == FieryRageId)
            {
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().FieryRage.Buff.Refresh(0, CooldownMode.Normal);
            }
            if (p.AbnormalityId == UnleashId)
            {
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().Unleash.Buff.Refresh(0, CooldownMode.Normal);
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOn = false;
                TccUtils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOff = true;
            }
        }

        private static void CheckUnleashAbnormals(S_ABNORMALITY_BEGIN p)
        {
            if(TimeManager.Instance.CurrentRegion == RegionEnum.KR)  // KR patch by HQ
            {
                if (p.AbnormalityId == SinisterKR && Game.IsMe(p.TargetId))
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == DexterKR && Game.IsMe(p.TargetId))
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Rampage && Game.IsMe(p.TargetId))
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = p.Stacks;
                }
            }
            else
            {
                if (p.AbnormalityId == Sinister && Game.IsMe(p.TargetId))
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Dexter && Game.IsMe(p.TargetId))
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Rampage && Game.IsMe(p.TargetId))
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = p.Stacks;
                }
            }
        }
        private static void CheckUnleashAbnormals(S_ABNORMALITY_REFRESH p)
        {
            if (TimeManager.Instance.CurrentRegion == RegionEnum.KR)  // KR patch by HQ
            {
                if (p.AbnormalityId == SinisterKR && Game.IsMe(p.TargetId))
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == DexterKR && Game.IsMe(p.TargetId))
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Rampage && Game.IsMe(p.TargetId))
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = p.Stacks;
                }
            }
            else
            {
                if (p.AbnormalityId == Sinister && Game.IsMe(p.TargetId))
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Dexter && Game.IsMe(p.TargetId))
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Rampage && Game.IsMe(p.TargetId))
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = p.Stacks;
                }
            }
        }
        private static void CheckUnleashAbnormals(S_ABNORMALITY_END p)
        {
            if (TimeManager.Instance.CurrentRegion == RegionEnum.KR)  // KR patch by HQ
            {
                if (p.AbnormalityId == SinisterKR)
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = 0;
                }
                if (p.AbnormalityId == DexterKR)
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = 0;
                }
                if (p.AbnormalityId == Rampage)
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = 0;
                }
            }
            else
            { 
                if (p.AbnormalityId == Sinister)
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = 0;
                }
                if (p.AbnormalityId == Dexter)
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = 0;
                }
                if (p.AbnormalityId == Rampage)
                {
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = 0;
                }
            }
        }

    }
}
