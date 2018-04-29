using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Windows;

namespace TCC.ViewModels
{

    public class CooldownWindowViewModel : TccWindowViewModel
    {
        private static CooldownWindowViewModel _instance;
        public static CooldownWindowViewModel Instance => _instance ?? (_instance = new CooldownWindowViewModel());
        public bool IsTeraOnTop => WindowManager.IsTccVisible;
        public bool ShowItems => SettingsManager.ShowItemsCooldown;

        private SynchronizedObservableCollection<SkillCooldown> _shortSkills;
        private SynchronizedObservableCollection<SkillCooldown> _longSkills;
        private SynchronizedObservableCollection<FixedSkillCooldown> mainSkills;
        private SynchronizedObservableCollection<FixedSkillCooldown> secondarySkills;
        private SynchronizedObservableCollection<SkillCooldown> otherSkills;
        private SynchronizedObservableCollection<SkillCooldown> itemSkills;

        public SynchronizedObservableCollection<SkillCooldown> ShortSkills
        {
            get => _shortSkills;
            set
            {
                if (_shortSkills == value) return;
                _shortSkills = value;
            }
        }
        public SynchronizedObservableCollection<SkillCooldown> LongSkills
        {
            get => _longSkills;
            set
            {
                if (_longSkills == value) return;
                _longSkills = value;
            }
        }
        public SynchronizedObservableCollection<FixedSkillCooldown> MainSkills
        {
            get => mainSkills;
            set
            {
                if (mainSkills == value) return;
                mainSkills = value;
            }
        }
        public SynchronizedObservableCollection<FixedSkillCooldown> SecondarySkills
        {
            get => secondarySkills;
            set
            {
                if (secondarySkills == value) return;
                secondarySkills = value;
            }
        }
        public SynchronizedObservableCollection<SkillCooldown> OtherSkills
        {
            get => otherSkills;
            set
            {
                if (otherSkills == value) return;
                otherSkills = value;
            }
        }
        public SynchronizedObservableCollection<SkillCooldown> ItemSkills
        {
            get => itemSkills;
            set
            {
                if (itemSkills == value) return;
                itemSkills = value;
            }
        }
        public SynchronizedObservableCollection<Skill> HiddenSkills { get; }

        public SynchronizedObservableCollection<Skill> ChoiceList
        {
            get
            {
                var list = new SynchronizedObservableCollection<Skill>();
                var c = SessionManager.CurrentPlayer.Class;
                var skillsForClass = SkillsDatabase.Skills[c];
                foreach (var skill in skillsForClass.Values)
                {
                    if (MainSkills.Any(x => x.Skill.IconName == skill.IconName)) continue;
                    if (SecondarySkills.Any(x => x.Skill.IconName == skill.IconName)) continue;
                    if (list.All(x => x.IconName != skill.IconName))
                    {
                        list.Add(skill);
                    }
                }
                return list;
            }
        }

        private static ClassManager _classManager => ClassManager.CurrentClassManager;

        private void FindAndUpdate(SynchronizedObservableCollection<SkillCooldown> list, SkillCooldown sk)
        {
            var existing = list.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (existing == null)
            {
                list.Add(sk);
                return;
            }
            existing.Refresh(sk.Cooldown);
        }

