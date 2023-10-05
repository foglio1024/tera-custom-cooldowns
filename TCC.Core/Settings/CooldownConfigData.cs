using System.Collections.Generic;

namespace TCC.Settings;

public class CooldownConfigData // todo: readonly record struct?
{
    public List<CooldownData> Main { get; } = new();
    public List<CooldownData> Secondary { get; } = new();
    public List<CooldownData> Hidden { get; } = new();
}