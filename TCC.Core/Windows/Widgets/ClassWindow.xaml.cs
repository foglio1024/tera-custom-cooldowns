﻿using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    /// <summary>
    /// Logica di interazione per WarriorLayout.xaml
    /// </summary>
    public partial class ClassWindow 
    {
        private ClassWindowViewModel VM { get; }

        public ClassWindow(ClassWindowViewModel vm)
        {
            DataContext = vm;
            VM = DataContext as ClassWindowViewModel;
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(App.Settings.ClassWindowSettings);
            App.Settings.ClassWindowSettings.EnabledChanged += OnEnabledChanged;

            if (!SessionManager.Logged) return;
            if (VM.CurrentManager == null)
                VM.CurrentClass = SessionManager.CurrentPlayer.Class;

        }

        private new void OnEnabledChanged() 
        {
            if (App.Settings.ClassWindowSettings.Enabled)
            {
                VM.CurrentClass = SessionManager.CurrentPlayer.Class;
            }
        }
    }
}
