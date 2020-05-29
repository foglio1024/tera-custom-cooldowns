using System.Windows;
using System.Windows.Input;

namespace TCC.UI.Controls.Skills
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
            DeleteButtonRef = DeleteButton;
        }
    }
}