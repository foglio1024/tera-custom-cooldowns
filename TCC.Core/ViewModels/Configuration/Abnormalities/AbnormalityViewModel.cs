using System.Windows.Input;
using Nostrum.WPF;
using TCC.Data;
using TCC.Data.Abnormalities;

namespace TCC.ViewModels.Configuration.Abnormalities;

public class AbnormalityViewModel : ObservableObject
{
    private bool _isFavorite;

    public bool IsFavorite
    {
        get => _isFavorite;
        set => RaiseAndSetIfChanged(value, ref _isFavorite);
    }

    public Abnormality Abnormality { get; }
    public AbnormalityConfigurator SelfConfigurator { get; }
    public AbnormalityConfigurator GroupConfigurator { get; }
    public ICommand MarkAsFavoriteCommand { get; }

    public bool CanMarkAsFavorite => Abnormality.Type is AbnormalityType.Special or AbnormalityType.Buff && Abnormality.IsBuff;

    public AbnormalityViewModel(Abnormality abnormality)
    {
        Abnormality = abnormality;
        IsFavorite = abnormality.Type is AbnormalityType.Special;

        SelfConfigurator = new AbnormalityConfigurator(abnormality, App.Settings.AbnormalitySettings.Self);
        GroupConfigurator = new AbnormalityConfigurator(abnormality, App.Settings.AbnormalitySettings.Group);

        MarkAsFavoriteCommand = new RelayCommand(MarkAsFavorite, _ => CanMarkAsFavorite);
    }

    private void MarkAsFavorite()
    {
        if (!Abnormality.IsBuff) return;

        IsFavorite = !IsFavorite;
        if (IsFavorite)
        {
            App.Settings.AbnormalitySettings.Favorites.Add(Abnormality.Id);
            Abnormality.Type = AbnormalityType.Special;
        }
        else
        {
            App.Settings.AbnormalitySettings.Favorites.Remove(Abnormality.Id);
            Abnormality.Type = AbnormalityType.Buff;
        }
        InvokePropertyChanged(nameof(Abnormality));
    }
}