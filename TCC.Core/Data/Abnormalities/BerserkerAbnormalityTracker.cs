using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class BerserkerAbnormalityTracker : AbnormalityTracker
{
    const int BloodlustId = 400701;
    const int FieryRageId = 400105;

    const int UnleashId = 401705;
    const int SinisterDexter = 401706; // no idea
    const int Rampage = 401710;


    public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        if (!IsViewModelAvailable<BerserkerLayoutVM>(out var vm)) return;
        CheckUnleashAbnormals(p);

        switch (p.AbnormalityId)
        {
            case BloodlustId:
                vm!.Bloodlust.StartEffect(p.Duration);
                break;
            case FieryRageId:
                vm!.FieryRage.StartEffect(p.Duration);
                break;
            case UnleashId:
                vm!.Unleash.StartEffect(p.Duration);
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
                vm!.Bloodlust.RefreshEffect(p.Duration);
                break;
            case FieryRageId:
                vm!.FieryRage.RefreshEffect(p.Duration);
                break;
            case UnleashId:
                vm!.Unleash.RefreshEffect(p.Duration);
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
                vm!.Bloodlust.StopEffect();
                break;
            case FieryRageId:
                vm!.FieryRage.StopEffect();
                break;
            case UnleashId:
                vm!.Unleash.StopEffect();
                vm.IsUnleashOn = false;
                vm.IsUnleashOff = true;
                break;
        }
    }

    static void CheckUnleashAbnormals(S_ABNORMALITY_BEGIN p)
    {
        if (!IsViewModelAvailable<BerserkerLayoutVM>(out var vm)) return;

        switch (p.AbnormalityId)
        {
            case SinisterDexter:
                vm!.SinisterTracker.Val = p.Stacks;
                vm.DexterTracker.Val = p.Stacks;
                break;
            case Rampage:
                vm!.RampageTracker.Val = p.Stacks;
                break;
        }
    }

    static void CheckUnleashAbnormals(S_ABNORMALITY_REFRESH p)
    {
        if (!IsViewModelAvailable<BerserkerLayoutVM>(out var vm)) return;

        switch (p.AbnormalityId)
        {
            case SinisterDexter:
                vm!.SinisterTracker.Val = p.Stacks;
                vm.DexterTracker.Val = p.Stacks;
                break;
            case Rampage:
                vm!.RampageTracker.Val = p.Stacks;
                break;
        }
    }

    static void CheckUnleashAbnormals(S_ABNORMALITY_END p)
    {
        if (!IsViewModelAvailable<BerserkerLayoutVM>(out var vm)) return;

        switch (p.AbnormalityId)
        {
            case SinisterDexter:
                vm!.SinisterTracker.Val = 0;
                vm.DexterTracker.Val = 0;
                break;
            case Rampage:
                vm!.RampageTracker.Val = 0;
                break;
        }
    }

}