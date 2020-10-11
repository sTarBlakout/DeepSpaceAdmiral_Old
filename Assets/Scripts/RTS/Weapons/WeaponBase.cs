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

        protected Transform ParentShip { get; private set; }
        protected ISelectable SelectableShip { get; private set; }
        
        public float AttackRange => attackRange;

        #endregion

        #region Public Methods
        
        public void InitWeapon(Transform parentShip)
        {
            ParentShip = parentShip;
            SelectableShip = ParentShip.GetComponent<ISelectable>();
            Init();
        }
        
        #endregion
        
        #region Abstract Methods

        protected abstract void Init();
        public abstract void ProcessWeapon(bool process);

        #endregion
    }
}