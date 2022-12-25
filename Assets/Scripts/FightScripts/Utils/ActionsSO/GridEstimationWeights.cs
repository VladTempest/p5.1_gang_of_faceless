using System;

namespace Editor.Scripts.Utils
{
    [Serializable]
    public struct GridEstimationWeights
    {
        public float healthLeftWeight;
        public float playerCharacterStatusWeight;
        public float pathLengthWeight;
        public float actionPointCostWeight;
        public float coolDownWeight;
        public float normalizedDamageWeight;
        public float additionalEffectsWeight;
    }
}