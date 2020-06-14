using System;
using System.Linq;
using Newtonsoft.Json;
using Nostrum;
using TCC.Data.Pc;
using TeraDataLite;

namespace TCC.Data
{
    public class Account : ICloneable
    {
        public bool IsElite { get; set; }
        [JsonIgnore]
        public Character CurrentCharacter { get; private set; }
        public TSObservableCollection<Character> Characters { get; }

        public void LoginCharacter(uint id)
        {
            CurrentCharacter = Characters.ToSyncList().FirstOrDefault(x => x.Id == id);
        }
        public Account()
        {
            Characters = new TSObservableCollection<Character>();
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

        public void AddCharacter(CharacterData item)
        {
            App.BaseDispatcher.InvokeAsync(() =>
            {
                Characters.Add(new Character(item));
            });
        }
    }
}