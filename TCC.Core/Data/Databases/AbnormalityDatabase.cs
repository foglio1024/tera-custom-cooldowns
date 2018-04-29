using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace TCC.Data.Databases
{
    public class AbnormalityDatabase
    {
        public Dictionary<uint, Abnormality> Abnormalities;
        public static List<uint> NoctIds = new List<uint> { 902, 910, 911, 912, 913, 916, 917, 999010000 };
        public static List<uint> BlueNoctIds = new List<uint> { 920, 921, 922};

        public AbnormalityDatabase(string lang)
        {
            //var f = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "/resources/data/abnormalities/abnormals-"+lang+".tsv");
            Abnormalities = new Dictionary<uint, Abnormality>();
            //while (true)
            //{
            //    var line = f.ReadLine();
            //    if (line == null) break;

            //    var s = line.Split('\t');

            //    var id = Convert.ToUInt32(s[0]);
            //    Enum.TryParse(s[1], out AbnormalityType t);
            //    var isShow = bool.Parse(s[2]);
            //    var isBuff = bool.Parse(s[3]);
            //    var infinity = bool.Parse(s[4]);
            //    var name = s[5];
            //    var tooltip = s[6].Replace("&#xA;", "\n");
            //    tooltip = tooltip.Replace("&#xD;", "\r");
            //    var iconName = s[7];

            //    //---add fixes here---//

            //    if(id == 78100006) //isBuff = true for lakan's souls world debuff (it's not supposed to make hp bar purple)
            //    {
            //        isBuff = true;
            //    }

            //    //--------------------//
            //    var ab = new Abnormality(id, isShow, isBuff, infinity, t);
            //    ab.SetIcon(iconName);
            //    ab.SetInfo(name, tooltip);

            //    Abnormalities.Add(id, ab);
            //}
            var hd = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "/resources/data/hotdot/hotdot-" + lang +".tsv");
            while (true)
            {
                var l = hd.ReadLine();
                if (l == null) break;

                var s = l.Split('\t');
                var id = uint.Parse(s[0]);
                var type = s[1];
                var amount = double.Parse(s[7], CultureInfo.InvariantCulture);
                var isBuff = bool.Parse(s[3]);
                var isShow = bool.Parse(s[14]);
                var name = s[8];
                var tooltip = s[11];
                var icon = s[13];
                var abType = (AbnormalityType) Enum.Parse(typeof(AbnormalityType), s[2]);
                //var isBuff = bool.Parse(s[15]);
                var infinite = s[5] == "0";
                var ab = new Abnormality(id,isShow, isBuff,  infinite, abType);
                ab.SetIcon(icon);
                ab.SetInfo(name, tooltip);
                if (type.IndexOf("Absorb", StringComparison.Ordinal) > -1)
                {
                    ab.SetShield((uint)amount); //TODO: may just parse everything instead of doing this
                }
                if (Abnormalities.ContainsKey(id))
                {
                    if (!Abnormalities[id].IsShield && ab.IsShield)
                    {
                        Abnormalities[id] = ab;
                    }
                    if (ab.Infinity)
                    {
                        if (!Abnormalities[id].Infinity)
                        {
                            Abnormalities[id].Infinity = true;
                        }
                    }
                    continue;
                }
                Abnormalities.Add(id, ab);
            }
        }
    }

}
