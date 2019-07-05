using ProjectRenaissance.UI;
using System.Collections.Generic;
using UnityEngine;
using Auroraland;
using UnityEngine.Assertions;
using ProjectRenaissance.Triggers;

namespace ProjectRenaissance
{
    public sealed class Game : MonoBehaviour
    {
        [Header("State")]
        public Gambler LocalGambler;
        public GameObject LocalAvatar;
        public List<Gambler> ActiveGamblers;
        public bool IsDone = true;

        [Header("Configurations")]
        public List<Gambler> Gamblers;
        [SerializeField]
        public Dealer Dealer;
        [SerializeField]
        NotificationManager _notificationManager;
        [SerializeField]
        List<AvatarTrigger> _avatarTriggers;
        [SerializeField]
        UIController _UIController;

        public GamblerEvent LocalGamblerFound = new GamblerEvent();
        public GamblerEvent LocalGamblerLeft = new GamblerEvent();

        void Start()
        {
            GameObject localAvatar = FindObjectOfType<NetPlayer>().gameObject;
            LocalAvatar = localAvatar;

            Assert.IsNotNull(Dealer);
            Assert.IsNotNull(localAvatar);
            Assert.IsNotNull(_notificationManager);
            Assert.IsNotNull(_UIController);

            foreach (Gambler gambler in Gamblers)
            {
                gambler.Betted.AddListener(OnGamblerBetted);
                gambler.Insured.AddListener(OnGamblerInsured);

                gambler.Hitted.AddListener(OnGamblerHitted);
                gambler.Doubled.AddListener(OnGamblerDoubled);
                gambler.Splitted.AddListener(OnGamblerSplitted);
                gambler.Passed.AddListener(OnGamblerPassed);
                gambler.Blackjacked.AddListener(OnGamblerBlackjacked);
                gambler.SplitHandStarted.AddListener(OnGamblerSplitHandStarted);

                gambler.Won.AddListener(OnGamblerWon);
                gambler.Lost.AddListener(OnGamblerLost);
                gambler.BrokeEven.AddListener(OnGamblerBrokeEven);

                gambler.Left.AddListener(OnGamblerLeft);
            }

            foreach (AvatarTrigger trigger in _avatarTriggers)
            {
                trigger.AvatarEntered.AddListener(OnAvatarTriggerAvatarEntered);
                trigger.AvatarExited.AddListener(OnAvatarTriggerAvatarExited);
            }

            Dealer.DealFinished.AddListener(OnDealerDealFinished);
            Dealer.Passed.AddListener(OnDealerPassed);
            Dealer.TurnBegan.AddListener(OnDealerTurnBegan);
            Dealer.RoundFinished.AddListener(OnDealerRoundFinished);
        }

        public void StartGame()
        {
            if (LocalGambler != null)
            {
                LocalAvatar.GetComponentInChildren<AvatarController>().AllowMovement = false;

                if (!LocalGambler.HasBeenGivenInitialChips)
                    _notificationManager.Notify("Welcome, player " + LocalGambler.Seat + ".\nView game notifications here.");
            }

            IsDone = false;
            Dealer.Gamblers = ActiveGamblers;
            Dealer.StartGame();
            _UIController.StartRound();
        }

        public void EnablePlayer(bool isLocal, int seat)
        {
            Gambler gambler = Gamblers[seat - 1];

            if (!ActiveGamblers.Contains(gambler))
            {
                ActiveGamblers.Add(gambler);

                if (isLocal)
                {
                    LocalGambler = gambler;
                    gambler.IsLocal = true;

                    _UIController.SetupLocalGambler(gambler);

                    LocalGamblerFound.Invoke(LocalGambler);
                }
            }
        }
        public void DisablePlayer(int seat)
        {
            Gambler gambler = Gamblers[seat - 1];
            ActiveGamblers.Remove(gambler);
            gambler.DisableGambler();

            if (gambler.IsLocal)
            {
                LocalGambler.Hitted.RemoveListener(OnGamblerHitted);
                LocalGambler.Doubled.RemoveListener(OnGamblerDoubled);
                LocalGambler = null;

                _UIController.DisableLocalGambler();
                
                LocalGamblerLeft.Invoke(gambler);
            }
        }

