using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Parsing;
using TCC.TeraCommon;
using Console = System.Console;

namespace TCC
{
    public class ProxyPacketSplitter
    {
        private const string StartToken = "\v:start:\v";
        private const string EndToken   = "\v:end:\v";

        private StringBuilder sb;

        public ConcurrentQueue<string> Packets { get; }
        public ProxyPacketSplitter()
        {
            sb = new StringBuilder();
            Packets = new ConcurrentQueue<string>();
        }
        
        public void Append(string data)
        {
            sb.Append(data);
            //Console.WriteLine($"Appending {data}");
            //Console.WriteLine($"StringBuilder: {sb}");
            var bufferSplit = sb.ToString().Split(new[] {"\v:start:\v"}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var b in bufferSplit)
            {
                if (!b.Contains(EndToken))
                {
                    //Console.WriteLine($"[Splitter] {b} -- is incomplete, continue");
                    continue;
                }
                Packets.Enqueue(b.Replace(EndToken, ""));
                sb = sb.Replace(StartToken + b, "");
            }

            //Console.WriteLine($"Left: {sb}");
        }
    }
}
