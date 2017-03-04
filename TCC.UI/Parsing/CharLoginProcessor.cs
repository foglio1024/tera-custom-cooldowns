using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.UI
{
    public class CharLoginProcessor
    {
        const int NAME_OFFSET_FROM_START = 290 * 2;
        const int ENTITY_ID_OFFSET_FROM_START = 18 * 2;
        const int CHAR_ID_OFFSET = ENTITY_ID_OFFSET_FROM_START + 16 + 8;


        public string GetName(string content)
        {
            return StringUtils.GetStringFromHex(content, NAME_OFFSET_FROM_START, "0000");
        }
        public string GetEntityId(string content)
        {
            return content.Substring(ENTITY_ID_OFFSET_FROM_START, 16);
        }
        public int GetCharId(string content)
        {
            return StringUtils.Hex4BStringToInt(content.Substring(CHAR_ID_OFFSET, 8));
        }

    }
}
