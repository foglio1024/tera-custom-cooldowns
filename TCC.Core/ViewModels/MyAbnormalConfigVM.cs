/* Add My Abnormals Setting by HQ
GroupConfigVM               -> MyAbnormalConfigVM
GroupAbnormalityViewModel          -> MyAbnormalityVM
GroupAbnormals              -> MyAbnormals

ShowAllGroupAbnormalities   -> ShowAllMyAbnormalities
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Nostrum;
using Nostrum.WPF.Extensions;
using Nostrum.WPF.Factories;
using Nostrum.WPF.ThreadSafe;
using TeraDataLite;

namespace TCC.ViewModels;

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
            _dispatcher.Invoke(() => ShowAllChanged?.Invoke());
            App.Settings.Save();
            InvokePropertyChanged();
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
        foreach (var abnormality in Game.DB!.AbnormalityDatabase.Abnormalities.Values.Where(a => a is { IsShow: true, CanShow: true }))
        {
            var abVM = new MyAbnormalityVM(abnormality) { Hidden = App.Settings.BuffWindowSettings.Hidden.Contains(abnormality.Id) };
            myAbnormals.Add(abVM);
        }

        AbnormalitiesView = CollectionViewFactory.CreateCollectionView(myAbnormals);
    }

    public void Dispose()
    {
        AbnormalitiesView.Free();
    }
}