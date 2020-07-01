using System.Windows;

namespace TCC.Publisher.ViewModels.Steps
{
    public class PushZipStepVM : StepVM
    {
        public PushZipStepVM(PublisherVM vm) : base(vm) { }

        protected override async void Execute()
        {
            var confirm = MessageBox.Show("Confirm zip push?", "TCC publisher", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes) return;

            VM.Progress = 7 / 0.07;
            await Publisher.Instance.Upload();
            Completed = true;
            VM.InvokeFinished();
        }

        protected override bool CanExecute()
        {
            return !Completed && VM.CreateReleaseStep.Completed;
        }
    }
}