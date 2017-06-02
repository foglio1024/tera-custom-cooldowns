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
    class SorcererBarManager : ClassManager
    {
        private static SorcererBarManager _instance;
        public static SorcererBarManager Instance => _instance ?? (_instance = new SorcererBarManager());

        public SorcererBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
        }
        protected override void LoadSpecialSkills()
        {

        }
    }
}
