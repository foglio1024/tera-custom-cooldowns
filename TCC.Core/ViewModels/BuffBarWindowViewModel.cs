using System;
using System.Windows;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Pc;
using TCC.Parsing;
using TeraPacketParser.Messages;

namespace TCC.ViewModels
{
    [TccModule]
    public class BuffBarWindowViewModel : TccWindowViewModel
    {
        public Player Player => Session.Me;
        public FlowDirection Direction => App.Settings.BuffWindowSettings.Direction;
        public ControlShape Shape => App.Settings.AbnormalityShape;

        public BuffBarWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Player.InitAbnormalityCollections(Dispatcher);
        }

        protected override void InstallHooks()
        {
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_BEGIN>(p =>
            {
                AbnormalityManager.BeginAbnormality(p.AbnormalityId, p.TargetId, p.CasterId, p.Duration, p.Stacks);
            });
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_REFRESH>(p =>
            {
                AbnormalityManager.BeginAbnormality(p.AbnormalityId, p.TargetId, p.TargetId, p.Duration, p.Stacks);
            });
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_END>(p =>
            {
                AbnormalityManager.EndAbnormality(p.TargetId, p.AbnormalityId);
            });
        }
    }


}
