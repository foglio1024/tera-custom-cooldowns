using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCC.Data;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per HarrowholdBossesControl.xaml
    /// </summary>
    public partial class HarrowholdBossesControl : UserControl
    {

        public HarrowholdBossesControl()
        {
            InitializeComponent();
            aquadrax.DataContext = null;
            terradrax.DataContext = null;
            ignidrax.DataContext = null;
            umbradrax.DataContext = null;
        }
        internal void Select(Dragon d)
        {
            //Dispatcher.Invoke(() =>
            //{
            //    try
            //    {
            //        foreach (var item in EntitiesManager.CurrentBosses.FirstOrDefault(x => x.Name == EntitiesManager.CurrentDragon.ToString()).Buffs)
            //        {
            //            item.Duration = item.DurationLeft;
            //        }
            //    }
            //    catch (Exception)
            //    { }

            //    switch (d)
            //    {
            //        case Dragon.Aquadrax:
            //            aquadrax.Opacity = 1;
            //            terradrax.Opacity = .6;
            //            ignidrax.Opacity = .6;
            //            umbradrax.Opacity = .6;
            //            try
            //            {
            //                abnormalities.ItemsSource = EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == aquadrax.EntityId).Buffs;
            //                abnormalities.DataContext = EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == aquadrax.EntityId).Buffs;
            //            }
            //            catch (Exception){ }

            //            break;
            //        case Dragon.Ignidrax:
            //            aquadrax.Opacity = .6;
            //            terradrax.Opacity = .6;
            //            ignidrax.Opacity = 1;
            //            umbradrax.Opacity = .6;
            //            try
            //            {
            //                abnormalities.ItemsSource = EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == ignidrax.EntityId).Buffs;
            //                abnormalities.DataContext = EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == ignidrax.EntityId).Buffs;
            //            }
            //            catch (Exception){}

            //            break;
            //        case Dragon.Umbradrax:
            //            aquadrax.Opacity = .6;
            //            terradrax.Opacity = .6;
            //            ignidrax.Opacity = .6;
            //            umbradrax.Opacity = 1;
            //            try
            //            {
            //                abnormalities.ItemsSource = EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == umbradrax.EntityId).Buffs;
            //                abnormalities.DataContext = EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == umbradrax.EntityId).Buffs;
            //            }
            //            catch (Exception){}

            //            break;
            //        case Dragon.Terradrax:
            //            aquadrax.Opacity = .6;
            //            terradrax.Opacity = 1;
            //            ignidrax.Opacity = .6;
            //            umbradrax.Opacity = .6;
            //            try
            //            {
            //                abnormalities.ItemsSource = EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == terradrax.EntityId).Buffs;
            //                abnormalities.DataContext = EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == terradrax.EntityId).Buffs;
            //            }
            //            catch (Exception){}
            //            break;
            //        default:
            //            abnormalities.ItemsSource = null;
            //            abnormalities.DataContext = null;
            //            break;
            //    }
            //});
        }
    }
}
