using System.Linq;
using TCC.UI;
using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class MysticAbnormalityTracker : AbnormalityTracker
{
    //const int HurricaneId = 60010;
    //const int HurricaneDuration = 120000;
    const int VowOfRebirthId = 700100;

    const int VolleyOfCursesId = 27160;
    const int ThrallOfVengeanceId = 702003;
    const int ThrallOfWrathId = 702004;

    static readonly uint[] CritAuraIDs = [700600, 700601, 700602, 700603];
    static readonly uint[] ManaAuraIDs = [700300];
    static readonly uint[] CritResAuraIDs = [700200, 700201, 700202, 700203];
    static readonly uint[] SwiftAuraIDs = [700700, 700701];
    static readonly uint[] ElementalizeIDs = [702000];

    //public static void CheckHurricane(S_ABNORMALITY_BEGIN msg)
    //{
    //    if (msg.AbnormalityId != HurricaneId || !Game.IsMe(msg.CasterId)) return;
    //    Game.DB.SkillsDatabase.TryGetSkill(HurricaneId, Class.Common, out var hurricane);
    //    SkillManager.AddSkillDirectly(hurricane, HurricaneDuration);
    //}

    public override void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!TryGetClassViewModel<MysticLayoutViewModel>(out var vm)) return;

        CheckVolleyOfCursesBegin(p);
        CheckAurasBegin(p, vm);
    }

    public override void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!TryGetClassViewModel<MysticLayoutViewModel>(out var vm)) return;

        CheckVolleyOfCursesRefresh(p);
        CheckAurasRefresh(p, vm);
    }

    public override void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        if (!TryGetClassViewModel<MysticLayoutViewModel>(out var vm)) return;

        CheckVolleyOfCursesEnd(p);
        CheckAurasEnd(p, vm);
    }

    static void CheckAurasBegin(S_ABNORMALITY_BEGIN p, MysticLayoutViewModel vm)
    {
        if (!Game.IsMe(p.TargetId)) return;

        if (CritAuraIDs.Contains(p.AbnormalityId))
        {
            vm.Auras.CritAura = true;
        }
        else if (ManaAuraIDs.Contains(p.AbnormalityId))
        {
            vm.Auras.ManaAura = true;
        }
        else if (CritResAuraIDs.Contains(p.AbnormalityId))
        {
            vm.Auras.CritResAura = true;
        }
        else if (SwiftAuraIDs.Contains(p.AbnormalityId))
        {
            vm.Auras.SwiftAura = true;
        }
        else if (p.AbnormalityId == VowOfRebirthId)
        {
            vm.Vow.StartEffect(p.Duration);
        }
        else if (ElementalizeIDs.Contains(p.AbnormalityId))
        {
            vm.Elementalize = true;
        }
        else if (p.AbnormalityId == ThrallOfVengeanceId)
        {
            vm.ThrallOfVengeance.StartEffect(p.Duration);
        }
        else if (p.AbnormalityId == ThrallOfWrathId)
        {
            vm.ThrallOfWrath.StartEffect(p.Duration);
        }
    }

    static void CheckAurasRefresh(S_ABNORMALITY_REFRESH p, MysticLayoutViewModel vm)
    {
        if (!Game.IsMe(p.TargetId)) return;

        if (CritAuraIDs.Contains(p.AbnormalityId))
        {
            vm.Auras.CritAura = true;
        }
        else if (ManaAuraIDs.Contains(p.AbnormalityId))
        {
            vm.Auras.ManaAura = true;
        }
        else if (CritResAuraIDs.Contains(p.AbnormalityId))
        {
            vm.Auras.CritResAura = true;
        }
        else if (SwiftAuraIDs.Contains(p.AbnormalityId))
        {
            vm.Auras.SwiftAura = true;
        }
        else if (p.AbnormalityId == VowOfRebirthId)
        {
            vm.Vow.RefreshEffect(p.Duration);
        }
        else if (p.AbnormalityId == ThrallOfVengeanceId)
        {
            vm.ThrallOfVengeance.RefreshEffect(p.Duration);
        }
        else if (p.AbnormalityId == ThrallOfWrathId)
        {
            vm.ThrallOfWrath.RefreshEffect(p.Duration);
        }
        else if (ElementalizeIDs.Contains(p.AbnormalityId))
        {
            vm.Elementalize = true;
        }
    }

    static void CheckAurasEnd(S_ABNORMALITY_END p, MysticLayoutViewModel vm)
    {
        if (!Game.IsMe(p.TargetId)) return;

        if (CritAuraIDs.Contains(p.AbnormalityId))
        {
            vm.Auras.CritAura = false;
        }
        else if (ManaAuraIDs.Contains(p.AbnormalityId))
        {
            vm.Auras.ManaAura = false;
        }
        else if (CritResAuraIDs.Contains(p.AbnormalityId))
        {
            vm.Auras.CritResAura = false;
        }
        else if (SwiftAuraIDs.Contains(p.AbnormalityId))
        {
            vm.Auras.SwiftAura = false;
        }
        else if (p.AbnormalityId == VowOfRebirthId)
        {
            vm.Vow.StopEffect();
        }
        else if (p.AbnormalityId == ThrallOfVengeanceId)
        {
            vm.ThrallOfVengeance.StopEffect();
        }
        else if (p.AbnormalityId == ThrallOfWrathId)
        {
            vm.ThrallOfWrath.StopEffect();
        }
        else if (ElementalizeIDs.Contains(p.AbnormalityId))
        {
            vm.Elementalize = false;
        }
    }

    static void CheckVolleyOfCursesBegin(S_ABNORMALITY_BEGIN p)
    {
        if (VolleyOfCursesId != p.AbnormalityId) return;
        if (!WindowManager.ViewModels.NpcVM.TryFindNPC(p.TargetId, out _)) return;

        if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
        InvokeMarkingRefreshed(p.Duration);
    }

    static void CheckVolleyOfCursesRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (VolleyOfCursesId != p.AbnormalityId) return;
        if (!WindowManager.ViewModels.NpcVM.TryFindNPC(p.TargetId, out _)) return;

        if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
        InvokeMarkingRefreshed(p.Duration);
    }

    static void CheckVolleyOfCursesEnd(S_ABNORMALITY_END p)
    {
        if (VolleyOfCursesId != p.AbnormalityId) return;

        if (MarkedTargets.Contains(p.TargetId)) MarkedTargets.Remove(p.TargetId);
        if (MarkedTargets.Count == 0) InvokeMarkingExpired();
    }
}