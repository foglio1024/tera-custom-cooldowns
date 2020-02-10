using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using TCC.Settings;
using TCC.Settings.WindowSettings;
using TCC.Utils;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    public class TccWidgetBuilder<TWindow, TViewModel> where TWindow : TccWidget where TViewModel : TccWindowViewModel
    {
        private TWindow _window;
        private TViewModel _vm;

        public async Task<TWindow> GetWindow()
        {
            return await Task.Factory.StartNew(() =>
            {
                while (_window == null) Thread.Sleep(100);
                return _window;
            });
        }
        public async Task<TViewModel> GetViewModel()
        {
            return await Task.Factory.StartNew(() =>
            {
                while (_vm == null) Thread.Sleep(100);
                return _vm;
            });
        }

        public TccWidgetBuilder(WindowSettingsBase ws)
        {
            Create(ws);
        }

        private void Create(WindowSettingsBase ws)
        {
            var thread = new Thread(() =>
        {
            SynchronizationContext.SetSynchronizationContext(
                new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            _vm = (TViewModel) Activator.CreateInstance(typeof(TViewModel), ws);
            _window = (TWindow)Activator.CreateInstance(typeof(TWindow), _vm);
            if (_vm.Settings.Enabled) _window.Show();
            App.AddDispatcher(Thread.CurrentThread.ManagedThreadId, Dispatcher.CurrentDispatcher);
            Dispatcher.Run();
            Log.CW($"[{typeof(TWindow).Name}] Dispatcher stopped.");
            App.RemoveDispatcher(Thread.CurrentThread.ManagedThreadId);
        })
            {
                Name = $"{typeof(TWindow).Name}Thread"
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}