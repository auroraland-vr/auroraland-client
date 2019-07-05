using UnityEngine;
using Newtonsoft.Json.Linq;
using Auroraland;
using UnityEngine.Assertions;
using ProjectRenaissance.Controls;
using UnityEngine.UI;

namespace ProjectRenaissance
{
    public sealed class NetworkController : MonoBehaviour
    {
        [SerializeField]
        Game _game;
        [SerializeField]
        Button _startButton;

        void Start()
        {
            NKGameManager.Instance.AuthoritateEventReceived += OnAuthoritateEventReceived;
            NKGameManager.Instance.PlayerJoins += OnPlayerJoins;
            NKGameManager.Instance.AuthoritatePlayerReadyEventReceived += OnAuthoritatePlayerReadyEventReceived;
            NKGameManager.Instance.EventReceived += OnEventReceived;
            NKGameManager.Instance.PlayerLeaves += OnPlayerLeaves;

            _game.LocalGamblerFound.AddListener(OnLocalGamblerFound);
            _game.LocalGamblerLeft.AddListener(OnLocalGamblerLeft);

            _startButton.onClick.AddListener(OnStartButtonClicked);
        }

        void OnPlayerJoins(object sender, NKGameManager.NKPlayerEvent e)
        {
            Assert.IsFalse(NKGameManager.Instance.SpaceID == "");

            // HACK hardcoded for the GameScene
            if (NKGameManager.Instance.SpaceID == "JC_uwo2YRc-6HOvG4T4Qaw")
            {
                if (_game.Dealer.ShuffleSeed != -1)
                {
                    JObject message = new JObject
                    {
                        new JProperty("function", "LateJoinerUpdate"),
                        new JProperty("seed", _game.Dealer.ShuffleSeed)
                    };

                    NKGameManager.Instance.SendTo(message.ToString(), e.UserId);
                }

                // TODO Optimize messages into one, instead of multiple messages
                for (int i = 0; i < _game.Gamblers.Count; i++)
                {
                    if (_game.ActiveGamblers.Contains(_game.Gamblers[i]))
                    {
                        JObject message1 = new JObject
                        {
                            new JProperty("function", "EnablePlayer"),
                            new JProperty("seat", i + 1)
                        };

                        NKGameManager.Instance.SendTo(message1.ToString(), e.UserId);
                    }
                }
            }
        }
        void OnPlayerLeaves(object sender, NKGameManager.NKPlayerEvent e)
        {
            // TODO Add disconnection code
        }
        void OnAuthoritatePlayerReadyEventReceived(object sender, NKGameManager.NKPlayerReadyEvent e)
        {
            JObject message = new JObject
            {
                new JProperty("function", "LateJoinerUpdate")
            };

            for (int i = 0; i < _game.Gamblers.Count; i++)
            {
                if (_game.ActiveGamblers.Contains(_game.Gamblers[i]))
                {
                    JObject message1 = new JObject
                    {
                        new JProperty("function", "EnablePlayer"),
                        new JProperty("seat", i + 1)
                    };

                    NKGameManager.Instance.SendTo(message1.ToString(), e.Payload.UserId);
                }
            }

            if (_game.Dealer.ShuffleSeed != -1)
                message["seed"] = _game.Dealer.ShuffleSeed;

            NKGameManager.Instance.SendTo(message.ToString(), e.Payload.UserId);
        }
        void OnAuthoritateEventReceived(object sender, NKGameManager.NKCustomEvent e)
        {
            JObject message = JObject.Parse(e.Payload.Metadata.Substring(1, e.Payload.Metadata.Length - 2));

            if (e.Payload.UserId != NKGameManager.Instance.UserID)
            {
                if (message["request"] != null)
                {
                    FulfillRequest(message);
                }
                else if (message["function"] != null)
                {
                    SimulateMessage(message);
                }
            }

            if (message["request"] == null)
                NKGameManager.Instance.BroadcastExcluding(message.ToString(), e.Payload.UserId);
        }
        void OnEventReceived(object sender, NKGameManager.NKCustomEvent e)
        {
            JObject message = JObject.Parse(e.Payload.Metadata.Substring(1, e.Payload.Metadata.Length - 2));
            SimulateMessage(message);
        }

