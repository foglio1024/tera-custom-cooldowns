using System;

namespace TCC.Exceptions
{
    public class DeadlockException : Exception
    {
        public DeadlockException(string msg) : base(msg)
        {
        }
    }
}