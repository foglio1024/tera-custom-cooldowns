﻿using System;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels.ClassManagers;

public class ReaperLayoutViewModel : BaseClassLayoutViewModel
{
    public SkillWithEffect ShadowReaping { get; }
    public SkillWithEffect ShroudedEscape { get; }
    public SkillWithEffect PowerlinkedGrimStrike { get; }
    public SkillWithEffect PowerlinkedDoubleShear { get; }

    public ReaperLayoutViewModel()
    {
        Game.DB!.SkillsDatabase.TryGetSkill(160100, Class.Reaper, out var sr);
        ShadowReaping = new SkillWithEffect(_dispatcher, sr);

        Game.DB.SkillsDatabase.TryGetSkill(180100, Class.Reaper, out var se);
        ShroudedEscape = new SkillWithEffect(_dispatcher, se);

        Game.DB.SkillsDatabase.TryGetSkill(50100, Class.Reaper, out var plgs);
        PowerlinkedGrimStrike = new SkillWithEffect(_dispatcher, plgs, false);

        Game.DB.SkillsDatabase.TryGetSkill(30100, Class.Reaper, out var plds);
        PowerlinkedDoubleShear = new SkillWithEffect(_dispatcher, plds, false);
    }

    public override void Dispose()
    {
        ShadowReaping.Dispose();
        ShroudedEscape.Dispose();
    }

    public override bool StartSpecialSkill(Cooldown sk)
    {
        if (sk.Skill.IconName == ShadowReaping.Cooldown.Skill.IconName)
        {
            ShadowReaping.StartCooldown(sk.Duration);
            return true;
        }

        if (sk.Skill.IconName != ShroudedEscape.Cooldown.Skill.IconName) return false;
        ShroudedEscape.StartCooldown(sk.Duration);
        return true;
    }
}