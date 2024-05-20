using System.Windows.Input;
using Nostrum.WPF;
using TCC.Data.Abnormalities;
using TCC.Settings;
using TeraDataLite;

namespace TCC.ViewModels.Configuration.Abnormalities;

public class AbnormalityWhitelistToggle : ObservableObject
{
    private readonly AbnormalitySettings _settings;
    private readonly Abnormality _abnormality;

    public Class Class { get; }

    private bool _isWhitelisted;

    public bool IsWhitelisted
    {
        get => _isWhitelisted;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _isWhitelisted)) return;

            if (value)
            {
                if (_settings.Whitelist[Class].Contains(_abnormality.Id)) return;

                _settings.Whitelist[Class].Add(_abnormality.Id);
            }
            else
            {
                _settings.Whitelist[Class].Remove(_abnormality.Id);
            }
        }
    }

    public ICommand ToggleWhitelistedCommand { get; }

    public AbnormalityWhitelistToggle(Abnormality abnormality, Class cl, AbnormalitySettings settings)
    {
        ToggleWhitelistedCommand = new RelayCommand(ToggleWhitelisted);

        Class = cl;
        _abnormality = abnormality;
        _settings = settings;
        _isWhitelisted = settings.Whitelist[Class].Contains(abnormality.Id);
    }

    private void ToggleWhitelisted()
    {
        IsWhitelisted = !IsWhitelisted;
    }
}