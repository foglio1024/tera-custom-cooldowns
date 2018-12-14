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
using TCC.Data.Pc;
using TCC.Parsing.Messages;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.ViewModels
{
    public class InfoWindowViewModel : TSPropertyChanged
    {

        public InfoWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Characters = new SynchronizedObservableCollection<Character>(Dispatcher);
            EventGroups = new SynchronizedObservableCollection<EventGroup>(Dispatcher);
            Markers = new SynchronizedObservableCollection<TimeMarker>(Dispatcher);
            SpecialEvents = new SynchronizedObservableCollection<DailyEvent>(Dispatcher);
            LoadCharDoc();
            if (Characters.Count > 0) SelectCharacter(Characters[0]);
            else SelectCharacter(new Character());
        }





        /* -- DEPRECATED ------------------------------------------- */
        private static InfoWindowViewModel _instance;
        public static InfoWindowViewModel Instance => _instance ?? (_instance = new InfoWindowViewModel());

        public void ShowWindow()
        {
            if (!Dispatcher.Thread.IsAlive) return;
            //LoadEvents(DateTime.Now.DayOfWeek, TimeManager.Instance.CurrentRegion);
            WindowManager.Dashboard.ShowWindow();
            N(nameof(SelectedCharacterExists));
            //SelectCharacter(SelectedCharacter);
        }
        public bool SelectedCharacterExists => SelectedCharacter != null;
        public Character SelectedCharacter => Characters.ToSyncArray().FirstOrDefault(x => x.Id == _selectedCharacterId);
        private uint _selectedCharacterId;
        public void SelectCharacter(Character c)
        {
            if (c == null) return;
            _selectedCharacterId = c.Id;
            foreach (var ch in Characters)
            {
                if (ch.Id == c.Id) ch.IsSelected = true;
                else ch.IsSelected = false;
            }
            N(nameof(SelectedCharacter));

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

        /* --------------------------------------------------------- */


        /* -- PORTED ----------------------------------------------- */
        public SynchronizedObservableCollection<EventGroup> EventGroups { get; }
        public SynchronizedObservableCollection<TimeMarker> Markers { get; }
        public SynchronizedObservableCollection<DailyEvent> SpecialEvents { get; }

        public bool DiscardFirstVanguardPacket = true;

        public SynchronizedObservableCollection<Character> Characters { get; set; }
        public Character CurrentCharacter => Characters.ToSyncArray().FirstOrDefault(x => x.Id == SessionManager.CurrentPlayer.PlayerId);

        public bool ShowElleonMarks => TimeManager.Instance.CurrentRegion == "EU";

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
                    new XAttribute("class", c.Class),
                    new XAttribute("pos", c.Position),
                    new XAttribute("vanguardCredits", c.VanguardCredits),
                    new XAttribute("guardianCredits", c.GuardianCredits),
                    new XAttribute("vanguardWeekly", c.VanguardWeekliesDone),
                    new XAttribute("vanguardDaily", c.VanguardDailiesDone),
                    new XAttribute("guardianQuests", c.ClaimedGuardianQuests),
                    new XAttribute("elleonMarks", c.ElleonMarks)
                    );

                var dungs = new XElement("Dungeons");

                foreach (var d in c.Dungeons)
                {
                    var dg = new XElement("Dungeon",
                        new XAttribute("id", d.Dungeon.Id),
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
            //TODO: make reader class and refactor this
            if (!File.Exists("resources/config/characters.xml")) return;
            var doc = XDocument.Load("resources/config/characters.xml");
            // ReSharper disable AssignNullToNotNullAttribute
            SessionManager.IsElite = bool.Parse(doc.Descendants().FirstOrDefault(x => x.Name == "Characters")?.Attribute("elite")?.Value);
            foreach (var c in doc.Descendants().Where(x => x.Name == "Character"))
            {
                var ch = new Character();
                c.Attributes().ToList().ForEach(attr =>
                {
                    if (attr.Name == "name") ch.Name = attr.Value;
                    else if (attr.Name == "id") ch.Id = Convert.ToUInt32(attr.Value);
                    else if (attr.Name == "pos") ch.Position = Convert.ToInt32(attr.Value);
                    else if (attr.Name == "vanguardCredits") ch.VanguardCredits = Convert.ToInt32(attr.Value);
                    else if (attr.Name == "guardianCredits") ch.GuardianCredits = Convert.ToInt32(attr.Value);
                    else if (attr.Name == "vanguardWeekly") ch.VanguardWeekliesDone = Convert.ToInt32(attr.Value);
                    else if (attr.Name == "vanguardDaily") ch.VanguardDailiesDone = Convert.ToInt32(attr.Value);
                    else if (attr.Name == "guardianQuests") ch.ClaimedGuardianQuests = Convert.ToInt32(attr.Value);
                    else if (attr.Name == "elleonMarks") ch.ElleonMarks = Convert.ToUInt32(attr.Value);
                    else if (attr.Name == "class") ch.Class = (Class)Enum.Parse(typeof(Class), attr.Value);
                });

                var dgDict = new Dictionary<uint, short>();
                foreach (var dgEl in c.Descendants().Where(x => x.Name == "Dungeon"))
                {
                    uint dgId = 0;
                    short dgEntries = 0;
                    var dgTotal = 0;

                    dgEl.Attributes().ToList().ForEach(attr =>
                    {
                        if (attr.Name == "id") dgId = Convert.ToUInt32(attr.Value);
                        else if (attr.Name == "entries") dgEntries = Convert.ToInt16(attr.Value);
                        else if (attr.Name == "total") dgTotal = Convert.ToInt16(attr.Value);
                    });
                    ch.SetDungeonClears(dgId, dgTotal);
                    dgDict.Add(dgId, dgEntries);
                }
                ch.UpdateDungeons(dgDict);
                var gear = new List<GearItem>();
                foreach (var gearEl in c.Descendants().Where(x => x.Name == "Gear"))
                {
                    uint pieceId = 0;
                    var pieceType = GearPiece.Weapon;
                    var pieceTier = GearTier.Low;
                    var pieceEnchant = 0;
                    uint exp = 0;

                    gearEl.Attributes().ToList().ForEach(attr =>
                    {
                        if (attr.Name == "id") pieceId = Convert.ToUInt32(attr.Value);
                        if (attr.Name == "piece") pieceType = (GearPiece)Enum.Parse(typeof(GearPiece), attr.Value);
                        if (attr.Name == "tier") pieceTier = (GearTier)Enum.Parse(typeof(GearTier), attr.Value);
                        if (attr.Name == "enchant") pieceEnchant = Convert.ToInt32(attr.Value);
                        if (attr.Name == "exp") exp = Convert.ToUInt32(attr.Value);
                    });
                    gear.Add(new GearItem(pieceId, pieceTier, pieceType, pieceEnchant, exp));
                }
                ch.UpdateGear(gear);
                Characters.Add(ch);
            }
            // ReSharper restore AssignNullToNotNullAttribute
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
                ch.VanguardWeekliesDone = x.WeeklyDone;
                ch.VanguardDailiesDone = x.DailyDone;
                ch.VanguardCredits = x.VanguardCredits;
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
                    N(nameof(CurrentCharacter));
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


        /* -------------------------------------------------------- */
    }
}

namespace TCC.Data
{
    public class CharactersXmlParser
    {
        private const string CharactersTag = "Characters";
        private const string CharacterTag = "Character";
        private const string NameTag = "name";
        private const string IdTag = "id";
        private const string PosTag = "pos";
        private const string VanguardCreditsTag = "vanguardCredits";
        private const string GuardianCreditsTag = "guardianCredits";
        private const string VanguardWeeklyTag = "vanguardWeekly";
        private const string VanguardDailyTag = "vanguardDaily";
        private const string GuardianQuestsTag = "guardianQuests";
        private const string ElleonMarksTag = "elleonMarks";
        private const string DragonwingScalesTag = "dragonwing";
        private const string PiecesOfDragonScrollTag = "scrollPieces";
        private const string ClassTag = "class";
        private const string LevelTag = "level";
        private const string ItemLevelTag = "ilvl";
        private const string DungeonTag = "Dungeon";
        private const string DungeonsTag = "Dungeons";
        private const string EntriesTag = "entries";
        private const string TotalTag = "total";
        private const string GearTag = "Gear";
        private const string GearPiecesTag = "GearPieces";
        private const string PieceTag = "piece";
        private const string TierTag = "tier";
        private const string EnchantTag = "enchant";
        private const string ExpTag = "exp";
        private const string EliteTag = "elite";

        private readonly string _path = Path.Combine(App.BasePath, "resources/config/characters.xml");
        private XDocument _doc;

        public static XDocument BuildCharacterFile(SynchronizedObservableCollection<Character> list)
        {
            var root = new XElement(CharactersTag, new XAttribute(EliteTag, SessionManager.IsElite));
            list.ToSyncArray().ToList().ForEach(c =>
            {
                var xChar = BuildGeneralDataXelement(c);
                xChar.Add(BuildDungeonDataXelement(c));
                xChar.Add(BuildGearDataXelement(c));
                root.Add(xChar);
            });

            return new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
        }
        private static XElement BuildGearDataXelement(Character c)
        {
            var xGear = new XElement(GearPiecesTag);
            c.Gear.ToSyncArray().ToList().ForEach(item =>
            {
                xGear.Add(new XElement(GearTag,
                    new XAttribute(IdTag, item.Id),
                    new XAttribute(PieceTag, item.Piece),
                    new XAttribute(TierTag, item.Tier),
                    new XAttribute(ExpTag, item.Experience),
                    new XAttribute(EnchantTag, item.Enchant)));
            });
            return xGear;
        }
        private static XElement BuildGeneralDataXelement(Character c)
        {
            return new XElement(CharacterTag,
                new XAttribute(NameTag, c.Name),
                new XAttribute(IdTag, c.Id),
                new XAttribute(ClassTag, c.Class),
                new XAttribute(PosTag, c.Position),
                new XAttribute(VanguardCreditsTag, c.VanguardCredits),
                new XAttribute(GuardianCreditsTag, c.GuardianCredits),
                new XAttribute(VanguardWeeklyTag, c.VanguardWeekliesDone),
                new XAttribute(VanguardDailyTag, c.VanguardDailiesDone),
                new XAttribute(GuardianQuestsTag, c.ClaimedGuardianQuests),
                new XAttribute(ElleonMarksTag, c.ElleonMarks),
                new XAttribute(DragonwingScalesTag, c.DragonwingScales),
                new XAttribute(LevelTag, c.Level),
                new XAttribute(ItemLevelTag, c.ItemLevel),
                new XAttribute(PiecesOfDragonScrollTag, c.PiecesOfDragonScroll)
            );
        }
        private static XElement BuildDungeonDataXelement(Character c)
        {
            var xDungeons = new XElement(DungeonsTag);
            c.Dungeons.ToSyncArray().ToList().ForEach(dungCd =>
            {
                xDungeons.Add(new XElement(DungeonTag,
                    new XAttribute(IdTag, dungCd.Dungeon.Id),
                    new XAttribute(EntriesTag, dungCd.Entries),
                    new XAttribute(TotalTag, dungCd.Clears)));
            });

            return xDungeons;
        }
        private static void ParseGeneralCharInfo(XElement xChar, Character ch)
        {
            xChar.Attributes().ToList().ForEach(attr =>
            {
                if (attr.Name == NameTag) ch.Name = attr.Value;
                else if (attr.Name == IdTag) ch.Id = Convert.ToUInt32(attr.Value);
                else if (attr.Name == PosTag) ch.Position = Convert.ToInt32(attr.Value);
                else if (attr.Name == VanguardCreditsTag) ch.VanguardCredits = Convert.ToInt32(attr.Value);
                else if (attr.Name == GuardianCreditsTag) ch.GuardianCredits = Convert.ToInt32(attr.Value);
                else if (attr.Name == VanguardWeeklyTag) ch.VanguardWeekliesDone = Convert.ToInt32(attr.Value);
                else if (attr.Name == VanguardDailyTag) ch.VanguardDailiesDone = Convert.ToInt32(attr.Value);
                else if (attr.Name == GuardianQuestsTag) ch.ClaimedGuardianQuests = Convert.ToInt32(attr.Value);
                else if (attr.Name == ElleonMarksTag) ch.ElleonMarks = Convert.ToUInt32(attr.Value);
                else if (attr.Name == DragonwingScalesTag) ch.DragonwingScales = Convert.ToUInt32(attr.Value);
                else if (attr.Name == PiecesOfDragonScrollTag) ch.PiecesOfDragonScroll = Convert.ToUInt32(attr.Value);
                else if (attr.Name == ClassTag) ch.Class = (Class)Enum.Parse(typeof(Class), attr.Value);
                else if (attr.Name == LevelTag) ch.Level = Convert.ToInt32(attr.Value);
                else if (attr.Name == ItemLevelTag) ch.ItemLevel = Convert.ToInt32(attr.Value);
            });
        }
        private static void ParseDungeonCharInfo(XElement xChar, Character ch)
        {
            var dungeons = new Dictionary<uint, short>();
            xChar.Descendants().Where(x => x.Name == DungeonTag).ToList().ForEach(xDung =>
            {
                uint id = 0;
                short entries = 0;
                var total = 0;

                xDung.Attributes().ToList().ForEach(attr =>
                {
                    if (attr.Name == IdTag) id = Convert.ToUInt32(attr.Value);
                    else if (attr.Name == EntriesTag) entries = Convert.ToInt16(attr.Value);
                    else if (attr.Name == TotalTag) total = Convert.ToInt16(attr.Value);
                });
                ch.SetDungeonClears(id, total);
                dungeons.Add(id, entries);
            });

            ch.UpdateDungeons(dungeons);
        }
        private static void ParseGearCharInfo(XElement xChar, Character ch)
        {
            var gear = new List<GearItem>();
            xChar.Descendants().Where(x => x.Name == GearTag).ToList().ForEach(xPiece =>
            {
                uint id = 0;
                var type = GearPiece.Weapon;
                var tier = GearTier.Low;
                var enchant = 0;
                uint exp = 0;

                xPiece.Attributes().ToList().ForEach(attr =>
                {
                    if (attr.Name == IdTag) id = Convert.ToUInt32(attr.Value);
                    if (attr.Name == PieceTag) type = (GearPiece)Enum.Parse(typeof(GearPiece), attr.Value);
                    if (attr.Name == TierTag) tier = (GearTier)Enum.Parse(typeof(GearTier), attr.Value);
                    if (attr.Name == EnchantTag) enchant = Convert.ToInt32(attr.Value);
                    if (attr.Name == ExpTag) exp = Convert.ToUInt32(attr.Value);
                });
                gear.Add(new GearItem(id, tier, type, enchant, exp));
            });
            ch.UpdateGear(gear);
        }

        public void Read(SynchronizedObservableCollection<Character> dest)
        {
            if (File.Exists(_path)) _doc = XDocument.Load(_path);
            if (_doc == null) return;

            ParseEliteStatus();

            _doc.Descendants().Where(x => x.Name == CharacterTag).ToList().ForEach(xChar =>
            {
                var ch = new Character();
                ParseGeneralCharInfo(xChar, ch);
                ParseDungeonCharInfo(xChar, ch);
                ParseGearCharInfo(xChar, ch);
                dest.Add(ch);
            });

        }
        private void ParseEliteStatus()
        {
            SessionManager.IsElite = bool.Parse(_doc.Descendants().FirstOrDefault(x => x.Name == CharactersTag)?.Attribute(EliteTag)?.Value);
        }
    }
}

