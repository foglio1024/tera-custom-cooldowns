using Nostrum;
using Nostrum.WPF.ThreadSafe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using TCC.Data.Abnormalities;
using TeraDataLite;

namespace TCC.ViewModels;

public class GroupConfigVM : ThreadSafeObservableObject
{

    public event Action? ShowAllChanged;

    public ThreadSafeObservableCollection<GroupAbnormalityVM> GroupAbnormals;
    public IEnumerable<Abnormality> Abnormalities => Game.DB!.AbnormalityDatabase.Abnormalities.Values.ToList();
    public ICollectionView AbnormalitiesView { get; set; }

    public bool ShowAll
    {
        get => App.Settings.GroupWindowSettings.ShowAllAbnormalities;
        set
        {
            if (App.Settings.GroupWindowSettings.ShowAllAbnormalities == value) return;
            App.Settings.GroupWindowSettings.ShowAllAbnormalities = value;
            _dispatcher.Invoke(() => ShowAllChanged?.Invoke());
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
    public GroupConfigVM()
    {
        GroupAbnormals = new ThreadSafeObservableCollection<GroupAbnormalityVM>(_dispatcher);
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
    void OnAbnormalitiesViewOnCurrentChanged(object? s, EventArgs ev)
    {
    }
}