//using System.Collections.Generic;

//namespace TCC.TeraCommon.Game.Abnormality
//{
//    public class PlayerAbnormals
//    {
//        private readonly Dictionary<NpcEntity, Death> _aggro;
//        public Death Death;
//        public Dictionary<HotDot, AbnormalityDuration> Times;

//        public PlayerAbnormals()
//        {
//            Times = new Dictionary<HotDot, AbnormalityDuration>();
//            Death = new Death();
//            _aggro = new Dictionary<NpcEntity, Death>();
//        }

//        public PlayerAbnormals(Dictionary<HotDot, AbnormalityDuration> times, Death death,
//            Dictionary<NpcEntity, Death> aggro)
//        {
//            Times = times;
//            Death = death;
//            _aggro = aggro;
//        }

//        public Death Aggro(NpcEntity entity)
//        {
//            Death death = null;
//            if (entity != null) _aggro.TryGetValue(entity, out death);
//            return death ?? new Death();
//        }
//    }
//}