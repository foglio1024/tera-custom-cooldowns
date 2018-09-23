
using System;
using TCC.Data;
using TCC.Parsing.Messages;

namespace TCC
{
    public static class FlyingGuardianDataProvider
    {
        private static uint AirEssenceID = 630400;
        private static uint FireEssenceID = 630500;
        private static uint SparkEssenceID = 631001;

        private static int _stacks = 0;
        private static FlightStackType _stackType;

        public static event Action<int> StacksChanged;
        public static event Action<FlightStackType> StackTypeChanged;
        public static event Action<bool> IsInProgressChanged;

        public static int Stacks
        {
            get => _stacks;
            set
            {
                if (_stacks == value) return;
                _stacks = value;
                StacksChanged?.Invoke(_stacks);
            }
        }

        public static FlightStackType StackType
        {
            get => _stackType;
            set
            {
                if(_stackType == value) return;
                _stackType = value;
                StackTypeChanged?.Invoke(_stackType);
            }
        }

        private static FlightStackType IdToStackType(uint id)
        {
            if (id == FireEssenceID) return FlightStackType.Fire;
            if (id == SparkEssenceID) return FlightStackType.Spark;
            if (id == AirEssenceID) return FlightStackType.Air;
            return FlightStackType.None;
        }
        private static bool IsEssence(uint id)
        {
            return id == AirEssenceID || id == FireEssenceID || id == SparkEssenceID;
        }

        public static bool IsInProgress => EntitiesManager.IsEntitySpawned(630, 9998) ||
                                           EntitiesManager.IsEntitySpawned(630, 2100) ||
                                           EntitiesManager.IsEntitySpawned(630, 2101) ||
                                           EntitiesManager.IsEntitySpawned(630, 2102) ||
                                           EntitiesManager.IsEntitySpawned(630, 2103) ||
                                           EntitiesManager.IsEntitySpawned(630, 2104) ||
                                           EntitiesManager.IsEntitySpawned(631, 1001) ||
                                           EntitiesManager.IsEntitySpawned(631, 1002) ||
                                           EntitiesManager.IsEntitySpawned(631, 3001) ||
                                           EntitiesManager.IsEntitySpawned(631, 9998);

        public static void InvokeProgressChanged()
        {
            IsInProgressChanged?.Invoke(IsInProgress);
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

        private static bool _ignoreNextEnd;
    }
}
