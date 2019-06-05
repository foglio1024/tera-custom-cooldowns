using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Settings
{
    public abstract class SettingsReaderBase
    {
        protected string FileName = "";

        protected static readonly List<uint> CommonDefault = new List<uint> { 4000, 4001, 4010, 4011, 4020, 4021, 4030, 4031, 4600, 4610, 4611, 4613, 5000003, 4830, 4831, 4833, 4841, 4886, 4861, 4953, 4955, 7777015, 902, 910, 911, 912, 913, 916, 920, 921, 922, 999010000 };
        protected static readonly List<uint> PriestDefault = new List<uint> { 201, 202, 805100, 805101, 805102, 98000109, 805600, 805601, 805602, 805603, 805604, 98000110, 800300, 800301, 800302, 800303, 800304, 801500, 801501, 801502, 801503, 98000107 };
        protected static readonly List<uint> MysticDefault = new List<uint> { 27120, 700630, 700631, 601, 602, 603, 700330, 700230, 700231, 800132, 700233, 700730, 700731, 700100 };

        //Add My Abnormals Setting by HQ ====================================================
        protected static readonly List<uint> MyCommonDefault = new List<uint> { 6001, 6002, 6003, 6004, 6012, 6013, 702004, 805800, 805803, 200700, 200701, 200731, 800300, 800301, 800302, 800303, 800304, 702001 };
        protected static readonly List<uint> MyArcherDefault = new List<uint> { 601400, 601450, 601460, 88608101, 88608102, 88608103, 88608104, 88608105, 88608106, 88608107, 88608108, 88608109, 88608110 };
        protected static readonly List<uint> MyBerserkerDefault = new List<uint> { 401705, 401707, 401709, 401710, 400500, 400501, 400508, 400710, 400711 };
        protected static readonly List<uint> MyBrawlerDefault = new List<uint> { 31020, 10153210 };
        protected static readonly List<uint> MyGunnerDefault = new List<uint> { 89105101, 89105102, 89105103, 89105104, 89105105, 89105106, 89105107, 89105108, 89105109, 89105110, 89105111, 89105112, 89105113, 89105114, 89105115, 89105116, 89105117, 89105118, 89105119, 89105120, 10152340, 10152351 };
        protected static readonly List<uint> MyLancerDefault = new List<uint> { 200230, 200231, 200232, 201701 };
        protected static readonly List<uint> MyMysticDefault = new List<uint>();
        protected static readonly List<uint> MyNinjaDefault = new List<uint> { 89314201, 89314202, 89314203, 89314204, 89314205, 89314206, 89314207, 89314208, 89314209, 89314210, 89314211, 89314212, 89314213, 89314214, 89314215, 89314216, 89314217, 89314218, 89314219, 89314220, 10154480, 10154450 };
        protected static readonly List<uint> MyPriestDefault = new List<uint>();
        protected static readonly List<uint> MyReaperDefault = new List<uint> { 10151010, 10151131, 10151192 };
        protected static readonly List<uint> MySlayerDefault = new List<uint> { 300800, 300801, 300805 };
        protected static readonly List<uint> MySorcererDefault = new List<uint> { 21170, 22120, 23180, 26250, 29011, 25170, 25171, 25201, 25202, 500100, 500150, 501600, 501650 };
        protected static readonly List<uint> MyValkyrieDefault = new List<uint> { 10155130, 10155551, 10155510, 10155512, 10155540, 10155541, 10155542 };
        protected static readonly List<uint> MyWarriorDefault = new List<uint> { 100800, 100801 };
        //===================================================================================

    }
}
