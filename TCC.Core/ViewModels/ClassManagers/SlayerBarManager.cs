using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TCC.Data;

namespace TCC.ViewModels
{
    public class SlayerBarManager : ClassManager
    {
        private static SlayerBarManager _instance;
        public static SlayerBarManager Instance => _instance ?? (_instance = new SlayerBarManager());

        public SlayerBarManager() : base()
        {
            _instance = this;
            CurrentClassManager = this;
        }

        protected override void LoadSpecialSkills()
        {
        }
    }
}
