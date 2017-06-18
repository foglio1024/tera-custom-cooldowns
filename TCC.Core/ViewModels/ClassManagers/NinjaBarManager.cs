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
    public class NinjaBarManager : ClassManager
    {
        private static NinjaBarManager _instance;
        public static NinjaBarManager Instance => _instance ?? (_instance = new NinjaBarManager());

        public NinjaBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            LoadSpecialSkills();
        }
        protected override void LoadSpecialSkills()
        {

        }
    }
}
