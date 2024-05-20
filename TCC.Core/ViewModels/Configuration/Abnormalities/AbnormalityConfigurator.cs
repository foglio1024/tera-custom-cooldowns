using Nostrum.WPF;
using System;
using System.Linq;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Settings;
using TeraDataLite;

namespace TCC.ViewModels.Configuration.Abnormalities;

public class AbnormalityConfigurator : ObservableObject
{
    private readonly AbnormalitySettings _settings;
    private readonly Abnormality _abnormality;

    private bool _isCollapsible;

    public bool IsCollapsible
    {
        get => _isCollapsible;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _isCollapsible)) return;

            if (value)
            {
                if (!_settings.Collapsible.Contains(_abnormality.Id))
                {
                    _settings.Collapsible.Add(_abnormality.Id);
                }
            }
            else
            {
                _settings.Collapsible.Remove(_abnormality.Id);
            }
        }
    }

    public bool IsWhitelisted => WhitelistToggles.Any(p => p.IsWhitelisted);

    public bool CanBeWhitelisted => _abnormality.Type is AbnormalityType.Buff && _abnormality.IsBuff && !_settings.ShowAll;
    public bool IsWhitelistingEnabled => !_settings.ShowAll;

    public AbnormalityWhitelistToggle[] WhitelistToggles { get; }

    public AbnormalityConfigurator(Abnormality abnormality, AbnormalitySettings settings)
    {
        _abnormality = abnormality;
        _settings = settings;
        _isCollapsible = _settings.Collapsible.Contains(_abnormality.Id);

        WhitelistToggles = Enum.GetValues<Class>()
            .Where(x => x is not Class.None)
            .Select(cl => new AbnormalityWhitelistToggle(abnormality, cl, settings))
            .ToArray();
    }
}