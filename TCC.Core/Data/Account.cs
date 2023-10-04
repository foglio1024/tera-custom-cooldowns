using System;
using System.Linq;
using Newtonsoft.Json;
using Nostrum.WPF.ThreadSafe;
using TCC.Data.Pc;

namespace TCC.Data;

public class Account : ICloneable
{
    public bool IsElite { get; set; }
    [JsonIgnore]
    public Character? CurrentCharacter { get; private set; }
    public ThreadSafeObservableCollection<Character> Characters { get; }

    public void LoginCharacter(uint id)
    {
        CurrentCharacter = Characters.ToSyncList().FirstOrDefault(x => x.Id == id);
    }
    public Account()
    {
        Characters = new ThreadSafeObservableCollection<Character>();
    }

    /// <summary>
    /// Returns a copy of the Account object to avoid concurrency.
    /// </summary>
    public object Clone()
    {
        var account = new Account {IsElite = IsElite};
        Characters.ToSyncList().ForEach(account.Characters.Add);
        return account;
    }
}