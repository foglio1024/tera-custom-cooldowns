using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.ViewModels
{
    public class TestWindowViewModel : BaseINPC
    {
        public int CurrentChestsCount
        {
            get => EntitiesManager.chestList.Count;
        }
        public TestWindowViewModel()
        {
            EntitiesManager.chestList.CollectionChanged += ChestList_CollectionChanged;
        }

        private void ChestList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("CurrentChestsCount");
        }
    }
}
