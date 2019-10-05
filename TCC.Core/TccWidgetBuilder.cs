using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using TCC.Settings;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC
{
    public class TccWidgetBuilder<TWindow, TViewModel> where TWindow : TccWidget where TViewModel : TccWindowViewModel
    {
        private TWindow _window;
        private TViewModel _vm;

        public async Task<TWindow> GetWindow()
        {
            while (_window == null) { Thread.Sleep(100); }
            return _window;
        }
        public async Task<TViewModel> GetViewModel()
        {
            while (_vm == null) { Thread.Sleep(100); }
            return _vm;
        }

        public TccWidgetBuilder(WindowSettings ws)
        {
            Create(ws);
        }

        private void Create(WindowSettings ws)
        {
            var thread = new Thread(() =>
        {
            SynchronizationContext.SetSynchronizationContext(
                new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            _vm = (TViewModel) Activator.CreateInstance(typeof(TViewModel), ws);
            _window = (TWindow)Activator.CreateInstance(typeof(TWindow), _vm);
            if (_vm.Settings.Enabled) _window.Show();
            WindowManager.AddDispatcher(Thread.CurrentThread.ManagedThreadId, Dispatcher.CurrentDispatcher);
            Dispatcher.Run();
            WindowManager.RemoveDispatcher(Thread.CurrentThread.ManagedThreadId);
        })
            {
                Name = $"{typeof(TWindow).Name}Thread"
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}