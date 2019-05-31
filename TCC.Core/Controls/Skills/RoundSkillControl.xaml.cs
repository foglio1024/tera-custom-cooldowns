using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data;

namespace TCC.Controls.Skills
{
    public partial class RoundSkillControl
    {
        public RoundSkillControl()
        {
            InitializeComponent();
            MainArcRef = Arc;
            PreArcRef = PreArc;
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);
            if (Context == null) return;

            if (Context.Duration == 0)
            {
                OnCooldownEnded(Context.Mode);
            }
        }

        protected override void OnCooldownEnded(CooldownMode mode)
        {
            base.OnCooldownEnded(mode);
            if(mode == CooldownMode.Normal) WindowManager.CooldownWindow.VM.Remove(Context.Skill);
        }
        
        private void SkillIconControl_OnToolTipOpening(object sender, ToolTipEventArgs e)
        {
            FocusManager.PauseTopmost = true;
        }

        private void SkillIconControl_OnToolTipClosing(object sender, ToolTipEventArgs e)
        {
            FocusManager.PauseTopmost = false;
        }

        private void HideButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowManager.CooldownWindow.VM.AddHiddenSkill(Context);
            OnCooldownEnded(Context.Mode);
        }

        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            HideButton.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButton.Visibility = Visibility.Collapsed;
        }
    }
}

