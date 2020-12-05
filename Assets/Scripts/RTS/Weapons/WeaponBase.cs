using RTS.Controls;
using UnityEngine;

namespace RTS.Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        #region Data
        
        [Header("General Stats")]
        
        [SerializeField] protected float damage = 1f;
        [SerializeField] protected float attackRange = 1f;
        [SerializeField] protected float fireRate = 1f;

        protected Transform ParentShipTransform { get; private set; }
        protected ISelectable SelectableShip { get; private set; }
        protected IDamageable DamageableShip { get; private set; }
        protected ITargetable TargetableShip { get; private set; }
        
        public float AttackRange => attackRange;

        #endregion

        #region Public Methods
        
        public void InitWeapon(Transform parentShip)
        {
            ParentShipTransform = parentShip;
            DamageableShip = ParentShipTransform.GetComponent<IDamageable>();
            TargetableShip = ParentShipTransform.GetComponent<ITargetable>();
            SelectableShip = ParentShipTransform.GetComponent<ISelectable>();
            Init();
        }
        
        #endregion
        
        #region Abstract Methods

        protected abstract void Init();
        public abstract void ProcessWeapon(bool process);

        #endregion
    }
}