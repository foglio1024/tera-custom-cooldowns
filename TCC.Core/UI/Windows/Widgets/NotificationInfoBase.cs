using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Nostrum.WPF;
using Nostrum.WPF.ThreadSafe;
using TCC.Utils;

namespace TCC.UI.Windows.Widgets;

public class ProgressNotificationInfo : NotificationInfoBase
{
    private double _progress;
    public double Progress
    {
        get => _progress;
        set => RaiseAndSetIfChanged(value, ref _progress);
    }

    public ProgressNotificationInfo(int id, string title, string message, NotificationType type, int duration, NotificationTemplate template) : base(id, title, message, type, duration, template)
    {
        CanClose = false;
    }

    public void Dispose(int msDelay)
    {
        InvokeDisposing(msDelay);
        if (msDelay == 0)
        {
            InvokeDisposed();
        }
        else
        {
            CanClose = true;
            Task.Delay(msDelay).ContinueWith(_ => InvokeDisposed());
        }
    }
}
public class NotificationInfoBase : ThreadSafeObservableObject
{
    public event Action? Disposed;
    public event Action<int>? Disposing;

    private string _message = "";
    private NotificationType _notificationType;

    public int Id { get; }
    public string Title { get; }
    public string Message
    {
        get => _message;
        set => RaiseAndSetIfChanged(value, ref _message);

    }
    public NotificationType NotificationType
    {
        get => _notificationType;
        set => RaiseAndSetIfChanged(value, ref _notificationType);
    }

    private bool _canClose;

    public bool CanClose
    {
        get => _canClose;
        set => RaiseAndSetIfChanged(value, ref _canClose);
    }

    public NotificationTemplate NotificationTemplate { get; }
    public int Duration { get; }
    public string TimeStamp { get; }
    public string Version => App.AppVersion;

    public ICommand DismissCommand { get; }

    public NotificationInfoBase(int id, string title, string message, NotificationType type, int duration, NotificationTemplate template)
    {
        Id = id;
        Title = title;
        Message = message;
        _notificationType = type;
        Duration = duration;
        TimeStamp = DateTime.Now.ToString("HH:mm:ss");
        NotificationTemplate = template;
        CanClose = true;

        DismissCommand = new RelayCommand(_ => InvokeDisposed());
    }

    protected void InvokeDisposing(int delay)
    {
        Disposing?.Invoke(delay);
    }

    protected void InvokeDisposed()
    {
        Disposed?.Invoke();
    }
}