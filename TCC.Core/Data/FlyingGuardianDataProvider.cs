using System;
using TCC.Utilities;
using TeraPacketParser.Messages;

namespace TCC.Data
{
    public static class FlyingGuardianDataProvider
    {
        private const uint AirEssenceId = 630400;
        private const uint FireEssenceId = 630500;
        private const uint SparkEssenceId = 631001;

        private static int _stacks;
        private static FlightStackType _stackType;
        private static bool _ignoreNextEnd;

        public static event Action StacksChanged;
        public static event Action StackTypeChanged;
        public static event Action IsInProgressChanged;

        public static int Stacks
        {
            get => _stacks;
            set
            {
                if (_stacks == value) return;
                _stacks = value;
                StacksChanged?.Invoke();
            }
        }
        public static FlightStackType StackType
        {
            get => _stackType;
            set
            {
                if(_stackType == value) return;
                _stackType = value;
                StackTypeChanged?.Invoke();
            }
        }
        public static bool IsInProgress => TccUtils.IsEntitySpawned(630, 9998) ||
                                           TccUtils.IsEntitySpawned(630, 2100) ||
                                           TccUtils.IsEntitySpawned(630, 2101) ||
                                           TccUtils.IsEntitySpawned(630, 2102) ||
                                           TccUtils.IsEntitySpawned(630, 2103) ||
                                           TccUtils.IsEntitySpawned(630, 2104) ||
                                           TccUtils.IsEntitySpawned(631, 1001) ||
                                           TccUtils.IsEntitySpawned(631, 1002) ||
                                           TccUtils.IsEntitySpawned(631, 3001) ||
                                           TccUtils.IsEntitySpawned(631, 9998);

        public static void InvokeProgressChanged()
        {
            IsInProgressChanged?.Invoke();
        }
        public static void HandleAbnormal(S_ABNORMALITY_END p)
        {
            if (!IsEssence(p.AbnormalityId)) return;
            if (_ignoreNextEnd)
            {
                _ignoreNextEnd = false;
                return;
            }
            Stacks = 0;
        }
        public static void HandleAbnormal(S_ABNORMALITY_REFRESH p)
        {
            if (!IsEssence(p.AbnormalityId)) return;
            Stacks = p.Stacks;
            StackType = IdToStackType(p.AbnormalityId);
        }
        public static void HandleAbnormal(S_ABNORMALITY_BEGIN p)
        {
            if (!IsEssence(p.AbnormalityId)) return;
            if (IdToStackType(p.AbnormalityId) != StackType) _ignoreNextEnd = true;
            Stacks = p.Stacks;
            StackType = IdToStackType(p.AbnormalityId);
        }

        private static FlightStackType IdToStackType(uint id)
        {
            return id switch
            {
                FireEssenceId => FlightStackType.Fire,
                SparkEssenceId => FlightStackType.Spark,
                AirEssenceId => FlightStackType.Air,
                _ => FlightStackType.None
            };
        }
        private static bool IsEssence(uint id)
        {
            return id == AirEssenceId || id == FireEssenceId || id == SparkEssenceId;
        }

    }
}
