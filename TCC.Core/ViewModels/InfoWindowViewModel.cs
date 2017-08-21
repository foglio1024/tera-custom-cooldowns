using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Parsing.Messages;
using TCC.Windows;

namespace TCC.ViewModels
{
    public class InfoWindowViewModel : TSPropertyChanged
    {
        static InfoWindowViewModel _instance;
        uint _selectedCharacterId;
        public bool DiscardFirstVanguardPacket = true;
        public static InfoWindowViewModel Instance => _instance ?? (_instance = new InfoWindowViewModel());
        public SynchronizedObservableCollection<Character> Characters { get; set; }
        public SynchronizedObservableCollection<EventGroup> EventGroups { get; set; }
        public SynchronizedObservableCollection<TimeMarker> Markers { get; set; }
        public SynchronizedObservableCollection<DailyEvent> SpecialEvents { get; set; }
        public ICollectionView SoloDungs { get; set; }
        public ICollectionView T2Dungs { get; set; }
        public ICollectionView T3Dungs { get; set; }
        public ICollectionView T4Dungs { get; set; }
        public ICollectionView T5Dungs { get; set; }
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
            EventGroups = new SynchronizedObservableCollection<EventGroup>(_dispatcher);
            Markers = new SynchronizedObservableCollection<TimeMarker>(_dispatcher);
            SpecialEvents = new SynchronizedObservableCollection<DailyEvent>(_dispatcher);
            LoadCharacters();
        }

        public void LoadEvents(DayOfWeek today, string region)
        {
            ClearEvents();
            var yesterday = today - 1;
            if (region.StartsWith("EU")) region = "EU";
            var path = $"resources/config/events/events-{region}.xml";
            if (!File.Exists(path))
            {
                XElement root = new XElement("Events");
                var eg = new XElement("EventGroup", new XAttribute("name", "Example event group"));
                var ev = new XElement("Event",
                    new XAttribute("name", "Example Event"),
                    new XAttribute("days", "*"),
                    new XAttribute("start", 12),
                    new XAttribute("end", 15),
                    new XAttribute("color", "ff5566"));
                var ev2 = new XElement("Event",
                        new XAttribute("name", "Example event 2"),
                        new XAttribute("days", "*"),
                        new XAttribute("start", 16),
                        new XAttribute("duration", 3),
                        new XAttribute("color", "ff5566"));
                eg.Add(ev);
                eg.Add(ev2);
                root.Add(eg);
                if (!Directory.Exists($"resources/config/events"))
                    Directory.CreateDirectory($"resources/config/events");
                root.Save(path);
            }
            XDocument d = XDocument.Load(path);
            foreach (var egElement in d.Descendants().Where(x => x.Name == "EventGroup"))
            {
                var egName = egElement.Attribute("name").Value;
                var egRc = egElement.Attribute("remote") != null && bool.Parse(egElement.Attribute("remote").Value);
                var egStart = egElement.Attribute("start") != null
                    ? DateTime.Parse(egElement.Attribute("start").Value)
                    : DateTime.MinValue;
                var egEnd = egElement.Attribute("end") != null
                    ? DateTime.Parse(egElement.Attribute("end").Value)
                    : DateTime.MaxValue;

                if (TimeManager.Instance.CurrentServerTime < egStart ||
                    TimeManager.Instance.CurrentServerTime > egEnd) continue;
                
                var eg = new EventGroup(egName, egStart, egEnd, egRc);
                foreach (var evElement in egElement.Descendants().Where(x => x.Name == "Event"))
                {
                    var isYesterday = false;
                    var isToday = false;

                    if (evElement.Attribute("days").Value != "*")
                    {
                        if (evElement.Attribute("days").Value.Contains(','))
                        {
                            var days = evElement.Attribute("days").Value.Split(',');
                            foreach (var dayString in days)
                            {
                                var day = (DayOfWeek) Enum.Parse(typeof(DayOfWeek), dayString);
                                if (day == today) isToday = true;
                                if (day == yesterday) isYesterday = true;
                            }
                        }
                        else
                        {
                            var eventDay = (DayOfWeek) Enum.Parse(typeof(DayOfWeek), evElement.Attribute("days").Value);
                            isToday = eventDay == today;
                            isYesterday = eventDay == yesterday;
                        }
                    }
                    else
                    {
                        isToday = true;
                        isYesterday = true;
                    }

                    if (!isToday && !isYesterday) continue;

                    var name = evElement.Attribute("name").Value;
                    int start = int.Parse(evElement.Attribute("start").Value);
                    int durationOrEnd;
                    bool isDuration;
                    if (evElement.Attribute("duration") != null)
                    {
                        durationOrEnd = int.Parse(evElement.Attribute("duration").Value);
                        isDuration = true;
                    }
                    else if (evElement.Attribute("end") != null)
                    {
                        durationOrEnd = int.Parse(evElement.Attribute("end").Value);
                        isDuration = false;
                    }
                    else
                    {
                        durationOrEnd = 0;
                        isDuration = true;
                    }

                    var color = "5599ff";
                    if (evElement.Attribute("color") != null)
                    {
                        color = evElement.Attribute("color").Value;
                    }
                    if (isYesterday)
                    {
                        if (!EventUtils.EndsToday(start, durationOrEnd, isDuration))
                        {
                            var e1 = new DailyEvent(name, start, 24, color, false);
                            durationOrEnd = start + durationOrEnd - 24;
                            start = 0;
                            var e2 = new DailyEvent(name, start, durationOrEnd, color, isDuration);
                            if(isToday) eg.AddEvent(e1);
                            eg.AddEvent(e2);
                        }
                        else if (isToday)
                        {
                            var ev = new DailyEvent(name, start, durationOrEnd, color, isDuration);
                            eg.AddEvent(ev);
                        }
                    }
                    else
                    {
                        var ev = new DailyEvent(name, start, durationOrEnd, color, isDuration);
                        eg.AddEvent(ev);
                    }
                }
                if (eg.Events.Count != 0) AddEventGroup(eg);
            }
            SpecialEvents.Add(new DailyEvent("Reset", TimeManager.Instance.ResetHour, 0, "ff0000"));
        }
        public void ClearEvents()
        {
            EventGroups.Clear();
            SpecialEvents.Clear();
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

            //_dispatcher.Invoke(() => WindowManager.InfoWindow.AnimateICitems());
        }
        public void ShowWindow()
        {
            if (!_dispatcher.Thread.IsAlive) return;
            WindowManager.InfoWindow.ShowWindow();
        }

