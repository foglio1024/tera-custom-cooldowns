using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using Nostrum.Factories;
using TCC.Data;
using TCC.Data.Pc;
using TCC.Settings;
using TCC.Utils;
using TCC.Windows;

namespace TCC.ViewModels.Widgets
{
    //NOTE: hooks handled by Game
    [TccModule]
    public class AbnormalityWindowViewModel : TccWindowViewModel
    {
        public Player Player => Game.Me;
        public FlowDirection Direction => ((BuffWindowSettings)Settings).Direction;
        public ControlShape Shape => App.Settings.AbnormalityShape;
        public ICollectionViewLiveShaping BuffsView { get; }
        public ICollectionViewLiveShaping SpecBuffsView { get; }
        public ICollectionViewLiveShaping InfBuffsView { get; }
        public ICollectionViewLiveShaping SpecInfBuffsView { get; }

        public AbnormalityWindowViewModel(WindowSettings settings) : base(settings)
        {
            Player.InitAbnormalityCollections(Dispatcher);

            ((BuffWindowSettings)settings).DirectionChanged += () => ExN(nameof(Direction));

            BuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.Buffs, a => a.Abnormality.Type != AbnormalityType.Special);
            SpecBuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.Buffs, a => a.Abnormality.Type == AbnormalityType.Special);
            InfBuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.InfBuffs, a => a.Abnormality.Type != AbnormalityType.Special);
            SpecInfBuffsView = CollectionViewFactory.CreateLiveCollectionView(Player.InfBuffs, a => a.Abnormality.Type == AbnormalityType.Special);

            KeyboardHook.Instance.RegisterCallback(App.Settings.AbnormalSettingsHotkey, OnShowAbnormalConfigHotkeyPressed);

        }

        private void OnShowAbnormalConfigHotkeyPressed()
        {
            //if (!Game.Logged) return;
            Dispatcher.InvokeAsync(() =>
            {
                MyAbnormalConfigWindow.Instance.ShowWindow();
            },
            DispatcherPriority.Background);
        }
    }


}
