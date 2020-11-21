using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameGlobal;
using RTS.Controls;
using Random = UnityEngine.Random;

namespace RTS.Ships
{
    public abstract class ShipBase : MonoBehaviour, IMoveable, IDamageable, IAttackable, ISelectable, IExplosible, IBehaviorSwitchable
    {
        #region Data

        [Header("General")]
        [SerializeField] public byte teamId; 
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
        [SerializeField] private float explosionForce;
        [SerializeField] private float explosionRadius;
        [SerializeField] private float coreForceModifier;
        [SerializeField] private float dragForCore;
        [SerializeField] private float dragForParts;
        [SerializeField] private GameObject mainExplosion;
        [SerializeField] private GameObject[] destructLvlMeshes;
        [SerializeField] private Transform hitPointsTransform;
        [Range(0, 1)] [SerializeField] private float partsStayOnExplChance;
        
        public Action<GameObject> OnShipDestroyed;

        private readonly List<ParticleManager> _mainExplosionParticles = new List<ParticleManager>();
        private readonly List<GameObject> _createdSpaceDerbis = new List<GameObject>();
        private List<Transform> _hitPositions;

        private float _slowDownEndPrec;
        private float _facingTargetPrec;

        private State _state;
        private State _stateToSwitch;
        private DestructionLevel _destrLvl;
        private FireMode _fireMode;

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

        private bool _shouldOnboardGunShoot;
        private bool _shouldMainGunShoot;

        #endregion

        #region Properties

        public byte TeamId => teamId;
        public bool IsReachedDestination => _isReachedDestination;
        public float ExplosionForce => explosionForce;
        public float ExplosionRadius => explosionRadius;
        public Vector3 Position => transform.position;
        public List<GameObject> CreatedSpaceDerbis => _createdSpaceDerbis;
        public List<Transform> HitPositions => _hitPositions;
        public FireMode CurrFireMode => _fireMode;

        #endregion

        #region Unity Events
        
        protected virtual void Awake()
        {
            _mainCollider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
            _weaponManager = transform.GetComponentInChildren<WeaponManager>();
            _engineManager = transform.GetComponentInChildren<EngineManager>();

            _slowDownEndPrec = GlobalData.Instance.BattleshipSlowDownEndPrec;
            _facingTargetPrec = GlobalData.Instance.BattleshipFacingTargetPrec;

            _currHealthPoints = maxHealthPoints;
            
            _stateToSwitch = State.Empty;
            _state = State.Idle;
            _isReachedDestination = true;
            
            _hitPositions = hitPointsTransform.Cast<Transform>().ToList();
        }

        protected virtual void Start()
        {
            SetDestructionLvl(DestructionLevel.New);
            InitParticles();
            
            SwitchFireMode(FireMode.NoGuns);
        }

        protected virtual void FixedUpdate()
        {
            ProcessState();

            var anyMovements = !GlobalData.VectorsApproxEqual(_rigidbody.velocity, Vector3.zero, _slowDownEndPrec);
            _isShipMoving = anyMovements && _state == State.MoveToPosition;
            _engineManager.UpdateEngines(_dotForward, _dotSide, _rigidbody.angularVelocity.y, _isShipMoving);
            
            _weaponManager.UpdateWeaponSystem(_shouldMainGunShoot, _shouldOnboardGunShoot, _currTarget);
        }
        
        #endregion

        #region Private/Protected Functions

        protected void ProcessState()
        {
            switch (_state)
            {
                case State.Idle: AutoTargetAttack(); break;
                case State.MoveToPosition: MoveToPositionBehavior(); break;
                case State.AttackTarget: AttackTargetBehavior(); break;
            }
        }
        
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
                _targetMovePos = _currTarget.transform.position;
                MoveToPositon(_targetMovePos, _state);
                return;
            }
            
            if (_fireMode == FireMode.NoGuns)
                SwitchFireMode(FireMode.OnlyMain, false);
            else if (_fireMode == FireMode.OnlyOnboard)
                SwitchFireMode(FireMode.AllGuns, false);
            
