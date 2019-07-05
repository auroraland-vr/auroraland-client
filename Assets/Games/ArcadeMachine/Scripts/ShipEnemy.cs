using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraShmup
{
    /// <summary>
    /// An enemy ship with basic AI for firing
    /// </summary>
    public class ShipEnemy : Ship
    {
        /// <summary>
        /// Determines how often an enemy will try to fire (in seconds)
        /// </summary>
        public float fireFrequency = 1.0f;
        /// <summary>
        /// The chance that the enemy will successfully fire
        /// </summary>
        public float fireChance = 0.01f;

        /// <summary>
        /// The timer used to limit time between shots
        /// </summary>
        private float fireTimer;
        /// <summary>
        /// Whether the ship enemy is free from obstruction to fire
        /// Used to avoid friendly fire
        /// </summary>
        private bool canFire {
            get {
                int numEnemies = transform.parent.childCount;
                for (int i = 0; i < numEnemies; i++)
                {   // Check if friendly enemies are in front of this ship
                    if (transform.parent.GetChild(i).GetComponent<BoxCollider>().
                        bounds.Contains(BulletOrigin.transform.position))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private new void Start()
        {
            base.Start();
            fireTimer = 0.0f;
            Weapon.Direction = Vector2.down;
            gameManager = transform.parent.parent.parent.GetComponent<GameManager>();
        }

        private void Update()
        {
            if (fireTimer < fireFrequency) {
                fireTimer += Time.deltaTime;
            }
            else if (canFire) Fire();
        }

        /// <summary>
        /// Tries to fire based fireChance
        /// </summary>
        public override void Fire()
        {
            if (Random.value < fireChance) {
                fireTimer = 0.0f;
                base.Fire();
            }
        }

        /// <summary>
        /// Notifies the GameManager that enemy has been killed
        /// </summary>
        public override void Destroy()
        {
            gameManager.OnPlayerScored();
            base.Destroy();
        }
    }
}