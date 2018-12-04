using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TCC.Data;

namespace TCC.R
{
    public static class Helpers
    {
        public static Geometry SvgClass(Class c)
        {
            switch (c)
            {
                case Class.Warrior:
                    return R.SVG.SvgClassWarrior;
                case Class.Lancer:
                    return R.SVG.SvgClassLancer;
                case Class.Slayer:
                    return R.SVG.SvgClassSlayer;
                case Class.Berserker:
                    return R.SVG.SvgClassBerserker;
                case Class.Sorcerer:
                    return R.SVG.SvgClassSorcerer;
                case Class.Archer:
                    return R.SVG.SvgClassArcher;
                case Class.Priest:
                    return R.SVG.SvgClassPriest;
                case Class.Mystic:
                    return R.SVG.SvgClassMystic;
                case Class.Reaper:
                    return R.SVG.SvgClassReaper;
                case Class.Gunner:
                    return R.SVG.SvgClassGunner;
                case Class.Brawler:
                    return R.SVG.SvgClassBrawler;
                case Class.Ninja:
                    return R.SVG.SvgClassNinja;
                case Class.Valkyrie:
                    return R.SVG.SvgClassValkyrie;
                default:
                    return R.SVG.SvgClassCommon;
            }
        }
    }
}
