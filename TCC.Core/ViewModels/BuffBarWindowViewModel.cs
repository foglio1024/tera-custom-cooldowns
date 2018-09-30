using System.Windows;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public class BuffBarWindowViewModel : TccWindowViewModel
    {
        private static BuffBarWindowViewModel _instance;
        public static BuffBarWindowViewModel Instance => _instance ?? (_instance = new BuffBarWindowViewModel());

        public FlowDirection Direction => Settings.BuffsDirection;
        public AbnormalityShape Shape => Settings.AbnormalityShape;
        public BuffBarWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Player = new Player();
        }

        public Player Player { get; set; }
    }


}
