using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data;
using TCC.R;
using TCC.Utils;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.UI.Windows;

//TODO: refactor and make multiple messageboxes
public partial class TccMessageBox
{
    static TccMessageBox _messageBox = null!;
    static MessageBoxResult _result = MessageBoxResult.No;
    public static bool IsOpen { get; set; }

    TccMessageBox()
    {
        InitializeComponent();
        Closing += OnClosing;
    }


    void OnClosing(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
        IsOpen = false;
    }

    static MessageBoxResult Show (string caption, string msg, MessageBoxType type)
    {
        return type switch
        {
            MessageBoxType.ConfirmationWithYesNo => Show(caption, msg, MessageBoxButton.YesNo, MessageBoxImage.Question),
            MessageBoxType.ConfirmationWithYesNoCancel => Show(caption, msg, MessageBoxButton.YesNoCancel, MessageBoxImage.Question),
            MessageBoxType.Information => Show(caption, msg, MessageBoxButton.OK, MessageBoxImage.Information),
            MessageBoxType.Error => Show(caption, msg, MessageBoxButton.OK, MessageBoxImage.Error),
            MessageBoxType.Warning => Show(caption, msg, MessageBoxButton.OK, MessageBoxImage.Warning),
            _ => MessageBoxResult.No
        };
    }
    public static MessageBoxResult Show(string msg, MessageBoxType type)
    {
        return Show("TCC", msg, type);
    }
    public static MessageBoxResult Show(string msg)
    {
        return Show(string.Empty, msg, MessageBoxButton.OK, MessageBoxImage.None);
    }
    public static MessageBoxResult Show (string caption, string text)
    {
        return Show(caption, text, MessageBoxButton.OK, MessageBoxImage.None);
    }
    public static MessageBoxResult Show(string caption, string text, MessageBoxButton button)
    {
        return Show(caption, text, button, MessageBoxImage.None);
    }
    public static MessageBoxResult Show (string caption, string text, MessageBoxButton button, MessageBoxImage image)
    {
        _messageBox.Dispatcher?.Invoke(() =>
        {
            _messageBox.TxtMsg.Text = text;
            _messageBox.MessageTitle.Text = caption;
            SetVisibilityOfButtons(button);
            SetImageOfMessageBox(image);
            IsOpen = true;
            _messageBox.ShowDialog();
        });
        return _result;
    }

    static void SetVisibilityOfButtons(MessageBoxButton button)
    {
        _messageBox.BtnCancel.Visibility = Visibility.Visible;
        _messageBox.BtnNo.Visibility = Visibility.Visible;
        _messageBox.BtnYes.Visibility = Visibility.Visible;
        _messageBox.BtnOk.Visibility = Visibility.Visible;

        switch (button)
        {
            case MessageBoxButton.OK:
                _messageBox.BtnCancel.Visibility = Visibility.Collapsed;
                _messageBox.BtnNo.Visibility = Visibility.Collapsed;
                _messageBox.BtnYes.Visibility = Visibility.Collapsed;
                _messageBox.BtnOk.Focus();
                break;
            case MessageBoxButton.OKCancel:
                _messageBox.BtnNo.Visibility = Visibility.Collapsed;
                _messageBox.BtnYes.Visibility = Visibility.Collapsed;
                _messageBox.BtnCancel.Focus();
                break;
            case MessageBoxButton.YesNo:
                _messageBox.BtnOk.Visibility = Visibility.Collapsed;
                _messageBox.BtnCancel.Visibility = Visibility.Collapsed;
                _messageBox.BtnNo.Focus();
                break;
            case MessageBoxButton.YesNoCancel:
                _messageBox.BtnOk.Visibility = Visibility.Collapsed;
                _messageBox.BtnCancel.Focus();
                break;
        }
    }

    static void SetImageOfMessageBox(MessageBoxImage image)
    {
        //return;
        switch (image)
        {
            case MessageBoxImage.Warning:
            case MessageBoxImage.Question:
                _messageBox.ColorRectFx.Fill = Brushes.TccYellowGradientBrush;
                _messageBox.ColorRect.Fill = Brushes.TccYellowGradientBrush;
                break;
            case MessageBoxImage.Error:
                _messageBox.ColorRect.Fill = Brushes.TccRedGradientBrush;
                _messageBox.ColorRectFx.Fill = Brushes.TccRedGradientBrush;
                break;
        }
    }

    void Button_Click(object sender, RoutedEventArgs e)
    {
        if (ReferenceEquals(sender, BtnOk))
            _result = MessageBoxResult.OK;
        else if (ReferenceEquals(sender, BtnYes))
            _result = MessageBoxResult.Yes;
        else if (ReferenceEquals(sender, BtnNo))
            _result = MessageBoxResult.No;
        else if (ReferenceEquals(sender, BtnCancel))
            _result = MessageBoxResult.Cancel;
        else
            _result = MessageBoxResult.None;
        BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(200)) { EasingFunction = new QuadraticEase() });
        Task.Delay(250).ContinueWith(_ =>
        {
            Dispatcher?.Invoke(() =>
            {
                _messageBox.Hide();
            });
        });
    }

    void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue != true) return;
        BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)) { EasingFunction = new QuadraticEase() });
        //((FrameworkElement)Content).RenderTransform.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(20, 0, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() });

        //RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(.5, 1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new ElasticEase() { Oscillations = 1 } });
        //RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(.5, 1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new ElasticEase() { Oscillations = 1 } });
    }

    void BG_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    public static void Create()
    {
        _messageBox = new TccMessageBox();

        Log.NewMessageBox += msg => Show(msg);

    }

    public static void CreateAsync()
    {
        var ssThread = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                Create();
                Dispatcher.Run();
            })
            { Name = "MessageBoxThread" };
        ssThread.SetApartmentState(ApartmentState.STA);
        ssThread.Start();
    }

}