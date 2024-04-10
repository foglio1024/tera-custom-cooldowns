using System;
using System.Timers;
using System.Windows.Input;
using TCC.Data.Chat;
using TCC.Interop.Proxy;

namespace TCC.Data;

public class ApplyCommand : ICommand
{
    private readonly Listing _listing;
    private readonly Timer _t;
    public ApplyCommand(Listing listing)
    {
        _listing = listing;
        _t = new Timer { Interval = 5000 };
        _t.Elapsed += OnTimerElapsed;
    }

    private void OnTimerElapsed(object? s, ElapsedEventArgs ev)
    {
        _t.Stop();
        _listing.CanApply = true;
    }
#pragma warning disable CS0067
    public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
    public bool CanExecute(object? parameter)
    {
        return _listing.CanApply;
    }

    public async void Execute(object? parameter)
    {
        var success = await StubInterface.Instance.StubClient.ApplyToGroup(_listing.LeaderId, _listing.ServerId); //ProxyOld.ApplyToLfg(_listing.LeaderId);
        if (!success) return;
        SystemMessagesProcessor.AnalyzeMessage($"@0\vUserName\v{_listing.LeaderName}", "SMT_PARTYBOARD_APPLY");
        _listing.CanApply = false;
        _t.Start();
    }
}