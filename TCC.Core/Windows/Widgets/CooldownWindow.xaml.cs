namespace TCC.Windows.Widgets
{
    public partial class CooldownWindow 
    {
        public CooldownWindow()
        {
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(Settings.SettingsHolder.CooldownWindowSettings, ignoreSize: true, undimOnFlyingGuardian: false);

        }
    }
}

