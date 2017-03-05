using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC
{
    public class ListParser
    {
        protected const int FIRST_POINTER_POSITION = 6;

        internal List<string> ParseList(string packet)
        {
            int packetLength = StringUtils.Hex2BStringToInt(packet.Substring(0, 4));
            int count = StringUtils.Hex2BStringToInt(packet.Substring(8, 4));

            int currentPointer = FIRST_POINTER_POSITION * 2;

            List<string> list = new List<string>();

            for (int i = 0; i < count; i++)
            {
                int pointer = ReadPointer(packet, currentPointer) * 2;
                int nextPointer = ReadPointer(packet, pointer + 4) * 2;

                if (nextPointer != 0)
                {
                    list.Add(packet.Substring(pointer + 8, nextPointer - pointer));
                }

                else
                {
                    list.Add(packet.Substring(pointer + 8));
                }

                currentPointer = nextPointer;
            }

            return list;
        }

        protected int ReadPointer(string hex, int pos)
        {
            return StringUtils.Hex2BStringToInt(hex.Substring(pos, 4));
        }
    }
}
