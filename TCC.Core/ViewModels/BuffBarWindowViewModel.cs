using System.Windows;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Pc;

namespace TCC.ViewModels
{
    public class BuffBarWindowViewModel : TccWindowViewModel
    {
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
