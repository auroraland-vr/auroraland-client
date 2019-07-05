using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraShmup
{
    /// <summary>
    /// The ship controllable by the player
    /// </summary>
    public class ShipPlayer : Ship
    {
        /// <summary>
        /// The ship's horizontal movement speed.
        /// </summary>
        public float HorizontalSpeed;
        /// <summary>
        /// The ship's vertical movement speed.
        /// </summary>
        public float VerticalSpeed;

        /// <summary>
        /// The player's score
        /// </summary>
        public int score { get; private set; }
        /// <summary>
        /// The ship's rect transform.
        /// </summary>
        private RectTransform rectTransform;
        /// <summary>
        /// The ship's anchored position.
        /// </summary>
        private Vector2 anchoredPosition;
        /// <summary>
        /// Half the width of the ship.
        /// </summary>
        private float halfWidth;
        /// <summary>
        /// Half the height of the ship.
        /// </summary>
        private float halfHeight;
        /// <summary>
        /// The direction to move the ship
        /// </summary>
        private Vector2 Direction;
        /// <summary>
        /// The player ships starting anchor position
        /// </summary>
        private Vector2 startingPos;

        private void Awake()
        {
            rectTransform = transform as RectTransform;
        }

        private new void Start()
        {
            base.Start();
            anchoredPosition = rectTransform.anchoredPosition;
            startingPos = anchoredPosition;
            halfWidth = rectTransform.rect.width / 2.0f;
            halfHeight = rectTransform.rect.height / 2.0f;
            Weapon.Direction = Vector2.up;
            gameManager = transform.parent.parent.GetComponent<GameManager>();
            Init();
        }

        /// <summary>
        /// Initializes the player ship's state
        /// </summary>
        public void Init() {
            score = 0;
            health = startHealth;
            anchoredPosition = startingPos;
            rectTransform.anchoredPosition = anchoredPosition;
            SetEnabled(false);
        }

        void Update()
        {
            MoveShip();
        }

        /// <summary>
        /// Changes the ship direction based on horizontal input
        /// </summary>
        /// <param name="input">The horizontal input.</param>
        public void InputHorizontal(float input)
        {
            Direction.x = input;
        }

        /// <summary>
        /// Changes the ship direction based on vertical input
        /// </summary>
        /// <param name="input">The vertical input.</param>
        public void InputVertical(float input)
        {
            Direction.y = input;
        }

        /// <summary>
        /// Moves the ship in a given direction.
        /// </summary>
        private void MoveShip() {
            anchoredPosition.x += Direction.x * HorizontalSpeed * Time.deltaTime;
            anchoredPosition.y += Direction.y * VerticalSpeed * Time.deltaTime;
            LimitBounds();
            rectTransform.anchoredPosition = anchoredPosition;
        }

        /// <summary>
        /// Limits the ship to stay inside boundary area.
        /// </summary>
        private void LimitBounds() {
            float xBounds = 0.75f;
            float yBounds = 0.75f;
            // Limit X bounds
            if (anchoredPosition.x - halfWidth < -xBounds) {
                anchoredPosition.x = -xBounds + halfWidth;
            }
            else if (anchoredPosition.x + halfWidth > xBounds)
            {
                anchoredPosition.x = xBounds - halfWidth;
            }

            // Limit Y bounds
            if (anchoredPosition.y - halfHeight < -yBounds)
            {
                anchoredPosition.y = -yBounds + halfHeight;
            }
            else if (anchoredPosition.y + halfHeight > yBounds)
            {
                anchoredPosition.y = yBounds - halfHeight;
            }
        }

        /// <summary>
        /// Handles updating score for ShipPlayer
        /// </summary>
        public void OnScore() {
            score++;
        }

        /// <summary>
        /// Notifies GameManager that ShipPlayer has taken damage
        /// </summary>
        public override void TakeDamage(int damage) {
            base.TakeDamage(damage);
            gameManager.UpdateHealth();
        }

        /// <summary>
        /// Enables/Disables the ShipPlayer's image and collider
        /// </summary>
        /// <param name="value">If set to <c>true</c> value.</param>
        public void SetEnabled(bool value)
        {
            gameObject.GetComponent<Image>().enabled = value;
            gameObject.GetComponent<BoxCollider>().enabled = value;
        }
    }
}