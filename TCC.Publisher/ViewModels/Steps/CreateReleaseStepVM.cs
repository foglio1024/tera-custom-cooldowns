using System.Windows;

namespace TCC.Publisher.ViewModels.Steps
{
    public class CreateReleaseStepVM : StepVM
    {
        private string _releaseCreatedLabel = "";
        public string ReleaseCreatedLabel
        {
            get => _releaseCreatedLabel;
            set
            {
                if (_releaseCreatedLabel == value) return;
                _releaseCreatedLabel = value;
                N();
            }
        }

        protected override async void Execute()
        {
            VM.Progress = 5 / 0.07;

            //var confirm = MessageBox.Show("Confirm release creation?", "TCC publisher", MessageBoxButton.YesNo);
            //if (confirm != MessageBoxResult.Yes) return;
            //if (string.IsNullOrWhiteSpace(VM.ReleaseNotes))
            //{
            //    var emptyNotesConf = MessageBox.Show("Changelog field is empty, continue anyway?",
            //        "TCC publisher", MessageBoxButton.YesNo);
            //    if (emptyNotesConf != MessageBoxResult.Yes) return;
            //}

            //await Publisher.Instance.CreateRelease(VM.ReleaseNotes);

            ReleaseCreatedLabel = "Release skipped";
            VM.Progress = 6 / 0.07;
            Completed = true;
        }

        protected override bool CanExecute()
        {
            return !Completed && VM.GenerateStep.Completed;
        }

        public CreateReleaseStepVM(PublisherVM vm) : base(vm) { }
    }
}