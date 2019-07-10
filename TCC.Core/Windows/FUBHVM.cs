namespace TCC.Windows
{
    public class FUBHVM : TSPropertyChanged
    {

        public string CloseMessage => $"Click to {(!_dontshowagain ? "temporarily " : "")}close this window";
        private bool _dontshowagain;

        public bool DontShowAgain
        {
            get => _dontshowagain;
            set
            {
                if (_dontshowagain == value) return;
                _dontshowagain = value;
                N();
                N(nameof(CloseMessage));
                App.Settings.DontShowFUBH = value;
            }
        }

    }
}