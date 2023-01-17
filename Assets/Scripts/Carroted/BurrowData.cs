using UnityEngine;

namespace Carroted
{
    [CreateAssetMenu(fileName = "Burrow Data", menuName = "Carroted/Burrow Data")]
    public class BurrowData : ScriptableObject
    {
        public float[] ZonesRadiuses;
        public float[] ZonesCooldowns;

        [Tooltip("If false, act like there is no timer (for menu)")]
        public bool GameTimer;
        [Tooltip("0 = normal spawn, higher = more spawn, 1 = max spawn & only golden carrots")]
        public AnimationCurve SpawnProgressionCurve;
        public int maxCarrotSpawnAtOnce;
        public float carrotSpawnProbabilityMultiplier;
    }
}
