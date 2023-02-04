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
using Nostrum.WPF.Factories;
using TeraDataLite;
using Nostrum.WPF.ThreadSafe;
using Nostrum.WPF.Extensions;

namespace TCC.ViewModels
{
    public class MyAbnormalConfigVM : ThreadSafeObservableObject, IDisposable
    {
        public event Action? ShowAllChanged;

        public ICollectionView AbnormalitiesView { get; set; }

        public bool ShowAll
        {
            get => App.Settings.BuffWindowSettings.ShowAll;
            set
            {
                if (App.Settings.BuffWindowSettings.ShowAll== value) return;
                App.Settings.BuffWindowSettings.ShowAll= value;
                _dispatcher?.Invoke(() => ShowAllChanged?.Invoke());
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
            var myAbnormals = new ThreadSafeObservableCollection<MyAbnormalityVM>(_dispatcher);
            foreach (var abnormality in Game.DB!.AbnormalityDatabase.Abnormalities.Values.Where(a => a.IsShow && a.CanShow))
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