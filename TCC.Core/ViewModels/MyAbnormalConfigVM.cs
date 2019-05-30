/* Add My Abnormals Setting by HQ
GroupConfigVM               -> MyAbnormalConfigVM
GroupAbnormalityVM          -> MyAbnormalityVM
GroupAbnormals              -> MyAbnormals

ShowAllGroupAbnormalities   -> ShowAllMyAbnormalities
*/
using FoglioUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Settings;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class MyAbnormalConfigVM : TSPropertyChanged
    {

        public event Action ShowAllChanged;

        public SynchronizedObservableCollection<MyAbnormalityVM> MyAbnormals;
        public IEnumerable<Abnormality> Abnormalities => SessionManager.DB.AbnormalityDatabase.Abnormalities.Values.ToList();
        public ICollectionView AbnormalitiesView { get; set; }

        public bool ShowAll
        {
            get => SettingsHolder.ShowAllMyAbnormalities;
            set
            {
                if (SettingsHolder.ShowAllMyAbnormalities== value) return;
                SettingsHolder.ShowAllMyAbnormalities= value;
                Dispatcher.Invoke(() => ShowAllChanged?.Invoke());
                SettingsWriter.Save();
                N();
            }
        }
        public List<Class> Classes
        {
            get
            {
                var l = EnumUtils.ListFromEnum<Class>();
                l.Remove(Class.None);
                return l;
            }
        }
        public MyAbnormalConfigVM()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            MyAbnormals = new SynchronizedObservableCollection<MyAbnormalityVM>(Dispatcher);
            foreach (var abnormality in Abnormalities)
            {
                var abVM = new MyAbnormalityVM(abnormality);

                MyAbnormals.Add(abVM);
            }
            AbnormalitiesView = new CollectionViewSource { Source = MyAbnormals }.View;
            AbnormalitiesView.CurrentChanged += OnAbnormalitiesViewOnCurrentChanged;
            AbnormalitiesView.Filter = null;
        }
        //to keep view referenced
        private void OnAbnormalitiesViewOnCurrentChanged(object s, EventArgs ev)
        {
        }
    }
}