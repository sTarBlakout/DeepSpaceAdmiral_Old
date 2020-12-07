using System;
using System.Collections.Generic;
using GameGlobal;
using RTS.Controls;
using RTS.Ships;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RTS
{
    public class RTSGameController : MonoBehaviour
    {
        #region Data

        [SerializeField] private float upwardExplosionModifier;
        [SerializeField] private Transform spaceDerbisTransform;

        private List<GameObject> _allShips = new List<GameObject>();
        private List<GameObject> _spaceDerbis = new List<GameObject>();
        
        #endregion

        #region Singleton

        private static RTSGameController _instance;
        public static RTSGameController Instance
        {
            get 
            {
                if (_instance == null)
                    _instance = FindObjectOfType<RTSGameController>();
                return _instance;
            }
        }
        
        #endregion
        
        #region Testing
        
        public GameObject shipToSpawn;

        public void TestShipBuild()
        {
            var spawnPos = new Vector3(0f, GlobalData.Instance.RtsShipsPosY, 0f);
            var ship = Instantiate(shipToSpawn, spawnPos, Quaternion.identity);
            ship.GetComponent<Battleship>().teamId = 2;
            ship.GetComponent<Battleship>().OnShipDestroyed += DestroyShip;
            ship.AddComponent<AIShipController>().enabled = false;

            ship.transform.Find("Weapons").gameObject.SetActive(false);
            
            _allShips.Add(ship);
        }

        #endregion

        #region Unity Events
        
        private void Start()
        {
            SpawnBattleshipAtPosition(new Vector3(-19, 0, 5), true);
            SpawnBattleshipAtPosition(new Vector3(-25, 0, 5), true);
        }
        
        #endregion

        #region Spawn Logic

        private GameObject SpawnBattleshipAtPosition(Vector3 position, bool isFriend)
        {
            var spawnPos = new Vector3(position.x, GlobalData.Instance.RtsShipsPosY, position.z);
            var shipGameObject = Instantiate(shipToSpawn, spawnPos, Quaternion.identity);
            var battleship = shipGameObject.GetComponent<Battleship>();
            battleship.teamId = 1;
            battleship.OnShipDestroyed += DestroyShip;
            shipGameObject.AddComponent<AIShipController>().enabled = !isFriend;

            _allShips.Add(shipGameObject);
            return shipGameObject;
        }

        #endregion

        #region Static Methods

        public static bool TargetExistAndReachable(ITargetable requester, ITargetable target, float range)
        {
            var conditionsMet = false;
            if (target != null && target.IsEnemy(requester.TeamId) && target.Damageable.CanBeDamaged())
            {
                if (Vector3.Distance(requester.Transform.position, target.Transform.position) <= range && IsTargetInLineOfSight(requester, target))
                    conditionsMet = true; 
            }
            return conditionsMet;
        }

        public static ITargetable GetClosestTarget(ITargetable requester, float range, ITargetable preferredTarget = null)
        {
            var possibleTargets = Physics.OverlapSphere(requester.Transform.position, range);
            var minDist = float.MaxValue;
            ITargetable possibleTarget = null;
            foreach (var targetCollider in possibleTargets)
            {
                if (targetCollider.transform == requester) continue;
                var target = targetCollider.GetComponent<ITargetable>();
                if (target == null) continue;
                if (!target.Damageable.CanBeDamaged() || !target.IsEnemy(requester.TeamId)) continue;
                if (target == preferredTarget)
                {
                    possibleTarget = target;
                    break;
                }

                var distToTarget = Vector3.Distance(targetCollider.transform.position, requester.Transform.position); 
                if (distToTarget < minDist)
                {
                    minDist = distToTarget;
                    possibleTarget = target;
                }
            }

            if (possibleTarget == null || !IsTargetInLineOfSight(requester, possibleTarget))
                return null;

            return possibleTarget;
        }

        public static bool IsTargetInLineOfSight(ITargetable requester, ITargetable target)
        {
            if (target == null) return false;
            if (Physics.Linecast(requester.Transform.position, target.Transform.position, out var hit))
            {
                var hitTarget = hit.collider.GetComponent<ITargetable>();
                if (hitTarget != null && hitTarget == target)
                    return true;
            }
            return false;
        }

        #endregion
        
        #region Private Functions
        
        private void CreateExplosionAtPos(Vector3 position, float radius, float force)
        {
            var collidersInRadius = Physics.OverlapSphere(position, radius);
            foreach (var colliderInRadius in collidersInRadius)
            {
                var randomModifier = Random.Range(-upwardExplosionModifier, upwardExplosionModifier);
                var rb = colliderInRadius.GetComponent<Rigidbody>();
                if (rb != null) 
                    rb.AddExplosionForce(force, position, radius, randomModifier);
            }
        }

        private void DestroyShip(GameObject shipGameObject)
        {
            var damageable = shipGameObject.GetComponent<IDamageable>();
            if (damageable != null)
                _allShips.Remove(shipGameObject);
            
            var explosible = shipGameObject.GetComponent<IExplosible>();
            if (explosible != null)
            {
                CreateExplosionAtPos(explosible.Position, explosible.ExplosionRadius, explosible.ExplosionForce);
                foreach (var derbis in explosible.CreatedSpaceDerbis)
                {
                    derbis.transform.SetParent(spaceDerbisTransform);
                    _spaceDerbis.Add(derbis);
                }
            }
        }
        
        #endregion
    }
}