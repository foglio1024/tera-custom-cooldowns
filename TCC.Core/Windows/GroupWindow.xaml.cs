using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;
using TCC.Data;
using TCC.Controls;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC.Converters
{
    public class ClassToFillConverter : IValueConverter
    {
        SolidColorBrush healerColor = new SolidColorBrush(Color.FromRgb(79, 255, 142));
        SolidColorBrush tankColor = new SolidColorBrush(Color.FromRgb(79, 190, 255));
        SolidColorBrush dpsColor = new SolidColorBrush(Color.FromRgb(255, 150, 70));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (Class)value;
            switch (c)
            {
                case Class.Lancer:
                    return App.Current.FindResource("Colors.GroupWindow.Tank");
                case Class.Fighter:
                    return App.Current.FindResource("Colors.GroupWindow.Tank");
                case Class.Priest:
                    return App.Current.FindResource("Colors.GroupWindow.Healer");
                case Class.Elementalist:
                    return App.Current.FindResource("Colors.GroupWindow.Healer");
                default:
                    return App.Current.FindResource("Colors.GroupWindow.Dps");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per GroupWindow.xaml
    /// </summary>
    public partial class GroupWindow : TccWindow
    {
        public GroupWindow()
        {
            InitializeComponent();
        }
        private void TccWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow(SettingsManager.GroupWindowSettings);
            //dps.DataContext = GroupWindowViewModel.Instance.Dps;
            //dps.ItemsSource = GroupWindowViewModel.Instance.Dps;
            //tanks.DataContext = GroupWindowViewModel.Instance.Tanks;
            //tanks.ItemsSource = GroupWindowViewModel.Instance.Tanks;
            //healers.DataContext = GroupWindowViewModel.Instance.Healers;
            //healers.ItemsSource = GroupWindowViewModel.Instance.Healers;
        }


        private void TccWindow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }

        public void SwitchTemplate(bool bigGroup)
        {
            Dispatcher.Invoke(() =>
            {
                if (bigGroup)
                {
                    foreach (var dps in GroupWindowViewModel.Instance.Dps)
                    {
                        foreach (var buff in dps.Buffs)
                        {
                            buff.IconSize = AbnormalityManager.RAID_AB_SIZE*.9;
                            buff.BackgroundEllipseSize = AbnormalityManager.RAID_AB_SIZE;
                            buff.IndicatorMargin = new Thickness(AbnormalityManager.RAID_AB_LEFT_MARGIN, 1, 1, 1);
                        }
                    }
                    foreach (var tank in GroupWindowViewModel.Instance.Tanks)
                    {
                        foreach (var buff in tank.Buffs)
                        {
                            buff.IconSize = AbnormalityManager.RAID_AB_SIZE * .9;
                            buff.BackgroundEllipseSize = AbnormalityManager.RAID_AB_SIZE;
                            buff.IndicatorMargin = new Thickness(AbnormalityManager.RAID_AB_LEFT_MARGIN, 1, 1, 1);
                        }
                    }
                    foreach (var heal in GroupWindowViewModel.Instance.Healers)
                    {
                        foreach (var buff in heal.Buffs)
                        {
                            buff.IconSize = AbnormalityManager.RAID_AB_SIZE * .9;
                            buff.BackgroundEllipseSize = AbnormalityManager.RAID_AB_SIZE;
                            buff.IndicatorMargin = new Thickness(AbnormalityManager.RAID_AB_LEFT_MARGIN, 1, 1, 1);
                        }
                    }
                    dps.ItemTemplate = this.FindResource("raid") as DataTemplate;
                    tanks.ItemTemplate = this.FindResource("raid") as DataTemplate;
                    healers.ItemTemplate = this.FindResource("raid") as DataTemplate;
                }
                else
                {
                    foreach (var dps in GroupWindowViewModel.Instance.Dps)
                    {
                        foreach (var buff in dps.Buffs)
                        {
                            buff.IconSize = AbnormalityManager.PARTY_AB_SIZE * .9;
                            buff.BackgroundEllipseSize = AbnormalityManager.PARTY_AB_SIZE;
                            buff.IndicatorMargin = new Thickness(AbnormalityManager.PARTY_AB_LEFT_MARGIN, 1, 1, 1);
                        }
                    }
                    foreach (var tank in GroupWindowViewModel.Instance.Tanks)
                    {
                        foreach (var buff in tank.Buffs)
                        {
                            buff.IconSize = AbnormalityManager.PARTY_AB_SIZE * .9;
                            buff.BackgroundEllipseSize = AbnormalityManager.PARTY_AB_SIZE;
                            buff.IndicatorMargin = new Thickness(AbnormalityManager.PARTY_AB_LEFT_MARGIN, 1, 1, 1);
                        }
                    }
                    foreach (var heal in GroupWindowViewModel.Instance.Healers)
                    {
                        foreach (var buff in heal.Buffs)
                        {
                            buff.IconSize = AbnormalityManager.PARTY_AB_SIZE * .9;
                            buff.BackgroundEllipseSize = AbnormalityManager.PARTY_AB_SIZE;
                            buff.IndicatorMargin = new Thickness(AbnormalityManager.PARTY_AB_LEFT_MARGIN, 1, 1, 1);
                        }
                    }
                    dps.ItemTemplate = this.FindResource("party") as DataTemplate;
                    tanks.ItemTemplate = this.FindResource("party") as DataTemplate;
                    healers.ItemTemplate = this.FindResource("party") as DataTemplate;

                }
            });
        }
    }
}