        public void SetVanguard(S_AVAILABLE_EVENT_MATCHING_LIST x)
        {
            if (DiscardFirstVanguardPacket)
            {
                DiscardFirstVanguardPacket = false;
                return;
            }
            var ch = Characters.FirstOrDefault(c => c.Id == SessionManager.CurrentPlayer.PlayerId);
            if (ch != null)
            {
                ch.WeekliesDone = x.WeeklyDone;
                ch.DailiesDone = x.DailyDone;
                ch.Credits = x.VanguardCredits;
                SaveToFile();
            }

        }
        public void SetLoggedIn(uint id)
        {
            foreach (var ch in Characters)
            {
                if (ch.Id == id)
                {
                    ch.IsLoggedIn = true;
                    DiscardFirstVanguardPacket = true;
                    NotifyPropertyChanged(nameof(CurrentCharacter));
                    SelectCharacter(CurrentCharacter);
                }
                else ch.IsLoggedIn = false;
            }

        }
        public void SetDungeons(Dictionary<uint, short> dungeonCooldowns)
        {
            var ch = Characters.FirstOrDefault(x => x.Id == SessionManager.CurrentPlayer.PlayerId);
            ch?.UpdateDungeons(dungeonCooldowns);
        }
        public void EngageDungeon(uint dgId)
        {
            CurrentCharacter.EngageDungeon(dgId);
        }
        public void SaveToFile()
        {
            XElement root = new XElement("Characters", new XAttribute("elite", SessionManager.IsElite));

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

            XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            var fs = new FileStream(Environment.CurrentDirectory + "/resources/config/characters.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            fs.SetLength(0);
            using (var sr = new StreamWriter(fs, new UTF8Encoding(true)))
            {
                sr.Write(doc.Declaration + Environment.NewLine + doc);
            }
            fs.Close();
            //root.Save("resources/config/characters.xml");
        }


        private void LoadCharacters()
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

                var ch = new Character(name, cl, id, pos, _dispatcher)
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

        public void AddEventGroup(EventGroup eg)
        {
            if (EventGroups.FirstOrDefault(x => x.Name == eg.Name) != null) return;
            EventGroups.Add(eg);
        }
    }
}
