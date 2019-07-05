using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectRenaissance
{
    [RequireComponent(typeof(Hand))]
    public sealed class Dealer : MonoBehaviour
    {
        public enum BetType { None, Main, Insure }

        [Header("State")]
        public int ShuffleSeed;
        public BetType CurrentAskedBet;
        public List<Gambler> Gamblers;
        [SerializeField]
        Gambler _localGambler;
        [SerializeField]
        int _bettedCount;
        [SerializeField]
        int _insuredCount;
        [SerializeField]
        int _currentGamblerIndex;
        [SerializeField]
        Gambler _currentGambler;

        [Header("Configuration")]
        [SerializeField]
        GameObject _highestValueChipPrefab;
        [SerializeField]
        Transform _deckSpawnpoint;
        [SerializeField]
        Transform _discardWaypoint;
        [SerializeField]
        int _decksToMake = 4;
        [SerializeField]
        CardFactory _cardFactory;

        Hand _hand;

        public UnityEvent TurnBegan;
        public UnityEvent RoundFinished;
        public BoolEvent DealFinished = new BoolEvent();
        public IntEvent Passed = new IntEvent();

        List<Card> _deck = new List<Card>();
        List<Card> _discard = new List<Card>();
        List<Card> _cardsInPlay = new List<Card>();
        Card _holeCard;

        void Start()
        {
            _hand = GetComponent<Hand>();
        }

        public void StartGame()
        {
            _bettedCount = 0;
            _insuredCount = 0;
            _currentGamblerIndex = 0;
            _currentGambler = null;
            _localGambler = null;

            foreach (Gambler gambler in Gamblers)
            {
                if (!gambler.HasBeenGivenInitialChips)
                {
                    gambler.HasBeenGivenInitialChips = true;
                    CreateChips(gambler, 1875);
                }

                if (gambler.IsLocal)
                    _localGambler = gambler;

                gambler.ResetState();

                // In case they stayed one more round
                gambler.Insured.RemoveListener(OnGamblerInsured);
                gambler.Betted.RemoveListener(OnGamblerBetted);
                gambler.Left.RemoveListener(OnGamblerLeft);
                gambler.Passed.RemoveListener(OnGamblerPassed);

                gambler.Insured.AddListener(OnGamblerInsured);
                gambler.Betted.AddListener(OnGamblerBetted);
                gambler.Left.AddListener(OnGamblerLeft);
                gambler.Passed.AddListener(OnGamblerPassed);
            }

            Cleanup();
            CurrentAskedBet = BetType.Main;
        }
        void StartPlayerTurns()
        {
            _currentGamblerIndex = 0;
            _currentGambler = Gamblers[0];
            _currentGambler.BeginTurn();
        }
        void AdvancePlayerTurn()
        {
            if (_currentGamblerIndex + 1 < Gamblers.Count)
            {
                _currentGamblerIndex++;
                _currentGambler = Gamblers[_currentGamblerIndex];
                _currentGambler.BeginTurn();
            }
            else
            {
                _currentGambler = null;
                TurnBegan.Invoke();
                StartCoroutine(AnimateDealerTurn());
            }
        }

        public void SetShuffleSeed(int seed)
        {
            ShuffleSeed = seed;
            Debug.Log("Deck shuffle seed: " + seed);
        }
        public Card Deal()
        {
            Card card = _deck[_deck.Count - 1];
            _deck.Remove(card);
            _cardsInPlay.Add(card);
            return card;
        }

        public void CreateChips(Gambler player, int chips)
        {
            Chip chip = _highestValueChipPrefab.GetComponent<Chip>();

            while (chip != null)
            {
                int count = chips / chip.Value;

                for (int i = 0; i < count; i++)
                {
                    Chip instance = Instantiate(chip.gameObject).GetComponent<Chip>();
                    Transform spawnpoint = player.transform.Find("Chip Spawnpoints/" + chip.gameObject.name);
                    instance.transform.position = spawnpoint.position + new Vector3(0, 1 + 0.2f * i);
                    instance.transform.SetParent(player.transform);
                    instance.Owner = player;
                    player.AddChip(instance);

                    chips -= instance.Value;
                }

                chip = chip.LesserChip;
            }
        }
        public void CreateBetChips(Gambler player, int chips)
        {
            Chip chip = _highestValueChipPrefab.GetComponent<Chip>();

            while (chip != null)
            {
                int count = chips / chip.Value;

                for (int i = 0; i < count; i++)
                {
                    Chip instance = Instantiate(chip.gameObject).GetComponent<Chip>();
                    Transform spawnpoint = player.transform.Find("Chip Spawnpoints/Chip Trigger");
                    instance.transform.position = spawnpoint.position + new Vector3(0, 1 + 0.2f * i);
                    instance.transform.SetParent(player.transform);
                    instance.Owner = player;
                    player.AddChip(instance);

                    chips -= instance.Value;
                }

                chip = chip.LesserChip;
            }
        }

        public void Cleanup()
        {
            _holeCard = null;

            Discard(_cardsInPlay);

            _cardsInPlay.Clear();
            _hand.ResetState();
        }
        public void Discard(List<Card> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                _discard.Add(card);
                card.RiseAndFlyTo(_discardWaypoint.position + new Vector3(0, 0.01f * i), _discardWaypoint.rotation);
                card.Arrived.AddListener(OnCardArrivedAtDiscard);
            }
        }

        void CreateDeck()
        {
            if (_deck.Count == 0)
            {
                Vector3 lastPosition = _deckSpawnpoint.position;

                for (int j = 0; j < _decksToMake; j++)
                {
                    foreach (Card.Suits suit in Enum.GetValues(typeof(Card.Suits)))
                    {
                        for (int i = 2; i < 11; i++)
                        {
                            Card card = _cardFactory.CreatePipCard(suit, i);
                            card.transform.position = lastPosition;
                            card.transform.rotation = _deckSpawnpoint.rotation;
                            card.transform.SetParent(_deckSpawnpoint);
                            _deck.Add(card);

                            lastPosition += new Vector3(0, 0.05f);
                        }

                        foreach (Card.Faces face in Enum.GetValues(typeof(Card.Faces)))
                        {
                            if (face != Card.Faces.None)
                            {
                                Card card = _cardFactory.CreateFaceCard(suit, face);
                                card.transform.position = lastPosition;
                                card.transform.rotation = _deckSpawnpoint.rotation;
                                card.transform.SetParent(_deckSpawnpoint);
                                _deck.Add(card);

                                lastPosition += new Vector3(0, 0.05f);
                            }
                        }
                    }
                }
            }
        }
        void Shuffle()
        {
            int count = _deck.Count;
            int last = count - 1;

            if (ShuffleSeed == 0)
                SetShuffleSeed(new System.Random().Next(0, 10000));

            System.Random r = new System.Random(ShuffleSeed);

            for (int i = 0; i < last; ++i)
            {
                int rnd = r.Next(0, count);
                Card object1 = _deck[i];
                Card object2 = _deck[rnd];
                Vector3 object1Pos = new Vector3(object1.transform.position.x, object1.transform.position.y, object1.transform.position.z);

                _deck[i] = object2;
                _deck[rnd] = object1;

                object1.transform.position = object2.transform.position;
                object2.transform.position = object1Pos;
            }
        }

        void OnGamblerInsured(Gambler gambler, int amount)
        {
            _insuredCount++;

            if (_insuredCount == Gamblers.Count)
                StartPlayerTurns();
        }
        void OnGamblerBetted(Gambler gambler, int amount)
        {
            _bettedCount++;

            if (_bettedCount == Gamblers.Count)
                StartCoroutine(AnimateCreateDeckAndDeal());
        }
        void OnGamblerPassed(Gambler gambler)
        {
            AdvancePlayerTurn();
        }
        void OnGamblerLeft(Gambler gambler)
        {
            Gamblers.Remove(gambler);
            Discard(gambler.GetCards());

            if (Gamblers.Count == 0)
                Cleanup();
            else if (gambler == _currentGambler)
                AdvancePlayerTurn();
        }

        void OnCardArrivedAtDiscard(Card card)
        {
            card.Arrived.RemoveListener(OnCardArrivedAtDiscard);
            card.transform.SetParent(_discardWaypoint);
        }

        IEnumerator AnimateCreateDeckAndDeal()
        {
            CreateDeck();

            Rigidbody lastCardRb = _deck[_deck.Count - 1].GetComponent<Rigidbody>();

            yield return new WaitWhile(() => { return lastCardRb.isKinematic == false; });

            Shuffle();

            bool shouldInsure = false;

            for (int i = 0; i < 2; i++)
            {
                Card card = Deal();

                if (i == 0 && card.Face == Card.Faces.Ace)
                    shouldInsure = true;

                if (i == 1)
                {
                    _holeCard = card;
                    card.IsRevealed = false;
                }

                _hand.AddCard(card);

                yield return new WaitForSeconds(0.1f);

                foreach (Gambler gambler in Gamblers)
                {
                    gambler.AcceptStartingCard(Deal());
                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(1f);
            
            if (_localGambler != null)
                _localGambler.UnlockChips();

            if (shouldInsure)
                CurrentAskedBet = BetType.Insure;
            else
                StartPlayerTurns();

            DealFinished.Invoke(shouldInsure);
            yield break;
        }
        IEnumerator AnimateDealerTurn()
        {
            yield return new WaitForSeconds(0.8f);

            if (!_holeCard.IsRevealed)
            {
                _holeCard.RiseAndReveal();
                yield return new WaitForSeconds(1.5f);
            }
            if (_hand.Total != 21)
            {
                while (_hand.Total < 17)
                {
                    Card card = Deal();
                    _hand.Hit(card);
                    yield return new WaitForSeconds(1f);
                }
            }

            yield return new WaitForSeconds(0.8f);

            Passed.Invoke(_hand.Total);

            foreach (Gambler gambler in Gamblers)
            {
                int netChips = gambler.CheckVictory(_hand.Total, _hand.HasNatural);

                if (netChips > 0)
                    CreateChips(gambler, netChips);
            }

            RoundFinished.Invoke();

            yield break;
        }
    }
}