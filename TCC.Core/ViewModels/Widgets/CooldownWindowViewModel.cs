using FoglioUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Databases;
using TCC.Data.Skills;
using TCC.Parsing;
using TCC.Settings;
using TCC.Utilities;
using TCC.Utils;
using TCC.Windows;
using TeraDataLite;
using TeraPacketParser.Messages;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.ViewModels.Widgets
{

    [TccModule(true)]
    public class CooldownWindowViewModel : TccWindowViewModel
    {
        public bool ShowItems => App.Settings.CooldownWindowSettings.ShowItems;

        public event Action SkillsLoaded;

        public SynchronizedObservableCollection<Cooldown> ShortSkills { get; set; }
        public SynchronizedObservableCollection<Cooldown> LongSkills { get; set; }
        public SynchronizedObservableCollection<Cooldown> MainSkills { get; set; }
        public SynchronizedObservableCollection<Cooldown> SecondarySkills { get; set; }
        public SynchronizedObservableCollection<Cooldown> OtherSkills { get; set; }
        public SynchronizedObservableCollection<Cooldown> ItemSkills { get; set; }
        public SynchronizedObservableCollection<Cooldown> HiddenSkills { get; }

        public ICollectionViewLiveShaping SkillsView { get; set; }
        public ICollectionViewLiveShaping ItemsView { get; set; }
        public ICollectionViewLiveShaping AbnormalitiesView { get; set; }
        public SynchronizedObservableCollection<Skill> SkillChoiceList { get; set; }
        public IEnumerable<Item> Items => Game.DB.ItemsDatabase.ItemSkills;
        public IEnumerable<Abnormality> Passivities => Game.DB.AbnormalityDatabase.Abnormalities.Values.ToList();

        private static BaseClassLayoutVM ClassManager => WindowManager.ViewModels.ClassVM.CurrentManager;

        private static bool FindAndUpdate(SynchronizedObservableCollection<Cooldown> list, Cooldown sk)
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
                if (other.Duration < SkillManager.LongSkillTreshold)
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
                Log.All($"[NormalMode_Update] Error in skill: {sk.Skill.Name}");
                return false;
            }
        }
        private void NormalMode_Change(Skill skill, uint cd)
        {
            if (!App.Settings.CooldownWindowSettings.Enabled) return;
            if (ClassManager.ChangeSpecialSkill(skill, cd)) return;

            try
            {
                if (cd < SkillManager.LongSkillTreshold)
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
                Log.All($"Error while changing cd on {skill.Name}");
                // ignored
            }
        }

        internal void AddHiddenSkill(Cooldown context)
        {
            context.Dispose();
            HiddenSkills.Add(context);
            Save();
        }

        internal void DeleteFixedSkill(Cooldown context)
        {
            if (MainSkills.Contains(context)) MainSkills.Remove(context);
            else if (SecondarySkills.Contains(context)) SecondarySkills.Remove(context);

            Save();
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

        internal void Save()
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (MainSkills.Count == 0 && SecondarySkills.Count == 0 && HiddenSkills.Count == 0) return;
                var root = new XElement("Skills");
                MainSkills.ToList().ForEach(mainSkill =>
                {
                    var tag = mainSkill.CooldownType.ToString();
                    root.Add(new XElement(tag, new XAttribute("id", mainSkill.Skill.Id), new XAttribute("row", 1), new XAttribute("name", mainSkill.Skill.ShortName)));
                });
                SecondarySkills.ToList().ForEach(secSkill =>
                {
                    var tag = secSkill.CooldownType.ToString();
                    root.Add(new XElement(tag, new XAttribute("id", secSkill.Skill.Id), new XAttribute("row", 2), new XAttribute("name", secSkill.Skill.ShortName)));
                });
                HiddenSkills.ToList().ForEach(sk =>
                {
                    var tag = sk.CooldownType.ToString();
                    root.Add(new XElement(tag, new XAttribute("id", sk.Skill.Id), new XAttribute("row", 3), new XAttribute("name", sk.Skill.ShortName)));
                });
                if (Game.Me.Class > (Class)12) return;
                root.Save(Path.Combine(App.ResourcesPath, "config/skills", $"{TccUtils.ClassEnumToString(Game.Me.Class).ToLower()}-skills.xml"));
            });
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
                Log.All("Error while refreshing skill");
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
                ShortSkills.ToList().ForEach(sk => sk.Dispose());
                LongSkills.ToList().ForEach(sk => sk.Dispose());
                MainSkills.ToList().ForEach(sk => sk.Dispose());
                SecondarySkills.ToList().ForEach(sk => sk.Dispose());
                OtherSkills.ToList().ForEach(sk => sk.Dispose());
                ItemSkills.ToList().ForEach(sk => sk.Dispose());

                ShortSkills.Clear();
                LongSkills.Clear();
                MainSkills.Clear();
                SecondarySkills.Clear();
                OtherSkills.Clear();
                ItemSkills.Clear();
                HiddenSkills.Clear();
            });
        }

        public void LoadSkills(Class c)
        {
            if (c == Class.None) return;
            var filename = TccUtils.ClassEnumToString(c).ToLower() + "-skills.xml";
            SkillConfigParser sp;
            //Dispatcher.Invoke(() =>
            //{
            if (!File.Exists(Path.Combine(App.ResourcesPath, "config/skills", filename)))
            {
                SkillUtils.BuildDefaultSkillConfig(filename, c);
            }

            try
            {
                sp = new SkillConfigParser(filename, c);
            }
            catch (Exception)
            {
                var res = TccMessageBox.Show("TCC",
                    $"There was an error while reading {filename}. Manually correct the error and press Ok to try again, else press Cancel to build a default config file.",
                    MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                if (res == MessageBoxResult.Cancel) File.Delete(Path.Combine(App.ResourcesPath, "config/skills/", filename));
                LoadSkills(c);
                return;
            }
            foreach (var sk in sp.Main)
            {
                MainSkills.Add(sk);
            }
            foreach (var sk in sp.Secondary)
            {
                SecondarySkills.Add(sk);
            }
            foreach (var sk in sp.Hidden)
            {
                HiddenSkills.Add(sk);
            }

            Dispatcher.Invoke(() => SkillsView = CollectionViewUtils.InitLiveView(SkillsDatabase.SkillsForClass, null, new string[] { }, new SortDescription[] { }));

            N(nameof(SkillsView));
            N(nameof(MainSkills));
            N(nameof(SecondarySkills));
            SkillsLoaded?.Invoke();
            //});
        }

        public CooldownBarMode Mode => App.Settings.CooldownWindowSettings.Mode;

        public void NotifyModeChanged()
        {
            N(nameof(Mode));
        }

        public CooldownWindowViewModel(WindowSettings settings) : base(settings)
        {
            ShortSkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);
            LongSkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);
            SecondarySkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);
            MainSkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);
            OtherSkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);
            ItemSkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);

            HiddenSkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);

            InitViews();

            KeyboardHook.Instance.RegisterCallback(App.Settings.SkillSettingsHotkey, OnShowSkillConfigHotkeyPressed);

            ((CooldownWindowSettings)settings).ShowItemsChanged += NotifyItemsDisplay;
            ((CooldownWindowSettings)settings).ModeChanged += NotifyModeChanged;
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
            ItemsView = CollectionViewUtils.InitLiveView(Items, null, new string[] { }, new SortDescription[] { });
            AbnormalitiesView = CollectionViewUtils.InitLiveView(Passivities, null, new string[] { }, new SortDescription[] { });
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
            ClearSkills();
        }


        private void CheckPassivity(Abnormality ab, uint cd)
        {
            if (PassivityDatabase.Passivities.TryGetValue(ab.Id, out var cdFromDb))
            {
                SkillManager.AddPassivitySkill(ab.Id, cdFromDb);
            }
            else if (MainSkills.Any(m => m.CooldownType == CooldownType.Passive && ab.Id == m.Skill.Id)
                    || SecondarySkills.Any(m => m.CooldownType == CooldownType.Passive && ab.Id == m.Skill.Id))
            {
                //note: can't do this correctly since we don't know passivity cooldown from database so we just add duration
                SkillManager.AddPassivitySkill(ab.Id, cd / 1000);
            }

        }
        private void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            if (App.Settings.EthicalMode) return;
            if (!AbnormalityUtils.Exists(p.AbnormalityId, out var ab) || !AbnormalityUtils.Pass(ab)) return;

            if (Game.IsMe(p.CasterId) || Game.IsMe(p.TargetId)) CheckPassivity(ab, p.Duration);
        }
        private void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            if (App.Settings.EthicalMode) return;
            if (!AbnormalityUtils.Exists(p.AbnormalityId, out var ab) || !AbnormalityUtils.Pass(ab)) return;

            if (Game.IsMe(p.TargetId)) CheckPassivity(ab, p.Duration);
        }


        private void OnLogin(S_LOGIN m)
        {
            ClearSkills();
            LoadSkills(m.CharacterClass);
        }
        private void OnReturnToLobby(S_RETURN_TO_LOBBY m)
        {
            ClearSkills();
        }
        private void OnGetUserList(S_GET_USER_LIST m)
        {
            ClearSkills();
        }
        private void OnDecreaseCooltimeSkill(S_DECREASE_COOLTIME_SKILL m)
        {
            SkillManager.ChangeSkillCooldown(m.SkillId, m.Cooldown);
        }
        private void OnStartCooltimeItem(S_START_COOLTIME_ITEM m)
        {
            SkillManager.AddItemSkill(m.ItemId, m.Cooldown);
        }
        private void OnStartCooltimeSkill(S_START_COOLTIME_SKILL m)
        {
            SkillManager.AddSkill(m.SkillId, m.Cooldown);
        }
        private void OnCrestMessage(S_CREST_MESSAGE m)
        {
            if (m.Type != 6) return;
            SkillManager.ResetSkill(m.SkillId);
        }
    }
}
