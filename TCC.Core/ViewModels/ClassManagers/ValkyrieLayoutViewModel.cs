using System;
using Nostrum;
using TCC.Data;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels.ClassManagers;

public class ValkyrieLayoutViewModel : BaseClassLayoutViewModel
{
    public Counter RunemarksCounter { get; set; }
    public SkillWithEffect Ragnarok { get; }
    public SkillWithEffect Godsfall { get; }

    public bool ShowRagnarok => App.Settings.ClassWindowSettings.ValkyrieShowRagnarok;
    public bool ShowGodsfall => App.Settings.ClassWindowSettings.ValkyrieShowGodsfall;

    public string RagnarokEffectSecondsText
    {
        get
        {
            var showDecimals = App.Settings.CooldownsDecimalMode switch
            {
                CooldownDecimalMode.Never => false,
                CooldownDecimalMode.LessThanOne when Ragnarok.Effect.Seconds< 1 => true,
                CooldownDecimalMode.LessThanTen when Ragnarok.Effect.Seconds < 10 => true,
                _ => false
            };

            return TimeUtils.FormatMilliseconds(
                Convert.ToInt64((Ragnarok.Effect.Seconds > uint.MaxValue ? 0 : Ragnarok.Effect.Seconds) * 1000),
                showDecimals);
        }
    }

    public ValkyrieLayoutViewModel()
    {
        RunemarksCounter = new Counter(7, false);

        Game.DB!.SkillsDatabase.TryGetSkill(120100, Class.Valkyrie, out var rag);
        Ragnarok = new SkillWithEffect(_dispatcher, rag);

        Game.DB.SkillsDatabase.TryGetSkill(250100, Class.Valkyrie, out var gf);
        Godsfall = new SkillWithEffect(_dispatcher, gf);

        Ragnarok.Effect.SecondsUpdated += OnEffectSecondsUpdated;
    }

    void OnEffectSecondsUpdated()
    {
        InvokePropertyChanged(nameof(RagnarokEffectSecondsText));
    }

    public override void Dispose()
    {
        Ragnarok.Effect.SecondsUpdated -= OnEffectSecondsUpdated;
        Ragnarok.Dispose();
        Godsfall.Dispose();
    }

    public override bool StartSpecialSkill(Cooldown sk)
    {

        if (sk.Skill.IconName == Ragnarok.Cooldown.Skill.IconName)
        {
            Ragnarok.StartCooldown(sk.Duration);
            return true;
        }

        if (sk.Skill.IconName != Godsfall.Cooldown.Skill.IconName) return false;
        Godsfall.StartCooldown(sk.Duration);
        return true;
    }
}