using UnityEngine;
using UnityEngine.Assertions;

namespace ProjectRenaissance.UI
{
    /// <summary>
    /// A high-level command point for UI elements.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField]
        GameObject _startGameCanvas;
        [SerializeField]
        GameObject _notificationCanvas;
        [SerializeField]
        ActionUI _actionUI;
        [SerializeField]
        BettingUI _bettingUI;

        void Start()
        {
            Assert.IsNotNull(_startGameCanvas);
            Assert.IsNotNull(_notificationCanvas);

            _startGameCanvas.SetActive(false);
            _notificationCanvas.SetActive(false);
        }

        public void SetupLocalGambler(Gambler localGambler)
        {
            _actionUI.SetGambler(localGambler);
            _bettingUI.SetGambler(localGambler);

            _actionUI.transform.SetParent(localGambler.transform);
            _actionUI.transform.localPosition = new Vector3(0, -1.2f, 0.4f);
            _actionUI.transform.localRotation = Quaternion.Euler(20, 0, 0);

            _bettingUI.transform.SetParent(localGambler.transform);
            _bettingUI.transform.localPosition = new Vector3(0, -1.5f, -0.95f);
            _bettingUI.transform.localRotation = Quaternion.Euler(60, 0, 0);

            _notificationCanvas.transform.SetParent(localGambler.transform);
            _notificationCanvas.transform.localPosition = new Vector3(0, -0.5f, 1);
            _notificationCanvas.transform.localRotation = Quaternion.Euler(0, 0, 0);

            _startGameCanvas.SetActive(true);
            _startGameCanvas.transform.SetParent(localGambler.transform);
            _startGameCanvas.transform.localPosition = new Vector3(0, 0.5f, 1.5f);
            _startGameCanvas.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        public void EliminateLocalGambler()
        {
            _startGameCanvas.SetActive(true);
        }
        public void DisableLocalGambler()
        {
            _startGameCanvas.SetActive(false);
            _notificationCanvas.SetActive(false);
        }

        public void StartRound()
        {
            _startGameCanvas.SetActive(false);
            _notificationCanvas.SetActive(true);
            _bettingUI.Show();
        }
        public void StartInsuring()
        {
            _bettingUI.Show();
        }
    }
}