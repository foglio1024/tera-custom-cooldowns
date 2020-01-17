using System.Diagnostics;

namespace TCC.Publisher.ViewModels.Steps
{
    public class GenerateStepVM : StepVM
    {
        private bool _running;
        public bool Running
        {
            get => _running;
            set
            {
                if (_running == value) return;
                _running = value;
                N();
            }
        }

        private string _versionCheckLabel;
        public string VersionCheckLabel
        {
            get => _versionCheckLabel;
            set
            {
                if (_versionCheckLabel == value) return;
                _versionCheckLabel = value;
                N();
            }
        }

        private bool _compressionDone;
        public bool CompressionDone
        {
            get => _compressionDone;
            set
            {
                if (_compressionDone == value) return;
                _compressionDone = value;
                N();
            }
        }

        private bool _versionCheckUpdateDone;
        public bool VersionCheckUpdateDone
        {
            get => _versionCheckUpdateDone;
            set
            {
                if (_versionCheckUpdateDone == value) return;
                _versionCheckUpdateDone = value;
                N();
            }
        }

        private string _compressionLabel;
        public string CompressionLabel
        {
            get => _compressionLabel;
            set
            {
                if (_compressionLabel == value) return;
                _compressionLabel = value;
                N();
            }
        }

        public GenerateStepVM(PublisherVM vm) : base(vm) { }

        protected override async void Execute()
        {
            Running = true;

            VM.Progress = 3 / 0.07;

            Logger.WriteLine("    Compressing release...");
            var sw = new Stopwatch();
            sw.Start();
            await Publisher.CompressRelease();
            sw.Stop();
            Logger.WriteLine("    Release compressed.");
            Logger.WriteLine("-------------");

            CompressionLabel = $"Compression done in {sw.Elapsed}";
            CompressionDone = true;
            VM.Progress = 3.8 / 0.07;
            Logger.WriteLine("    Updating version check file...");
            Publisher.UpdateVersionCheckFile();
            Logger.WriteLine("    Version check file updated.");
            Logger.WriteLine("-------------");

            VersionCheckLabel = "Version check file updated";
            VersionCheckUpdateDone = true;
            Running = false;
            VM.Progress = 4.2 / 0.07;

            Completed = true;
        }
        protected override bool CanExecute()
        {
            return !Completed && VM.GetVersionStep.Completed;
        }

    }
}