using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.ViewModels
{
    public class TccWindowViewModel : TSPropertyChanged
    {
        protected double _scale;
        public double Scale
        {
            get => _scale;
            set
            {
                if(_scale == value)return;
                _scale = value;
                NPC(nameof(Scale));
            }
        }
    }
}
