using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Data
{
    public class EnumVM<T>
    {
        public T Value { get; private set; }
        public string Name { get; private set; }

        public EnumVM(T val, string name)
        {
            Value = val;
            Name = name;
        }
    }


}
