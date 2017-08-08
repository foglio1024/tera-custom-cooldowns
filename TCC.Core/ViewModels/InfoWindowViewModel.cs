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
        public SynchronizedObservableCollection<EventGroup> EventGroups { get; set; }
        public SynchronizedObservableCollection<TimeMarker> Markers { get; set; }
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

            Markers.Add(new TimeMarker(DateTime.Now, "Local time"));
            LoadCharacters();
        }

        public void LoadEvents(DayOfWeek today, string region)
        {
            ClearEvents();
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
                var eg = new EventGroup(egElement.Attribute("name").Value);
                foreach (var evElement in egElement.Descendants().Where(x => x.Name == "Event"))
                {
                    if (evElement.Attribute("days").Value != "*")
                    {
                        if (evElement.Attribute("days").Value.Contains(','))
                        {
                            var days = evElement.Attribute("days").Value.Split(',');
                            bool isToday = false;
                            foreach (var dayString in days)
                            {
                                var day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), dayString);
                                if (day == today) isToday = true;
                            }
                            if (!isToday) continue;
                        }
                        else
                        {
                            var eventDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), evElement.Attribute("days").Value);
                            if (eventDay != today)
                            {
                                continue;
                            }
                        }
                    }


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
                    var ev = new DailyEvent(name, start, durationOrEnd, color, isDuration);
                    eg.AddEvent(ev);
                }
                AddEventGroup(eg);
            }
        }
        public void ClearEvents()
        {
            EventGroups.Clear();
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

            root.Save("resources/config/characters.xml");
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

    public class TimeMarker : TSPropertyChanged
    {
        private readonly DispatcherTimer t = new DispatcherTimer();
        private DateTime _dateTime;

        public string TimeString
        {
            get => _dateTime.ToShortTimeString();
        }
        public double TimeFactor
        {
            get => ((_dateTime.Hour * 60 + _dateTime.Minute) * 60) / TimeManager.SecondsInDay;
        }
        public string Name { get; }
        public string Color { get; }
        public TimeMarker(DateTime time, string name, string color = "ffffff")
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Name = name;
            Color = color;
            _dateTime = time;
            t.Interval = TimeSpan.FromSeconds(1);
            t.Tick += T_Tick;
            t.Start();
        }

        private void T_Tick(object sender, EventArgs e)
        {
            _dateTime = _dateTime.AddSeconds(1);
            NotifyPropertyChanged(nameof(TimeString));
            NotifyPropertyChanged(nameof(TimeFactor));
        }
    }

    public class DailyEvent : TSPropertyChanged
    {
        DateTime _start;
        TimeSpan _duration;
        TimeSpan _realDuration;
        public double StartFactor
        {
            get
            {
                return 60 * (_start.Hour * 60 + _start.Minute) / TimeManager.SecondsInDay;
            }
        }
        public double DurationFactor
        {
            get
            {
                return _duration.TotalSeconds / TimeManager.SecondsInDay;

            }
        }
        public string Name { get; }
        public string ToolTip
        {
            get
            {
                var d = _duration > TimeSpan.FromHours(0) ? " to " + _start.Add(_realDuration).ToShortTimeString() : "";
                return Name + " " + _start.ToShortTimeString() + d;
            }
        }
        public string Color { get; }
        public DailyEvent(string name, double startHour, double durationOrEndHour, string color = "30afff", bool isDuration = true)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _start = DateTime.Parse(startHour + ":00");
            var d = isDuration ? durationOrEndHour : durationOrEndHour - startHour;
            _duration = TimeSpan.FromHours(d);
            _realDuration = _duration;
            var dayend = DateTime.Parse("00:00").AddDays(1);
            if (_start.Add(_duration) > dayend)
            {
                _duration = dayend - _start;
            }
            Name = name;
            Color = color;
        }
    }
}
