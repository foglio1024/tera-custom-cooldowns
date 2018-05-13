namespace TCC.ViewModels
{
    public class TccWindowViewModel : TSPropertyChanged
    {
        protected double _scale;
        public double Scale
        {
            get => _scale;
            set
            {
                if(_scale == value)return;
                _scale = value;
                NPC(nameof(Scale));
            }
        }
    }
}
