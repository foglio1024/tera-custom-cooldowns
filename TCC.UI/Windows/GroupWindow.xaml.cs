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
using TCC.UI_elements;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC.Converters
{
    public class ClassToFillConverter : IValueConverter
    {
        SolidColorBrush healerColor = new SolidColorBrush(Color.FromRgb(79, 255, 142));
        SolidColorBrush tankColor = new SolidColorBrush(Color.FromRgb(79, 190, 255));
        SolidColorBrush dpsColor = new SolidColorBrush(Color.FromRgb(255, 100, 70));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (Class)value;
            switch (c)
            {
                case Class.Lancer:
                    return tankColor;
                case Class.Fighter:
                    return tankColor;
                case Class.Priest:
                    return healerColor;
                case Class.Elementalist:
                    return healerColor;
                default:
                    return dpsColor;
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
            InitWindow();
            dps.DataContext = GroupWindowManager.Instance.Dps;
            dps.ItemsSource = GroupWindowManager.Instance.Dps;
            tanks.DataContext = GroupWindowManager.Instance.Tanks;
            tanks.ItemsSource = GroupWindowManager.Instance.Tanks;
            healers.DataContext = GroupWindowManager.Instance.Healers;
            healers.ItemsSource = GroupWindowManager.Instance.Healers;

            //add settings
            Left = SettingsManager.GroupWindowSettings.X;
            Top = SettingsManager.GroupWindowSettings.Y;
            Visibility = SettingsManager.GroupWindowSettings.Visibility;
            SetClickThru(SettingsManager.GroupWindowSettings.ClickThru);
        }

        private void TccWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            //add settings
            SettingsManager.GroupWindowSettings.X = Left;
            SettingsManager.GroupWindowSettings.Y = Top;
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
                    foreach (var dps in GroupWindowManager.Instance.Dps)
                    {
                        foreach (var buff in dps.Buffs)
                        {
                            buff.IconSize = AbnormalityManager.RAID_AB_SIZE*.9;
                            buff.BackgroundEllipseSize = AbnormalityManager.RAID_AB_SIZE;
                            buff.IndicatorMargin = new Thickness(AbnormalityManager.RAID_AB_LEFT_MARGIN, 1, 1, 1);
                        }
                    }
                    foreach (var tank in GroupWindowManager.Instance.Tanks)
                    {
                        foreach (var buff in tank.Buffs)
                        {
                            buff.IconSize = AbnormalityManager.RAID_AB_SIZE * .9;
                            buff.BackgroundEllipseSize = AbnormalityManager.RAID_AB_SIZE;
                            buff.IndicatorMargin = new Thickness(AbnormalityManager.RAID_AB_LEFT_MARGIN, 1, 1, 1);
                        }
                    }
                    foreach (var heal in GroupWindowManager.Instance.Healers)
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
                    foreach (var dps in GroupWindowManager.Instance.Dps)
                    {
                        foreach (var buff in dps.Buffs)
                        {
                            buff.IconSize = AbnormalityManager.PARTY_AB_SIZE * .9;
                            buff.BackgroundEllipseSize = AbnormalityManager.PARTY_AB_SIZE;
                            buff.IndicatorMargin = new Thickness(AbnormalityManager.PARTY_AB_LEFT_MARGIN, 1, 1, 1);
                        }
                    }
                    foreach (var tank in GroupWindowManager.Instance.Tanks)
                    {
                        foreach (var buff in tank.Buffs)
                        {
                            buff.IconSize = AbnormalityManager.PARTY_AB_SIZE * .9;
                            buff.BackgroundEllipseSize = AbnormalityManager.PARTY_AB_SIZE;
                            buff.IndicatorMargin = new Thickness(AbnormalityManager.PARTY_AB_LEFT_MARGIN, 1, 1, 1);
                        }
                    }
                    foreach (var heal in GroupWindowManager.Instance.Healers)
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
