using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Controls;
using TCC.Controls.Dashboard;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Pc;
using TCC.Parsing.Messages;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.ViewModels
{
    public class DashboardViewModel : TccWindowViewModel
    {
        /* -- Fields ----------------------------------------------- */

        private bool _discardFirstVanguardPacket = true;
        private ICollectionViewLiveShaping _sortedColumns;
        private ObservableCollection<CharacterViewModel> _characters;
        private ObservableCollection<DungeonColumnViewModel> _columns;
        private Character _selectedCharacter;

        /* -- Properties ------------------------------------------- */

        public SynchronizedObservableCollection<Character> Characters { get; }


        public Character CurrentCharacter => Characters.ToSyncArray().FirstOrDefault(x => x.Id == SessionManager.CurrentPlayer.PlayerId);
        public Character SelectedCharacter
        {
            get => _selectedCharacter;
            set
            {
                if (_selectedCharacter == value) return;
                _selectedCharacter = value;
                N();
            }
        }

        public bool ShowElleonMarks => Settings.SettingsHolder.LastRegion.Contains("EU");


        public ICollectionViewLiveShaping SortedCharacters { get; }

        public ICollectionViewLiveShaping SortedColumns
        {
            get
            {
                return _sortedColumns ?? (_sortedColumns = Utils.InitLiveView(o => ((DungeonColumnViewModel)o).Dungeon.Show, Columns,
                                            new[] { $"{nameof(Dungeon)}.{nameof(Dungeon.Show)}", $"{nameof(Dungeon)}.{nameof(Dungeon.Index)}" },
                                            new[] { new SortDescription($"{nameof(Dungeon)}.{nameof(Dungeon.Index)}", ListSortDirection.Ascending) }));
            }
        }

        public ObservableCollection<InventoryItem> InventoryViewList
        {
            get
            {
                var ret = new ObservableCollection<InventoryItem>();
                Task.Factory.StartNew(() =>
                {
                    SelectedCharacter.Inventory.ToList().ForEach(item =>
                    {
                        App.BaseDispatcher.BeginInvoke(new Action(() =>
                        {
                            ret.Add(item);
                        }), DispatcherPriority.Background);
                    });
                });
                return ret;
            }
        }

        public ICollectionViewLiveShaping SelectedCharacterInventory { get; set; }


        public int TotalElleonMarks
        {
            get
            {
                int ret = 0;
                Characters.ToSyncArray().ToList().ForEach(c => ret += c.ElleonMarks);
                return ret;
            }
        }
        public int TotalVanguardCredits
        {
            get
            {
                int ret = 0;
                Characters.ToSyncArray().ToList().ForEach(c => ret += c.VanguardCredits);
                return ret;
            }
        }
        public int TotalGuardianCredits
        {
            get
            {
                int ret = 0;
                Characters.ToSyncArray().ToList().ForEach(c => ret += c.GuardianCredits);
                return ret;
            }
        }

        public ObservableCollection<CharacterViewModel> CharacterViewModels
        {
            get
            {
                if (_characters != null) return _characters;
                _characters = new ObservableCollection<CharacterViewModel>();
                foreach (var o in ((ICollectionView)SortedCharacters).Cast<Character>())
                {
                    _characters.Add(new CharacterViewModel { Character = o });
                }
                return _characters;
            }
        }
        public ObservableCollection<DungeonColumnViewModel> Columns
        {
            get
            {
                if (_columns != null) return _columns;
                _columns = new ObservableCollection<DungeonColumnViewModel>();
                return _columns;
            }
        }
        public RelayCommand LoadDungeonsCommand { get; }
        /* -- Constructor ------------------------------------------ */
        bool _loaded = false;
        public DashboardViewModel()
        {
            Characters = new SynchronizedObservableCollection<Character>();
            EventGroups = new SynchronizedObservableCollection<EventGroup>();
            Markers = new SynchronizedObservableCollection<TimeMarker>();
            SpecialEvents = new SynchronizedObservableCollection<DailyEvent>();
            LoadDungeonsCommand = new RelayCommand(o =>
            {
                if (_loaded) return;

                Task.Factory.StartNew(() =>
                {
                    SessionManager.DungeonDatabase.Dungeons.Values.ToList().ForEach(dungeon =>
                    {
                        App.BaseDispatcher.BeginInvoke(new Action(() =>
                        {
                            var dvc = new DungeonColumnViewModel() { Dungeon = dungeon };
                            CharacterViewModels?.ToList().ForEach(charVm =>
                                    dvc.DungeonsList.Add(
                                        new DungeonCooldownViewModel
                                        {
                                            Owner = charVm.Character,
                                            Cooldown = charVm.Character.Dungeons.FirstOrDefault(x =>
                                                x.Dungeon.Id == dungeon.Id)
                                        }));
                            _columns.Add(dvc);
                        }), DispatcherPriority.Background);
                    });
                });
                _loaded = true;
            }, c => !_loaded);
            SortedCharacters = Utils.InitLiveView(o => o != null, Characters, new string[] { }, new[]
            {
                new SortDescription(nameof(Character.Position), ListSortDirection.Ascending)

            });
            LoadCharacters();
        }

        /* -- Methods ---------------------------------------------- */

        public void SaveCharacters()
        {
            SaveCharDoc(CharactersXmlParser.BuildCharacterFile(Characters));
        }
        public void SetLoggedIn(uint id)
        {
            _discardFirstVanguardPacket = true;
            Characters.ToSyncArray().ToList().ForEach(x => x.IsLoggedIn = x.Id == id);
        }
        public void SetDungeons(Dictionary<uint, short> dungeonCooldowns)
        {
            CurrentCharacter?.UpdateDungeons(dungeonCooldowns);

        }
        public void SetDungeons(uint charId, Dictionary<uint, short> dungeonCooldowns)
        {
            Characters.FirstOrDefault(x => x.Id == charId)?.UpdateDungeons(dungeonCooldowns);

        }
        public void SetVanguard(S_AVAILABLE_EVENT_MATCHING_LIST x)
        {
            if (_discardFirstVanguardPacket)
            {
                _discardFirstVanguardPacket = false;
                return;
            }

            if (CurrentCharacter == null) return;
            CurrentCharacter.VanguardWeekliesDone = x.WeeklyDone;
            CurrentCharacter.VanguardDailiesDone = x.DailyDone;
            CurrentCharacter.VanguardCredits = x.VanguardCredits;
            SaveCharacters();
            N(nameof(TotalVanguardCredits));
        }
        public void SetVanguardCredits(int pCredits)
        {
            CurrentCharacter.VanguardCredits = pCredits;
            N(nameof(TotalVanguardCredits));
        }
        public void SetGuardianCredits(int pCredits)
        {
            CurrentCharacter.GuardianCredits = pCredits;
            N(nameof(TotalGuardianCredits));
        }
        public void SetElleonMarks(int val)
        {
            CurrentCharacter.ElleonMarks = val;
            N(nameof(TotalElleonMarks));
        }
        private void LoadCharacters()
        {
            try
            {
                new CharactersXmlParser().Read(Characters);
            }
            catch (Exception)
            {
                var res = TccMessageBox.Show("TCC", $"There was an error while reading characters.xml. Manually correct the error and press Ok to try again, else press Cancel to delete current data.", MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK) LoadCharacters();
                else
                {
                    File.Delete(Path.Combine(App.BasePath, "resources/config/characters.xml"));
                    LoadCharacters();
                }
            }
        }
        private static void SaveCharDoc(XDocument doc)
        {
            try
            {
                var fs = new FileStream(Path.Combine(App.BasePath, "resources/config/characters.xml"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
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

        public void SelectCharacter(Character character)
        {
            //if(SelectedCharacter == character) return;
            SelectedCharacter = character;
            SelectedCharacterInventory = Utils.InitLiveView(o => o != null, SelectedCharacter.Inventory, new string[] { }, new SortDescription[]
            {
                new SortDescription("Item.Id", ListSortDirection.Ascending), 
            });
            WindowManager.Dashboard.ShowDetails();
            Task.Delay(300).ContinueWith(t => Task.Factory.StartNew(() => N(nameof(SelectedCharacterInventory))));
        }

        /* -- EVENTS: TO BE REFACTORED (TODO)----------------------- */

        public SynchronizedObservableCollection<EventGroup> EventGroups { get; }
        public SynchronizedObservableCollection<TimeMarker> Markers { get; }
        public SynchronizedObservableCollection<DailyEvent> SpecialEvents { get; }

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
            if (SessionManager.Logged) TimeManager.Instance.SetGuildBamTime(false);

        }
        public void ClearEvents()
        {
            EventGroups.Clear();
            SpecialEvents.Clear();
        }
        private void LoadEventFile(DayOfWeek today, string region)
        {
            var yesterday = today - 1;
            if (region.StartsWith("EU")) region = "EU";
            var path = Path.Combine(App.BasePath, $"resources/config/events/events-{region}.xml");
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

        public void UpdateBuffs()
        {
            SessionManager.CurrentPlayer.Buffs.ToList().ForEach(b =>
            {
                var existing = CurrentCharacter.Buffs.FirstOrDefault(x => x.Id == b.Abnormality.Id);
                if (existing == null) CurrentCharacter.Buffs.Add(new AbnormalityData { Id = b.Abnormality.Id, Duration = b.DurationLeft, Stacks = b.Stacks });
                else
                {
                    existing.Id = b.Abnormality.Id;
                    existing.Duration = b.DurationLeft;
                    existing.Stacks = b.Stacks;
                }
            });
            SessionManager.CurrentPlayer.Debuffs.ToList().ForEach(b =>
            {
                var existing = CurrentCharacter.Buffs.FirstOrDefault(x => x.Id == b.Abnormality.Id);
                if (existing == null) CurrentCharacter.Buffs.Add(new AbnormalityData { Id = b.Abnormality.Id, Duration = b.DurationLeft, Stacks = b.Stacks });
                else
                {
                    existing.Id = b.Abnormality.Id;
                    existing.Duration = b.DurationLeft;
                    existing.Stacks = b.Stacks;
                }
            });
        }

        public void UpdateInventory()
        {
            if (S_INVEN.Items.ContainsKey(151643)) SetElleonMarks(S_INVEN.Items[151643]);
            if (S_INVEN.Items.ContainsKey(45474)) CurrentCharacter.DragonwingScales = S_INVEN.Items[45474];
            if (S_INVEN.Items.ContainsKey(45482)) CurrentCharacter.PiecesOfDragonScroll = S_INVEN.Items[45482];

            foreach (var id in S_INVEN.Items.Keys)
            {
                var existing = CurrentCharacter.Inventory.FirstOrDefault(x => x.Item.Id == id);
                if (existing != null) existing.Amount = S_INVEN.Items[id];
                else CurrentCharacter.Inventory.Add(new InventoryItem(id, S_INVEN.Items[id]));
            }

            var toRemove = new List<uint>();

            foreach (var item in CurrentCharacter.Inventory)
            {
                if (!S_INVEN.Items.ContainsKey(item.Item.Id)) toRemove.Add(item.Item.Id);
            }
            
            toRemove.ForEach(id =>
            {
                var target = CurrentCharacter.Inventory.FirstOrDefault(i => i.Item.Id == id);
                if (target != null) CurrentCharacter.Inventory.Remove(target);
            });

            N(nameof(SelectedCharacterInventory));
        }
    }
}
