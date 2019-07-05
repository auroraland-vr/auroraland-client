using TMPro;
using UnityEngine;

namespace AuroraShmup
{
    /// <summary>
    /// Class to manage creating/destroying enemies and handling levels
    /// </summary>
    public class EnemyManager : MonoBehaviour
    {
        /// <summary>
        /// The prefab object of an EnemyShip
        /// </summary>
        public GameObject EnemyPrefab;
        /// <summary>
        /// The TextMeshPro component for level text in UI
        /// </summary>
        public TextMeshProUGUI levelText;

        /// <summary>
        /// The current level of enemies
        /// </summary>
        private int level = 0;
        /// <summary>
        /// The maximum level to play
        /// </summary>
        private int maxLevel = 12;
        /// <summary>
        /// The width of the enemy spawn area
        /// </summary>
        private float width;
        /// <summary>
        /// Whether enemies exist.
        /// </summary>
        private bool enemiesExist {
            get {
                return (transform.childCount != 0);
            }
        }
        /// <summary>
        /// A reference to the GameManager
        /// </summary>
        private GameManager gameManager;

        private void Start()
        {
            width = (transform as RectTransform).rect.width;
            gameManager = transform.parent.parent.GetComponent<GameManager>();
        }

        private void Update()
        {
            if (gameManager.isRunning) {
                GenerateEnemies();
            }
        }

        /// <summary>
        /// Generates a set of enemies if none exist
        /// </summary>
        public void GenerateEnemies() {
            if (!enemiesExist)
            {
                if (level < maxLevel) ++level;
                GenerateEnemyRow(level);
                SetLevelText();
            }
        }

        /// <summary>
        /// Generates a row of EnemyShips spacing them uniformly
        /// </summary>
        /// <param name="numEnemies">Number of enemies.</param>
        private void GenerateEnemyRow(int numEnemies)
        {
            float spacing = width / (numEnemies + 1);
            float xPosition = -width / 2;
            Vector3 position = transform.position;
            for (int i = 0; i < numEnemies; ++i) {
                GameObject enemy = Instantiate(EnemyPrefab, transform.position, 
                                        transform.rotation, transform);
                Vector2 anchoredPosition = (enemy.transform as RectTransform).
                                        anchoredPosition;
                xPosition += spacing;
                anchoredPosition.x = xPosition;
                anchoredPosition.y = 0.5f;
                (enemy.transform as RectTransform).anchoredPosition = 
                                        anchoredPosition;
            }
        }

        /// <summary>
        /// Sets the level text in UI
        /// </summary>
        public void SetLevelText() {
            levelText.text = "Level: " + level.ToString();
        }

        /// <summary>
        /// Destroys the enemies that exist and resets to level 0
        /// </summary>
        public void Reset()
        {
            if (enemiesExist) {
                for (int i = 0; i < transform.childCount; i++) {
                    Ship ship = transform.GetChild(i).GetComponent<Ship>();
                    if (ship) ship.Destroy();
                }
            }
            level = 0;
        }
    }
}