        private void NormalMode_Update(SkillCooldown sk)
        {

            if (SettingsManager.ClassWindowSettings.Enabled && _classManager.StartSpecialSkill(sk)) return;
            if (!SettingsManager.CooldownWindowSettings.Enabled) return;
            if (sk.Type == CooldownType.Item)
            {
                FindAndUpdate(ItemSkills, sk);
                return;
            }
            try
            {
                if (sk.Cooldown < SkillManager.LongSkillTreshold)
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
                            existing.Refresh(sk.Cooldown);
                        }
                        return;
                    }
                    existing.Refresh(sk.Cooldown);
                }
            }
            catch {/* ignored*/}
        }
        private void NormalMode_Change(Skill skill, uint cd)
        {
            if (!SettingsManager.CooldownWindowSettings.Enabled) return;

            SkillCooldown sk;
            try
            {
                if (cd < SkillManager.LongSkillTreshold)
                {
                    var existing = ShortSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == skill.Name);
                    if (existing == null)
                    {
                        sk = new SkillCooldown(skill, cd, CooldownType.Skill, _dispatcher);
                        ShortSkills.Add(sk);
                        return;
                    }
                    existing.Refresh(cd);
                }
                else
                {
                    var existing = LongSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == skill.Name);
                    if (existing == null)
                    {
                        existing = ShortSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == skill.Name);
                        if (existing == null)
                        {
                            sk = new SkillCooldown(skill, cd, CooldownType.Skill, _dispatcher);
                            LongSkills.Add(sk);
                        }
                        else
                        {
                            existing.Refresh(cd);
                        }
                        return;
                    }
                    existing.Refresh(cd);
                }
            }
            catch
            {

            }
        }

        internal void AddHiddenSkill(SkillCooldown context)
        {
            HiddenSkills.Add(context.Skill);
        }


        internal void AddHiddenSkill(FixedSkillCooldown context)
        {
            HiddenSkills.Add(context.Skill);
        }

        internal void DeleteFixedSkill(FixedSkillCooldown context)
        {
            if (MainSkills.Contains(context)) MainSkills.Remove(context);
            else if (SecondarySkills.Contains(context)) SecondarySkills.Remove(context);

            SaveSkillsConfig();
        }

        private void SaveSkillsConfig()
        {
            //throw new NotImplementedException();
        }

        private void NormalMode_Remove(Skill sk)
        {
            if (!SettingsManager.CooldownWindowSettings.Enabled) return;

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

            }
        }

        internal void Save()
        {
            var root = new XElement("Skills");
            MainSkills.ToList().ForEach(mainSkill =>
            {
                root.Add(new XElement("Skill", new XAttribute("id", mainSkill.Skill.Id), new XAttribute("row", 1)));
            });
            SecondarySkills.ToList().ForEach(secSkill =>
            {
                root.Add(new XElement("Skill", new XAttribute("id", secSkill.Skill.Id), new XAttribute("row", 2)));
            });
            HiddenSkills.ToList().ForEach(sk =>
            {
                root.Add(new XElement("Skill", new XAttribute("id", sk.Id), new XAttribute("row", 3)));
            });
            root.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/config/skills", $"{SessionManager.CurrentPlayer.Class.ToString().ToLowerInvariant()}-skills.xml"));
        }

        private void FixedMode_Update(SkillCooldown sk)
        {
            if (SettingsManager.ClassWindowSettings.Enabled && _classManager.StartSpecialSkill(sk)) return;
            if (!SettingsManager.CooldownWindowSettings.Enabled) return;

            var hSkill = HiddenSkills.ToSyncArray().FirstOrDefault(x => x.IconName == sk.Skill.IconName);
            if (hSkill != null) return;

            var skill = MainSkills.FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (skill != null)
            {
                skill.Start(sk.Cooldown);
                return;
            }
            skill = SecondarySkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (skill != null)
            {
                skill.Start(sk.Cooldown);
                return;
            }


            UpdateOther(sk);
        }
        private void FixedMode_Change(Skill sk, uint cd)
        {
            if (!SettingsManager.CooldownWindowSettings.Enabled) return;

            var hSkill = HiddenSkills.ToSyncArray().FirstOrDefault(x => x.IconName == sk.IconName);
            if (hSkill != null) return;


            var skill = MainSkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(cd);
                return;
            }
            skill = SecondarySkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(cd);
                return;
            }
            try
            {
                var otherSkill = OtherSkills.ToSyncArray().FirstOrDefault(x => x.Skill.Name == sk.Name);
                if (otherSkill != null)
                {

                    //OtherSkills.Remove(otherSkill);
                    otherSkill.Refresh(cd);
                }
            }
            catch { }
        }
        private void FixedMode_Remove(Skill sk)
        {
            //sk.SetDispatcher(Dispatcher);
            if (!SettingsManager.CooldownWindowSettings.Enabled) return;

            var skill = MainSkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(0);
                return;
            }
            skill = SecondarySkills.ToSyncArray().FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(0);
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
            catch { }
        }

        private void UpdateOther(SkillCooldown sk)
        {
            if (!SettingsManager.CooldownWindowSettings.Enabled) return;

            sk.SetDispatcher(_dispatcher);

            try
            {
                if (sk.Type == CooldownType.Item)
                {
                    FindAndUpdate(ItemSkills, sk);
                    return;
                }
                FindAndUpdate(OtherSkills, sk);
            }
            catch
            {
                Debug.WriteLine("Error while refreshing skill");
            }
        }

        public void AddOrRefresh(SkillCooldown sk)
        {
            switch (SettingsManager.CooldownBarMode)
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
            switch (SettingsManager.CooldownBarMode)
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
            switch (SettingsManager.CooldownBarMode)
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
            ShortSkills.Clear();
            LongSkills.Clear();
            MainSkills.Clear();
            SecondarySkills.Clear();
            OtherSkills.Clear();
            ItemSkills.Clear();
        }

        public void LoadSkills(string filename, Class c)
        {
            SkillConfigParser sp = null;
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
                var res = TccMessageBox.Show("TCC", $"There was an error while reading {filename}. Manually correct the error and press Ok to try again, else press Cancel to build a default config file.", MessageBoxButton.OKCancel);

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
                HiddenSkills.Add(sk.Skill);
            }
        }

        public CooldownBarMode Mode => SettingsManager.CooldownBarMode;

        public void NotifyModeChanged()
        {
            NPC(nameof(Mode));
        }
        //public bool IsClassWindowOn
        //{
        //    get => SettingsManager.CooldownBarMode;
        //    set => NotifyPropertyChanged(nameof(IsClassWindowOn));
        //}
        public CooldownWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _scale = SettingsManager.CooldownWindowSettings.Scale;
            ShortSkills = new SynchronizedObservableCollection<SkillCooldown>(_dispatcher);
            LongSkills = new SynchronizedObservableCollection<SkillCooldown>(_dispatcher);
            SecondarySkills = new SynchronizedObservableCollection<FixedSkillCooldown>(_dispatcher);
            MainSkills = new SynchronizedObservableCollection<FixedSkillCooldown>(_dispatcher);
            OtherSkills = new SynchronizedObservableCollection<SkillCooldown>(_dispatcher);
            HiddenSkills = new SynchronizedObservableCollection<Skill>(_dispatcher);
            ItemSkills = new SynchronizedObservableCollection<SkillCooldown>(_dispatcher);
            //ChoiceList = new SynchronizedObservableCollection<FixedSkillCooldown>(_dispatcher);
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                NPC("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    WindowManager.CooldownWindow.RefreshTopmost();
                }
            };


        }

        public void NotifyItemsDisplay()
        {
            NPC(nameof(ShowItems));
        }
    }
}
