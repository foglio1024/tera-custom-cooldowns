using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Databases;
using TCC.Data.Skills;
using TCC.Windows;

namespace TCC.ViewModels
{

    public class CooldownWindowViewModel : TccWindowViewModel
    {
        private static CooldownWindowViewModel _instance;
        public static CooldownWindowViewModel Instance => _instance ?? (_instance = new CooldownWindowViewModel());
        public bool ShowItems => Settings.Settings.ShowItemsCooldown;

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
        public IEnumerable<Item> Items => SessionManager.ItemsDatabase.ItemSkills;
        public IEnumerable<Abnormality> Passivities => SessionManager.AbnormalityDatabase.Abnormalities.Values.ToList();
        //{
        //    get
        //    {
        //        var list = new SynchronizedObservableCollection<Skill>();
        //        var c = SessionManager.CurrentPlayer.Class;
        //        var skillsForClass = SessionManager.SkillsDatabase.Skills[c];
        //        foreach (var skill in skillsForClass.Values)
        //        {
        //            if (MainSkills.Any(x => x.Skill.IconName == skill.IconName)) continue;
        //            if (SecondarySkills.Any(x => x.Skill.IconName == skill.IconName)) continue;
        //            if (list.All(x => x.IconName != skill.IconName))
        //            {
        //                list.Add(skill);
        //            }
        //        }
        //        return list;
        //    }
        //}

        private static ClassManager ClassManager => ClassWindowViewModel.Instance.CurrentManager;

        private void FindAndUpdate(SynchronizedObservableCollection<Cooldown> list, Cooldown sk)
        {
            var existing = list.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (existing == null)
            {
                list.Add(sk);
                return;
            }
            if(existing.Mode == CooldownMode.Pre && sk.Mode== CooldownMode.Normal) existing.Start(sk);
            else existing.Refresh(sk.Skill.Id, sk.Duration, sk.Mode);
            sk.Dispose();
        }

