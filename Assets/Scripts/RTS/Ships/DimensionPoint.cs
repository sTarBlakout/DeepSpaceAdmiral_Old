using UnityEngine;

namespace RTS.Ships
{
    public class DimensionPoint : MonoBehaviour
    {
        [SerializeField] private DimensionPointPos position;

        public DimensionPointPos PositionName => position;
        public Vector3 Position => transform.position;
    }
}