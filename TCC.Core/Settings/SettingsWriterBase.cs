using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Settings
{
    public abstract class SettingsWriterBase
    {
        protected string FileName = "";

        public abstract void Save();
    }
}
