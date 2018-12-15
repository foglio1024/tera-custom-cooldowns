using System.Windows;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Pc;

namespace TCC.ViewModels
{
    public class BuffBarWindowViewModel : TccWindowViewModel
    {
        private static BuffBarWindowViewModel _instance;
        public static BuffBarWindowViewModel Instance => _instance ?? (_instance = new BuffBarWindowViewModel());

        public FlowDirection Direction => Settings.SettingsHolder.BuffsDirection;
        public ControlShape Shape => Settings.SettingsHolder.AbnormalityShape;
        public BuffBarWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            SessionManager.CurrentPlayer.PropertyChanged += CurrentPlayer_PropertyChanged;
            //Player = new Player();
        }

        private void CurrentPlayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            N(e.PropertyName);
        }

        public Player Player => SessionManager.CurrentPlayer;
        //public Player Player { get; set; }
    }


}
