using System.Windows;

namespace TCC.Publisher;

public partial class App
{
    void Application_Startup(object sender, StartupEventArgs e)
    {
        Publisher.Init();
    }
}