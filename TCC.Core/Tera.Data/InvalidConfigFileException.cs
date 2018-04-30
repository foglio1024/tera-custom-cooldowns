using System;

namespace TCC.Tera.Data
{
    public class InvalidConfigFileException : Exception
    {
        public InvalidConfigFileException()
        {
        }

        public InvalidConfigFileException(string message)
            : base(message)
        {
        }

        public InvalidConfigFileException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}