using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.ViewModels
{
    public class AbnormalityViewModel : BaseINPC
    {
        AbnormalityDuration _ab;



        public AbnormalityViewModel(AbnormalityDuration ab)
        {
            _ab = ab;
        }
    }
}
