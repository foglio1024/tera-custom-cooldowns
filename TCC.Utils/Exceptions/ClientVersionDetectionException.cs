using System;

namespace TCC.Utils.Exceptions
{
    public class ClientVersionDetectionException : Exception
    {
        public ClientVersionDetectionException(string msg, Exception inner) : base(msg, inner)
        {

        }
    }
}