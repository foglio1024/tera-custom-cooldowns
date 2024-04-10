using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Nostrum.WPF;
using Nostrum.WPF.Extensions;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TeraDataLite;

namespace TCC.ViewModels;

internal class LaurelSelectionViewModel : ObservableObject
{
    private CustomLaurel _currentLaurel;

    public CustomLaurel CurrentLaurel
    {
        get => _currentLaurel;
        set => RaiseAndSetIfChanged(value, ref _currentLaurel);
    }

    public Class ExampleClass { get; }

    public ICommand PrevLaurelCommand { get; }
    public ICommand NextLaurelCommand { get; }
    public ICommand ConfirmCommand { get; }

    public readonly List<CustomLaurel> AvailableLaurels = Enum.GetValues<CustomLaurel>().ToList();

    private int _laurelIdx;

    public LaurelSelectionViewModel(Class cl, CustomLaurel selectedLaurel)
    {
        if (cl == Class.None) cl = Class.Warrior;

        ExampleClass = cl;
        _currentLaurel = selectedLaurel;

        _laurelIdx = AvailableLaurels.IndexOf(_currentLaurel);

        PrevLaurelCommand = new RelayCommand(PrevLaurel);
        NextLaurelCommand = new RelayCommand(NextLaurel);
        ConfirmCommand = new RelayCommand(Confirm);
    }

    private void Confirm()
    {
        var settings = (CharacterWindowSettings?)WindowManager.ViewModels.CharacterVM.Settings;
        if(settings != null) settings.CustomLaurel = CurrentLaurel;

        WindowManager.ViewModels.DashboardVM.SaveCharacters();
        var win = Application.Current.Windows.ToList().FirstOrDefault(x => x.DataContext == this);
        win?.Close();
    }

    private void PrevLaurel()
    {
        if (_laurelIdx == 0) _laurelIdx = AvailableLaurels.Count - 1;
        else _laurelIdx--;

        CurrentLaurel = AvailableLaurels[_laurelIdx];
    }

    private void NextLaurel()
    {
        if (_laurelIdx == AvailableLaurels.Count - 1) _laurelIdx = 0;
        else _laurelIdx++;

        CurrentLaurel = AvailableLaurels[_laurelIdx];
    }
}