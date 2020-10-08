using Plane;
using UnityEngine;
namespace Ship
{
    [CreateAssetMenu( fileName = "ShipData", menuName ="Ship Data")]
    public class ShipData : ScriptableObject
    {
        public float MaxLinearSpeed;
        public float MaxAngularSpeed;
        public float LinearAcceleration;
        public float AngularAcceleration;
        public float WaterResistance;
        public int PlaneCount;
        public float PlaneDistance;
        public float PlaneRotationSpeed;
        public GameObject PlanePrefab;
        public PlaneData PlaneModel;
    }
}
