using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.ViewModels
{
    public class InfoWindowViewModel : TSPropertyChanged
    {
        private static InfoWindowViewModel _instance;
        private uint _selectedCharacterId;
        public bool DiscardFirstVanguardPacket = true;
        public static InfoWindowViewModel Instance => _instance ?? (_instance = new InfoWindowViewModel());
        public SynchronizedObservableCollection<Character> Characters { get; set; }
        public SynchronizedObservableCollection<EventGroup> EventGroups { get; }
        public SynchronizedObservableCollection<TimeMarker> Markers { get; }
        // ReSharper disable once CollectionNeverQueried.Global
        public SynchronizedObservableCollection<DailyEvent> SpecialEvents { get; }
        public Character CurrentCharacter => Characters.ToSyncArray().FirstOrDefault(x => x.Id == SessionManager.CurrentPlayer.PlayerId);

        public Character SelectedCharacter => Characters.ToSyncArray().FirstOrDefault(x => x.Id == _selectedCharacterId);
        public bool SelectedCharacterExists => SelectedCharacter != null;
        public bool ShowElleonMarks => TimeManager.Instance.CurrentRegion == "EU";
        public InfoWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Characters = new SynchronizedObservableCollection<Character>(Dispatcher);
            EventGroups = new SynchronizedObservableCollection<EventGroup>(Dispatcher);
            Markers = new SynchronizedObservableCollection<TimeMarker>(Dispatcher);
            SpecialEvents = new SynchronizedObservableCollection<DailyEvent>(Dispatcher);
            LoadCharDoc();
            if (Characters.Count > 0) SelectCharacter(Characters[0]);
            else SelectCharacter(new Character("", Class.None, 0, 0, Dispatcher));
        }

        public void LoadEvents(DayOfWeek today, string region)
        {
            ClearEvents();
            if (region == null)
            {
                WindowManager.FloatingButton.NotifyExtended("Info window", "No region specified; cannot load events.", NotificationType.Error);
                ChatWindowManager.Instance.AddTccMessage("Unable to load events.");
                return;
            }
            LoadEventFile(today, region);
            if(SessionManager.Logged) TimeManager.Instance.SetGuildBamTime(false);

        }
        private void LoadEventFile(DayOfWeek today, string region)
        {
            var yesterday = today - 1;
            if (region.StartsWith("EU")) region = "EU";
            var path = $"resources/config/events/events-{region}.xml";
            if (!File.Exists(path))
            {
                var root = new XElement("Events");
                var eg = new XElement("EventGroup", new XAttribute("name", "Example event group"));
                var ev = new XElement("Event",
                    new XAttribute("name", "Example Event"),
                    new XAttribute("days", "*"),
                    new XAttribute("start", "12:00"),
                    new XAttribute("end", "15:00"),
                    new XAttribute("color", "ff5566"));
                var ev2 = new XElement("Event",
                        new XAttribute("name", "Example event 2"),
                        new XAttribute("days", "*"),
                        new XAttribute("start", "16:00"),
                        new XAttribute("duration", "3:00"),
                        new XAttribute("color", "ff5566"));
                eg.Add(ev);
                eg.Add(ev2);
                root.Add(eg);
                if (!Directory.Exists($"resources/config/events"))
                    Directory.CreateDirectory($"resources/config/events");
                root.Save(path);
            }

            try
            {
                var d = XDocument.Load(path);
                foreach (var egElement in d.Descendants().Where(x => x.Name == "EventGroup"))
                {
                    var egName = egElement.Attribute("name").Value;
                    var egRc = egElement.Attribute("remote") != null && bool.Parse(egElement.Attribute("remote").Value);
                    var egStart = egElement.Attribute("start") != null
                        ? DateTime.Parse(egElement.Attribute("start").Value)
                        : DateTime.MinValue;
                    var egEnd = egElement.Attribute("end") != null
                        ? DateTime.Parse(egElement.Attribute("end").Value).AddDays(1)
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
                                    var day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), dayString);
                                    if (day == today) isToday = true;
                                    if (day == yesterday) isYesterday = true;
                                }
                            }
                            else
                            {
                                var eventDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), evElement.Attribute("days").Value);
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
                        var parsedStart = DateTime.Parse(evElement.Attribute("start").Value, CultureInfo.InvariantCulture);
                        var parsedDuration = TimeSpan.Zero;
                        var parsedEnd = DateTime.Now;
                        bool isDuration;
                        if (evElement.Attribute("duration") != null)
                        {
                            parsedDuration = TimeSpan.Parse(evElement.Attribute("duration").Value, CultureInfo.InvariantCulture);
                            isDuration = true;
                        }
                        else if (evElement.Attribute("end") != null)
                        {
                            parsedEnd = DateTime.Parse(evElement.Attribute("end").Value, CultureInfo.InvariantCulture);
                            isDuration = false;
                        }
                        else
                        {
                            parsedDuration = TimeSpan.Zero;
                            parsedEnd = parsedStart;
                            isDuration = true;
                        }

                        var color = "5599ff";

                        var start = parsedStart.Hour + parsedStart.Minute / 60D;
                        var end = isDuration ? parsedDuration.Hours + parsedDuration.Minutes / 60D : parsedEnd.Hour + parsedEnd.Minute / 60D;

                        if (evElement.Attribute("color") != null)
                        {
                            color = evElement.Attribute("color").Value;
                        }
                        if (isYesterday)
                        {
                            if (!EventUtils.EndsToday(start, end, isDuration))
                            {
                                var e1 = new DailyEvent(name, parsedStart.Hour, 24, 0, color, false);
                                end = start + end - 24;
                                var e2 = new DailyEvent(name, parsedStart.Hour, parsedStart.Minute, end, color, isDuration);
                                if (isToday) eg.AddEvent(e1);
                                eg.AddEvent(e2);
                            }
                            else if (isToday)
                            {
                                var ev = new DailyEvent(name, parsedStart.Hour, parsedStart.Minute, end, color, isDuration);
                                eg.AddEvent(ev);
                            }
                        }
                        else
                        {
                            var ev = new DailyEvent(name, parsedStart.Hour, parsedStart.Minute, end, color, isDuration);
                            eg.AddEvent(ev);
                        }
                    }
                    if (eg.Events.Count != 0) AddEventGroup(eg);
                }
                SpecialEvents.Add(new DailyEvent("Reset", TimeManager.Instance.ResetHour, 0, 0, "ff0000"));



            }
            catch (Exception)
            {
                var res = TccMessageBox.Show("TCC", $"There was an error while reading events-{region}.xml. Manually correct the error and and press Ok to try again, else press Cancel to build a default config file.", MessageBoxButton.OKCancel);

                if (res == MessageBoxResult.Cancel) File.Delete(path);
                LoadEventFile(today, region);
            }
        }

        public void ClearEvents()
        {
            EventGroups.Clear();
            SpecialEvents.Clear();
        }
        public void SelectCharacter(Character c)
        {
            if (c == null) return;
            _selectedCharacterId = c.Id;
            foreach (var ch in Characters)
            {
                if (ch.Id == c.Id) ch.IsSelected = true;
                else ch.IsSelected = false;
            }
            NPC(nameof(SelectedCharacter));

            //AllDungeons = new CollectionViewSource { Source = SelectedCharacter.Dungeons }.View;
            //SoloDungs = new CollectionViewSource { Source = SelectedCharacter.Dungeons }.View;
            //T2Dungs = new CollectionViewSource { Source = SelectedCharacter.Dungeons }.View;
            //T3Dungs = new CollectionViewSource { Source = SelectedCharacter.Dungeons }.View;
            //T4Dungs = new CollectionViewSource { Source = SelectedCharacter.Dungeons }.View;
            //T5Dungs = new CollectionViewSource { Source = SelectedCharacter.Dungeons }.View;
            //Items = new CollectionViewSource { Source = SelectedCharacter.Gear }.View;

            //AllDungeons.Filter = null;
            //Items.Filter = null;
            //SoloDungs.Filter = d => DungeonDatabase.Instance.Dungeons[((DungeonCooldown)d).Id].Tier == DungeonTier.Solo;
            //T2Dungs.Filter = d => DungeonDatabase.Instance.  Dungeons[((DungeonCooldown)d).Id].Tier == DungeonTier.Tier2;
            //T3Dungs.Filter = d => DungeonDatabase.Instance.  Dungeons[((DungeonCooldown)d).Id].Tier == DungeonTier.Tier3;
            //T4Dungs.Filter = d => DungeonDatabase.Instance.  Dungeons[((DungeonCooldown)d).Id].Tier == DungeonTier.Tier4;
            //T5Dungs.Filter = d => DungeonDatabase.Instance.  Dungeons[((DungeonCooldown)d).Id].Tier == DungeonTier.Tier5;

            //AllDungeons.SortDescriptions.Add(new SortDescription("Tier", ListSortDirection.Ascending));
            //Items.SortDescriptions.Add(new SortDescription("Piece", ListSortDirection.Ascending));

            //NotifyPropertyChanged(nameof(AllDungeons));
            //NotifyPropertyChanged(nameof(SoloDungs));
            //NotifyPropertyChanged(nameof(T2Dungs));
            //NotifyPropertyChanged(nameof(T3Dungs));
            //NotifyPropertyChanged(nameof(T4Dungs));
            //NotifyPropertyChanged(nameof(T5Dungs));
            //NotifyPropertyChanged(nameof(Items));
            //_dispatcher.Invoke(() => WindowManager.InfoWindow.AnimateICitems());
        }
        public void ShowWindow()
        {
            if (!Dispatcher.Thread.IsAlive) return;
            //LoadEvents(DateTime.Now.DayOfWeek, TimeManager.Instance.CurrentRegion);
            WindowManager.InfoWindow.ShowWindow();
            NPC(nameof(SelectedCharacterExists));
            //SelectCharacter(SelectedCharacter);
        }

        public void SetVanguard(S_AVAILABLE_EVENT_MATCHING_LIST x)
        {
            if (DiscardFirstVanguardPacket)
            {
                DiscardFirstVanguardPacket = false;
                return;
            }
            var ch = Characters.ToSyncArray().FirstOrDefault(c => c.Id == SessionManager.CurrentPlayer.PlayerId);
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
                    NPC(nameof(CurrentCharacter));
                    SelectCharacter(CurrentCharacter);
                }
                else ch.IsLoggedIn = false;
            }

        }
        public void SetDungeons(Dictionary<uint, short> dungeonCooldowns)
        {
            var ch = Characters.ToSyncArray().FirstOrDefault(x => x.Id == SessionManager.CurrentPlayer.PlayerId);
            ch?.UpdateDungeons(dungeonCooldowns);
        }
        public void EngageDungeon(uint dgId)
        {
            CurrentCharacter.EngageDungeon(dgId);
        }
        public void SaveToFile()
        {
            var root = new XElement("Characters", new XAttribute("elite", SessionManager.IsElite));

            foreach (var c in Characters)
            {
                var ce = new XElement("Character",
                    new XAttribute("name", c.Name),
                    new XAttribute("id", c.Id),
                    new XAttribute("pos", c.Position),
                    new XAttribute("credits", c.Credits),
                    new XAttribute("weekly", c.WeekliesDone),
                    new XAttribute("daily", c.DailiesDone),
                    new XAttribute("class", c.Class),
                    new XAttribute("guardianQuests", c.GuardianQuests),
                    new XAttribute("elleonMarks", c.ElleonMarks)
                    );

                var dungs = new XElement("Dungeons");

                foreach (var d in c.Dungeons)
                {
                    var dg = new XElement("Dungeon",
                        new XAttribute("id", d.Id),
                        new XAttribute("entries", d.Entries),
                        new XAttribute("total", d.Clears));
                    dungs.Add(dg);
                }

                var gear = new XElement("GearPieces");

                foreach (var gearItem in c.Gear)
                {
                    var g = new XElement("Gear",
                        new XAttribute("id", gearItem.Id),
                        new XAttribute("piece", gearItem.Piece),
                        new XAttribute("tier", gearItem.Tier),
                        new XAttribute("exp", gearItem.Experience),
                        new XAttribute("enchant", gearItem.Enchant));
                    gear.Add(g);
                }
                ce.Add(gear);
                ce.Add(dungs);
                root.Add(ce);
            }

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            SaveCharDoc(doc);
        }

        private void SaveCharDoc(XDocument doc)
        {
            try
            {
                var fs = new FileStream(Path.GetDirectoryName(typeof(App).Assembly.Location)+ "/resources/config/characters.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                fs.SetLength(0);
                using (var sr = new StreamWriter(fs, new UTF8Encoding(true)))
                {
                    sr.Write(doc.Declaration + Environment.NewLine + doc);
                }
                fs.Close();
            }
            catch (Exception)
            {
                var res = TccMessageBox.Show("TCC", "Could not write character data to characters.xml. File is being used by another process. Try again?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res == MessageBoxResult.Yes) SaveCharDoc(doc);
            }
        }

        private void LoadCharDoc()
        {
            try
            {
                LoadCharacters();
            }
            catch (Exception)
            {
                var res = TccMessageBox.Show("TCC", $"There was an error while reading characters.xml. Manually correct the error and press Ok to try again, else press Cancel to delete current data.", MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK) LoadCharDoc();
                else
                {
                    File.Delete("resources/config/characters.xml");
                    LoadCharDoc();
                }
            }
        }
        private void LoadCharacters()
        {
            if (!File.Exists("resources/config/characters.xml")) return;
            var doc = XDocument.Load("resources/config/characters.xml");
            // ReSharper disable AssignNullToNotNullAttribute
            SessionManager.IsElite = bool.Parse(doc.Descendants().FirstOrDefault(x => x.Name == "Characters")?.Attribute("elite")?.Value);
            foreach (var c in doc.Descendants().Where(x => x.Name == "Character"))
            {
                var name = c.Attribute("name")?.Value;
                var cr = Convert.ToInt32(c.Attribute("credits")?.Value);
                var w = Convert.ToInt32(c.Attribute("weekly")?.Value);
                var d = Convert.ToInt32(c.Attribute("daily")?.Value);
                var id = Convert.ToUInt32(c.Attribute("id")?.Value);
                var pos = Convert.ToInt32(c.Attribute("pos")?.Value);
                var guard = c.Attribute("guardianQuests") != null ? Convert.ToInt32(c.Attribute("guardianQuests")?.Value) : 0;
                var marks = c.Attribute("elleonMarks") != null ? Convert.ToUInt32(c.Attribute("elleonMarks")?.Value) : 0;
                var classString = c.Attribute("class")?.Value;
                if (!Enum.TryParse<Class>(classString, out var cl))
                {
                    //keep retrocompatibility
                    if (classString == "Elementalist") classString = "Mystic";
                    else if (classString == "Fighter") classString = "Brawler";
                    else if (classString == "Engineer") classString = "Gunner";
                    else if (classString == "Soulless") classString = "Reaper";
                    else if (classString == "Glaiver") classString = "Valkyrie";
                    else if (classString == "Assassin") classString = "Ninja";
                }
                cl = (Class)Enum.Parse(typeof(Class), classString);

                var ch = new Character(name, cl, id, pos, Dispatcher)
                {
                    Credits = cr,
                    WeekliesDone = w,
                    DailiesDone = d,
                    GuardianQuests = guard,
                    ElleonMarks = marks
                };
                var dgDict = new Dictionary<uint, short>();
                foreach (var dgEl in c.Descendants().Where(x => x.Name == "Dungeon"))
                {
                    var dgId = Convert.ToUInt32(dgEl.Attribute("id")?.Value);
                    var dgEntries = Convert.ToInt16(dgEl.Attribute("entries")?.Value);
                    var dgTotal = dgEl.Attribute("total") != null ? Convert.ToInt16(dgEl.Attribute("total")?.Value) : 0;
                    ch.SetDungeonTotalRuns(dgId, dgTotal);
                    dgDict.Add(dgId, dgEntries);
                }
                ch.UpdateDungeons(dgDict);
                var gear = new List<GearItem>();
                foreach (var gearEl in c.Descendants().Where(x => x.Name == "Gear"))
                {
                    var pieceId = Convert.ToUInt32(gearEl.Attribute("id")?.Value);
                    var pieceType = (GearPiece)Enum.Parse(typeof(GearPiece), gearEl.Attribute("piece")?.Value);
                    var pieceTier = (GearTier)Enum.Parse(typeof(GearTier), gearEl.Attribute("tier")?.Value);
                    var pieceEnchant = Convert.ToInt32(gearEl.Attribute("enchant")?.Value);
                    var exp = Convert.ToUInt32(gearEl.Attribute("exp")?.Value);
                    gear.Add(new GearItem(pieceId, pieceTier, pieceType, pieceEnchant, exp));
                }
                ch.UpdateGear(gear);
                Characters.Add(ch);
            }
            // ReSharper restore AssignNullToNotNullAttribute
        }

        public void AddEventGroup(EventGroup eg)
        {
            var g = EventGroups.ToSyncArray().FirstOrDefault(x => x.Name == eg.Name);
            if (g != null)
            {
                foreach (var ev in eg.Events)
                {
                    g.AddEvent(ev);
                }
            }
            else
            {
                EventGroups.Add(eg);
            }
        }
    }
}
