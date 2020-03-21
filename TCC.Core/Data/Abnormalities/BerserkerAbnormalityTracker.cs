using TCC.Utilities;
using TCC.ViewModels;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities
{
    public class BerserkerAbnormalityTracker : AbnormalityTracker
    {
        private const int BloodlustId = 400701;
        private const int FieryRageId = 400105;

        private const int UnleashId = 401705;
        private const int SinisterDexter = 401706; // no idea
        private const int Rampage = 401710;


        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckUnleashAbnormals(p);

            switch (p.AbnormalityId)
            {
                case BloodlustId:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().Bloodlust.Buff.Start(p.Duration);
                    break;
                case FieryRageId:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().FieryRage.Buff.Start(p.Duration);
                    break;
                case UnleashId:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().Unleash.Buff.Start(p.Duration);
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOn = true;
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOff = false;
                    break;
            }
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckUnleashAbnormals(p);

            switch (p.AbnormalityId)
            {
                case BloodlustId:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().Bloodlust.Buff.Refresh(p.Duration, CooldownMode.Normal);
                    break;
                case FieryRageId:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().FieryRage.Buff.Refresh(p.Duration, CooldownMode.Normal);
                    break;
                case UnleashId:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().Unleash.Buff.Refresh(p.Duration, CooldownMode.Normal);
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOn = true;
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOff = false;
                    break;
            }
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckUnleashAbnormals(p);

            switch (p.AbnormalityId)
            {
                case BloodlustId:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().Bloodlust.Buff.Refresh(0, CooldownMode.Normal);
                    break;
                case FieryRageId:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().FieryRage.Buff.Refresh(0, CooldownMode.Normal);
                    break;
                case UnleashId:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().Unleash.Buff.Refresh(0, CooldownMode.Normal);
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOn = false;
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOff = true;
                    break;
            }
        }

        private static void CheckUnleashAbnormals(S_ABNORMALITY_BEGIN p)
        {
            switch (p.AbnormalityId)
            {
                case SinisterDexter:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = p.Stacks;
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = p.Stacks;
                    break;
                case Rampage:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = p.Stacks;
                    break;
            }
        }
        private static void CheckUnleashAbnormals(S_ABNORMALITY_REFRESH p)
        {
            switch (p.AbnormalityId)
            {
                case SinisterDexter:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = p.Stacks;
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = p.Stacks;
                    break;
                case Rampage:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = p.Stacks;
                    break;
            }
        }
        private static void CheckUnleashAbnormals(S_ABNORMALITY_END p)
        {
            switch (p.AbnormalityId)
            {
                case SinisterDexter:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = 0;
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = 0;
                    break;
                case Rampage:
                    TccUtils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = 0;
                    break;
            }
        }

    }
}
