using UnityEngine;
using UnityEngine.Assertions;

namespace AuroraShmup
{
    /// <summary>
    /// Base class for ship
    /// </summary>
    public class Ship : MonoBehaviour
    {
        /// <summary>
        /// The ship's health.
        /// </summary>
        public int health { get; set; }
        /// <summary>
        /// The ship's starting health.
        /// </summary>
        public int startHealth;
        /// <summary>
        /// The ship's weapon.
        /// </summary>
        public Weapon Weapon { get; set; }
        /// <summary>
        /// The explosion FX prefab for this ship
        /// </summary>
        public GameObject explosionFXPrefab;

        /// <summary>
        /// A reference to the GameManager
        /// </summary>
        protected GameManager gameManager;
        /// <summary>
        /// The ship's bullet origin.
        /// </summary>
        protected RectTransform BulletOrigin;

        public void Start()
        {
            BulletOrigin =  transform.Find("BulletOrigin") as RectTransform;
            health = startHealth;
            Weapon = GetComponent<Weapon>();
        }

        /// <summary>
        /// Fires the weapon on this ship
        /// </summary>
        public virtual void Fire()
        {
            Weapon.Fire(BulletOrigin, this);
        }

        void OnCollisionEnter(Collision collision)
        {   // Handle bullet collisions
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            
            if (bullet != null && bullet.Owner != this)
            {   
                bullet.OnImpact(this);
                Destroy(bullet.gameObject);
            }
        }

        /// <summary>
        /// Destroys the ship
        /// </summary>
        public virtual void Destroy()
        {
            if (gameObject.CompareTag("Player")) {
                gameManager.GameOver();
            }
            else {
                gameManager.UpdateScore();
                Destroy(gameObject);
            }
            gameManager.SpawnExplosion(explosionFXPrefab,
                                                transform.position, 
                                                transform.rotation);

        }

        /// <summary>
        /// Applies damage to the ship
        /// </summary>
        /// <param name="damage">Amount of damage taken.</param>
        public virtual void TakeDamage(int damage) {
            health -= damage;
            if (health <= 0) Destroy();
        }
    }
}