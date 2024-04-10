using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.Data.Databases;

public class SkillsDatabase : DatabaseBase
{
    protected override string FolderName => "skills";
    protected override string Extension => "tsv";

    private Dictionary<Class, Dictionary<uint, Skill>> Skills { get; } = [];

    private static readonly Dictionary<Class, List<string>> IgnoredSkills = new()
    {
        {
            Class.Archer,
            [
                "icon_skills.arrowshot_tex",
                "icon_skills.webtrap_tex",
                "icon_skills.focusstance_moveslow_tex"
            ]
        },
        {
            Class.Berserker,
            [
                "icon_skills.comboattack_tex",
                "icon_skills.weapondefence_tex"
            ]
        },
        {
            Class.Brawler,
            [
                "icon_skills.comboattack01_tex",
                "icon_skills.comboattack02_tex",
                "icon_skills.comboattack03_tex",
                "icon_skills.comboattack04_tex",
                "icon_skills.smashattack01_tex",
                "icon_skills.smashattack02_tex",
                "icon_skills.smashattack03_tex",
                "icon_skills.smashattack04_tex",
                "icon_skills.pet_mushroom_tex",
                "icon_skills.rampage_tex"
            ]
        },
        {
            Class.Gunner,
            [
                "icon_skills.cannonshot_tex",
                "icon_skills.gatlingshot_tex",
                "icon_skills.superrocketjump_tex",
                "icon_skills.command_electricballshot_tex"
            ]
        },
        {
            Class.Lancer,
            [
                "icon_skills.comboattack_tex",
                "icon_skills.defence_tex",
                "icon_skills.backstep_tex"
            ]
        },
        {
            Class.Mystic,
            [
                "icon_skills.elementalshot_tex",
                "icon_skills.mpsupplycharge_tex",
                "icon_skills.energiesofrestriction_tex",
                "icon_skills.energiesofquickness_tex",
                "icon_skills.energiesofwillpower_tex"
            ]
        },
        {
            Class.Ninja,
            [
                "icon_skills.c12_meleecombo"
            ]
        },
        {
            Class.Priest,
            [
                "icon_skills.magicshot_tex",
                "icon_skills.adventgoddess_tex"
            ]
        },
        {
            Class.Reaper,
            [
                "icon_skills.comboattack2_tex",
                "icon_skills.shieldattack_tex",
                "icon_skills.tornadoprison_tex"
            ]
        },
        {
            Class.Slayer,
            [
                "icon_skills.comboattack_tex"
            ]
        },
        {
            Class.Sorcerer,
            [
                "icon_skills.fireball_tex",
                "icon_skills.tornadoprison_tex",
                "icon_skills.contractofquickness_tex"
            ]
        },
        {
            Class.Valkyrie,
            [
                "icon_skills.combo_tex"
            ]
        },
        {
            Class.Warrior,
            [
                "icon_skills.comboattack_tex",
                "icon_skills.twinswordsdefence_tex"
            ]
        }
    };

    public SkillsDatabase(string lang) : base(lang)
    {
    }

    public bool TryGetSkill(uint id, Class c, out Skill sk)
    {
        if (!Skills[c].TryGetValue(id, out var skRet))
        {
            sk = new Skill(0, Class.None, string.Empty, string.Empty);
            return false;
        }

        sk = skRet;
        return true;
    }

    //TODO do this better one day
    public IEnumerable<Skill> SkillsForClass(Class c, bool includeCommon = true)
    {
        var ret = Skills[c].Values
            .Where(skill => !IsIgnoredSkill(skill))
            .DistinctBy(x => x.IconName)
            .ToList();

        if (includeCommon)
        {
            ret.AddRange(Skills[Class.Common].Values.Where(skill => skill.Detail is not "mount" and not "eventseed"));
        }
        return ret;
    }

    private static bool IsIgnoredSkill(Skill skill)
    {
        return IgnoredSkills[skill.Class].Any(x => x == skill.IconName);
    }

    public bool TryGetSkillByIconName(string iconName, Class c, out Skill? sk)
    {
        sk = Skills[c].Values.ToList().FirstOrDefault(x => x.IconName == iconName);
        return sk != null;
    }

    public override void Load()
    {
        Skills.Clear();
        for (var i = 0; i <= 12; i++)
        {
            Skills.Add((Class)i, new Dictionary<uint, Skill>());
        }
        Skills.Add(Class.Common, new Dictionary<uint, Skill>());
        Skills.Add(Class.None, new Dictionary<uint, Skill>());

        var lines = File.ReadAllLines(FullPath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) break;

            var s = line.Split('\t');
            var id = Convert.ToUInt32(s[0]);
            _ = Enum.TryParse(s[3], out Class c);
            var name = s[4];
            var detail = s[6];
            var iconName = s[7];
            var sk = new Skill(id, c, name, "")
            {
                IconName = iconName,
                Detail = detail.ToLowerInvariant()
            };
            Skills[c][id] = sk;
        }
    }
}