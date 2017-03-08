using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC
{
    public class CharListProcessor : ListParser
    {
        const int CLASS_OFFSET_FROM_START = 28 * 2;
        const int LEVEL_OFFSET_FROM_START = 32 * 2;
        const int LOCATION_OFFSET_FROM_START = 52 * 2;
        const int LAST_ONLINE_OFFSET_FROM_START = 56 * 2;
        const int LAUREL_OFFSET_FROM_START = 298 * 2;
        const int POSITION_OFFSET_FROM_START = 302 * 2;
        const int GUILD_ID_OFFSET_FROM_START = 306 * 2;
        const int NAME_OFFSET_FROM_START = 310 * 2;
        const int GUILD_NAME_OFFSET_FROM_NAME = 96 * 2;
        const int FIRST_POINTER = 6;

        List<string> charStrings = new List<string>();
        List<Character> CurrentList = new List<Character>();
        private string GetName(string s)
        {
            StringBuilder b = new StringBuilder();
            bool eos = false;
            int i = 0;
            string c = "";
            while (!eos)
            {
                c = s.Substring(NAME_OFFSET_FROM_START + 4 * i, 4);
                if (c != "0000")
                {
                    b.Append(c);
                    i++;
                }
                else
                {
                    eos = true;
                }
            }

            b.Replace("00", "");
            string name = Encoding.UTF7.GetString(StringUtils.StringToByteArray(b.ToString()));
            return name;

        }
        private Class GetClass(string s)
        {
            StringBuilder b = new StringBuilder();
            string c = s.Substring(CLASS_OFFSET_FROM_START, 8);

            for (int i = 4; i > 0; i--)
            {
                b.Append(c[2 * (i - 1)]);
                b.Append(c[2 * (i - 1) + 1]);
            }

            uint classIndex = Convert.ToUInt32(b.ToString(), 16);

            Class cl = (Class)classIndex;
            return cl;

        }
        private Laurel GetLaurel(string s)
        {
            StringBuilder b = new StringBuilder();
            string c = s.Substring(LAUREL_OFFSET_FROM_START, 8);

            for (int i = 4; i > 0; i--)
            {
                b.Append(c[2 * (i - 1)]);
                b.Append(c[2 * (i - 1) + 1]);
            }

            uint laurelIndex = Convert.ToUInt32(b.ToString(), 16);

            Laurel cl = (Laurel)laurelIndex;
            return cl;

        }
        private Class GetClassFromName(string name)
        {
            return CurrentList.Where(x => x.Name == name).Single().Class;
        }
        public Laurel GetLaurelFromName(string name)
        {
            return CurrentList.Where(x => x.Name == name).Single().Laurel;
        }
        Character StringToCharacter(string s)
        {
            string name = GetName(s);
            Class charClass = GetClass(s);
            Laurel charLaurel = GetLaurel(s);
            return new Character(_name: name,
                                c: charClass,
                                l: charLaurel);
        }
        public void ParseCharacters(string p)
        {
            List<Character> _charList = new List<Character>();
            charStrings = ParseList(p);
            foreach (var str in charStrings)
            {
                var c = StringToCharacter(str);
                _charList.Add(c);
            }
            CurrentList = _charList;
        }
        public void Clear()
        {
            charStrings.Clear();
        }

    }

}
