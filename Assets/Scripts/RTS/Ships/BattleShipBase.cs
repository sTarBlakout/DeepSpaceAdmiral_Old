using System;
using GameGlobal;
using UnityEngine;
using RTS.Controls;

namespace RTS.Ships
{
    public class BattleshipBase : MonoBehaviour, IMoveable, IDamageable, IAttackable, ISelectable
    {
        [Header("General")] 
        [SerializeField] private bool isFriend; 
        
        [Header("Movement")]
        [SerializeField] private float movementSpeed = 1f;
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private float reachedDistOffset = 1f;
        [SerializeField] private float slowDownCoef = 0f;

        [Header("Attack")] 
        [SerializeField] private float attackRange = 1f;

        [Header("Visuals")] 
        [SerializeField] private GameObject selectedMarker;

        private const float SlowDownEndPrec = 0.1f;
        
        private Stance _stance;
        private Stance _stanceToSwitch;

        private Rigidbody _rigidbody;

        private Vector3 _targetMovePos;
        private Vector3 _moveDirection;
        private Vector3 _movement;
        private float _moveThrust;
        private bool _isReachedDestination;

        private MonoBehaviour _currTarget;

        public bool IsFriend => isFriend;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _stanceToSwitch = Stance.Empty;
            _stance = Stance.Idle;
            _isReachedDestination = true;

            if (isFriend)
            {
                selectedMarker.transform.Translate(Vector3.down * GlobalData.Instance.RtsShipsPosY, Space.World);
                selectedMarker.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            if (isFriend)
                Debug.Log(_stance + " " + _stanceToSwitch);
            switch (_stance)
            {
                case Stance.Idle:
                    break;
                case Stance.MoveToPosition:
                    MoveToPositionBehavior();
                    break;
                case Stance.AttackTarget:
                    AttackTargetBehavior();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AttackTargetBehavior()
        {
            var distToTarget = Vector3.Distance(transform.position, _currTarget.transform.position);
            if (distToTarget > attackRange)
            {
                _targetMovePos = _currTarget.transform.position;
                MoveToPositon(_targetMovePos, _stance);
            }
        }

        private void MoveToPositionBehavior()
        {
            if (!_isReachedDestination)
            {
                UpdateMoving();
                UpdateRotating();
            }
            else
            {
                if (!GlobalData.VectorsApproxEqual(_rigidbody.velocity, Vector3.zero, SlowDownEndPrec))
                    SlowDown();
                else
                {
                    _rigidbody.velocity = Vector3.zero;
                    if (_stanceToSwitch == Stance.Empty)
                    { 
                        _stance = Stance.Idle;
                    }
                    else
                    {
                        _stance = _stanceToSwitch;
                        _stanceToSwitch = Stance.Empty;
                    }
                }
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

        public void MoveToPositon(Vector3 position, Stance stance = Stance.MoveToPosition)
        {
            _targetMovePos = position;
            _isReachedDestination = false;
            _stance = Stance.MoveToPosition;
            if (stance != Stance.MoveToPosition)
            {
                _stanceToSwitch = stance;
            }
        }

        #endregion
        
        #region IAttackable Implementation

        public void AttackTarget(MonoBehaviour target)
        {
            _currTarget = target;
            _stance = Stance.AttackTarget;
        }
        
        #endregion

        #region ISelectable Implementation
        
        public void Select()
        {
            selectedMarker.SetActive(true);
        }

        public void Unselect()
        {
            selectedMarker.SetActive(false);
        }
        
        #endregion
    }
}
