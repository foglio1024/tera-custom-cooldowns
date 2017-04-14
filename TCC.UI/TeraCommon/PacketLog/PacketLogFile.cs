using System;
using System.Collections.Generic;
using System.IO;

namespace Tera.PacketLog
{
    public class PacketLogFile
    {
        private readonly Func<Stream> _openStream;

        private PacketLogFile(Func<Stream> openStream)
        {
            _openStream = openStream;
            using (var stream = _openStream())
            {
                var reader = new PacketLogReader(stream);
                Header = reader.Header;
            }
        }

        public PacketLogFile(string filename)
            : this(() => new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
        }

        public LogHeader Header { get; private set; }

        public IEnumerable<Message> Messages
        {
            get
            {
                using (var stream = _openStream())
                {
                    var reader = new PacketLogReader(stream);
                    Message message;
                    while ((message = reader.ReadMessage()) != null)
                    {
                        yield return message;
                    }
                }
            }
        }
    }
}