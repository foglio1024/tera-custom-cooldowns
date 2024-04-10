using System;
using System.Collections.Generic;
using TCC.Data.Skills;
using TCC.ViewModels.ClassManagers;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class SorcererAbnormalityTracker : AbnormalityTracker
{
    private static readonly List<uint> ManaBoostIds = [500150, 501602, 503061];
    private const int FlameFusionIncreaseId = 502070;   // Equipoise-Flame
    private const int FrostFusionIncreaseId = 502071;   // Equipoise-Frost
    private const int ArcaneFusionIncreaseId = 502072;  // Equipoise-Arcane

    private const int FireIceFusionId = 502020;
    //private const int FireArcaneFusionId = 502030;
    //private const int IceArcaneFusionId = 502040;

    private readonly Skill _fireIceFusion;
    //private static Skill _fireArcaneFusion;
    //private static Skill _iceArcaneFusion;

    public static event Action<FusionElements>? BoostChanged;

    public SorcererAbnormalityTracker()
    {
        Game.DB!.AbnormalityDatabase.Abnormalities.TryGetValue(FireIceFusionId, out var ab);
        _fireIceFusion = new Skill(ab ?? throw new NullReferenceException("Skill not found"), Class.Sorcerer);
    }

    public override void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckFusionsBegin(p);
        CheckManaBoostBegin(p);
        CheckFusionBoostBegin(p);
    }

    public override void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckManaBoostRefresh(p);
        CheckFusionBoostRefresh(p);
    }

    public override void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckManaBoostEnd(p);
        CheckFusionBoostEnd(p);
        CheckFusionsEnd(p);
    }

    private static void CheckManaBoostBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!ManaBoostIds.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<SorcererLayoutViewModel>(out var vm)) return;

        vm.ManaBoost.StartEffect(p.Duration);
    }

    private static void CheckManaBoostRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!ManaBoostIds.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<SorcererLayoutViewModel>(out var vm)) return;

        vm.ManaBoost.RefreshEffect(p.Duration);
    }

    private static void CheckManaBoostEnd(S_ABNORMALITY_END p)
    {
        if (!ManaBoostIds.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<SorcererLayoutViewModel>(out var vm)) return;

        vm.ManaBoost.StopEffect();
    }

    private static void CheckFusionBoostBegin(S_ABNORMALITY_BEGIN p)
    {
        FusionElements element;
        switch (p.AbnormalityId)
        {
            case FlameFusionIncreaseId:
                element = FusionElements.Flame;
                break;

            case FrostFusionIncreaseId:
                element = FusionElements.Frost;
                break;

            case ArcaneFusionIncreaseId:
                element = FusionElements.Arcane;
                break;

            default:
                return;
        }

        BoostChanged?.Invoke(element);
    }

    private static void CheckFusionBoostRefresh(S_ABNORMALITY_REFRESH p)
    {
        FusionElements element;

        switch (p.AbnormalityId)
        {
            case FlameFusionIncreaseId:
                element = FusionElements.Flame;
                break;
            case FrostFusionIncreaseId:
                element = FusionElements.Frost;
                break;

            case ArcaneFusionIncreaseId:
                element = FusionElements.Arcane;
                break;

            default:
                return;
        }

        BoostChanged?.Invoke(element);
    }

    private static void CheckFusionBoostEnd(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId is not 
            (FlameFusionIncreaseId
            or FrostFusionIncreaseId
            or ArcaneFusionIncreaseId))
            return;

        BoostChanged?.Invoke(FusionElements.None);
    }

    private void CheckFusionsBegin(S_ABNORMALITY_BEGIN p)
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

    private static void CheckFusionsEnd(S_ABNORMALITY_END p)
    {
        if (FireIceFusionId != p.AbnormalityId) return;
        if (!TryGetClassViewModel<SorcererLayoutViewModel>(out var vm)) return;

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
}