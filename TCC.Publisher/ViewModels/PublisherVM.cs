using System;
using Nostrum;
using Nostrum.WPF.ThreadSafe;
using TCC.Publisher.ViewModels.Steps;

namespace TCC.Publisher.ViewModels
{
    public class PublisherVM : ThreadSafePropertyChanged
    {
        private double _progress;
        private bool _completed;
        private string _releaseNotes = "";

        public event Action? ProgressChanged;
        public event Action? Finished;

        public double Progress
        {
            get => _progress;
            set
            {
                if (_progress == value) return;
                _progress = value;
                ProgressChanged?.Invoke();
            }
        }
        public bool Completed
        {
            get => _completed;
            set
            {
                if (_completed == value) return;
                _completed = value;
                N();
            }
        }
        public string ReleaseNotes
        {
            get => _releaseNotes;
            set
            {
                if(_releaseNotes == value) return;
                _releaseNotes = value;
                N();
            }
        }

        public ThreadSafeObservableCollection<string> LogData { get; }

        public GetVersionStepVM GetVersionStep { get; }
        public GenerateStepVM GenerateStep { get; }
        public CreateReleaseStepVM CreateReleaseStep { get; }
        public PushZipStepVM PushZipStep { get; }

        public PublisherVM()
        {
            GetVersionStep = new GetVersionStepVM(this);
            GenerateStep = new GenerateStepVM(this);
            CreateReleaseStep = new CreateReleaseStepVM(this);
            PushZipStep = new PushZipStepVM(this);

            LogData = new ThreadSafeObservableCollection<string>();

            Logger.NewLine += OnLogger_NewLine;
            Logger.AppendedLine += OnLogger_AppendedLine;
        }

        private void OnLogger_AppendedLine(string msg)
        {
            msg = msg.Replace("\n", "");
            if (LogData.Count == 0) OnLogger_NewLine(msg);
            else LogData[LogData.Count - 1] += msg;
        }

        private void OnLogger_NewLine(string line)
        {
            line = line.Replace("\n", "");
            LogData.Add($"[{DateTime.Now.ToShortTimeString()}] {line}");
        }

        public void InvokeFinished()
        {
            Finished?.Invoke();
            Completed = true;
        }
    }
}