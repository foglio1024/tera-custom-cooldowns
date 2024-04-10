using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using Nostrum.WPF.Controls;
using Nostrum.WPF.Factories;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.UI.Controls.Classes.Elements;

public partial class RagnarokBuffControl : INotifyPropertyChanged
{
    private bool _running;
    private SkillWithEffect? _context;
    private readonly DoubleAnimation _an;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string SecondsText => WindowManager.ViewModels.ClassVM.CurrentManager.StaminaTracker.Val.ToString();
    public bool Running
    {
        get => _running;
        set
        {
            if (_running == value) return;
            _running = value;
            if (_running)
            {
                SecondaryGrid.Opacity = 1;
                InternalArc.Opacity = 0;
            }
            else
            {
                SecondaryGrid.Opacity = 0;
                InternalArc.Opacity = 1;
            }
        }
    }

    public RagnarokBuffControl()
    {
        _an = AnimationFactory.CreateDoubleAnimation(from: 0, to: 359.9, ms: 1, completed: (_, _) => Running = false);
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DesignerProperties.GetIsInDesignMode(this)) return;
        _context = (SkillWithEffect)DataContext;
        _context.Effect.Started += OnRagnarokStarted;
        WindowManager.ViewModels.ClassVM.CurrentManager.StaminaTracker.PropertyChanged += ST_PropertyChanged;
    }

    private void OnRagnarokStarted(ulong duration, CooldownMode mode)
    {
        Running = true;
        _an.Duration = TimeSpan.FromMilliseconds(duration);
        ExternalArc.BeginAnimation(Arc.EndAngleProperty, _an);
    }


    private void ST_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != "Val") return;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SecondsText)));
        IconGlow.Opacity = WindowManager.ViewModels.ClassVM.CurrentManager.StaminaTracker.Factor == 1 ? 1 : 0;
        if (Running) return;
        var an = new DoubleAnimation((1 - WindowManager.ViewModels.ClassVM.CurrentManager.StaminaTracker.Factor) * 359.9, TimeSpan.FromMilliseconds(50));
        InternalArc.BeginAnimation(Arc.EndAngleProperty, an);
    }



}