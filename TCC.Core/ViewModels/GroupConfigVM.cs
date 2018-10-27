using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Data;
using TCC.Settings;

namespace TCC.ViewModels
{
    public class GroupConfigVM : TSPropertyChanged
    {

        public event Action ShowAllChanged;

        public SynchronizedObservableCollection<GroupAbnormalityVM> GroupAbnormals;
        public IEnumerable<Abnormality> Abnormalities => SessionManager.AbnormalityDatabase.Abnormalities.Values.ToList();
        public ICollectionView AbnormalitiesView { get; set; }

        public bool ShowAll
        {
            get => Settings.Settings.ShowAllGroupAbnormalities;
            set
            {
                if (Settings.Settings.ShowAllGroupAbnormalities == value) return;
                Settings.Settings.ShowAllGroupAbnormalities = value;
                Dispatcher.Invoke(() => ShowAllChanged?.Invoke());
                SettingsWriter.Save();
                NPC();
            }
        }
        public List<Class> Classes
        {
            get
            {
                var l = Utils.ListFromEnum<Class>();
                l.Remove(Class.None);
                return l;
            }
        }
        public GroupConfigVM()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            GroupAbnormals = new SynchronizedObservableCollection<GroupAbnormalityVM>(Dispatcher);
            foreach (var abnormality in Abnormalities)
            {
                var abVM = new GroupAbnormalityVM(abnormality);

                GroupAbnormals.Add(abVM);
            }
            AbnormalitiesView = new CollectionViewSource { Source = GroupAbnormals }.View;
            AbnormalitiesView.CurrentChanged += OnAbnormalitiesViewOnCurrentChanged;
            AbnormalitiesView.Filter = null;
        }
        //to keep view referenced
        private void OnAbnormalitiesViewOnCurrentChanged(object s, EventArgs ev)
        {
        }
    }
}