        void OnDealerTurnBegan()
        {
            _notificationManager.Notify("Dealer begins.");
        }
        void OnDealerRoundFinished()
        {
            if (LocalGambler != null)
            {
                if (LocalGambler.ChipTotal != 0)
                    _UIController.EliminateLocalGambler();
                else
                    ActiveGamblers.Remove(LocalGambler);
            }

            IsDone = true;
            LocalAvatar.GetComponentInChildren<AvatarController>().AllowMovement = true;
        }
        void OnDealerPassed(int handTotal)
        {
            _notificationManager.Notify("Dealer has " + handTotal + ".");
        }
        void OnDealerDealFinished(bool shouldInsure)
        {
            if (shouldInsure)
                _UIController.StartInsuring();
        }

        public void OnGamblerBetted(Gambler gambler, int amount)
        {
            if (gambler.IsLocal)
                _notificationManager.Notify("You bet " + amount + ".");
            else
                _notificationManager.Notify("Player " + gambler.Seat + " bets " + amount + ".");
        }
        public void OnGamblerInsured(Gambler gambler, int amount)
        {
            if (gambler.IsLocal)
                _notificationManager.Notify("You insure for " + amount + ".");
            else
                _notificationManager.Notify("Player " + gambler.Seat + " insures for " + amount + ".");
        }

        public void OnGamblerHitted(Gambler gambler, Card card)
        {
            if (gambler.IsLocal)
                _notificationManager.Notify("You hit " + card.CardName + ". (" + gambler.CurrentHandTotal + " total)");
            else
                _notificationManager.Notify("Player " + gambler.Seat + " hits " + card.CardName + ". (" + gambler.CurrentHandTotal + " total)");
        }
        public void OnGamblerDoubled(Gambler gambler, Card card)
        {
            if (gambler.IsLocal)
                _notificationManager.Notify("You double and get " + card.CardName + ". (" + gambler.CurrentHandTotal + " total)");
            else
                _notificationManager.Notify("Player " + gambler.Seat + " doubles and gets " + card.CardName + ". (" + gambler.CurrentHandTotal + " total)");
        }
        public void OnGamblerSplitted(Gambler gambler, Card card, Card card2)
        {
            if (gambler.IsLocal)
                _notificationManager.Notify("You split, " + card.CardName + " (" + gambler.MainHandTotal + " main) and " + card2.CardName + " (" + (gambler.SplitHandTotal + card2.Value) + " split)");
            else
                _notificationManager.Notify("Player " + gambler + " splits, " + card.CardName + " (" + gambler.MainHandTotal + " main) and " + card2.CardName + " (" + gambler.SplitHandTotal + card2.Value + " split)");
        }
        public void OnGamblerPassed(Gambler gambler)
        {
            if (gambler.IsLocal)
                _notificationManager.Notify("You stood.");
            else
                _notificationManager.Notify("Player " + gambler.Seat + " stood.");
        }
        public void OnGamblerSplitHandStarted(Gambler gambler)
        {
            if (gambler.IsLocal)
                _notificationManager.Notify("You are now playing your split hand.");
            else
                _notificationManager.Notify("Player " + gambler.Seat + " is now playing his split hand.");
        }
        public void OnGamblerBlackjacked(Gambler gambler)
        {
            if (gambler.IsLocal)
                _notificationManager.Notify("You have blackjack!");
            else
                _notificationManager.Notify("Player " + gambler.Seat + " has blackjack!");
        }

        public void OnGamblerWon(Gambler gambler, int amount)
        {
            if (gambler.IsLocal)
                _notificationManager.Notify("You won " + amount + "!");
            else
                _notificationManager.Notify("Player " + gambler.Seat + " won " + amount + ".");
        }
        public void OnGamblerLost(Gambler gambler, int amount)
        {
            if (gambler.IsLocal)
                _notificationManager.Notify("You lost " + amount + "!");
            else
                _notificationManager.Notify("Player " + gambler.Seat + " lost " + amount + ".");
        }
        public void OnGamblerBrokeEven(Gambler gambler)
        {
            if (gambler.IsLocal)
                _notificationManager.Notify("You broke even!");
            else
                _notificationManager.Notify("Player " + gambler.Seat + " broke even.");
        }

        public void OnGamblerLeft(Gambler gambler)
        {
            _notificationManager.Notify("Player " + gambler.Seat + " has left the game.");
        }

        public void OnAvatarTriggerAvatarEntered(GameObject avatar, int seat)
        {
            if (avatar == LocalAvatar)
                EnablePlayer(true, seat);
            else
                EnablePlayer(false, seat);
        }
        public void OnAvatarTriggerAvatarExited(GameObject avatar, int seat)
        {
            DisablePlayer(seat);
        }
    }
}
