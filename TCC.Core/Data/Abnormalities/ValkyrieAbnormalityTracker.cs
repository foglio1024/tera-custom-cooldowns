using System;
using System.Collections.Generic;
using TCC.Data.Skills;
using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class ValkyrieAbnormalityTracker : AbnormalityTracker
{
    private const uint RagnarokId = 10155130;
    private const uint GrugnirsBiteId = 10155530;
    private const uint GodsfallId = 10155512;
    private static readonly List<uint> GodsfallPreCdIds = [10155510, 10155512];
    private static readonly List<uint> TwilightWaltzIds = [10155530, 10155540, 10155541, 10155542];

    private readonly Skill _godsfall;
    private readonly Skill _twilightWaltz;
    private readonly Skill _grugnirsBite;

    public ValkyrieAbnormalityTracker()
    {
        Game.DB!.SkillsDatabase.TryGetSkillByIconName("icon_skills.rageslash_tex", Game.Me.Class, out var tw);
        Game.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.warbegin_tex", Game.Me.Class, out var gf);
        Game.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.halfmoon_tex", Game.Me.Class, out var gb);

        _twilightWaltz = tw ?? throw new NullReferenceException("Skill not found!");
        _godsfall = gf ?? throw new NullReferenceException("Skill not found!");
        _grugnirsBite = gb ?? throw new NullReferenceException("Skill not found!");
    }

    public override void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckGrugnirsBiteBegin(p);
        CheckTwilightWaltzBegin(p);
        CheckRagnarokBegin(p);
        CheckGodsfallPrecdBegin(p);
        CheckGodsfallBegin(p);
    }

    public override void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckRagnarokRefresh(p);
        CheckGodsfallRefresh(p);
    }

    public override void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckRagnarokEnd(p);
        CheckGodsfallEnd(p);
    }

    private static void CheckRagnarokBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != RagnarokId) return;
        if (!TryGetClassViewModel<ValkyrieLayoutViewModel>(out var vm)) return;

        vm.Ragnarok.StartEffect(p.Duration);
    }

    private static void CheckRagnarokEnd(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != RagnarokId) return;
        if (!TryGetClassViewModel<ValkyrieLayoutViewModel>(out var vm)) return;

        vm.Ragnarok.StopEffect();
    }

    private static void CheckRagnarokRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != RagnarokId) return;
        if (!TryGetClassViewModel<ValkyrieLayoutViewModel>(out var vm)) return;

        vm.Ragnarok.RefreshEffect(p.Duration);
    }

    private static void CheckGodsfallBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != GodsfallId) return;
        if (!TryGetClassViewModel<ValkyrieLayoutViewModel>(out var vm)) return;

        vm.Godsfall.StartEffect(p.Duration);
    }

    private static void CheckGodsfallRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != GodsfallId) return;
        if (!TryGetClassViewModel<ValkyrieLayoutViewModel>(out var vm)) return;

        vm.Godsfall.RefreshEffect(p.Duration);
    }

    private static void CheckGodsfallEnd(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != GodsfallId) return;
        if (!TryGetClassViewModel<ValkyrieLayoutViewModel>(out var vm)) return;

        vm.Godsfall.StopEffect();
    }

    private void CheckTwilightWaltzBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!TwilightWaltzIds.Contains(p.AbnormalityId)) return;
        if (p is { AbnormalityId: 10155530, Duration: 7000 }) return; // ewww

        StartPrecooldown(_twilightWaltz, p.Duration);
    }

    private void CheckGodsfallPrecdBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!GodsfallPreCdIds.Contains(p.AbnormalityId)) return;

        StartPrecooldown(_godsfall, p.Duration);
    }

    private void CheckGrugnirsBiteBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != GrugnirsBiteId) return;

        StartPrecooldown(_grugnirsBite, p.Duration);
    }
}