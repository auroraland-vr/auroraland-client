using UnityEngine;
using UnityEngine.UI;

namespace ProjectRenaissance.UI
{
    public sealed class ActionUI : MonoBehaviour
    {
        Gambler _gambler;
        [SerializeField]
        Button _hitButton;
        [SerializeField]
        Button _standButton;
        [SerializeField]
        Button _doubleButton;
        [SerializeField]
        Button _splitButton;

        void Start()
        {
            _hitButton.onClick.AddListener(() => _gambler.Hit());
            _standButton.onClick.AddListener(() => _gambler.Stand());
            _doubleButton.onClick.AddListener(() => _gambler.Double());
            _splitButton.onClick.AddListener(() => _gambler.Split());

            // TODO: Check if this affects late joiners
            Hide();
        }

        public void SetGambler(Gambler gambler)
        {
            if (_gambler != null && _gambler != gambler)
            {
                _gambler.TurnBegan.RemoveListener(OnGamblerTurnBegan);
                _gambler.Hitted.RemoveListener(OnGamblerHit);
                _gambler.Doubled.RemoveListener(OnGamblerDoubled);
                _gambler.Splitted.RemoveListener(OnGamblerDidSplit);
                _gambler.Passed.RemoveListener(OnGamblerPassed);
            }

            _gambler = gambler;

            gambler.TurnBegan.AddListener(OnGamblerTurnBegan);
            gambler.Hitted.AddListener(OnGamblerHit);
            gambler.Doubled.AddListener(OnGamblerDoubled);
            gambler.Splitted.AddListener(OnGamblerDidSplit);
            gambler.Passed.AddListener(OnGamblerPassed);
        }

        void OnGamblerTurnBegan(Gambler gambler)
        {
            Show();

            if (gambler.ChipTotal >= gambler.MainBet)
                _doubleButton.gameObject.SetActive(true);
            else
                _doubleButton.gameObject.SetActive(false);

            if (gambler.CanSplit)
                _splitButton.gameObject.SetActive(true);
        }
        void OnGamblerPassed(Gambler gambler)
        {
            Hide();
        }
        void OnGamblerHit(Gambler gambler, Card card)
        {
            _doubleButton.gameObject.SetActive(false);
            _splitButton.gameObject.SetActive(false);
        }
        void OnGamblerDoubled(Gambler gambler, Card card)
        {
            _doubleButton.gameObject.SetActive(false);
            _splitButton.gameObject.SetActive(false);
        }
        void OnGamblerDidSplit(Gambler gambler, Card card1, Card card2)
        {
            _doubleButton.gameObject.SetActive(false);
            _splitButton.gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _doubleButton.gameObject.SetActive(false);
            _splitButton.gameObject.SetActive(false);
        }
        public void Hide()
        {
            _doubleButton.gameObject.SetActive(false);
            _splitButton.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}