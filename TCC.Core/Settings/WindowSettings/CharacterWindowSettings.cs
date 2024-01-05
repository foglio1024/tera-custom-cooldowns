using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Input;
using Nostrum.WPF;
using Nostrum.WPF.Extensions;
using TCC.Data;
using TCC.UI.Windows;
using TCC.UI.Windows.Widgets;
using TCC.ViewModels;
using TeraDataLite;

namespace TCC.Settings.WindowSettings;

public class CharacterWindowSettings : WindowSettingsBase
{
    public event Action? SorcererShowElementsChanged;
    public event Action? WarriorShowEdgeChanged;
    public event Action? ShowStaminaChanged;
    public event Action? CustomLaurelChanged;

    bool _sorcererShowElements;
    bool _warriorShowEdge;
    bool _compactMode;
    bool _showStamina;
    CustomLaurel _customLaurel;

    public bool CompactMode
    {
        get => _compactMode;
        set => RaiseAndSetIfChanged(value, ref _compactMode);
    }
    public bool SorcererShowElements
    {
        get => _sorcererShowElements;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _sorcererShowElements)) return;
            SorcererShowElementsChanged?.Invoke();
            InvokePropertyChanged();
        }
    }
    public bool WarriorShowEdge
    {
        get => _warriorShowEdge;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _warriorShowEdge)) return;
            WarriorShowEdgeChanged?.Invoke();
            InvokePropertyChanged();
        }
    }
    public bool ShowStamina
    {
        get => _showStamina;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _showStamina)) return;
            ShowStaminaChanged?.Invoke();
        }
    }
    public CustomLaurel CustomLaurel
    {
        get => _customLaurel;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _customLaurel)) return;
            CustomLaurelChanged?.Invoke();
        }
    }

    [JsonIgnore] public ICommand ChooseCustomLaurelCommand { get; }

    public CharacterWindowSettings()
    {
        _visible = true;
        _clickThruMode = ClickThruMode.Never;
        _scale = 1;
        _autoDim = true;
        _dimOpacity = .5;
        _showAlways = false;
        _enabled = true;
        _allowOffScreen = false;
        Positions = new ClassPositions(.4, 1, ButtonsPosition.Above);

        _customLaurel = CustomLaurel.Game;
        CompactMode = true;
        UndimOnFlyingGuardian = false;
        GpkNames.Add("CharacterWindow");
        ChooseCustomLaurelCommand = new RelayCommand(ChooseCustomLaurel);

    }

    void ChooseCustomLaurel()
    {
        var w = Application.Current.Windows.ToList().OfType<LaurelSelectionWindow>().FirstOrDefault();

        if (w != null)
        {
            w.Focus();
            return;
        }
        var vm = new LaurelSelectionViewModel(Game.Me.Class, CustomLaurel);
        w = new LaurelSelectionWindow { DataContext = vm };
        w.Show();
    }
}