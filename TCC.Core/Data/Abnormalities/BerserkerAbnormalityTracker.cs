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
            if (!IsViewModelAvailable<BerserkerLayoutVM>(out var vm)) return;
            CheckUnleashAbnormals(p);

            switch (p.AbnormalityId)
            {
                case BloodlustId:
                    vm.Bloodlust.Buff.Start(p.Duration);
                    break;
                case FieryRageId:
                    vm.FieryRage.Buff.Start(p.Duration);
                    break;
                case UnleashId:
                    vm.Unleash.Buff.Start(p.Duration);
                    vm.IsUnleashOn = true;
                    vm.IsUnleashOff = false;
                    break;
            }
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            if (!IsViewModelAvailable<BerserkerLayoutVM>(out var vm)) return;

            CheckUnleashAbnormals(p);

            switch (p.AbnormalityId)
            {
                case BloodlustId:
                    vm.Bloodlust.Buff.Refresh(p.Duration, CooldownMode.Normal);
                    break;
                case FieryRageId:
                    vm.FieryRage.Buff.Refresh(p.Duration, CooldownMode.Normal);
                    break;
                case UnleashId:
                    vm.Unleash.Buff.Refresh(p.Duration, CooldownMode.Normal);
                    vm.IsUnleashOn = true;
                    vm.IsUnleashOff = false;
                    break;
            }
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            if (!IsViewModelAvailable<BerserkerLayoutVM>(out var vm)) return;

            CheckUnleashAbnormals(p);

            switch (p.AbnormalityId)
            {
                case BloodlustId:
                    vm.Bloodlust.Buff.Refresh(0, CooldownMode.Normal);
                    break;
                case FieryRageId:
                    vm.FieryRage.Buff.Refresh(0, CooldownMode.Normal);
                    break;
                case UnleashId:
                    vm.Unleash.Buff.Refresh(0, CooldownMode.Normal);
                    vm.IsUnleashOn = false;
                    vm.IsUnleashOff = true;
                    break;
            }
        }

        private static void CheckUnleashAbnormals(S_ABNORMALITY_BEGIN p)
        {
            if (!IsViewModelAvailable<BerserkerLayoutVM>(out var vm)) return;

            switch (p.AbnormalityId)
            {
                case SinisterDexter:
                    vm.SinisterTracker.Val = p.Stacks;
                    vm.DexterTracker.Val = p.Stacks;
                    break;
                case Rampage:
                    vm.RampageTracker.Val = p.Stacks;
                    break;
            }
        }
        private static void CheckUnleashAbnormals(S_ABNORMALITY_REFRESH p)
        {
            if (!IsViewModelAvailable<BerserkerLayoutVM>(out var vm)) return;

            switch (p.AbnormalityId)
            {
                case SinisterDexter:
                    vm.SinisterTracker.Val = p.Stacks;
                    vm.DexterTracker.Val = p.Stacks;
                    break;
                case Rampage:
                    vm.RampageTracker.Val = p.Stacks;
                    break;
            }
        }
        private static void CheckUnleashAbnormals(S_ABNORMALITY_END p)
        {
            if (!IsViewModelAvailable<BerserkerLayoutVM>(out var vm)) return;

            switch (p.AbnormalityId)
            {
                case SinisterDexter:
                    vm.SinisterTracker.Val = 0;
                    vm.DexterTracker.Val = 0;
                    break;
                case Rampage:
                    vm.RampageTracker.Val = 0;
                    break;
            }
        }

    }
}
