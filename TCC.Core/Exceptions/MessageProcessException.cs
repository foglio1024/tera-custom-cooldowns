using System;

namespace TCC.Exceptions
{
    public class MessageProcessException : Exception
    {
        public MessageProcessException(string msg) : base(msg)
        {
        }

        public MessageProcessException(string msg, Exception inner) : base(msg, inner)
        {
            
        }
    }
}