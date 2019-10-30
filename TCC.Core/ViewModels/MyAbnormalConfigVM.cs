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
using System.Windows.Threading;
using FoglioUtils.Extensions;
using TCC.Utilities;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class MyAbnormalConfigVM : TSPropertyChanged, IDisposable
    {

        public event Action ShowAllChanged;

        private TSObservableCollection<MyAbnormalityVM> _myAbnormals;
        public ICollectionView AbnormalitiesView { get; set; }

        public bool ShowAll
        {
            get => App.Settings.BuffWindowSettings.ShowAll;
            set
            {
                if (App.Settings.BuffWindowSettings.ShowAll== value) return;
                App.Settings.BuffWindowSettings.ShowAll= value;
                Dispatcher.Invoke(() => ShowAllChanged?.Invoke());
                App.Settings.Save();
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
            _myAbnormals = new TSObservableCollection<MyAbnormalityVM>(Dispatcher);
            foreach (var abnormality in Game.DB.AbnormalityDatabase.Abnormalities.Values.Where(a => a.IsShow && AbnormalityUtils.Pass(a)))
            {
                var abVM = new MyAbnormalityVM(abnormality);

                _myAbnormals.Add(abVM);
            }

            AbnormalitiesView = CollectionViewUtils.InitView(_myAbnormals);
        }

        public void Dispose()
        {
            AbnormalitiesView.Free();
        }
    }
}