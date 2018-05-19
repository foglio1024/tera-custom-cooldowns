using System.Windows.Input;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per AbnormalitiesWindow.xaml
    /// </summary>
    public partial class BuffWindow
    {
        public BuffWindow()
        {
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = content;
            Init(SettingsManager.BuffWindowSettings, ignoreSize: true);
        }
    }
}
