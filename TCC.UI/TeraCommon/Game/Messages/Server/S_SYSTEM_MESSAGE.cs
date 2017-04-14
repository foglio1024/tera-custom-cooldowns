
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tera.Game.Messages
{
    public class S_SYSTEM_MESSAGE : ParsedMessage
    {
        internal S_SYSTEM_MESSAGE(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(2);//offset
		    RawMessage = reader.ReadTeraString();
            var parts=RawMessage.Split(new[] {'\v'});
            ushort id;
            ushort.TryParse(parts[0].Replace("@", ""), out id);
            MsgType = reader.SysMsgNamer?.GetName(id) ?? id.ToString();
            int i = 1;
            while (i + 2 <= parts.Length)
            {
                Parameters[parts[i]] = parts[i + 1];
                i = i + 2;
            }
            //todo add various strsheet_*.xml to reconstruct game message as it seen by user (if needed?)
            Debug.WriteLine(MsgType + ":   "+string.Join(";\t",Parameters.Select(x=>x.Key+": "+x.Value)));
        }

        public string RawMessage { get; private set; }
        public string MsgType { get; private set; }
        public Dictionary<string,string> Parameters=new Dictionary<string, string>();

    }
}