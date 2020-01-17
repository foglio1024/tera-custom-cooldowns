using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace TCC.Publisher
{
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Publisher.Init();
        }
    }
}
