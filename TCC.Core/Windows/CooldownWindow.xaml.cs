namespace TCC.Windows
{
    public partial class CooldownWindow 
    {
        public CooldownWindow()
        {
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = content;
            Init(Settings.CooldownWindowSettings, ignoreSize: true, undimOnFlyingGuardian: false);

        }


        //public void SwitchMode()
        //{
        //    Dispatcher.InvokeIfRequired(() =>
        //    {
        //        if (Settings.CooldownBarMode == CooldownBarMode.Fixed)
        //        {
        //            controlContainer.Content = new FixedSkillContainers();
        //        }
        //        else
        //        {
        //            controlContainer.Content = new NormalSkillContainer();
        //        }

        //        ((FrameworkElement)controlContainer.Content).DataContext = CooldownWindowViewModel.Instance;

        //    }, DispatcherPriority.Normal);
        //}

    }
}

/*
* C_START_SKILL            0xCBC5
* S_START_COOLTIME_SKILL   0x7BA8
* C_USE_ITEM               0x750F
* S_START_COOLTIME_ITEM    0xAD63
*/