        private void NormalMode_Update(Cooldown sk)
        {

            if (Settings.Settings.ClassWindowSettings.Enabled && ClassManager.StartSpecialSkill(sk)) return;
            if (!Settings.Settings.CooldownWindowSettings.Enabled) return;
            if (sk.CooldownType == CooldownType.Item)
            {
                FindAndUpdate(ItemSkills, sk);
                return;
            }
            try
            {
                if (sk.Duration < SkillManager.LongSkillTreshold)
                {
                    FindAndUpdate(ShortSkills, sk);
                }
                else
                {
                    var existing = LongSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == sk.Skill.Name);
                    if (existing == null)
                    {
                        existing = ShortSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == sk.Skill.Name);
                        if (existing == null)
                        {
                            LongSkills.Add(sk);
                        }
                        else
                        {
                            existing.Refresh(sk.Skill.Id, sk.Duration, CooldownMode.Normal);
                        }
                        return;
                    }
                    existing.Refresh(sk.Skill.Id, sk.Duration, CooldownMode.Normal);
                }
            }
            catch {/* ignored*/}
        }
        private void NormalMode_Change(Skill skill, uint cd)
        {
            if (!Settings.Settings.CooldownWindowSettings.Enabled) return;
            if (ClassManager.ChangeSpecialSkill(skill, cd)) return;

            var sk = new Cooldown(skill, cd);
            try
            {
                if (cd < SkillManager.LongSkillTreshold)
                {
                    var existing = ShortSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == skill.Name);
                    if (existing == null)
                    {
                        ShortSkills.Add(sk);
                        return;
                    }
                    existing.Refresh(sk.Skill.Id, sk.Duration, CooldownMode.Normal);
                }
                else
                {
                    var existing = LongSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == skill.Name);
                    if (existing == null)
                    {
                        existing = ShortSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == skill.Name);
                        if (existing == null)
                        {
                            LongSkills.Add(sk);
                        }
                        else
                        {
                            existing.Refresh(sk.Skill.Id, sk.Duration, CooldownMode.Normal);
                        }
                        return;
                    }
                    existing.Refresh(sk.Skill.Id, sk.Duration, CooldownMode.Normal);
                }
            }
            catch
            {
                // ignored
            }
        }

        internal void AddHiddenSkill(Cooldown context)
        {
            context.Dispose();
            HiddenSkills.Add(context);
            Save();
        }
        //internal void AddHiddenSkill(Cooldown context)
        //{
        //    HiddenSkills.Add(context.Skill);
        //    Save();
        //}

        internal void DeleteFixedSkill(Cooldown context)
        {
            if (MainSkills.Contains(context)) MainSkills.Remove(context);
            else if (SecondarySkills.Contains(context)) SecondarySkills.Remove(context);

            Save();
        }

        private void NormalMode_Remove(Skill sk)
        {
            if (!Settings.Settings.CooldownWindowSettings.Enabled) return;

            try
            {
                var longSkill = LongSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == sk.Name);
                if (longSkill != null)
                {
                    LongSkills.Remove(longSkill);
                    longSkill.Dispose();
                }
                var shortSkill = ShortSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == sk.Name);
                if (shortSkill != null)
                {

                    ShortSkills.Remove(shortSkill);
                    shortSkill.Dispose();
                }
                var itemSkill = ItemSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == sk.Name);
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
            if(MainSkills.Count == 0 && SecondarySkills.Count == 0 && HiddenSkills.Count == 0) return;
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
            if (SessionManager.CurrentPlayer.Class > (Class)12) return;
            root.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/config/skills", $"{Utils.ClassEnumToString(SessionManager.CurrentPlayer.Class).ToLower()}-skills.xml"));
        }

        private void FixedMode_Update(Cooldown sk)
        {
            if (Settings.Settings.ClassWindowSettings.Enabled && ClassManager.StartSpecialSkill(sk))
            {
                return;
            }

            if (!Settings.Settings.CooldownWindowSettings.Enabled)
            {
                sk.Dispose();

                return;
            }

            var hSkill = HiddenSkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (hSkill != null)
            {
                sk.Dispose();

                return;
            }

            var skill = MainSkills.FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (skill != null)
            {
                if (skill.Duration == sk.Duration && !skill.IsAvailable && sk.Mode == skill.Mode)
                {
                    //Log.All($"Discarded update for {sk.Skill.Name} [{sk.Skill.Id}]");
                    return;
                }
                skill.Start(sk);
                return;
            }
            skill = SecondarySkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (skill != null)
            {
                if (skill.Duration == sk.Duration && !skill.IsAvailable && sk.Mode == skill.Mode)
                {
                    //Log.All($"Discarded update for {sk.Skill.Name} [{sk.Skill.Id}]");
                    return;
                }
                skill.Start(sk);
                return;
            }


            UpdateOther(sk);
        }
        private void FixedMode_Change(Skill sk, uint cd)
        {
            if (!Settings.Settings.CooldownWindowSettings.Enabled) return;
            if (Settings.Settings.ClassWindowSettings.Enabled && ClassManager.ChangeSpecialSkill(sk, cd)) return;

            var hSkill = HiddenSkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (hSkill != null) return;


            var skill = MainSkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(sk.Id,cd, CooldownMode.Normal);
                return;
            }
            skill = SecondarySkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(sk.Id,cd, CooldownMode.Normal);
                return;
            }
            try
            {
                var otherSkill = OtherSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == sk.Name); //TODO: shouldn't this check on IconName???
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
            if (!Settings.Settings.CooldownWindowSettings.Enabled) return;

            var skill = MainSkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(sk.Id,0, CooldownMode.Normal);
                return;
            }
            skill = SecondarySkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(sk.Id,0, CooldownMode.Normal);
                return;
            }

            var item = ItemSkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (item != null)
            {

                ItemSkills.Remove(item);
                item.Dispose();
                return;
            }

            try
            {
                var otherSkill = OtherSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == sk.Name);
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

        private void UpdateOther(Cooldown sk)
        {
            if (!Settings.Settings.CooldownWindowSettings.Enabled)
            {
                sk.Dispose();
                return;
            }

            sk.SetDispatcher(Dispatcher);

            try
            {
                if (sk.CooldownType == CooldownType.Item)
                {
                    FindAndUpdate(ItemSkills, sk);
                    return;
                }
                FindAndUpdate(OtherSkills, sk);
            }
            catch
            {
                sk.Dispose();
                Debug.WriteLine("Error while refreshing skill");
            }
        }

        public void AddOrRefresh(Cooldown sk)
        {
            switch (Settings.Settings.CooldownBarMode)
            {
                case CooldownBarMode.Fixed:
                    FixedMode_Update(sk);
                    break;
                default:
                    NormalMode_Update(sk);
                    break;
            }
        }
        public void Change(Skill skill, uint cd)
        {
            switch (Settings.Settings.CooldownBarMode)
            {
                case CooldownBarMode.Fixed:
                    FixedMode_Change(skill, cd);
                    break;
                default:
                    NormalMode_Change(skill, cd);
                    break;
            }
        }
        public void Remove(Skill sk)
        {
            switch (Settings.Settings.CooldownBarMode)
            {
                case CooldownBarMode.Fixed:
                    FixedMode_Remove(sk);
                    break;
                default:
                    NormalMode_Remove(sk);
                    break;
            }
        }

        public void ClearSkills()
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
        }

        public void LoadSkills(string filename, Class c)
        {
            SkillConfigParser sp;
            Dispatcher.Invoke(() =>
            {
                if (!File.Exists("resources/config/skills/" + filename))
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
                        MessageBoxButton.OKCancel);

                    if (res == MessageBoxResult.Cancel) File.Delete("resources/config/skills/" + filename);
                    LoadSkills(filename, c);
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

                Dispatcher.Invoke(() =>
                {
                    SkillChoiceList.Clear();
                    foreach (var skill in SkillsDatabase.SkillsForClass)
                    {
                        SkillChoiceList.Add(skill);
                    }

                    SkillsView = Utils.InitLiveView(null, SkillChoiceList, new string[] { }, new SortDescription[] { });
                });
                NPC(nameof(SkillsView));
                NPC(nameof(MainSkills));
                NPC(nameof(SecondarySkills));
                SkillsLoaded?.Invoke();
            });
        }

        public CooldownBarMode Mode => Settings.Settings.CooldownBarMode;

        public void NotifyModeChanged()
        {
            NPC(nameof(Mode));
        }
        //public bool IsClassWindowOn
        //{
        //    get => Settings.CooldownBarMode;
        //    set => NotifyPropertyChanged(nameof(IsClassWindowOn));
        //}
        public CooldownWindowViewModel()
        {
            Dispatcher = App.BaseDispatcher;

            ShortSkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);
            LongSkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);
            SecondarySkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);
            MainSkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);
            OtherSkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);
            ItemSkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);

            HiddenSkills = new SynchronizedObservableCollection<Cooldown>(Dispatcher);
            SkillChoiceList = new SynchronizedObservableCollection<Skill>(Dispatcher);

            SkillsView = Utils.InitLiveView(null, SkillChoiceList, new string[] { }, new SortDescription[] { });
            ItemsView = Utils.InitLiveView(null, Items.ToList(), new string[] { }, new SortDescription[] { });
            AbnormalitiesView = Utils.InitLiveView(null, Passivities, new string[] { }, new SortDescription[] { });
        }

        public void NotifyItemsDisplay()
        {
            NPC(nameof(ShowItems));
        }

        public void ResetSkill(Skill skill)
        {
            if (!Settings.Settings.CooldownWindowSettings.Enabled) return;
            if (Settings.Settings.CooldownBarMode == CooldownBarMode.Normal) return;
            if (ClassManager.ResetSpecialSkill(skill)) return;

            var sk = MainSkills.FirstOrDefault(x => x.Skill.IconName == skill.IconName) ?? SecondarySkills.FirstOrDefault(x => x.Skill.IconName == skill.IconName);
            sk?.ProcReset();
        }

        public void RemoveHiddenSkill(Cooldown skill)
        {
            var target = HiddenSkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == skill.Skill.IconName);
            if (target != null) HiddenSkills.Remove(target);
        }
    }
}
