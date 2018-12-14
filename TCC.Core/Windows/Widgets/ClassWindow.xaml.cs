using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    /// <summary>
    /// Logica di interazione per WarriorBar.xaml
    /// </summary>
    public partial class ClassWindow 
    {
        public ClassWindow()
        {
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(Settings.SettingsStorage.ClassWindowSettings, ignoreSize: true, undimOnFlyingGuardian:false);
            Settings.SettingsStorage.ClassWindowSettings.EnabledChanged += OnEnabledChanged;

            if (!SessionManager.Logged) return;
            if (ClassWindowViewModel.Instance.CurrentManager == null)
                ClassWindowViewModel.Instance.CurrentClass = SessionManager.CurrentPlayer.Class;


        }



        private new void OnEnabledChanged() 
        {
            if (Settings.SettingsStorage.ClassWindowSettings.Enabled)
                ClassWindowViewModel.Instance.CurrentClass = SessionManager.CurrentPlayer.Class;
            base.OnEnabledChanged();
        }

        
    }
}
