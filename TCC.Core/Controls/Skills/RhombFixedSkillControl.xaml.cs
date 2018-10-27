using System.Windows;
using System.Windows.Input;
using TCC.ViewModels;

namespace TCC.Controls.Skills
{
    /// <summary>
    ///     Logica di interazione per FixedSkillControl.xaml
    /// </summary>
    public partial class RhombFixedSkillControl 
    {
        public RhombFixedSkillControl()
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
            CooldownWindowViewModel.Instance.DeleteFixedSkill(Context);
        }
    }
}