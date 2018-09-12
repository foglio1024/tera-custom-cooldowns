using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace TCC.Data.Databases
{
    public class AbnormalityDatabase
    {
        public readonly Dictionary<uint, Abnormality> Abnormalities;
        public static readonly List<uint> NoctIds = new List<uint> { 902, 910, 911, 912, 913, 916, 917, 999010000 };
        public static List<uint> BlueNoctIds = new List<uint> { 920, 921, 922};

        //TODO: maybe refactor this one day
        public AbnormalityDatabase(string lang)
        {
            Abnormalities = new Dictionary<uint, Abnormality>();
            var hd = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "/resources/data/hotdot/hotdot-" + lang +".tsv");
            while (true)
            {
                var l = hd.ReadLine();
                if (l == null) break;
                if(l == "") continue;
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
                            Abnormalities[id].Infinity = false;
                        }
                    }

                    if (!isBuff) Abnormalities[id].IsBuff = false;
                    continue;
                }
                Abnormalities.Add(id, ab);
            }
        }
    }

}
