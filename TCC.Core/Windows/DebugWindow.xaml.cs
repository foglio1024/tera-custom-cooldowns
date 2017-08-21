using System.Windows;

namespace TCC.Windows
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            PacketInspector.NewStat();
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            PacketInspector.Stop();
        }

        private void Dump(object sender, RoutedEventArgs e)
        {
            PacketInspector.Dump();
        }

        private void Gbam(object sender, RoutedEventArgs e)
        {
            TimeManager.Instance.UploadGuildBamTimestamp();
        }
    }
}
