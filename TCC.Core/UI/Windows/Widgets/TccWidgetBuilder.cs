using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TCC.Settings.WindowSettings;
using TCC.ViewModels;

namespace TCC.UI.Windows.Widgets;

public class WindowBuilderBase<TWindow, TViewModel> where TWindow : Window
    where TViewModel : TccWindowViewModel
{
    protected TWindow? _window;
    protected TViewModel? _vm;

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
}

//public class TccWindowBuildier<TWindow, TViewModel> : WindowBuilderBase<TWindow, TViewModel>
//where TWindow : TccWindow
//where TViewModel : TccWindowViewModel
//{
//    public TccWindowBuildier(bool canClose)
//    {
//        Create(canClose);
//    }

//    private void Create(bool canClose)
//    {
//        var thread = new Thread(() =>
//        {
//            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
//            _vm = (TViewModel)Activator.CreateInstance(typeof(TViewModel), canClose);
//            _window = (TWindow)Activator.CreateInstance(typeof(TWindow), _vm);
//            if (_vm.Settings.Enabled) _window.Show();
//            App.AddDispatcher(Thread.CurrentThread.ManagedThreadId, Dispatcher.CurrentDispatcher);
//            Dispatcher.Run();
//            Log.CW($"[{typeof(TWindow).Name}] Dispatcher stopped.");
//            App.RemoveDispatcher(Thread.CurrentThread.ManagedThreadId);
//        })
//        {
//            Name = $"{typeof(TWindow).Name}Thread"
//        };
//        thread.SetApartmentState(ApartmentState.STA);
//        thread.Start();
//    }

//}

public class TccWidgetBuilder<TWindow, TViewModel> : WindowBuilderBase<TWindow, TViewModel>
    where TWindow : Window
    where TViewModel : TccWindowViewModel
{
    public TccWidgetBuilder(WindowSettingsBase? ws)
    {
        Create(ws);
    }

    private void Create(WindowSettingsBase? ws)
    {
        var thread = new Thread(() =>
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            _vm = (TViewModel?)Activator.CreateInstance(typeof(TViewModel), ws);
            _window = (TWindow?)Activator.CreateInstance(typeof(TWindow), _vm);
            if (ws is { Enabled: true })
            {
                _window?.Show();
            }
            App.AddDispatcher(Thread.CurrentThread.ManagedThreadId, Dispatcher.CurrentDispatcher);
            Dispatcher.Run();
            //Log.CW($"[{typeof(TWindow).Name}] Dispatcher stopped.");
            App.RemoveDispatcher(Thread.CurrentThread.ManagedThreadId);
        })
        {
            Name = $"{typeof(TWindow).Name}Thread"
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
    }
}