        void OnStartButtonClicked()
        {
            bool isOnline = !string.IsNullOrEmpty(NKGameManager.Instance.SpaceID);

            if (isOnline)
            {
                if (_game.Dealer.ShuffleSeed == 0)
                {
                    bool isNotMasterClient = !(NKController.Instance.GetLocalUserId() == NKController.Instance.GetMasterClientUserId());

                    if (isNotMasterClient)
                    {
                        JObject message = new JObject
                        {
                            new JProperty("request", "Seed")
                        };

                        NKGameManager.Instance.SendToMasterClient(message.ToString());
                    }
                    else
                    {
                        int shuffleSeed = new System.Random().Next(0, 10000);
                        _game.Dealer.SetShuffleSeed(shuffleSeed);

                        JObject message = new JObject
                        {
                            new JProperty("function", "Seed"),
                            new JProperty("value", shuffleSeed)
                        };

                        NKGameManager.Instance.BroadcastExcluding(message.ToString(), "");
                        _game.StartGame();
                    }
                }
                else
                {
                    JObject message = new JObject
                    {
                        new JProperty("function", "StartGame")
                    };

                    NKGameManager.Instance.SendToMasterClient(message.ToString());
                    _game.StartGame();
                }
            }
            else
                _game.StartGame();
        }

        void OnLocalGamblerFound(Gambler gambler)
        {
            if (NKGameManager.Instance.RightController.GetComponent<ControllerInput>() == null)
            {
                ControllerInput ci = NKGameManager.Instance.RightController.AddComponent<ControllerInput>();
                ci.LocalGambler = gambler;
            }

            JObject message = new JObject
            {
                new JProperty("function", "EnablePlayer"),
                new JProperty("seat", gambler.Seat)
            };

            NKGameManager.Instance.SendToMasterClient(message.ToString());

            gambler.Hitted.AddListener(OnLocalGamblerDidHit);
            gambler.Doubled.AddListener(OnLocalGamblerDoubled);
            gambler.Passed.AddListener(OnLocalGamblerPassed);
            gambler.Splitted.AddListener(OnLocalGamblerDidSplit);
            gambler.Blackjacked.AddListener(OnLocalGamblerGotBlackjack);
            gambler.Betted.AddListener(OnLocalGamblerBetted);
            gambler.Insured.AddListener(OnLocalGamblerInsured);
        }
        void OnLocalGamblerLeft(Gambler gambler)
        {
            gambler.Hitted.RemoveListener(OnLocalGamblerDidHit);
            gambler.Doubled.RemoveListener(OnLocalGamblerDoubled);
            gambler.Passed.RemoveListener(OnLocalGamblerPassed);
            gambler.Blackjacked.RemoveListener(OnLocalGamblerGotBlackjack);
            gambler.Splitted.RemoveListener(OnLocalGamblerDidSplit);
            gambler.Betted.RemoveListener(OnLocalGamblerBetted);
            gambler.Insured.RemoveListener(OnLocalGamblerInsured);

            ControllerInput left = NKGameManager.Instance.LeftController.GetComponent<ControllerInput>();
            ControllerInput right = NKGameManager.Instance.RightController.GetComponent<ControllerInput>();

            if (left != null)
                Destroy(left);

            if (right != null)
                Destroy(right);

            JObject message = new JObject
                {
                    new JProperty("function", "DisablePlayer"),
                    new JProperty("seat", gambler.Seat)
                };

            NKGameManager.Instance.SendToMasterClient(message.ToString());
        }

