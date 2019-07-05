using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraShmup
{
    /// <summary>
    /// A bullet fired as a single shot
    /// </summary>
    public class BulletSingleShot : Bullet
    {
        /// <summary>
        /// The impact FX prefab to spawn when bullet hits something
        /// </summary>
        public GameObject impactFXPrefab;

        /// <summary>
        /// Spawns impact FX and applies damage to ship
        /// </summary>
        /// <param name="ship">The ship this bullet impacted.</param>
        public override void OnImpact(Ship ship)
        {
            SpawnImpactFX();
            ship.TakeDamage(Damage);
        }

        /// <summary>
        /// Spawns the impact fx.
        /// </summary>
        private void SpawnImpactFX() {
            GameObject impactFX = Instantiate(impactFXPrefab, transform.position, 
                                              transform.rotation,
                                              transform.parent.parent);
            impactFX.transform.LookAt(impactFX.transform.position - impactFX.transform.up);
            Vector3 localPos = impactFX.transform.localPosition;
            localPos.z = -0.01f; // Spawn fx in front of canvas
            impactFX.transform.localPosition = localPos;
            Destroy(impactFX, impactFX.GetComponent<ParticleSystem>().main.duration);
        }
    }
}