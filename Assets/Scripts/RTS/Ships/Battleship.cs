using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameGlobal;
using RTS.Controls;
using RTS.Weapons;
using Random = UnityEngine.Random;

namespace RTS.Ships
{
    public class Battleship : MonoBehaviour, IMoveable, IDamageable, IAttackable, ISelectable
    {
        #region Data

        [Header("General")]
        [SerializeField] public bool isFriend; //public for testing purpose
        [SerializeField] private float maxHealthPoints;

        [Header("Movement")]
        [SerializeField] private float maxMovementSpeed = 1f;
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private float reachedDistOffset = 1f;
        [SerializeField] private float slowDownCoef;
        [SerializeField] private float accelerationCoef;

        [Header("Markers")] 
        [SerializeField] private ParticleSystem selectedMarker;

        [Header("Destruction")] 
        [SerializeField] private GameObject mainExplosion;
        [SerializeField] private GameObject[] destructLvlMeshes;
        [Range(0, 1)] [SerializeField] private float partsStayOnExplChance;

        private readonly List<ParticleManager> _mainExplosionParticles = new List<ParticleManager>();

        private float _slowDownEndPrec;
        private float _facingTargetPrec;

        private Stance _stance;
        private Stance _stanceToSwitch;
        private DestructionLevel _destrLvl;

        private Rigidbody _rigidbody;
        private Collider _mainCollider;

        private WeaponManager _weaponManager;
        private EngineManager _engineManager;

        private float _currHealthPoints;

        private Vector3 _targetMovePos;
        private Vector3 _moveDirection;
        private Vector3 _movement;
        private float _currSpeed;
        private float _dotForward;
        private float _dotSide;
        private bool _isReachedDestination;
        private bool _isShipMoving;

        private MonoBehaviour _currTarget;
        private IDamageable _currTargetDamageable;

        #endregion

        #region Unity Events
        
        private void Awake()
        {
            _mainCollider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
            _weaponManager = transform.GetComponentInChildren<WeaponManager>();
            _engineManager = transform.GetComponentInChildren<EngineManager>();

            _slowDownEndPrec = GlobalData.Instance.BattleshipSlowDownEndPrec;
            _facingTargetPrec = GlobalData.Instance.BattleshipFacingTargetPrec;

            _currHealthPoints = maxHealthPoints;
            
            _stanceToSwitch = Stance.Empty;
            _stance = Stance.Idle;
            _isReachedDestination = true;
        }

        private void Start()
        {
            SetDestructionLvl(DestructionLevel.New);
            InitParticles();
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
                
                case Stance.Destroyed:
                    break;
                
                case Stance.Empty:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _isShipMoving = !GlobalData.VectorsApproxEqual(_rigidbody.velocity, Vector3.zero, _slowDownEndPrec);
            _engineManager.UpdateEngines(_dotForward, _dotSide, _isShipMoving);
        }
        
        #endregion

        #region Private Functions

        private void InitParticles()
        {
            selectedMarker.transform.Translate(Vector3.down * GlobalData.Instance.RtsShipsPosY, Space.World);
            
            foreach (Transform explosion in mainExplosion.transform)
                _mainExplosionParticles.Add(explosion.GetComponent<ParticleManager>());
        }

        #endregion

        #region Attack Logic

        private void AttackTargetBehavior()
        {
            if (!_currTargetDamageable.CanBeDamaged())
            {
                StopAttack();
                return;
            }
            
            var distToTarget = Vector3.Distance(transform.position, _currTarget.transform.position);
            if (distToTarget > _weaponManager.AttackRange)
            {
                _weaponManager.UpdateWeaponSystem(false);
                _targetMovePos = _currTarget.transform.position;
                MoveToPositon(_targetMovePos, _stance);
                return;
            }

            var rotation = Vector3.zero;
            switch (_weaponManager.WeaponLocation)
            {
                case WeaponLocation.Front:
                    rotation = (_currTarget.transform.position - transform.position).normalized;
                    break;
                
                case WeaponLocation.Sides:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            UpdateRotating(rotation, out _dotForward, out _dotSide);
            _weaponManager.UpdateWeaponSystem(true, _currTarget);
        }

        private void StopAttack()
        {
            _stance = Stance.Idle;
            _weaponManager.UpdateWeaponSystem(false);
            _currTarget = null;
            _currTargetDamageable = null;
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

        #region Destruction Logic

        protected virtual float CalculateTakenDamage(float damage)
        {
            return Mathf.Max(_currHealthPoints - damage, 0f);
        }

        protected virtual void StartDestroySelf()
        {
            _mainCollider.enabled = false;
            _engineManager.TurnOffEngines();

            foreach (var explosion in _mainExplosionParticles)
                explosion.ActivateParticle();

            var destructedMesh = SetDestructionLvl(DestructionLevel.Destroyed);
            
            var corePartsTransform = destructedMesh.transform.GetChild(0);
            var destructiblePartsTransform = destructedMesh.transform.GetChild(1);
            
            corePartsTransform.SetParent(null);
            var rbCore = corePartsTransform.gameObject.AddComponent<Rigidbody>();
            rbCore.mass = _rigidbody.mass;
            rbCore.useGravity = false;
            rbCore.drag = rbCore.angularDrag = 0.5f;
            rbCore.AddForce(_movement * 2.5f);
            
            var destructibleParts = destructiblePartsTransform.Cast<Transform>().ToList();
            foreach (var part in destructibleParts)
            {
                var shouldPush = Random.value > partsStayOnExplChance;
                if (!shouldPush)
                {
                    part.SetParent(corePartsTransform);
                    continue;
                }

                part.SetParent(null);
                var rbPart = part.gameObject.AddComponent<Rigidbody>();
                rbPart.useGravity = false;
                rbPart.drag = rbPart.angularDrag = 0.2f;

                var randomVector = new Vector3(
                    Random.Range(-100,100),
                    Random.Range(-100,100),
                    Random.Range(-100,100));

                rbPart.AddForce(randomVector);
                rbPart.AddTorque(randomVector);
            }

            Destroy(gameObject);
        }

        private GameObject SetDestructionLvl(DestructionLevel level)
        {
            foreach (var mesh in destructLvlMeshes)
            {
                mesh.SetActive(false);
            }

            var meshGameObject = destructLvlMeshes[(int) level];
            meshGameObject.SetActive(true);
            return meshGameObject;
        }

        #endregion

        #region IMoveable Implementation

        public bool IsReachedDestination => _isReachedDestination;

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
            _currTargetDamageable = target.GetComponent<IDamageable>();
            _stance = Stance.AttackTarget;
        }
        
        #endregion

        #region ISelectable Implementation

        public bool CanSelect()
        {
            return _stance != Stance.Destroyed;
        }
        
        public void Select()
        {
            selectedMarker.Play();
        }

        public void Unselect()
        {
            selectedMarker.Stop();
        }
        
        #endregion

        #region IDamageable Implementation

        public bool IsFriend => isFriend;

        public bool CanBeDamaged()
        {
            return _stance != Stance.Destroyed;
        }

        public void Damage(float damage)
        {
            if (_stance == Stance.Destroyed) return;
            
            _currHealthPoints = CalculateTakenDamage(damage);
            if (_currHealthPoints <= 0f)
            {
                _stance = Stance.Destroyed;
                StartDestroySelf();
            }
        }

        #endregion
    }
}
