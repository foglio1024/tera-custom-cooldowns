using System.Windows;
using TCC.Data;
using TCC.Data.Pc;
using TCC.Parsing;
using TCC.Settings;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets
{
    [TccModule]
    public class BuffBarWindowViewModel : TccWindowViewModel
    {
        public Player Player => Session.Me;
        public FlowDirection Direction => App.Settings.BuffWindowSettings.Direction;
        public ControlShape Shape => App.Settings.AbnormalityShape;

        public BuffBarWindowViewModel(WindowSettings settings) : base(settings)
        {
            Player.InitAbnormalityCollections(Dispatcher);
        }

        protected override void InstallHooks()
        {
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_BEGIN>(OnAbnormalityBegin);
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_REFRESH>(OnAbnormalityRefresh);
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_END>(OnAbnormalityEnd);
        }

        protected override void RemoveHooks()
        {
            PacketAnalyzer.NewProcessor.Unhook<S_ABNORMALITY_BEGIN>(OnAbnormalityBegin);
            PacketAnalyzer.NewProcessor.Unhook<S_ABNORMALITY_REFRESH>(OnAbnormalityRefresh);
            PacketAnalyzer.NewProcessor.Unhook<S_ABNORMALITY_END>(OnAbnormalityEnd);
        }

        //TODO: take only buff bar part from these methods and call them directly
        private void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            AbnormalityManager.BeginAbnormality(p.AbnormalityId, p.TargetId, p.CasterId, p.Duration, p.Stacks);
        }
        private void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            AbnormalityManager.BeginAbnormality(p.AbnormalityId, p.TargetId, p.TargetId, p.Duration, p.Stacks);
        }
        private void OnAbnormalityEnd(S_ABNORMALITY_END p)
        {
            AbnormalityManager.EndAbnormality(p.TargetId, p.AbnormalityId); 
        }
    }


}
