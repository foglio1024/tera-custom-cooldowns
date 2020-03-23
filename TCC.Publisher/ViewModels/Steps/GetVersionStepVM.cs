namespace TCC.Publisher.ViewModels.Steps
{
    public class GetVersionStepVM : StepVM
    {
        private string _tccVersion;
        public string TccVersion
        {
            get => _tccVersion;
            set
            {
                if (_tccVersion == value) return;
                _tccVersion = value;
                N();
            }
        }

        private bool _printed;
        public bool Printed
        {
            get => _printed;
            set
            {
                if (_printed == value) return;
                _printed = value;
                N();
            }
        }

        public GetVersionStepVM(PublisherVM vm) : base(vm) { }

        protected override void Execute()
        {
            VM.Progress = 2 / 0.07;
            var ver = Publisher.GetVersion();
            TccVersion = $"TCC version is {ver}";
            Printed = true;
            Completed = true;
        }
        protected override bool CanExecute()
        {
            return !Completed;
        }
    }
}