        void OnLocalGamblerDidHit(Gambler gambler, Card card)
        {
            JObject message = new JObject
            {
                new JProperty("function", "Hit"),
                new JProperty("seat", gambler.Seat)
            };
            NKGameManager.Instance.SendToMasterClient(message.ToString());
        }
        void OnLocalGamblerDoubled(Gambler gambler, Card card)
        {
            JObject message = new JObject
            {
                new JProperty("function", "Double"),
                new JProperty("seat", gambler.Seat)
            };
            NKGameManager.Instance.SendToMasterClient(message.ToString());
        }
        void OnLocalGamblerPassed(Gambler gambler)
        {
            JObject message = new JObject
            {
                new JProperty("function", "Stand"),
                new JProperty("seat", gambler.Seat)
            };
            NKGameManager.Instance.SendToMasterClient(message.ToString());
        }
        void OnLocalGamblerDidSplit(Gambler gambler, Card card, Card card2)
        {
            JObject message = new JObject
            {
                new JProperty("function", "Split"),
                new JProperty("seat", gambler.Seat)
            };
            NKGameManager.Instance.SendToMasterClient(message.ToString());
        }
        void OnLocalGamblerGotBlackjack(Gambler gambler)
        {
            JObject message = new JObject
            {
                new JProperty("function", "Stand"),
                new JProperty("seat", gambler.Seat)
            };

            NKGameManager.Instance.SendToMasterClient(message.ToString());
        }
        void OnLocalGamblerBetted(Gambler gambler, int amount)
        {
            JObject betMessage = new JObject
            {
                new JProperty("function", "Bet"),
                new JProperty("player", gambler.Seat),
                new JProperty("value", gambler.MainBet),
            };

            NKGameManager.Instance.SendToMasterClient(betMessage.ToString());
        }
        void OnLocalGamblerInsured(Gambler gambler, int amount)
        {
            JObject insureMessage = new JObject
            {
                new JProperty("function", "Insure"),
                new JProperty("player", gambler.Seat),
                new JProperty("value", gambler.InsuranceBet),
            };

            NKGameManager.Instance.SendToMasterClient(insureMessage.ToString());
        }

        void FulfillRequest(JObject message)
        {
            switch (message["request"].ToString())
            {
                case "Seed":
                    int shuffleSeed = new System.Random().Next(0, 10000);
                    _game.Dealer.SetShuffleSeed(shuffleSeed);
                    _game.StartGame();

                    JObject response = new JObject {
                        new JProperty ("function", "Seed"),
                        new JProperty ("value", shuffleSeed)
                    };
                    NKGameManager.Instance.BroadcastExcluding(response.ToString(), "");
                    break;
            }
        }
        void SimulateMessage(JObject message)
        {
            switch (message["function"].ToString())
            {
                case "Seed":
                    _game.Dealer.SetShuffleSeed(message["value"].ToObject<int>());
                    _game.StartGame();
                    break;
                case "StartGame":
                    _game.StartGame();
                    break;
                case "LateJoinerUpdate":
                    // TODO: Fulfill this
                    _game.Dealer.SetShuffleSeed(message["seed"].ToObject<int>());
                    break;
                case "EnablePlayer":
                    _game.EnablePlayer(false, message["seat"].ToObject<int>());
                    break;
                case "DisablePlayer":
                    _game.DisablePlayer(message["seat"].ToObject<int>());
                    break;
                case "Bet":
                    int bettingPlayer = message["player"].ToObject<int>() - 1;
                    int bet = message["value"].ToObject<int>();
                    _game.Gamblers[bettingPlayer].Bet(bet);
                    break;
                case "Insure":
                    int insuringPlayer = message["player"].ToObject<int>() - 1;
                    int insurance = message["value"].ToObject<int>();
                    _game.Gamblers[insuringPlayer].Insure(insurance);
                    break;
                case "Hit":
                    _game.Gamblers[message["seat"].ToObject<int>() - 1].Hit();
                    break;
                case "Double":
                    _game.Gamblers[message["seat"].ToObject<int>() - 1].Double();
                    break;
                case "Stand":
                    _game.Gamblers[message["seat"].ToObject<int>() - 1].Stand();
                    break;
                case "Split":
                    _game.Gamblers[message["seat"].ToObject<int>() - 1].Split();
                    break;
            }
        }
    }
}