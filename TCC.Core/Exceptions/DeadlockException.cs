using System;
using System.Collections.Generic;

namespace TCC.Exceptions
{
    public class DeadlockException : Exception
    {

        public DeadlockException(string msg, List<string> threadNames) : base(msg)
        {
            ThreadNames = threadNames;
        }

        public List<string> ThreadNames { get; }
    }
}