using Nostrum;
using Nostrum.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using Nostrum.Factories;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Databases;
using TCC.Data.Skills;
using TCC.Analysis;
using TCC.Settings;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TCC.UI.Windows;
using TCC.Utils;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets
{
    [TccModule]
    public class CooldownWindowViewModel : TccWindowViewModel
    {
        public bool ShowItems => App.Settings.CooldownWindowSettings.ShowItems;

        public event Action SkillsLoaded;
        private const int LongSkillTreshold = 40000; //TODO: make configurable?

        public TSObservableCollection<Cooldown> ShortSkills { get; set; }
        public TSObservableCollection<Cooldown> LongSkills { get; set; }
        public TSObservableCollection<Cooldown> MainSkills { get; set; }
        public TSObservableCollection<Cooldown> SecondarySkills { get; set; }
        public TSObservableCollection<Cooldown> OtherSkills { get; set; }
        public TSObservableCollection<Cooldown> ItemSkills { get; set; }
        public TSObservableCollection<Cooldown> HiddenSkills { get; }

        public ICollectionViewLiveShaping SkillsView { get; set; }
        public ICollectionViewLiveShaping ItemsView { get; set; }
        public ICollectionViewLiveShaping AbnormalitiesView { get; set; }
        public TSObservableCollection<Skill> SkillChoiceList { get; set; }
        public IEnumerable<Item> Items => Game.DB.ItemsDatabase.ItemSkills;
        public IEnumerable<Abnormality> Passivities => Game.DB.AbnormalityDatabase.Abnormalities.Values.ToList();

        //TODO: aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
        private static BaseClassLayoutVM ClassManager => WindowManager.ViewModels.ClassVM.CurrentManager;

        private static bool FindAndUpdate(TSObservableCollection<Cooldown> list, Cooldown sk)
        {
            var existing = list.ToSyncList().FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (existing == null)
            {
                list.Add(sk);
                return true;
            }

            if (existing.Mode == CooldownMode.Pre && sk.Mode == CooldownMode.Normal)
            {
                existing.Start(sk);
                return true;
            }
            else
            {
                existing.Refresh(sk.Skill.Id, sk.Duration, sk.Mode);
                return true;
            }
        }

        private bool NormalMode_Update(Cooldown sk)
        {
            //if (App.Settings.ClassWindowSettings.Enabled && ClassManager.StartSpecialSkill(sk)) return false;
            if (!App.Settings.CooldownWindowSettings.Enabled) return false;

            var other = new Cooldown(sk.Skill, sk.CooldownType == CooldownType.Item ? sk.OriginalDuration / 1000 : sk.OriginalDuration, sk.CooldownType, sk.Mode, Dispatcher);

            var hSkill = HiddenSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (hSkill != null) return false;
            if (other.CooldownType == CooldownType.Item) return FindAndUpdate(ItemSkills, other);

            try
            {
                if (other.Duration < LongSkillTreshold)
                {
                    return FindAndUpdate(ShortSkills, other);
                }
                else
                {
                    var existing = LongSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == other.Skill.IconName);
                    if (existing == null)
                    {
                        existing = ShortSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == other.Skill.IconName);
                        if (existing == null)
                        {
                            LongSkills.Add(other);
                        }
                        else
                        {
                            existing.Refresh(other);
                        }

                        return true;
                    }
                    else existing.Refresh(other);
                    return true;
                }
            }
            catch
            {
                Log.CW($"[NormalMode_Update] Error in skill: {sk.Skill.Name}");
                return false;
            }
        }
        private void NormalMode_Change(Skill skill, uint cd)
        {
            if (!App.Settings.CooldownWindowSettings.Enabled) return;
            if (ClassManager.ChangeSpecialSkill(skill, cd)) return;

            try
            {
                if (cd < LongSkillTreshold)
                {
                    var existing = ShortSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == skill.IconName);
                    if (existing == null)
                    {
                        if (skill.Id % 10 != 0) return; //TODO: check this; discards updates if new id is not base
                        ShortSkills.Add(new Cooldown(skill, cd));
                    }
                    else
                    {
                        existing.Refresh(skill.Id, cd, CooldownMode.Normal);
                    }
                }
                else
                {
                    var existing = LongSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == skill.IconName);
                    if (existing == null)
                    {
                        existing = ShortSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == skill.IconName);
                        if (existing == null)
                        {
                            LongSkills.Add(new Cooldown(skill, cd));
                        }
                        else
                        {
                            existing.Refresh(skill.Id, cd, CooldownMode.Normal);
                        }
                        return;
                    }
                    existing.Refresh(skill.Id, cd, CooldownMode.Normal);
                }
            }
            catch
            {
                Log.CW($"Error while changing cd on {skill.Name}");
                // ignored
            }
        }

        internal void AddHiddenSkill(Cooldown context)
        {
            context.Dispose();
            HiddenSkills.Add(context);
            SaveConfig();
        }

        internal void DeleteFixedSkill(Cooldown context)
        {
            if (MainSkills.Contains(context)) MainSkills.Remove(context);
            else if (SecondarySkills.Contains(context)) SecondarySkills.Remove(context);

            SaveConfig();
        }

        private void NormalMode_Remove(Skill sk)
        {
            if (!App.Settings.CooldownWindowSettings.Enabled) return;

            try
            {
                var longSkill = LongSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
                if (longSkill != null)
                {
                    LongSkills.Remove(longSkill);
                    longSkill.Dispose();
                }
                var shortSkill = ShortSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
                if (shortSkill != null)
                {

                    ShortSkills.Remove(shortSkill);
                    shortSkill.Dispose();
                }
                var itemSkill = ItemSkills.ToSyncList().FirstOrDefault(x => x.Skill.Name == sk.Name);
                if (itemSkill != null)
                {

                    ItemSkills.Remove(itemSkill);
                    itemSkill.Dispose();
                }
            }
            catch
            {
                // ignored
            }
        }


        private bool FixedMode_Update(Cooldown sk)
        {
            //if (App.Settings.ClassWindowSettings.Enabled && ClassManager.StartSpecialSkill(sk)) return false;

            if (!App.Settings.CooldownWindowSettings.Enabled)
            {
                return false;
            }

            var hSkill = HiddenSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (hSkill != null)
            {
                return false;
            }

            var skill = MainSkills.FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (skill != null)
            {
                if (skill.Duration == sk.Duration && !skill.IsAvailable && sk.Mode == skill.Mode)
                {
                    return false;
                }
                skill.Start(sk);
                return true;
            }
            skill = SecondarySkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (skill != null)
            {
                if (skill.Duration == sk.Duration && !skill.IsAvailable && sk.Mode == skill.Mode)
                {
                    return false;
                }
                skill.Start(sk);
                return true;
            }
            return UpdateOther(sk);
        }
        private void FixedMode_Change(Skill sk, uint cd)
        {
            if (!App.Settings.CooldownWindowSettings.Enabled) return;
            if (App.Settings.ClassWindowSettings.Enabled && ClassManager.ChangeSpecialSkill(sk, cd)) return;

            var hSkill = HiddenSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (hSkill != null) return;


            var skill = MainSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(sk.Id, cd, CooldownMode.Normal);
                return;
            }
            skill = SecondarySkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(sk.Id, cd, CooldownMode.Normal);
                return;
            }
            try
            {
                var otherSkill = OtherSkills.ToSyncList().FirstOrDefault(x => x.Skill.Name == sk.Name); //TODO: shouldn't this check on IconName???
                //OtherSkills.Remove(otherSkill);
                otherSkill?.Refresh(sk.Id, cd, CooldownMode.Normal);
            }
            catch
            {
                // ignored
            }
        }
        private void FixedMode_Remove(Skill sk)
        {
            //sk.SetDispatcher(Dispatcher);
            if (!App.Settings.CooldownWindowSettings.Enabled) return;

            var skill = MainSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(sk.Id, 0, CooldownMode.Normal);
                return;
            }
            skill = SecondarySkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(sk.Id, 0, CooldownMode.Normal);
                return;
            }

            var item = ItemSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (item != null)
            {

                ItemSkills.Remove(item);
                item.Dispose();
                return;
            }

            try
            {
                var otherSkill = OtherSkills.ToSyncList().FirstOrDefault(x => x.Skill.Name == sk.Name);
                if (otherSkill != null)
                {
                    OtherSkills.Remove(otherSkill);
                    otherSkill.Dispose();
                }
            }
            catch
            {
                // ignored
            }
        }

        private bool UpdateOther(Cooldown sk)
        {
            if (!App.Settings.CooldownWindowSettings.Enabled)
            {
                return false;
            }
            //create it again with cd window dispatcher
            var other = new Cooldown(sk.Skill, sk.CooldownType == CooldownType.Item ? sk.OriginalDuration / 1000 : sk.OriginalDuration, sk.CooldownType, sk.Mode, Dispatcher);
            sk.Dispose();

            try
            {
                if (other.CooldownType != CooldownType.Item) return FindAndUpdate(OtherSkills, other);
                return FindAndUpdate(ItemSkills, other) || FindAndUpdate(OtherSkills, other);
            }
            catch
            {
                Log.CW("Error while refreshing skill");
                return false;
            }
        }

        public void AddOrRefresh(Cooldown sk)
        {
            Dispatcher.InvokeAsync(() =>
            {
                switch (App.Settings.CooldownWindowSettings.Mode)
                {
                    case CooldownBarMode.Fixed:
                        if (!FixedMode_Update(sk)) sk.Dispose();
                        break;
                    default:
                        if (!NormalMode_Update(sk)) sk.Dispose();
                        break;
                }
            });
        }
        public void Change(Skill skill, uint cd)
        {
            Dispatcher.InvokeAsync(() =>
            {
                switch (App.Settings.CooldownWindowSettings.Mode)
                {
                    case CooldownBarMode.Fixed:
                        FixedMode_Change(skill, cd);
                        break;
                    default:
                        NormalMode_Change(skill, cd);
                        break;
                }
            });
        }
        public void Remove(Skill sk)
        {
            Dispatcher.InvokeAsync(() =>
            {
                switch (App.Settings.CooldownWindowSettings.Mode)
                {
                    case CooldownBarMode.Fixed:
                        FixedMode_Remove(sk);
                        break;
                    default:
                        NormalMode_Remove(sk);
                        break;
                }
            });
        }

        public void ClearSkills()
        {
            Dispatcher.InvokeAsync(() =>
            {
                ShortSkills.ToSyncList().ForEach(sk => sk.Dispose());
                LongSkills.ToSyncList().ForEach(sk => sk.Dispose());
                MainSkills.ToSyncList().ForEach(sk => sk.Dispose());
                SecondarySkills.ToSyncList().ForEach(sk => sk.Dispose());
                OtherSkills.ToSyncList().ForEach(sk => sk.Dispose());
                ItemSkills.ToSyncList().ForEach(sk => sk.Dispose());

                ShortSkills.Clear();
                LongSkills.Clear();
                MainSkills.Clear();
                SecondarySkills.Clear();
                OtherSkills.Clear();
                ItemSkills.Clear();
                HiddenSkills.Clear();
            });
        }

        public void LoadConfig(Class c)
        {
            if (c == Class.None || c == Class.Common) return;

            Dispatcher.InvokeAsyncIfRequired(() =>
            {
                var data = new CooldownConfigParser(c).Data;

                data.Main.ForEach(cdData => TryAddToList(cdData, MainSkills));
                data.Secondary.ForEach(cdData => TryAddToList(cdData, SecondarySkills));
                data.Hidden.ForEach(cdData => TryAddToList(cdData, HiddenSkills));

                Dispatcher.Invoke(() => SkillsView = CollectionViewFactory.CreateLiveCollectionView(SkillsDatabase.SkillsForClass));

                N(nameof(SkillsView));
                N(nameof(MainSkills));
                N(nameof(SecondarySkills));

                SkillsLoaded?.Invoke();

                #region Local

                void TryAddToList(CooldownData cdData, TSObservableCollection<Cooldown> list)
                {
                    if (!Game.DB.GetSkillFromId(cdData.Id, c, cdData.Type, out var sk)) return;
                    list.Add(new Cooldown(sk, false, cdData.Type, Dispatcher));
                }
                #endregion
            }, DispatcherPriority.Background);
        }
        internal void SaveConfig()
        {
            if (MainSkills.Count == 0 && SecondarySkills.Count == 0 && HiddenSkills.Count == 0) return;
            var data = new CooldownConfigData();

            MainSkills.ToList().ForEach(sk => data.Main.Add(new CooldownData(sk.Skill.Id, sk.CooldownType)));
            SecondarySkills.ToList().ForEach(sk => data.Secondary.Add(new CooldownData(sk.Skill.Id, sk.CooldownType)));
            HiddenSkills.ToList().ForEach(sk => data.Hidden.Add(new CooldownData(sk.Skill.Id, sk.CooldownType)));
            var path = Path.Combine(App.ResourcesPath, "config","skills", $"{Game.Me.Class.ToString().ToLower()}-skills.json");
            if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
            File.WriteAllText(path, JsonConvert.SerializeObject(data));
            try
            {
                File.Delete(path.Replace(".json", ".xml"));
            }
            catch { }
        }

        public CooldownBarMode Mode => App.Settings.CooldownWindowSettings.Mode;

        public void NotifyModeChanged()
        {
            N(nameof(Mode));
        }

        public CooldownWindowViewModel(WindowSettingsBase settings) : base(settings)
        {
            ShortSkills = new TSObservableCollection<Cooldown>(Dispatcher);
            LongSkills = new TSObservableCollection<Cooldown>(Dispatcher);
            SecondarySkills = new TSObservableCollection<Cooldown>(Dispatcher);
            MainSkills = new TSObservableCollection<Cooldown>(Dispatcher);
            OtherSkills = new TSObservableCollection<Cooldown>(Dispatcher);
            ItemSkills = new TSObservableCollection<Cooldown>(Dispatcher);

            HiddenSkills = new TSObservableCollection<Cooldown>(Dispatcher);

            InitViews();

            KeyboardHook.Instance.RegisterCallback(App.Settings.SkillSettingsHotkey, OnShowSkillConfigHotkeyPressed);

            ((CooldownWindowSettings)settings).ShowItemsChanged += NotifyItemsDisplay;
            ((CooldownWindowSettings)settings).ModeChanged += NotifyModeChanged;

            AbnormalityTracker.PrecooldownStarted += OnPrecooldownStarted;
        }


        private void OnShowSkillConfigHotkeyPressed()
        {
            if (!Game.Logged) return;
            Dispatcher.InvokeAsync(() =>
            {
                if (WindowManager.SkillConfigWindow != null && WindowManager.SkillConfigWindow.IsVisible) WindowManager.SkillConfigWindow.Close();
                else new SkillConfigWindow().ShowWindow();
            }, DispatcherPriority.Background);
        }

        private void InitViews()
        {
            if (ItemsView != null && AbnormalitiesView != null) return;
            ItemsView = CollectionViewFactory.CreateLiveCollectionView(Items);
            AbnormalitiesView = CollectionViewFactory.CreateLiveCollectionView(Passivities);
        }
        public void NotifyItemsDisplay()
        {
            N(nameof(ShowItems));
        }
        public void ResetSkill(Skill skill)
        {
            if (!App.Settings.CooldownWindowSettings.Enabled) return;
            if (App.Settings.CooldownWindowSettings.Mode == CooldownBarMode.Normal) return;
            Dispatcher.InvokeAsync(() =>
            {
                if (ClassManager.ResetSpecialSkill(skill)) return;
                var sk = MainSkills.FirstOrDefault(x => x.Skill.IconName == skill.IconName) ?? SecondarySkills.FirstOrDefault(x => x.Skill.IconName == skill.IconName);
                sk?.ProcReset();
            });
        }
        public void RemoveHiddenSkill(Cooldown skill)
        {
            var target = HiddenSkills.ToSyncList().FirstOrDefault(x => x.Skill.IconName == skill.Skill.IconName);
            if (target != null) HiddenSkills.Remove(target);
        }

        protected override void InstallHooks()
        {
            PacketAnalyzer.Sniffer.EndConnection += OnDisconnected;

            PacketAnalyzer.Processor.Hook<S_LOGIN>(OnLogin);
            PacketAnalyzer.Processor.Hook<S_GET_USER_LIST>(OnGetUserList);
            PacketAnalyzer.Processor.Hook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.Processor.Hook<S_START_COOLTIME_SKILL>(OnStartCooltimeSkill);
            PacketAnalyzer.Processor.Hook<S_START_COOLTIME_ITEM>(OnStartCooltimeItem);
            PacketAnalyzer.Processor.Hook<S_DECREASE_COOLTIME_SKILL>(OnDecreaseCooltimeSkill);
            PacketAnalyzer.Processor.Hook<S_CREST_MESSAGE>(OnCrestMessage);
            PacketAnalyzer.Processor.Hook<S_ABNORMALITY_BEGIN>(OnAbnormalityBegin);
            PacketAnalyzer.Processor.Hook<S_ABNORMALITY_REFRESH>(OnAbnormalityRefresh);

        }
        protected override void RemoveHooks()
        {
            PacketAnalyzer.Processor.Unhook<S_LOGIN>(OnLogin);
            PacketAnalyzer.Processor.Unhook<S_GET_USER_LIST>(OnGetUserList);
            PacketAnalyzer.Processor.Unhook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.Processor.Unhook<S_START_COOLTIME_SKILL>(OnStartCooltimeSkill);
            PacketAnalyzer.Processor.Unhook<S_START_COOLTIME_ITEM>(OnStartCooltimeItem);
            PacketAnalyzer.Processor.Unhook<S_DECREASE_COOLTIME_SKILL>(OnDecreaseCooltimeSkill);
            PacketAnalyzer.Processor.Unhook<S_CREST_MESSAGE>(OnCrestMessage);
            PacketAnalyzer.Processor.Unhook<S_ABNORMALITY_BEGIN>(OnAbnormalityBegin);
            PacketAnalyzer.Processor.Unhook<S_ABNORMALITY_REFRESH>(OnAbnormalityRefresh);

        }

        private void OnDisconnected()
        {
            SaveConfig();
            ClearSkills();
        }


        private void CheckPassivity(Abnormality ab, uint cd)
        {
            if (!PassivityDatabase.TryGetPassivitySkill(ab.Id, out var skill)) return;
            if (PassivityDatabase.Passivities.TryGetValue(ab.Id, out var cdFromDb))
            {
                //SkillManager.AddPassivitySkill(ab.Id, cdFromDb);
                RouteSkill(new Cooldown(skill, cdFromDb * 1000, CooldownType.Passive));
            }
            else if (MainSkills.Any(m => m.CooldownType == CooldownType.Passive && ab.Id == m.Skill.Id)
                  || SecondarySkills.Any(m => m.CooldownType == CooldownType.Passive && ab.Id == m.Skill.Id))
            {
                //note: can't do this correctly since we don't know passivity cooldown from database so we just add duration
                //SkillManager.AddPassivitySkill(ab.Id, cd / 1000);
                RouteSkill(new Cooldown(skill, cd, CooldownType.Passive));
            }

        }
        private void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            if (App.Settings.EthicalMode) return;
            if (!Game.DB.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;

            if (Game.IsMe(p.CasterId) || Game.IsMe(p.TargetId)) CheckPassivity(ab, p.Duration);
        }
        private void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            if (App.Settings.EthicalMode) return;
            if (!Game.DB.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;

            if (Game.IsMe(p.TargetId)) CheckPassivity(ab, p.Duration);
        }
        private void OnLogin(S_LOGIN m)
        {
            ClearSkills();
            LoadConfig(m.CharacterClass);
        }
        private void OnReturnToLobby(S_RETURN_TO_LOBBY m)
        {
            SaveConfig();
            ClearSkills();
        }
        private void OnGetUserList(S_GET_USER_LIST m)
        {
            ClearSkills();
        }
        private void OnDecreaseCooltimeSkill(S_DECREASE_COOLTIME_SKILL m)
        {
            if (!Game.DB.SkillsDatabase.TryGetSkill(m.SkillId, Game.Me.Class, out var skill)) return;
            if (!Pass(skill)) return;
            Change(skill, m.Cooldown);
        }
        private void OnStartCooltimeItem(S_START_COOLTIME_ITEM m)
        {
            if (!Game.DB.ItemsDatabase.TryGetItemSkill(m.ItemId, out var itemSkill)) return;
            RouteSkill(new Cooldown(itemSkill, m.Cooldown, CooldownType.Item));
        }
        private void OnStartCooltimeSkill(S_START_COOLTIME_SKILL m)
        {
            if (!Game.DB.SkillsDatabase.TryGetSkill(m.SkillId, Game.Me.Class, out var skill)) return;
            if (!Pass(skill)) return;
            RouteSkill(new Cooldown(skill, m.Cooldown));
        }
        private void OnCrestMessage(S_CREST_MESSAGE m)
        {
            if (m.Type != 6) return;
            if (!Game.DB.SkillsDatabase.TryGetSkill(m.SkillId, Game.Me.Class, out var skill)) return;
            if (!Pass(skill)) return;
            ResetSkill(skill);
        }


        private void RouteSkill(Cooldown skillCooldown)
        {
            if (skillCooldown.Duration == 0)
            {
                skillCooldown.Dispose();
                Remove(skillCooldown.Skill);
            }
            else
            {
                AddOrRefresh(skillCooldown);
            }
        }

        private void OnPrecooldownStarted(Skill sk, uint duration)
        {
            //SkillManager.AddSkillDirectly(sk, duration, CooldownType.Skill, CooldownMode.Pre);
            RouteSkill(new Cooldown(sk, duration, CooldownType.Skill, CooldownMode.Pre));
        }

        private static bool Pass(Skill sk)
        {
            if (sk.Detail == "off") return false;
            return sk.Class != Class.Common && sk.Class != Class.None;
        }
    }
}
