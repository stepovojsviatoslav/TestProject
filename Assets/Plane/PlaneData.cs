using UnityEngine;
namespace Plane
{
    [CreateAssetMenu(fileName = "PlaneData", menuName = "Plane Data")]
    public class PlaneData : ScriptableObject
    {
        public float LifeTimeSeconds;
        public float MinLifeTimeDeltaWillLand;
        public float MinDistanceForLanding;
        public float MaxLinearSpeed;
        public float MinLinearSpeed;
        public float MaxAngularSpeed;
        public float MinOtherPlaneDistance;
    }
}
