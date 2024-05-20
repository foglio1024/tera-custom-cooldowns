using System.Windows;

namespace TCC.UI.Controls.Abnormalities;

public partial class AbnormalityToolTipControl
{

    public string AbnormalityName
    {
        get => (string)GetValue(AbnormalityNameProperty);
        set => SetValue(AbnormalityNameProperty, value);
    }
    public static readonly DependencyProperty AbnormalityNameProperty = DependencyProperty.Register(nameof(AbnormalityName), typeof(string), typeof(AbnormalityToolTipControl), new PropertyMetadata("Abnormality Name"));

    public string AbnormalityToolTip
    {
        get => (string)GetValue(AbnormalityToolTipProperty);
        set => SetValue(AbnormalityToolTipProperty, value);
    }
    public static readonly DependencyProperty AbnormalityToolTipProperty = DependencyProperty.Register(nameof(AbnormalityToolTip), typeof(string), typeof(AbnormalityToolTipControl), new PropertyMetadata("Abnormality tooltip."));

    public uint Id
    {
        get => (uint)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }
    public static readonly DependencyProperty IdProperty = DependencyProperty.Register(nameof(Id), typeof(uint), typeof(AbnormalityToolTipControl), new PropertyMetadata(0U));

    public AbnormalityToolTipControl()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded -= OnLoaded;
    }

    private void ParseToolTip(string t)
    {
        foreach (var inline in new TooltipParser(t).Parse()) 
            ToolTipTb.Inlines.Add(inline);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ToolTipTb.Text = "";
        ToolTipTb.FontSize = 11;
        try
        {
            ParseToolTip(AbnormalityToolTip);
            if (ToolTipTb.Text != "") return;
            ToolTipTb.Text = Id.ToString();
            System.Diagnostics.Debug.WriteLine("Unknown abnoramlity: {0}", Id.ToString());
        }
        catch
        {
            // ignored
        }
    }
}
