using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class BrawlerAbnormalityTracker : AbnormalityTracker
{
    const int GrowingFuryId = 10153040;
    const int CounterGlyphId = 31020;

    public override void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckGrowingFuryBegin(p);
        CheckCounterProcBegin(p);
    }

    public override void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckGrowingFuryRefresh(p);
        CheckCounterProcRefresh(p);
    }

    public override void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckGrowingFuryEnd(p);
        CheckCounterProcEnd(p);
    }

    static void CheckGrowingFuryBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != GrowingFuryId) return;
        if (!TryGetClassViewModel<BrawlerLayoutViewModel>(out var vm)) return;

        vm.IsGfOn = true;
    }

    static void CheckGrowingFuryRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != GrowingFuryId) return;
        if (!TryGetClassViewModel<BrawlerLayoutViewModel>(out var vm)) return;

        vm.IsGfOn = true;
    }

    static void CheckGrowingFuryEnd(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != GrowingFuryId) return;
        if (!TryGetClassViewModel<BrawlerLayoutViewModel>(out var vm)) return;

        vm.IsGfOn = false;
    }

    static void CheckCounterProcBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != CounterGlyphId) return;
        if (!TryGetClassViewModel<BrawlerLayoutViewModel>(out var vm)) return;

        vm.Counter.Start(p.Duration);
        vm.CounterProc = true;
    }

    static void CheckCounterProcRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != CounterGlyphId) return;
        if (!TryGetClassViewModel<BrawlerLayoutViewModel>(out var vm)) return;

        vm.Counter.Start(p.Duration);
        vm.CounterProc = true;
    }

    static void CheckCounterProcEnd(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != CounterGlyphId) return;
        if (!TryGetClassViewModel<BrawlerLayoutViewModel>(out var vm)) return;

        vm.Counter.Stop();
        vm.CounterProc = false;
    }
}