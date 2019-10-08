using System;

namespace TCC.Utils
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TccModuleAttribute : Attribute
    {
        public bool Enabled { get; set; }

        public TccModuleAttribute(bool enabled = true)
        {
            this.Enabled = enabled;
        }
    }
}