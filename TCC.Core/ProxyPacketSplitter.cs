using System;
using System.Collections.Concurrent;
using System.Text;

namespace TCC
{
    public class ProxyPacketSplitter
    {
        private const string StartToken = "\v:start:\v";
        private const string EndToken   = "\v:end:\v";

        private StringBuilder _sb;

        public ConcurrentQueue<string> Packets { get; }
        public ProxyPacketSplitter()
        {
            _sb = new StringBuilder();
            Packets = new ConcurrentQueue<string>();
        }
        
        public void Append(string data)
        {
            _sb.Append(data);
            //Console.WriteLine($"Appending {data}");
            //Console.WriteLine($"StringBuilder: {sb}");
            var bufferSplit = _sb.ToString().Split(new[] {"\v:start:\v"}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var b in bufferSplit)
            {
                if (!b.Contains(EndToken))
                {
                    //Console.WriteLine($"[Splitter] {b} -- is incomplete, continue");
                    continue;
                }
                Packets.Enqueue(b.Replace(EndToken, ""));
                _sb = _sb.Replace(StartToken + b, "");
            }

            //Console.WriteLine($"Left: {sb}");
        }
    }
}
