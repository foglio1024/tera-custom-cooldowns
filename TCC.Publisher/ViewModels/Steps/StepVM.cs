using System.Windows.Input;
using Nostrum;
using Nostrum.WPF;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Publisher.ViewModels.Steps
{
    public class StepVM : ThreadSafePropertyChanged
    {
        private bool _completed;
        public bool Completed
        {
            get => _completed;
            set
            {
                if (_completed == value) return;
                _completed = value;
                CommandManager.InvalidateRequerySuggested();
                N();
            }
        }

        protected PublisherVM VM { get; }

        public ICommand Command { get; }

        protected StepVM(PublisherVM vm)
        {
            VM = vm;
            Command = new RelayCommand(_ => Execute(), _ => CanExecute());

        }

        protected virtual void Execute() { }
        protected virtual bool CanExecute() => true;
    }
}