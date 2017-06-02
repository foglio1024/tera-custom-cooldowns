using System;
using System.IO;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Data;

namespace TCC.ViewModels
{
    internal class MysticBarManager : ClassManager
    {
        private static MysticBarManager _instance;
        public static MysticBarManager Instance => _instance ?? (_instance = new MysticBarManager());

        public AurasTracker Auras { get; private set; }

        public MysticBarManager()
        {
            _instance = this;
            CurrentClassManager = this;
            Auras = new AurasTracker();
        }
        protected override void LoadSpecialSkills()
        {
        }
    }
}