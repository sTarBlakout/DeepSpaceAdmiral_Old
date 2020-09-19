using UnityEngine;

namespace RTS.Weapons
{
    public abstract class MainWeaponBase : MonoBehaviour
    {
        #region Data
        
        [SerializeField] private WeaponLocation weaponLocation;
        
        [SerializeField] protected float damage = 1f;
        [SerializeField] protected float attackRange = 1f;
        [SerializeField] protected float fireRate = 1f;
        
        [SerializeField] protected float minGunTemp = -1;
        [SerializeField] protected float maxGunTemp = 1;
        [SerializeField] protected float borderGunTemp;
        [SerializeField] protected float mainGunWarmFactor;
        [SerializeField] protected float mainGunCoolFactor;

        public float AttackRange => attackRange;
        public WeaponLocation WeaponLocation => weaponLocation;
        
        #endregion

        #region Propreties

        public bool ShouldHeat { get; protected set; }
        public bool ShouldLock { get; protected set; }
        
        public float MainGunTemp { get; protected set; }

        #endregion

        #region Abstract Methods

        public abstract void InitWeapon();
        public abstract void ProcessWeapon(bool process);
        public abstract void ProcessWeaponTemperature();

        #endregion
    }
}