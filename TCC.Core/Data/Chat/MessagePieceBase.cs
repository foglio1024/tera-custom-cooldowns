using Nostrum.WPF;
using Nostrum.WPF.Extensions;
using Nostrum.WPF.ThreadSafe;
using System;
using System.Windows.Input;
using TCC.Debugging;
using TCC.Utilities;
using TCC.Utils;
using TCC.ViewModels;

namespace TCC.Data.Chat;

public class MessagePieceBase : ThreadSafeObservableObject, IDisposable
{
    protected bool _customSize;
    protected int _fontSize = 18;
    bool _isVisible;
    ChatMessage? _container;

    public string Text { get; set; } = "";
    public int Size
    {
        get => _customSize ? _fontSize : App.Settings.FontSize;
        init
        {
            _fontSize = value;
            _customSize = value != App.Settings.FontSize;
        }
    }
    public string Color { get; set; } = "";
    public virtual bool IsHovered { get; set; }
    public virtual ICommand ClickCommand { get; }
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            /// todo: can we do this?
            //if (!RaiseAndSetIfChanged(value, ref _isVisible)) return;

            if (value)
            {
                SettingsWindowViewModel.FontSizeChanged += OnFontSizeChanged;
            }
            else
            {
                SettingsWindowViewModel.FontSizeChanged -= OnFontSizeChanged;
            }

            if (_isVisible == value) return;
            _isVisible = value;
            N();
        }
    }
    public ChatMessage? Container
    {
        protected get => _container;
        set
        {
            if (_container == value) return;
            _container = value;
            if (string.IsNullOrEmpty(Color))
            {
                Color = TccUtils.ChatChannelToBrush(Container?.Channel ?? ChatChannel.Say).Color.ToHex();
            }
        }
    }

    protected MessagePieceBase()
    {
        ObjectTracker.Register(GetType());
        Dispatcher = ChatManager.Instance.Dispatcher;
        ClickCommand = new RelayCommand(_ => { });
    }

    ~MessagePieceBase()
    {
        ObjectTracker.Unregister(GetType());
    }

    void OnFontSizeChanged()
    {
        InvokePropertyChanged(nameof(Size));
    }

    public void Dispose()
    {
        _container = null;
        SettingsWindowViewModel.FontSizeChanged -= OnFontSizeChanged;
    }
}
