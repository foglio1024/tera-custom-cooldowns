using System;
using TCC.Data.Skills;
using TCC.ViewModels.ClassManagers;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class SorcererAbnormalityTracker : AbnormalityTracker
{
    const int ManaBoostId = 500150;
    const int ManaBoost2Id = 501602;
    const int ManaBoost2MId = 503061;
    const int FlameFusionIncreaseId = 502070;   // Equipoise-Flame
    const int FrostFusionIncreaseId = 502071;   // Equipoise-Frost
    const int ArcaneFusionIncreaseId = 502072;  // Equipoise-Arcane

    const int FireIceFusionId = 502020;
    //private const int FireArcaneFusionId = 502030;
    //private const int IceArcaneFusionId = 502040;

    readonly Skill _fireIceFusion;
    //private static Skill _fireArcaneFusion;
    //private static Skill _iceArcaneFusion;

    public static event Action? BoostChanged;

    static bool IsManaBoost(uint id)
    {
        return id is ManaBoostId or ManaBoost2Id or ManaBoost2MId;
    }

    public SorcererAbnormalityTracker()
    {
        Game.DB!.AbnormalityDatabase.Abnormalities.TryGetValue(FireIceFusionId, out var ab);
        _fireIceFusion = new Skill(ab ?? throw new NullReferenceException("Skill not found"), Class.Sorcerer);
    }

    static void CheckManaBoost(S_ABNORMALITY_BEGIN p)
    {
        if (!IsManaBoost(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<SorcererLayoutVM>(out var vm)) return;

        vm.ManaBoost.StartEffect(p.Duration);

    }

    static void CheckManaBoost(S_ABNORMALITY_REFRESH p)
    {
        if (!IsManaBoost(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<SorcererLayoutVM>(out var vm)) return;

        vm.ManaBoost.RefreshEffect(p.Duration);

    }

    static void CheckManaBoost(S_ABNORMALITY_END p)
    {
        if (!IsManaBoost(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<SorcererLayoutVM>(out var vm)) return;

        vm.ManaBoost.StopEffect();
    }

    static void CheckFusionBoost(S_ABNORMALITY_BEGIN p)
    {
        switch (p.AbnormalityId)
        {
            case FlameFusionIncreaseId:
                Game.SetSorcererElementsBoost(true, false, false);
                break;
            case FrostFusionIncreaseId:
                Game.SetSorcererElementsBoost(false, true, false);
                break;
            case ArcaneFusionIncreaseId:
                Game.SetSorcererElementsBoost(false, false, true);
                break;
            default:
                return;
        }

        BoostChanged?.Invoke();
    }

    static void CheckFusionBoost(S_ABNORMALITY_REFRESH p)
    {
        switch (p.AbnormalityId)
        {
            case FlameFusionIncreaseId:
                Game.SetSorcererElementsBoost(true, false, false);
                break;
            case FrostFusionIncreaseId:
                Game.SetSorcererElementsBoost(false, true, false);
                break;
            case ArcaneFusionIncreaseId:
                Game.SetSorcererElementsBoost(false, false, true);
                break;
            default:
                return;
        }

        BoostChanged?.Invoke();
    }

    static void CheckFusionBoost(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId is FlameFusionIncreaseId
                            or FrostFusionIncreaseId
                            or ArcaneFusionIncreaseId)
        {
            Game.SetSorcererElementsBoost(false, false, false);
        }
        else return;
        BoostChanged?.Invoke();

    }

    void CheckFusions(S_ABNORMALITY_BEGIN p)
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

    static void CheckFusions(S_ABNORMALITY_END p)
    {
        if (FireIceFusionId != p.AbnormalityId) return;

        if (!IsViewModelAvailable<SorcererLayoutVM>(out var vm)) return;

        vm.EndFireIcePre();
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
        if (!Game.IsMe(p.TargetId)) return;
        CheckFusions(p);
        CheckManaBoost(p);
        CheckFusionBoost(p);
    }
    public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        CheckManaBoost(p);
        CheckFusionBoost(p);
    }
    public override void CheckAbnormality(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        CheckManaBoost(p);
        CheckFusionBoost(p);
        CheckFusions(p);
    }


}