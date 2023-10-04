namespace TCC.UI.Controls.Group;

public partial class PartyMember
{
    public PartyMember()
    {
        InitialAbnormalityDataTemplateSelector = R.TemplateSelectors.PartyAbnormalityTemplateSelector;
        InitializeComponent();
    }
}