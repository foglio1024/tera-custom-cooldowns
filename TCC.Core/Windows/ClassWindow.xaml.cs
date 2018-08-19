using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Windows
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
            MainContent = content;
            Init(SettingsManager.ClassWindowSettings, ignoreSize: true, undimOnFlyingGuardian:false);
            SettingsManager.ClassWindowSettings.EnabledChanged += OnEnabledChanged;

            if (!SessionManager.Logged) return;
            if (ClassWindowViewModel.Instance.CurrentManager == null)
                ClassWindowViewModel.Instance.CurrentClass = SessionManager.CurrentPlayer.Class;


        }



        private new void OnEnabledChanged() 
        {
            if (SettingsManager.ClassWindowSettings.Enabled)
                ClassWindowViewModel.Instance.CurrentClass = SessionManager.CurrentPlayer.Class;
            base.OnEnabledChanged();
        }

        
    }
}
