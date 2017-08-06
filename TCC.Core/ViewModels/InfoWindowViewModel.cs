using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Windows;

namespace TCC.ViewModels
{
    public class InfoWindowViewModel : TSPropertyChanged
    {
        static InfoWindowViewModel _instance;
        uint _selectedCharacterId;
        public static InfoWindowViewModel Instance => _instance ?? (_instance = new InfoWindowViewModel());
        public SynchronizedObservableCollection<Character> Characters { get; set; }
        public ICollectionView SoloDungs { get; set; }
        public ICollectionView T2Dungs  {get; set; }
        public ICollectionView T3Dungs  {get; set; }
        public ICollectionView T4Dungs  {get; set; }
        public ICollectionView T5Dungs  {get; set; }
        public Character CurrentCharacter
        {
            get => Characters.FirstOrDefault(x => x.Id == SessionManager.CurrentPlayer.PlayerId);
        }
        public Character SelectedCharacter
        {
            get => Characters.FirstOrDefault(x => x.Id == _selectedCharacterId);
        }


        public InfoWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Characters = new SynchronizedObservableCollection<Character>(_dispatcher);

            LoadCharacters();
        }




        public void SelectCharacter(Character c)
        {
            _selectedCharacterId = c.Id;
            foreach (var ch in Characters)
            {
                if (ch.Id == c.Id) ch.IsSelected = true;
                else ch.IsSelected = false;
            }
            NotifyPropertyChanged(nameof(SelectedCharacter));

            SoloDungs = new CollectionViewSource { Source = SelectedCharacter.Dungeons }.View;
            T2Dungs = new CollectionViewSource { Source = SelectedCharacter.Dungeons }.View;
            T3Dungs = new CollectionViewSource { Source = SelectedCharacter.Dungeons }.View;
            T4Dungs = new CollectionViewSource { Source = SelectedCharacter.Dungeons }.View;
            T5Dungs = new CollectionViewSource { Source = SelectedCharacter.Dungeons }.View;

            SoloDungs.Filter = d => DungeonDatabase.Instance.DungeonDefinitions[((DungeonCooldown)d).Id].Tier == DungeonTier.Solo;
            T2Dungs.Filter = d => DungeonDatabase.Instance.DungeonDefinitions[((DungeonCooldown)d).Id].Tier == DungeonTier.Tier2;
            T3Dungs.Filter = d => DungeonDatabase.Instance.DungeonDefinitions[((DungeonCooldown)d).Id].Tier == DungeonTier.Tier3;
            T4Dungs.Filter = d => DungeonDatabase.Instance.DungeonDefinitions[((DungeonCooldown)d).Id].Tier == DungeonTier.Tier4;
            T5Dungs.Filter = d => DungeonDatabase.Instance.DungeonDefinitions[((DungeonCooldown)d).Id].Tier == DungeonTier.Tier5;

            NotifyPropertyChanged(nameof(SoloDungs));
            NotifyPropertyChanged(nameof(T2Dungs));
            NotifyPropertyChanged(nameof(T3Dungs));
            NotifyPropertyChanged(nameof(T4Dungs));
            NotifyPropertyChanged(nameof(T5Dungs));

            _dispatcher.Invoke(() => WindowManager.InfoWindow.AnimateICitems());
        }
        public void ShowWindow()
        {
            if (!_dispatcher.Thread.IsAlive) return;
            WindowManager.InfoWindow.ShowWindow();
        }
        public void SetLoggedIn(uint id)
        {
            foreach (var ch in Characters)
            {
                if (ch.Id == id)
                {
                    ch.IsLoggedIn = true;
                    NotifyPropertyChanged(nameof(CurrentCharacter));
                    SelectCharacter(CurrentCharacter);
                }
                else ch.IsLoggedIn = false;
            }

        }
        public void SetDungeons(Dictionary<uint, short> dungeonCooldowns)
        {
            Characters.FirstOrDefault(x => x.Id == SessionManager.CurrentPlayer.PlayerId).UpdateDungeons(dungeonCooldowns);
        }
        public void SaveToFile()
        {
            XElement root = new XElement("Characters" ,new XAttribute("elite", SessionManager.IsElite));
            
            foreach (var c in Characters)
            {
                XElement ce = new XElement("Character",
                    new XAttribute("name", c.Name),
                    new XAttribute("id", c.Id),
                    new XAttribute("pos", c.Position),
                    new XAttribute("credits", c.Credits),
                    new XAttribute("weekly", c.WeekliesDone),
                    new XAttribute("daily", c.DailiesDone),
                    new XAttribute("class", c.Class)
                    );

                XElement dungs = new XElement("Dungeons");

                foreach (var d in c.Dungeons)
                {
                    XElement dg = new XElement("Dungeon",
                        new XAttribute("id", d.Id),
                        new XAttribute("entries", d.Entries));
                    dungs.Add(dg);
                }

                ce.Add(dungs);
                root.Add(ce);
            }
            
            root.Save("resources/config/characters.xml");
        }

        internal void EngageDungeon(uint dgId)
        {
            CurrentCharacter.EngageDungeon(dgId);
        }

        public void LoadCharacters()
        {
            if (!File.Exists("resources/config/characters.xml")) return;
            XDocument doc = XDocument.Load("resources/config/characters.xml");
            SessionManager.IsElite = Boolean.Parse(doc.Descendants().FirstOrDefault(x => x.Name == "Characters").Attribute("elite").Value);
            foreach (var c in doc.Descendants().Where(x => x.Name == "Character"))
            {
                var name = c.Attribute("name").Value;
                var cr = Convert.ToInt32(c.Attribute("credits").Value);
                var w = Convert.ToInt32(c.Attribute("weekly").Value);
                var d = Convert.ToInt32(c.Attribute("daily").Value);
                var id = Convert.ToUInt32(c.Attribute("id").Value);
                var pos = Convert.ToInt32(c.Attribute("pos").Value);
                var cl = (Class)Enum.Parse(typeof(Class), c.Attribute("class").Value);

                var ch = new Character(name, cl, id,pos, _dispatcher)
                {
                    Credits = cr,
                    WeekliesDone = w,
                    DailiesDone = d
                };
                var dgDict = new Dictionary<uint, short>();
                foreach (var dgEl in c.Descendants().Where(x => x.Name == "Dungeon"))
                {
                    var dgId = Convert.ToUInt32(dgEl.Attribute("id").Value);
                    var dgEntries = Convert.ToInt16(dgEl.Attribute("entries").Value);
                    dgDict.Add(dgId, dgEntries);
                }
                ch.UpdateDungeons(dgDict);
                Characters.Add(ch);
            }
        }

    }
}
