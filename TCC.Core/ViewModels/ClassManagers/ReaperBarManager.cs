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
    public class ReaperBarManager : ClassManager
    {
        private static ReaperBarManager _instance;
        public static ReaperBarManager Instance => _instance ?? (_instance = new ReaperBarManager());

        public ReaperBarManager() : base()
        {
            _instance = this;
            CurrentClassManager = this;
        }
        protected override void LoadSpecialSkills()
        {
        }
    }
}
