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
    public class BerserkerBarManager : ClassManager
    {
        private static BerserkerBarManager _instance;
        public static BerserkerBarManager Instance => _instance ?? (_instance = new BerserkerBarManager());

        public BerserkerBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
        }

        protected override void LoadSpecialSkills()
        {
        }
    }
}
