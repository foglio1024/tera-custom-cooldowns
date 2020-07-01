/* Add My Abnormals Setting by HQ
GroupConfigVM               -> MyAbnormalConfigVM
GroupAbnormalityVM          -> MyAbnormalityVM
GroupAbnormals              -> MyAbnormals

ShowAllGroupAbnormalities   -> ShowAllMyAbnormalities
*/
using Nostrum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using Nostrum.Extensions;
using Nostrum.Factories;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class MyAbnormalConfigVM : TSPropertyChanged, IDisposable
    {
        public event Action ShowAllChanged = null!;

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
            var myAbnormals = new TSObservableCollection<MyAbnormalityVM>(Dispatcher);
            foreach (var abnormality in Game.DB.AbnormalityDatabase.Abnormalities.Values.Where(a => a.IsShow && a.CanShow))
            {
                var abVM = new MyAbnormalityVM(abnormality);

                myAbnormals.Add(abVM);
            }

            AbnormalitiesView = CollectionViewFactory.CreateCollectionView(myAbnormals);
        }

        public void Dispose()
        {
            AbnormalitiesView.Free();
        }
    }
}