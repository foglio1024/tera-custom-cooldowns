using System.Windows;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Pc;

namespace TCC.ViewModels
{
    public class BuffBarWindowViewModel : TccWindowViewModel
    {
        //private static BuffBarWindowViewModel _instance;
        //public static BuffBarWindowViewModel Instance => _instance ?? (_instance = new BuffBarWindowViewModel());

        public FlowDirection Direction => App.Settings.BuffsDirection;
        public ControlShape Shape => App.Settings.AbnormalityShape;

        public BuffBarWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Player.InitAbnormalityCollections(Dispatcher);
        }

        public Player Player => SessionManager.CurrentPlayer;
    }


}
