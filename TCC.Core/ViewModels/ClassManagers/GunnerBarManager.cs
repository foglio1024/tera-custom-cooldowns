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
    public class GunnerBarManager : ClassManager
    {
        private static GunnerBarManager _instance;
        public static GunnerBarManager Instance => _instance ?? (_instance = new GunnerBarManager());

        public GunnerBarManager() : base()
        {
            _instance = this;
            CurrentClassManager = this;
        }
        protected override void LoadSpecialSkills()
        {


        }
    }
}
