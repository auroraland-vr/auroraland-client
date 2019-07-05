using UnityEngine;

namespace AuroraShmup
{
    /// <summary>
    /// Base class for a Weapon
    /// </summary>
    public abstract class Weapon : MonoBehaviour
    {
        /// <summary>
        /// Inspector reference to the weapon's bullet prefab.
        /// </summary>
        [SerializeField]
        GameObject _bulletPrefab = null;

        /// <summary>
        /// The weapon's cooldown rate, before it can fire again.
        /// </summary>
        /// Property in place in case we want to trigger something on get/set.
        public float Cooldown;
        /// <summary>
        /// The weapon's bullet damage.
        /// </summary>
        public int Damage;
        public Vector2 Direction;

        float _cooldownTimer;

        private void Start()
        {
            _cooldownTimer = 0.0f;
        }

        void Update()
        {
            if (_cooldownTimer < Cooldown)
                _cooldownTimer += Time.deltaTime;
        }

        /// <summary>
        /// Asks the weapon to try and fire.
        /// </summary>
        /// <param name="bulletOrigin">Where the bullet should be spawned</param>
        public virtual Bullet Fire(Transform bulletOrigin, Ship owner)
        {
            if (_cooldownTimer >= Cooldown)
            {
                _cooldownTimer = 0.0f; // Reset cooldown timer
                GameObject bulletObj = Instantiate(_bulletPrefab, bulletOrigin.position, bulletOrigin.rotation, transform.parent);
                Bullet bullet = bulletObj.GetComponent<Bullet>();
                bullet.Damage = Damage;
                bullet.Owner = owner;
                return bullet;
            }
            return null;
        }
    }
}