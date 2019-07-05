using UnityEngine;
using UnityEngine.UI;

namespace ProjectRenaissance.UI
{
    public sealed class BettingUI : MonoBehaviour
    {
        Gambler _gambler;

        [SerializeField]
        Dealer _dealer;
        [SerializeField]
        Button _confirmButton;
        [SerializeField]
        Text _confirmText;
        [SerializeField]
        GameObject _betText;

        void Start()
        {
            // TODO: Check if this affects late joiners
            Hide();
            _confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        public void SetGambler(Gambler gambler)
        {
            if (_gambler != null && _gambler != gambler)
            {
                _gambler.MainBetChanged.RemoveListener(OnGamblerMainBetChanged);
                _gambler.InsureBetChanged.RemoveListener(OnGamblerInsureBetChanged);
                _gambler.Betted.RemoveListener(OnGamblerBetted);
                _gambler.Insured.RemoveListener(OnGamblerBetted);
            }

            _gambler = gambler;

            gambler.MainBetChanged.AddListener(OnGamblerMainBetChanged);
            gambler.InsureBetChanged.AddListener(OnGamblerInsureBetChanged);
            gambler.Betted.AddListener(OnGamblerBetted);
            gambler.Insured.AddListener(OnGamblerBetted);
        }

        void OnConfirmButtonClicked()
        {
            switch (_dealer.CurrentAskedBet)
            {
                case Dealer.BetType.Main:
                    _gambler.Bet();
                    break;
                case Dealer.BetType.Insure:
                    _gambler.Insure();
                    break;
            }
        }

        void OnGamblerBetted(Gambler gambler, int amount)
        {
            Hide();
        }
        void OnGamblerMainBetChanged(int amount)
        {
            if (amount == 0)
                ShowBettingPromptText();
            else
                ShowBettingConfirmButton(amount, false);
        }
        void OnGamblerInsureBetChanged(int amount)
        {
            ShowBettingConfirmButton(amount, true);
        }

        void ShowBettingConfirmButton(int chipTotal, bool isInsuring)
        {
            _confirmText.text = isInsuring ? "Insure " + chipTotal : "Bet " + chipTotal;
            _confirmButton.gameObject.SetActive(true);
            _betText.SetActive(false);
        }
        void ShowBettingPromptText()
        {
            _confirmButton.gameObject.SetActive(false);
            _betText.SetActive(true);
        }

        public void Show()
        {
            gameObject.SetActive(true);

            if (_dealer.CurrentAskedBet == Dealer.BetType.Insure)
                ShowBettingConfirmButton(0, true);
            else
                ShowBettingPromptText();
        }
        public void Hide()
        {
            _betText.SetActive(false);
            _confirmButton.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}