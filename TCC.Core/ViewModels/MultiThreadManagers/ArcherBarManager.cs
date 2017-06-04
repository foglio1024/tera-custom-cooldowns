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
    public class ArcherBarManager : ClassManager
    {
        private static ArcherBarManager _instance;
        public static ArcherBarManager Instance => _instance ?? (_instance = new ArcherBarManager());

        public ArcherFocusTracker Focus { get; set; }
        public StanceTracker<ArcherStance> Stance { get; set; }
        public ArcherBarManager()
        {
            _instance = this;
            Focus = new ArcherFocusTracker();
            Stance = new StanceTracker<ArcherStance>();
            CurrentClassManager = this;
        }

        protected override void LoadSpecialSkills()
        {


        }
    }
}
