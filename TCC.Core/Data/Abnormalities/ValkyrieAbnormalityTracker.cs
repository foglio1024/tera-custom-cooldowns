using System;
using System.Collections.Generic;
using TCC.Data.Skills;
using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class ValkyrieAbnormalityTracker : AbnormalityTracker
{
    const uint RagnarokId = 10155130;
    const uint GrugnirsBiteId = 10155530;
    const uint GodsfallId = 10155512;
    static readonly List<uint> GodsfallPreCdIds = new() { 10155510, 10155512 };
    static readonly List<uint> TwilightWaltzIds = new() { 10155530, 10155540, 10155541, 10155542 };

    readonly Skill _godsfall;
    readonly Skill _twilightWaltz;
    readonly Skill _grugnirsBite;

    public ValkyrieAbnormalityTracker()
    {
        Game.DB!.SkillsDatabase.TryGetSkillByIconName("icon_skills.rageslash_tex", Game.Me.Class, out var tw);
        Game.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.warbegin_tex", Game.Me.Class, out var gf);
        Game.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.halfmoon_tex", Game.Me.Class, out var gb);

        _twilightWaltz = tw ?? throw new NullReferenceException("Skill not found!");
        _godsfall = gf ?? throw new NullReferenceException("Skill not found!");
        _grugnirsBite = gb ?? throw new NullReferenceException("Skill not found!");
    }

    public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        CheckGrugnirsBite(p);
        CheckTwilightWaltz(p);
        CheckRagnarok(p);
        CheckGodsfallPrecd(p);
        CheckGodsfall(p);
    }
    public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        CheckRagnarok(p);
        CheckGodsfall(p);
    }
    public override void CheckAbnormality(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        CheckRagnarok(p);
        CheckGodsfall(p);
    }

    static void CheckRagnarok(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != RagnarokId) return;
        if (!IsViewModelAvailable<ValkyrieLayoutViewModel>(out var vm)) return;

        vm.Ragnarok.StartEffect(p.Duration);
    }

    static void CheckRagnarok(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != RagnarokId) return;
        if (!IsViewModelAvailable<ValkyrieLayoutViewModel>(out var vm)) return;
        vm.Ragnarok.StopEffect();
    }

    static void CheckRagnarok(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != RagnarokId) return;
        if (!IsViewModelAvailable<ValkyrieLayoutViewModel>(out var vm)) return;
        vm.Ragnarok.RefreshEffect(p.Duration);
    }

    static void CheckGodsfall(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != GodsfallId) return;
        if (!IsViewModelAvailable<ValkyrieLayoutViewModel>(out var vm)) return;
        vm.Godsfall.StartEffect(p.Duration);
    }

    static void CheckGodsfall(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != GodsfallId) return;
        if (!IsViewModelAvailable<ValkyrieLayoutViewModel>(out var vm)) return;
        vm.Godsfall.RefreshEffect(p.Duration);
    }

    static void CheckGodsfall(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != GodsfallId) return;
        if (!IsViewModelAvailable<ValkyrieLayoutViewModel>(out var vm)) return;
        vm.Godsfall.StopEffect();
    }

    void CheckTwilightWaltz(S_ABNORMALITY_BEGIN p)
    {
        if (!TwilightWaltzIds.Contains(p.AbnormalityId)) return;
        if (p is { AbnormalityId: 10155530, Duration: 7000 }) return; // ewww
        //Log.CW($"Starting TW {p.AbnormalityId} {p.Duration}");
        StartPrecooldown(_twilightWaltz, p.Duration);
    }

    void CheckGodsfallPrecd(S_ABNORMALITY_BEGIN p)
    {
        if (!GodsfallPreCdIds.Contains(p.AbnormalityId)) return;
        StartPrecooldown(_godsfall, p.Duration);
    }

    void CheckGrugnirsBite(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != GrugnirsBiteId) return;
        StartPrecooldown(_grugnirsBite, p.Duration);
    }

}