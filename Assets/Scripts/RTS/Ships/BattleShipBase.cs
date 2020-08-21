using UnityEngine;

namespace RTS.Ships
{
    public class BattleshipBase : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 1f;
        [SerializeField] private float rotationSpeed = 1f;

        private Rigidbody _rigidbody;
        
        private bool _isSelected;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }
}
