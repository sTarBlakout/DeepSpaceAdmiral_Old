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
        [SerializeField] private float maxMovementSpeed = 1f;
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private float reachedDistOffset = 1f;
        [SerializeField] private float slowDownCoef;
        [SerializeField] private float accelerationCoef;

        [Header("Visuals")] 
        [SerializeField] private GameObject selectedMarker;
        
        private float _slowDownEndPrec;
        private float _facingTargetPrec;

        private Stance _stance;
        private Stance _stanceToSwitch;

        private Rigidbody _rigidbody;

        private WeaponManager _weaponManager;
        private EngineManager _engineManager;

        private Vector3 _targetMovePos;
        private Vector3 _moveDirection;
        private Vector3 _movement;
        private float _currSpeed;
        private float _dotForward;
        private float _dotSide;
        private bool _isReachedDestination;
        private bool _isShipMoving;

        private MonoBehaviour _currTarget;

        #endregion

        #region Unity Events
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _weaponManager = transform.GetComponentInChildren<WeaponManager>();
            _engineManager = transform.GetComponentInChildren<EngineManager>();

            _slowDownEndPrec = GlobalData.Instance.BattleshipSlowDownEndPrec;
            _facingTargetPrec = GlobalData.Instance.BattleshipFacingTargetPrec;
        }

        private void Start()
        {
            _stanceToSwitch = Stance.Empty;
            _stance = Stance.Idle;
            _isReachedDestination = true;

            if (isFriend)
            {
                _weaponManager.InitWeaponSystem(_facingTargetPrec);
            }

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

            if (isFriend)
            {
                _isShipMoving = !GlobalData.VectorsApproxEqual(_rigidbody.velocity, Vector3.zero, _slowDownEndPrec);
                _engineManager.UpdateEngines(_dotForward, _dotSide, _isShipMoving);
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
            
            UpdateRotating(rotation, out _dotForward, out _dotSide);
            _weaponManager.UpdateWeaponSystem(true, _dotForward, _currTarget);
        }

        #endregion

        #region Moving Logic
        
        private void MoveToPositionBehavior()
        {
            if (!_isReachedDestination)
            {
                if (_stanceToSwitch == Stance.AttackTarget)
                {
                    _targetMovePos = _currTarget.transform.position;
                    MoveToPositon(_targetMovePos, _stance);
                    var distToTarget = Vector3.Distance(transform.position, _targetMovePos);
                    if (distToTarget <= _weaponManager.AttackRange)
                        _isReachedDestination = true;
                }
                
                UpdateMoving();
                UpdateRotating(_moveDirection, out _dotForward, out _dotSide);
            }
            else
            {
                if (_isShipMoving)
                    SlowDown();
                else
                {
                    _rigidbody.velocity = Vector3.zero;
                    _currSpeed = 0f;
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
            _currSpeed = Mathf.Max(_currSpeed - accelerationCoef, 0f);
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
                _currSpeed = Mathf.Min(_currSpeed + accelerationCoef, maxMovementSpeed);
                _movement = transform.forward * (moveThrust * _moveDirection.magnitude * _currSpeed);
                _rigidbody.AddForce(_movement);
            }
            else
                _isReachedDestination = true;
        }

        private void UpdateRotating(Vector3 rotateTo, out float dotForward, out float dotSide)
        {
            dotForward = Vector3.Dot(rotateTo.normalized, transform.forward);
            dotSide = Vector3.Dot(rotateTo.normalized, transform.right);
            if (dotForward >= _facingTargetPrec) return;

            float rotationAmount;
            if (dotSide < 0)
                rotationAmount = -rotationSpeed;
            else
                rotationAmount = rotationSpeed;
            
            _rigidbody.AddTorque(0, rotationAmount, 0);
        }
        
        #endregion

        #region IMoveable Implementation

        public void MoveToPositon(Vector3 position, Stance stance = Stance.MoveToPosition)
        {
            _targetMovePos = position;
            _isReachedDestination = false;
            _stance = Stance.MoveToPosition;
            if (stance != Stance.MoveToPosition)
            {
                _stanceToSwitch = stance;
                if (_stanceToSwitch != Stance.AttackTarget)
                {
                    _weaponManager.UpdateWeaponSystem(false);
                }
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
            // Debug.Log(gameObject.name + " got " + damage + " damage!");
        }

        #endregion
    }
}
