using UnityEngine;

namespace RTS.Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        #region Data
        
        [Header("General Stats")]
        
        [SerializeField] private ActiveDirection activeDirection;
        [SerializeField] protected float damage = 1f;
        [SerializeField] protected float attackRange = 1f;
        [SerializeField] protected float fireRate = 1f;

        public ActiveDirection ActiveDirection => activeDirection;
        public float AttackRange => attackRange;

        #endregion
        
        #region Abstract Methods

        public abstract void InitWeapon();
        public abstract void ProcessWeapon(bool process);
        public abstract void ProcessWeaponTemperature();

        #endregion
    }
}