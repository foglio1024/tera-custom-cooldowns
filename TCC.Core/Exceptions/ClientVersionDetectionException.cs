using System;

namespace TCC.Exceptions
{
    public class ClientVersionDetectionException : Exception
    {
        public ClientVersionDetectionException(string msg, Exception inner) : base(msg, inner)
        {

        }
    }
}