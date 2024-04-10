using System.IO;
using Newtonsoft.Json;
using TCC.Data;
using TCC.Utilities;
using TeraDataLite;

namespace TCC.Settings;

public class CooldownConfigParser
{
    public CooldownConfigData Data { get; }

    public CooldownConfigParser(Class c)
    {
        var filePath = Path.Combine(App.ResourcesPath, "config", "skills", $"{c.ToString().ToLower()}-skills.json");

        if (File.Exists(filePath))
        {
            Data = JsonConvert.DeserializeObject<CooldownConfigData>(File.ReadAllText(filePath), TccUtils.GetDefaultJsonSerializerSettings())!;
            return;
        }
            
        Data = GetDefaults(c);
    }

    private static CooldownConfigData GetDefaults(Class c)
    {
        var ret = new CooldownConfigData();
        switch (c)
        {
            case Class.Warrior:
                ret.Main.AddRange(new[]
                {
                    new CooldownData(410100, CooldownType.Skill),
                    new CooldownData(300100, CooldownType.Skill),
                    new CooldownData(181100, CooldownType.Skill),
                    new CooldownData(40100,  CooldownType.Skill),
                    new CooldownData(280730, CooldownType.Skill),
                    new CooldownData(160700, CooldownType.Skill),
                    new CooldownData(191000, CooldownType.Skill),
                    new CooldownData(400100, CooldownType.Skill),
                    new CooldownData(290730, CooldownType.Skill)
                });
                ret.Secondary.AddRange(new[]
                {
                    new CooldownData(420100, CooldownType.Skill)
                });
                break;
            case Class.Lancer:
                ret.Main.AddRange(new[]
                {
                    new CooldownData(50100, CooldownType.Skill),
                    new CooldownData(30100, CooldownType.Skill),
                    new CooldownData(180100, CooldownType.Skill),
                    new CooldownData(100100, CooldownType.Skill),
                    new CooldownData(131100, CooldownType.Skill),
                    new CooldownData(80100, CooldownType.Skill),
                    new CooldownData(250100, CooldownType.Skill),
                    new CooldownData(280100, CooldownType.Skill)
                });
                ret.Secondary.AddRange(new[]
                {
                    new CooldownData(160700, CooldownType.Skill),
                    new CooldownData(220100, CooldownType.Skill),
                    new CooldownData(300100, CooldownType.Skill)
                });
                break;
            case Class.Slayer:
                ret.Main.AddRange(new[]
                {
                    new CooldownData(21400, CooldownType.Skill),
                    new CooldownData(30100, CooldownType.Skill),
                    new CooldownData(80100, CooldownType.Skill),
                    new CooldownData(120100, CooldownType.Skill),
                    new CooldownData(220200, CooldownType.Skill),
                    new CooldownData(230900, CooldownType.Skill)
                });
                ret.Secondary.AddRange(new[]
                {
                    new CooldownData(60100, CooldownType.Skill)
                });
                break;
            case Class.Berserker:
                ret.Main.AddRange(new[]
                {
                    new CooldownData(31400, CooldownType.Skill),
                    new CooldownData(101400, CooldownType.Skill),
                    new CooldownData(151200, CooldownType.Skill),
                    new CooldownData(240500, CooldownType.Skill)
                });
                ret.Secondary.AddRange(new[]
                {
                    new CooldownData(270100, CooldownType.Skill),
                    new CooldownData(80700, CooldownType.Skill)
                });
                break;
            case Class.Sorcerer:
                ret.Main.AddRange(new[]
                {
                    new CooldownData(21500, CooldownType.Skill),
                    new CooldownData(41500, CooldownType.Skill),
                    new CooldownData(120900, CooldownType.Skill),
                    new CooldownData(271000, CooldownType.Skill),
                    new CooldownData(301000, CooldownType.Skill)
                });
                ret.Secondary.AddRange(new[]
                {
                    new CooldownData(70100, CooldownType.Skill),
                    new CooldownData(260100, CooldownType.Skill)
                });
                break;
            case Class.Archer:
                ret.Main.AddRange(new[]
                {
                    new CooldownData(31400, CooldownType.Skill),
                    new CooldownData(41400, CooldownType.Skill),
                    new CooldownData(51000, CooldownType.Skill),
                    new CooldownData(81200, CooldownType.Skill),
                    new CooldownData(221100, CooldownType.Skill),
                    new CooldownData(221100, CooldownType.Skill),
                    new CooldownData(320300, CooldownType.Skill)
                });
                ret.Secondary.AddRange(new[]
                {
                    new CooldownData(60300, CooldownType.Skill),
                    new CooldownData(70700, CooldownType.Skill),
                    new CooldownData(360100, CooldownType.Skill)
                });
                break;
            case Class.Priest:
                ret.Main.AddRange(new[]
                {
                    new CooldownData(181200, CooldownType.Skill),
                    new CooldownData(193200, CooldownType.Skill),
                    new CooldownData(220500, CooldownType.Skill),
                    new CooldownData(370200, CooldownType.Skill),
                    new CooldownData(401000, CooldownType.Skill)
                });
                ret.Secondary.AddRange(new[]
                {
                    new CooldownData(120100, CooldownType.Skill),
                    new CooldownData(300800, CooldownType.Skill),
                    new CooldownData(330300, CooldownType.Skill)
                });
                break;
            case Class.Mystic:
                ret.Main.AddRange(new[]
                {
                    new CooldownData(53100, CooldownType.Skill),
                    new CooldownData(423200, CooldownType.Skill),
                    new CooldownData(70100, CooldownType.Skill),
                    new CooldownData(100100, CooldownType.Skill)
                });
                ret.Secondary.AddRange(new[]
                {
                    new CooldownData(60100, CooldownType.Skill),
                    new CooldownData(170100, CooldownType.Skill),
                    new CooldownData(280600, CooldownType.Skill)
                });
                break;
            case Class.Reaper:
                ret.Main.AddRange(new[]
                {
                    new CooldownData(30300, CooldownType.Skill),
                    new CooldownData(50300, CooldownType.Skill),
                    new CooldownData(60200, CooldownType.Skill),
                    new CooldownData(40300, CooldownType.Skill),
                    new CooldownData(100200, CooldownType.Skill),
                    new CooldownData(80200, CooldownType.Skill),
                    new CooldownData(190100, CooldownType.Skill),
                    new CooldownData(120200, CooldownType.Skill),
                    new CooldownData(90100, CooldownType.Skill)

                });
                ret.Secondary.AddRange(new[]
                {
                    new CooldownData(230100, CooldownType.Skill),
                    new CooldownData(110200, CooldownType.Skill),
                    new CooldownData(150300, CooldownType.Skill)
                });
                break;
            case Class.Gunner:
                ret.Main.AddRange(new[]
                {
                    new CooldownData(151000, CooldownType.Skill),
                    new CooldownData(31100, CooldownType.Skill),
                    new CooldownData(71100, CooldownType.Skill),
                    new CooldownData(61100, CooldownType.Skill),
                    new CooldownData(91000, CooldownType.Skill),
                    new CooldownData(430100, CooldownType.Skill)
                });
                ret.Secondary.AddRange(new[]
                {
                    new CooldownData(470100, CooldownType.Skill),
                    new CooldownData(110800, CooldownType.Skill),
                    new CooldownData(400100, CooldownType.Skill),
                    new CooldownData(210300, CooldownType.Skill)
                });
                ret.Hidden.AddRange(new[]
                {
                    new CooldownData(110731, CooldownType.Skill)
                });
                break;
            case Class.Brawler:
                ret.Main.AddRange(new[]
                {
                    new CooldownData(41000, CooldownType.Skill),
                    new CooldownData(71000, CooldownType.Skill), //rhk
                    new CooldownData(91000, CooldownType.Skill), //jh
                    new CooldownData(81000, CooldownType.Skill), //pd
                    new CooldownData(61000, CooldownType.Skill), //hm
                    new CooldownData(240100, CooldownType.Skill)
                });
                ret.Secondary.AddRange(new[]
                {
                    new CooldownData(260100, CooldownType.Skill),
                    new CooldownData(50900, CooldownType.Skill)
                });
                break;
            case Class.Ninja:
                ret.Main.AddRange(new[]
                {
                    new CooldownData(30800, CooldownType.Skill),
                    new CooldownData(71000, CooldownType.Skill),
                    new CooldownData(141100, CooldownType.Skill),
                    new CooldownData(121100, CooldownType.Skill),
                    new CooldownData(131000, CooldownType.Skill),
                    new CooldownData(61000, CooldownType.Skill),
                    new CooldownData(190500, CooldownType.Skill),
                    new CooldownData(220100, CooldownType.Skill),
                    new CooldownData(210190, CooldownType.Skill)
                });
                ret.Secondary.AddRange(new[]
                {
                    new CooldownData(20100, CooldownType.Skill),
                    new CooldownData(53100, CooldownType.Skill),
                    new CooldownData(90100, CooldownType.Skill)
                });
                break;
            case Class.Valkyrie:
                ret.Main.AddRange(new[]
                {
                    new CooldownData(55500, CooldownType.Skill),
                    new CooldownData(115300, CooldownType.Skill),
                    new CooldownData(66000, CooldownType.Skill),
                    new CooldownData(75800, CooldownType.Skill),
                    new CooldownData(96000, CooldownType.Skill),
                    new CooldownData(106000, CooldownType.Skill),
                    new CooldownData(136100, CooldownType.Skill),
                    new CooldownData(166000, CooldownType.Skill),
                    new CooldownData(245100, CooldownType.Skill),
                    new CooldownData(230100, CooldownType.Skill)
                });
                ret.Secondary.AddRange(new[]
                {
                    new CooldownData(80100, CooldownType.Skill),
                    new CooldownData(205800, CooldownType.Skill)
                });
                break;
        }

        return ret;
    }
}