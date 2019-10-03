using System.Windows;
using TCC.Data;
using TCC.Data.Pc;
using TCC.Settings;

namespace TCC.ViewModels.Widgets
{
    //NOTE: hooks handled by Game
    [TccModule]
    public class BuffBarWindowViewModel : TccWindowViewModel
    {
        public Player Player => Game.Me;
        public FlowDirection Direction => ((BuffWindowSettings)Settings).Direction;
        public ControlShape Shape => App.Settings.AbnormalityShape;

        public BuffBarWindowViewModel(WindowSettings settings) : base(settings)
        {
            Player.InitAbnormalityCollections(Dispatcher);

            ((BuffWindowSettings) settings).DirectionChanged += () => ExN(nameof(Direction));
        }

    }


}
