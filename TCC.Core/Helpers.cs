using System.Windows.Media;
using TCC.Data;
using TeraDataLite;

namespace TCC.R
{
    public static class Helpers
    {
        public static Geometry SvgClass(Class c)
        {
            switch (c)
            {
                case Class.Warrior:
                    return SVG.SvgClassWarrior;
                case Class.Lancer:
                    return SVG.SvgClassLancer;
                case Class.Slayer:
                    return SVG.SvgClassSlayer;
                case Class.Berserker:
                    return SVG.SvgClassBerserker;
                case Class.Sorcerer:
                    return SVG.SvgClassSorcerer;
                case Class.Archer:
                    return SVG.SvgClassArcher;
                case Class.Priest:
                    return SVG.SvgClassPriest;
                case Class.Mystic:
                    return SVG.SvgClassMystic;
                case Class.Reaper:
                    return SVG.SvgClassReaper;
                case Class.Gunner:
                    return SVG.SvgClassGunner;
                case Class.Brawler:
                    return SVG.SvgClassBrawler;
                case Class.Ninja:
                    return SVG.SvgClassNinja;
                case Class.Valkyrie:
                    return SVG.SvgClassValkyrie;
                default:
                    return SVG.SvgClassCommon;
            }
        }
    }
}
