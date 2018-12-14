namespace TCC.Windows.Widgets
{
    public partial class CooldownWindow 
    {
        public CooldownWindow()
        {
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(Settings.SettingsStorage.CooldownWindowSettings, ignoreSize: true, undimOnFlyingGuardian: false);

        }
    }
}

