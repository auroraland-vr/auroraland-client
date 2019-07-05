using UnityEngine;

namespace AuroraShmup
{
    /// <summary>
    /// Base class for a Bullet
    /// </summary>
    public abstract class Bullet : MonoBehaviour
    {
        /// <summary>
        /// The bullet's damage.
        /// </summary>
        public int Damage { get; set; }
        /// <summary>
        /// The bullet's direction.
        /// </summary>
        public Vector2 Direction { get; set; }
        /// <summary>
        /// The bullet's speed.
        /// </summary>
        public float Speed;
        /// <summary>
        /// The AudioClip to play when this bullet is fired
        /// </summary>
        public AudioClip fireAudio;

        public Ship Owner;
        /// <summary>
        /// A reference to this Bullet's RectTransform
        /// </summary>
        private RectTransform rectTransform;
        /// <summary>
        /// This Bullet's anchored position.
        /// </summary>
        private Vector2 anchoredPosition;

        /// <summary>
        /// Called by the ship to let the bullet affect the ship. Override and add custom logic.
        /// </summary>
        /// <param name="ship">The ship that was hit</param>
        public abstract void OnImpact(Ship ship);

        private void Start()
        {
            rectTransform = (transform as RectTransform);
            anchoredPosition = rectTransform.anchoredPosition;

            PlayAudio();
        }

        private void Update()
        {
            Move(Time.deltaTime);
        }

        /// <summary>
        /// Plays the fire audio
        /// </summary>
        private void PlayAudio() {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = fireAudio;
            audioSource.Play();
        }

        /// <summary>
        /// Moves the bullet forward per frame
        /// </summary>
        /// <param name="timeDelta">Time since last frame.</param>
        private void Move(float timeDelta) {
            anchoredPosition += Direction * Speed * timeDelta;

            // Destroy bullet when off screen
            float yBounds = 0.75f;
            if (anchoredPosition.y > yBounds ||
                anchoredPosition.y < -yBounds) {
                Destroy(gameObject);
            }

            rectTransform.anchoredPosition = anchoredPosition;
            Vector3 localPostion = rectTransform.localPosition;
            localPostion.z = 0.0f;
            rectTransform.localPosition = localPostion;
        }
    }
}