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
    public class BrawlerBarManager : ClassManager
    {
        private static BrawlerBarManager _instance;
        public static BrawlerBarManager Instance => _instance ?? (_instance = new BrawlerBarManager());
        public BrawlerBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
        }

        protected override void LoadSpecialSkills()
        {
            
        }
    }
}