            ProcessShootMovements();
        }

        private void StopAttack(bool isForceStop = false)
        {
            if (isForceStop)
                SwitchFireMode(FireMode.NoGuns);
            else
                SwitchFireMode(_fireMode);
            _state = State.Idle;
            _stateToSwitch = State.Idle;
            _currTarget = null;
            _currTargetDamageable = null;
        }

        private void SwitchFireMode(FireMode mode, bool isMainFireMode = true)
        {
            if (isMainFireMode) _fireMode = mode;

            switch (mode)
            {
                case FireMode.NoGuns:
                    _shouldMainGunShoot = false;
                    _shouldOnboardGunShoot = false;
                    break;
                case FireMode.OnlyMain:
                    _shouldMainGunShoot = true;
                    _shouldOnboardGunShoot = false;
                    break;
                case FireMode.OnlyOnboard:
                    _shouldMainGunShoot = false;
                    _shouldOnboardGunShoot = true;
                    break;
                case FireMode.AllGuns:
                    _shouldMainGunShoot = true;
                    _shouldOnboardGunShoot = true;
                    break;
            }
        }

        private void ProcessShootMovements()
        {
            var rotation = _weaponManager.CalculateRequiredRotation();
            if (rotation == Vector3.up) return;
            UpdateRotating(rotation, out _dotForward, out _dotSide);
        }
        
        private void AutoTargetAttack()
        {
            if (_fireMode == FireMode.NoGuns || _fireMode == FireMode.OnlyOnboard) return;
            
            if (RTSGameController.TargetExistAndReachable(transform, teamId, _weaponManager.AttackRange, _currTarget))
            {
                ProcessShootMovements();
                return;
            }

            _currTarget = RTSGameController.GetClosestTarget(transform, teamId, _weaponManager.AttackRange);
            _currTargetDamageable = _currTarget != null ? _currTarget.GetComponent<IDamageable>() : null;
        }

        #endregion

        #region Moving Logic
        
        private void MoveToPositionBehavior()
        {
            if (!_isReachedDestination)
            {
                if (_stateToSwitch == State.AttackTarget)
                {
                    _targetMovePos = _currTarget.transform.position;
                    MoveToPositon(_targetMovePos, _state);
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
                    if (_stateToSwitch == State.Empty)
                    { 
                        _state = State.Idle;
                    }
                    else
                    {
                        _state = _stateToSwitch;
                        _stateToSwitch = State.Empty;
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
            rbCore.drag = rbCore.angularDrag = dragForCore;
            rbCore.AddForce(_movement * coreForceModifier);
            
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
                rbPart.drag = rbPart.angularDrag = dragForParts;
                if (part.gameObject != null)
                    _createdSpaceDerbis.Add(part.gameObject);
            }
            
            OnShipDestroyed?.Invoke(gameObject);
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

        public void MoveToPositon(Vector3 position, State state = State.MoveToPosition)
        {
            _targetMovePos = position;
            _isReachedDestination = false;
            _state = State.MoveToPosition;
            if (state != State.MoveToPosition)
                _stateToSwitch = state;
        }

        public void ForceStop()
        {
            MoveToPositon(transform.position);
        }

        #endregion
        
        #region IAttackable Implementation

        public void AttackTarget(MonoBehaviour target)
        {
            _currTarget = target;
            _currTargetDamageable = target.GetComponent<IDamageable>();
            _state = State.AttackTarget;
        }

        public void ForceLooseTarget()
        {
            StopAttack(true);
        }

        #endregion

        #region ISelectable Implementation

        public bool CanSelect()
        {
            return _state != State.Destroyed;
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

        public bool IsEnemy(byte askerTeamId)
        {
            return askerTeamId != teamId;
        }
        
        public bool CanBeDamaged()
        {
            return _state != State.Destroyed;
        }

        public void Damage(float damage)
        {
            if (_state == State.Destroyed) return;
            
            _currHealthPoints = CalculateTakenDamage(damage);
            if (_currHealthPoints <= 0f)
            {
                _state = State.Destroyed;
                StartDestroySelf();
            }
        }

        #endregion

        #region IBehaviorSwitchable Implementation
        
        public virtual void SwitchBehavior(Enum behavior)
        {
            if (behavior is FireMode fireMode)
                SwitchFireMode(fireMode);
        }
        
        #endregion
    }
}