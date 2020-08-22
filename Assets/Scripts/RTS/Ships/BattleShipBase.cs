using UnityEngine;
using RTS.Controls;

namespace RTS.Ships
{
    public class BattleshipBase : MonoBehaviour, IMoveable, IDamageable
    {
        [Header("General")] 
        [SerializeField] private bool isFriend; 
        
        [Header("Movement")]
        [SerializeField] private float movementSpeed = 1f;
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private float reachedDistOffset = 1f;
        [SerializeField] private float slowDownCoef = 0f;

        private Rigidbody _rigidbody;

        private Vector3 _targetMovePos;
        private Vector3 _moveDirection;
        private Vector3 _movement;
        private float _moveThrust;
        private bool _isReachedDestination;

        public bool IsFriend => isFriend;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _isReachedDestination = true;
        }

        private void FixedUpdate()
        {
            if (!_isReachedDestination)
            {
                UpdateMoving();
                UpdateRotating();
            }
            else
            {
                SlowDown();
            }
        }

        private void SlowDown()
        {
            _movement *= slowDownCoef;
            _rigidbody.AddForce(_movement);
        }
        
        private void UpdateMoving()
        {
            var distToTarget = Vector3.Distance(transform.position, _targetMovePos);
            if (distToTarget > reachedDistOffset)
            {
                _moveDirection = (_targetMovePos - transform.position).normalized;
                _moveThrust = Vector3.Dot(_moveDirection.normalized, transform.forward);

                if (_moveThrust < 0) return;
                _movement = transform.forward * (_moveThrust * _moveDirection.magnitude * movementSpeed);
                _rigidbody.AddForce(_movement);
            }
            else
                _isReachedDestination = true;
        }

        private void UpdateRotating()
        {
            if (_moveThrust >= 0.999f || _isReachedDestination) return;

            float rotationAmount;
            var rotation = Vector3.Dot(_moveDirection.normalized, transform.right);
            if (rotation < 0)
                rotationAmount = -rotationSpeed;
            else
                rotationAmount = rotationSpeed;
            
            _rigidbody.AddTorque(0, rotationAmount, 0);
        }

        #region IMoveable Implementation

        public void MoveToPositon(Vector3 position)
        {
            _targetMovePos = position;
            _isReachedDestination = false;
        }

        #endregion
    }
}
