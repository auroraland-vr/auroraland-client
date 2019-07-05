using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraShmup
{
    /// <summary>
    /// A weapon that fires a single shot 
    /// </summary>
    public class WeaponSingleShot : Weapon
    {
        /// <summary>
        /// Fires a shot from the specified bulletOrigin.
        /// </summary>
        /// <returns>The Bullet object fired</returns>
        /// <param name="bulletOrigin">Origin to spawn Bullet.</param>
        public override Bullet Fire(Transform bulletOrigin, Ship owner) {
            Bullet bullet = base.Fire(bulletOrigin, owner);
            if (bullet) {
                bullet.Direction = Direction;
                if (Direction == Vector2.down) {
                    // Enemies shoot down, so rotate the Bullet object
                    bullet.transform.localRotation = new Quaternion(0.0f, 0.0f, 1.0f, 0.0f);
                }
                bullet.Speed = 1.0f;
            }
            return bullet;
        }
    }
}