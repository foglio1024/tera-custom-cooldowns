using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using TCC.Controls.Dashboard;
using TCC.Data;
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

        /* -- Properties ------------------------------------------- */

        public SynchronizedObservableCollection<Character> Characters { get; }
        public SynchronizedObservableCollection<EventGroup> EventGroups { get; }
        public SynchronizedObservableCollection<TimeMarker> Markers { get; }


        public Character CurrentCharacter => Characters.ToSyncArray().FirstOrDefault(x => x.Id == SessionManager.CurrentPlayer.PlayerId);
        public bool ShowElleonMarks => Settings.Settings.LastRegion.Contains("EU");


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

        public uint TotalElleonMarks
        {
            get
            {
                uint ret = 0;
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
                SessionManager.DungeonDatabase.Dungeons.Values.ToList().ForEach(dungeon =>
                {
                    var dvc = new DungeonColumnViewModel() { Dungeon = dungeon };
                    CharacterViewModels?.ToList().ForEach(charVm => dvc.DungeonsList.Add(new DungeonCooldownViewModel
                    {
                        Owner = charVm.Character,
                        Cooldown = charVm.Character.Dungeons.FirstOrDefault(x => x.Dungeon.Id == dungeon.Id)
                    }));
                    _columns.Add(dvc);
                });
                return _columns;
            }
        }


        /* -- Constructor ------------------------------------------ */

        public DashboardViewModel()
        {
            Characters = new SynchronizedObservableCollection<Character>();
            EventGroups = new SynchronizedObservableCollection<EventGroup>();
            Markers = new SynchronizedObservableCollection<TimeMarker>();
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
            NPC(nameof(TotalVanguardCredits));
        }

        public void SetElleonMarks(uint val)
        {
            CurrentCharacter.ElleonMarks = val;
            NPC(nameof(TotalElleonMarks));
        }
        public void LoadEvents(DayOfWeek dayOfWeek, string currentRegion)
        {
            //TODO
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


        public void SetVanguardCredits(int pCredits)
        {
            CurrentCharacter.VanguardCredits = pCredits;
            NPC(nameof(TotalVanguardCredits));
        }

        public void SetGuardianCredits(int pCredits)
        {
            CurrentCharacter.GuardianCredits = pCredits;
            NPC(nameof(TotalGuardianCredits));
        }
    }
}
