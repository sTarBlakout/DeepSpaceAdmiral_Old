using System;
using UnityEngine;
using GameGlobal;
using RTS.Controls;

namespace RTS.Ships
{
    public class Battleship : MonoBehaviour, IMoveable, IDamageable, IAttackable, ISelectable
    {
        #region Data

        [Header("General")] 
        [SerializeField] private bool isFriend; 
        
        [Header("Movement")]
        [SerializeField] private float movementSpeed = 1f;
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private float reachedDistOffset = 1f;
        [SerializeField] private float slowDownCoef;

        [Header("Visuals")] 
        [SerializeField] private GameObject selectedMarker;

        private const float SLOW_DOWN_END_PREC = 0.1f;
        private const float FACING_TARGET_PREC = 0.9999f;
        
        private Stance _stance;
        private Stance _stanceToSwitch;

        private Rigidbody _rigidbody;

        private WeaponManager _weaponManager;

        private Vector3 _targetMovePos;
        private Vector3 _moveDirection;
        private Vector3 _movement;
        private float _dotForward;
        private bool _isReachedDestination;

        private MonoBehaviour _currTarget;

        #endregion

        #region Unity Events
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _weaponManager = transform.GetComponentInChildren<WeaponManager>();
        }

        private void Start()
        {
            _stanceToSwitch = Stance.Empty;
            _stance = Stance.Idle;
            _isReachedDestination = true;
            
            if (isFriend)
                _weaponManager.InitWeaponSystem(FACING_TARGET_PREC);
            
            if (isFriend)
            {
                selectedMarker.transform.Translate(Vector3.down * GlobalData.Instance.RtsShipsPosY, Space.World);
                selectedMarker.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
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
                
                case Stance.Empty:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        #endregion

        #region Attack Logic

        private void AttackTargetBehavior()
        {
            var distToTarget = Vector3.Distance(transform.position, _currTarget.transform.position);
            if (distToTarget > _weaponManager.AttackRange)
            {
                _weaponManager.UpdateWeaponSystem(false);
                _targetMovePos = _currTarget.transform.position;
                MoveToPositon(_targetMovePos, _stance);
                return;
            }

            var rotation = Vector3.zero;
            switch (_weaponManager.MainWeaponType)
            {
                case MainWeaponType.Front:
                    rotation = (_currTarget.transform.position - transform.position).normalized;
                    break;
                
                case MainWeaponType.Sides:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            UpdateRotating(rotation, out _dotForward);
            _weaponManager.UpdateWeaponSystem(true, _dotForward, _currTarget);
        }

        private void ResetAttackStance()
        {
            _weaponManager.UpdateWeaponSystem(false);
        }
        
        #endregion

        #region Moving Logic
        
        private void MoveToPositionBehavior()
        {
            if (!_isReachedDestination)
            {
                if (_stanceToSwitch != Stance.Empty)
                {
                    var distToTarget = Vector3.Distance(transform.position, _currTarget.transform.position);
                    if (distToTarget <= _weaponManager.AttackRange)
                        _isReachedDestination = true;
                }
                
                UpdateMoving();
                UpdateRotating(_moveDirection, out _dotForward);
            }
            else
            {
                if (!GlobalData.VectorsApproxEqual(_rigidbody.velocity, Vector3.zero, SLOW_DOWN_END_PREC))
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
                var shipTransform = transform;
                _moveDirection = (_targetMovePos - shipTransform.position).normalized;
                var moveThrust = Vector3.Dot(_moveDirection.normalized, shipTransform.forward);

                if (moveThrust < 0) return;
                _movement = transform.forward * (moveThrust * _moveDirection.magnitude * movementSpeed);
                _rigidbody.AddForce(_movement);
            }
            else
                _isReachedDestination = true;
        }

        private void UpdateRotating(Vector3 rotateTo, out float dotForward)
        {
            dotForward = Vector3.Dot(rotateTo.normalized, transform.forward);
            if (dotForward >= FACING_TARGET_PREC) return;

            float rotationAmount;
            var rotation = Vector3.Dot(rotateTo.normalized, transform.right);
            if (rotation < 0)
                rotationAmount = -rotationSpeed;
            else
                rotationAmount = rotationSpeed;
            
            _rigidbody.AddTorque(0, rotationAmount, 0);
        }
        
        #endregion

        #region IMoveable Implementation

        public void MoveToPositon(Vector3 position, Stance stance = Stance.MoveToPosition)
        {
            ResetAttackStance();
            
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

        #region IDamageable Implementation

        public bool IsFriend => isFriend;
        
        public void Damage(float damage)
        {
            Debug.Log(gameObject.name + " got " + damage + " damage!");
        }

        #endregion
    }
}
