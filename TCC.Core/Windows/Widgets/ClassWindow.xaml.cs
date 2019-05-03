using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    /// <summary>
    /// Logica di interazione per WarriorLayout.xaml
    /// </summary>
    public partial class ClassWindow 
    {
        public ClassWindowViewModel VM { get; }

        public ClassWindow()
        {
            DataContext = new ClassWindowViewModel();
            VM = DataContext as ClassWindowViewModel;
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(Settings.SettingsHolder.ClassWindowSettings, ignoreSize: true, undimOnFlyingGuardian:false);
            Settings.SettingsHolder.ClassWindowSettings.EnabledChanged += OnEnabledChanged;

            if (!SessionManager.Logged) return;
            if (WindowManager.ClassWindow.VM.CurrentManager == null)
                WindowManager.ClassWindow.VM.CurrentClass = SessionManager.CurrentPlayer.Class;


        }



        private new void OnEnabledChanged() 
        {
            if (Settings.SettingsHolder.ClassWindowSettings.Enabled)
            {
                WindowManager.ClassWindow.VM.CurrentClass = SessionManager.CurrentPlayer.Class;
            }
            //base.OnEnabledChanged();
        }

        
    }
}
