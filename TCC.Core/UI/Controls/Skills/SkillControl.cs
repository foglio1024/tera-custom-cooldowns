using System.Windows;
using System.Windows.Input;

namespace TCC.UI.Controls.Skills;

public class SkillControl : SkillControlBase
{
    protected FrameworkElement? HideButtonRef;

    protected void HideButtonClicked(object sender, MouseButtonEventArgs e)
    {
        if (Context == null) return;
        WindowManager.ViewModels.CooldownsVM.AddHiddenSkill(Context);
        OnCooldownEnded(Context.Mode);
    }

    protected void ActivatorMouseEnter(object sender, MouseEventArgs e)
    {
        if (HideButtonRef == null) return;
        HideButtonRef.Visibility = Visibility.Visible;
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        if (HideButtonRef == null) return;
        HideButtonRef.Visibility = Visibility.Collapsed;
    }
}