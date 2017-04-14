using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Tera.Game
{
    public class HotDotDatabase
    {
        public enum HotOrDot
        {
            Dot = 131071,
            Hot = 65536
            //  SystemHot = 655360, // natural regen
            //  CrystalHpHot = 196608,   Not 
            //  StuffMpHot = 262144,
            //  NaturalMpRegen = 0
        }



        public enum StaticallyUsedBuff
        {
            JoyOfPartying100 = 999001021,
            JoyOfPartying50 = 999001020,
            JoyOfPartying20 = 999001019,
            JoyOfPartying0 = 999001018,
            Enraged = 8888888,
            Slaying = 8888889,
            Contagion1 = 701700,
            Contagion2 = 701701,
            Hurricane = 60010

        }

        public HotDot Enraged { get; }
        public HotDot Slaying { get; }
        public readonly HotDot JoyOfPartying0;
        public readonly HotDot JoyOfPartying20;
        public readonly HotDot JoyOfPartying50;
        public readonly HotDot JoyOfPartying100;
        public readonly HotDot Contagion1;
        public readonly HotDot Contagion2;
        public readonly HotDot Enrage;
        public readonly HotDot Hurricane;


        private readonly Dictionary<int, HotDot> _hotdots =
            new Dictionary<int, HotDot>();


        public HotDotDatabase(string folder, string language)
        {
            var reader = new StreamReader(File.OpenRead(Path.Combine(folder, $"hotdot\\hotdot-{language}.tsv")));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null) continue;
                var values = line.Split('\t');
                var id = int.Parse(values[0]);
                var type = values[1];
                var hp = double.Parse(values[2], CultureInfo.InvariantCulture);
                var mp = double.Parse(values[3], CultureInfo.InvariantCulture);
                var method = (HotDot.DotType) Enum.Parse(typeof(HotDot.DotType), values[4]);
                var time = int.Parse(values[5]);
                var tick = int.Parse(values[6]);
                var amount = double.Parse(values[7], CultureInfo.InvariantCulture);
                var name = values[8];
                var itemName = values[10];
                var tooltip = values[11];
                var iconName = values[12];
                if (_hotdots.ContainsKey(id))
                    _hotdots[id].Update(id, type, hp, mp, amount, method, time, tick, name, itemName, tooltip, iconName);
                else
                    _hotdots[id] = new HotDot(id, type, hp, mp, amount, method, time, tick, name, itemName, tooltip, iconName);
            }
            _hotdots[(int)StaticallyUsedBuff.Enraged] = new HotDot((int)StaticallyUsedBuff.Enraged, "Endurance", 0, 0, 0, 0, 0, 0, "Enrage", "", "", "enraged");
            _hotdots[(int)StaticallyUsedBuff.Slaying] = new HotDot((int)StaticallyUsedBuff.Slaying, "CritPower", 0, 0, 0, 0, 0, 0, "Slaying", "",
                "'Slaying' crystal is working (if equipped) when player in this state.", "slaying");

            var shortnames = Path.Combine(folder, $"hotdot\\hotdot-short-{language}.tsv");
            if (File.Exists(shortnames))
            {
                reader = new StreamReader(File.OpenRead(shortnames));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) continue;
                    var values = line.Split('\t');
                    var id = int.Parse(values[0]);
                    var shortname = values[1];
                    if (_hotdots.ContainsKey(id)) _hotdots[id].ShortName = shortname;
                }
            }

            Enraged = _hotdots[(int) StaticallyUsedBuff.Enraged];
            Slaying = _hotdots[(int) StaticallyUsedBuff.Slaying];
            _hotdots.TryGetValue((int)StaticallyUsedBuff.JoyOfPartying0, out JoyOfPartying0);
            _hotdots.TryGetValue((int)StaticallyUsedBuff.JoyOfPartying20, out JoyOfPartying20);
            _hotdots.TryGetValue((int)StaticallyUsedBuff.JoyOfPartying50, out JoyOfPartying50);
            _hotdots.TryGetValue((int)StaticallyUsedBuff.JoyOfPartying100, out JoyOfPartying100);

            _hotdots.TryGetValue((int)StaticallyUsedBuff.Enraged, out Enrage);
            _hotdots.TryGetValue((int)StaticallyUsedBuff.Contagion1, out Contagion1);
            _hotdots.TryGetValue((int)StaticallyUsedBuff.Contagion2, out Contagion2);
            _hotdots.TryGetValue((int)StaticallyUsedBuff.Hurricane, out Hurricane);

        }

        public void Add(HotDot dot)
        {
            _hotdots[dot.Id] = dot;
        }

        public HotDot Get(int skillId)
        {
            return !_hotdots.ContainsKey(skillId) ? null : _hotdots[skillId];
        }
    }
}