namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per SquareAbnormalityIndicator.xaml
    /// </summary>
    public partial class SquareAbnormalityIndicator : AbnormalityIndicatorBase
    {
        public SquareAbnormalityIndicator()
        {
            InitializeComponent();
            _durationLabel = DurationLabel;
            _mainArc = MainArc;
        }
    }
}
