using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AuroraShmup
{
    /// <summary>
    /// This singleton class manages the main game loop.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Returns whether this <see cref="T:AuroraShmup.GameManager"/> is running.
        /// </summary>
        /// <value><c>true</c> if GameManager running; otherwise, <c>false</c>.</value>
        public bool isRunning { get; private set; }
        /// <summary>
        /// Value determines wheither this GameManager is restarting.
        /// </summary>
        /// <value><c>true</c> if is restarting; otherwise, <c>false</c>.</value>
        public bool isRestarting { get; private set; }
        /// <summary>
        /// A reference to the ShipPlayer script component of PlayerShip
        /// </summary>
        public ShipPlayer shipPlayer { get; private set; }
        /// <summary>
        /// A reference to the ArcadeMachineManager
        /// </summary>
        public ArcadeMachineManager arcadeManager;

        /// <summary>
        /// A reference to the EnemyManager
        /// </summary>
        private EnemyManager enemyManager;
        /// <summary>
        /// A reference to the PlayerShip game object
        /// </summary>
        private GameObject playerShip;
        /// <summary>
        /// The TextMeshPro component for game loop text
        /// </summary>
        private TextMeshProUGUI gameLoopText;
        /// <summary>
        /// The TextMeshPro component for score text
        /// </summary>
        private TextMeshProUGUI scoreText;
        /// <summary>
        /// The TextMeshPro component for health text
        /// </summary>
        private TextMeshProUGUI healthText;


        /// <summary>
        /// The time in seconds to countdown while showing game over text
        /// </summary>
        private const float GAMEOVERCOUNTDOWN = 5.0f;
        /// <summary>
        /// The game's starting text
        /// </summary>
        private const string STARTGAMETEXT = "Ready?\nFire to Play!";
        /// <summary>
        /// The game over text
        /// </summary>
        private const string GAMEOVERTEXT = "Game\nOver";

        // Initialize the GameManager's private variables
        private void Awake() {
            enemyManager = transform.Find("GamePanel").Find("Enemies").
                                    GetComponent<EnemyManager>();
            playerShip = transform.Find("GamePanel").Find("PlayerShip").
                            gameObject;
            shipPlayer = playerShip.GetComponent<ShipPlayer>();
            gameLoopText = transform.Find("GamePanel").Find("GameLoopText").
                            GetComponent<TextMeshProUGUI>();
            scoreText = transform.parent.Find("TextCanvas").Find("ScorePanel").
                            Find("ScoreText").GetComponent<TextMeshProUGUI>();
            healthText = transform.parent.Find("TextCanvas").Find("ScorePanel").
                            Find("HealthText").GetComponent<TextMeshProUGUI>();
        }

        private void Start() {
            Initialize();
        }

        /// <summary>
        /// Initializes the GameManager to its starting state
        /// </summary>
        private void Initialize() {
            isRunning = false;
            isRestarting = false;
            gameLoopText.text = STARTGAMETEXT;
            shipPlayer.Init();
            UpdateHealth();
            UpdateScore();
            enemyManager.SetLevelText();
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame() {
            isRunning = true;
            isRestarting = false;
            shipPlayer.SetEnabled(true);
            gameLoopText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Suspends the game loop and displays game over text
        /// </summary>
        public void GameOver()
        {
            StartCoroutine(WaitBeforeDisengage());
            isRunning = false;
            isRestarting = true;
            enemyManager.Reset();
            gameLoopText.text = GAMEOVERTEXT;
            gameLoopText.gameObject.SetActive(true);
            shipPlayer.SetEnabled(false);
            StartCoroutine("CountdownToReset");
        }

        IEnumerator WaitBeforeDisengage()
        {
            yield return new WaitForSeconds(2.5f);
            arcadeManager.Disengage();
        }

        /// <summary>
        /// Countdowns to reset the game
        /// </summary>
        IEnumerator CountdownToReset() {
            yield return new WaitForSeconds(GAMEOVERCOUNTDOWN);
            Initialize();
        }

        /// <summary>
        /// Updates the score on UI
        /// </summary>
        public void UpdateScore() {
            scoreText.text = "Score: " + shipPlayer.score;
        }

        /// <summary>
        /// Notifies ShipPlayer they scored
        /// </summary>
        public void OnPlayerScored() {
            if (!isRunning) return;
            shipPlayer.OnScore();
        }

        /// <summary>
        /// Updates the health in UI
        /// </summary>
        public void UpdateHealth()
        {
            healthText.text = "Health: " + shipPlayer.health;
        }

        /// <summary>
        /// Spawns an explosion when a ship is destroyed
        /// </summary>
        /// <param name="explosionFXPrefab">Explosion FX Prefab.</param>
        /// <param name="position">Position of ship.</param>
        /// <param name="rotation">Rotation of ship.</param>
        public void SpawnExplosion(GameObject explosionFXPrefab, 
                                   Vector3 position, 
                                   Quaternion rotation) 
        {
            GameObject explosion = Instantiate(explosionFXPrefab,
                                               position,
                                               rotation,
                                               transform);
            Vector3 localPos = explosion.transform.localPosition;
            localPos.z = -0.01f; // Offset the explosion to be drawn in front
            explosion.transform.localPosition = localPos;
            Destroy(explosion, explosion.GetComponent<AudioSource>().clip.length);
        }
    }
}