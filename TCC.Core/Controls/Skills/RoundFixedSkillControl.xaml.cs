using System.Windows;
using System.Windows.Input;

namespace TCC.Controls.Skills
{
    public partial class RoundFixedSkillControl 
    {
        public RoundFixedSkillControl()
        {
            InitializeComponent();
            MainArcRef = Arc;
            PreArcRef = PreArc;
            ResetArcRef = ResetArc;
            GlowRef = Glow;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            DeleteButton.Visibility = Visibility.Visible;
        }
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            DeleteButton.Visibility = Visibility.Collapsed;
        }
        private void DeleteButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowManager.ViewModels.Cooldowns.DeleteFixedSkill(Context);
        }
    }
}