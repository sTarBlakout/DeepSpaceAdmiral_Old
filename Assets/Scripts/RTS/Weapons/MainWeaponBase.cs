using UnityEngine;

namespace RTS.Weapons
{
    public abstract class MainWeaponBase : WeaponBase
    {
        #region Data
        
        [Header("Main Weapon")]

        [SerializeField] private ActiveDirection activeDirection;
        [SerializeField] protected float minGunTemp = -1;
        [SerializeField] protected float maxGunTemp = 1;
        [SerializeField] protected float borderGunTemp;
        [SerializeField] protected float mainGunWarmFactor;
        [SerializeField] protected float mainGunCoolFactor;

        #endregion

        #region Propreties

        public bool ShouldHeat { get; protected set; }
        public bool ShouldLock { get; protected set; }
        public float MainGunTemp { get; protected set; }
        public ActiveDirection ActiveDirection => activeDirection;

        #endregion

        #region Abstract Methods

        public abstract void ProcessWeaponTemperature();
        
        #endregion
    }
}