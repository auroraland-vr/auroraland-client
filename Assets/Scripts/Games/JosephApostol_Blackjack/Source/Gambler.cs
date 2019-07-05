using System.Collections.Generic;
using UnityEngine;
using ProjectRenaissance.VictoryEvaluators;

namespace ProjectRenaissance
{
    public class Gambler : MonoBehaviour
    {
        [Header("State")]
        public bool HasPlayer;
        public bool HasBeenGivenInitialChips;
        public bool IsLocal;
        public bool IsTakingTurn;

        [Header("Configurations")]
        public Dealer Dealer;
        public int Seat;
        [SerializeField]
        Hand _mainHand;
        [SerializeField]
        Hand _splitHand;
        [SerializeField]
        Transform _betWaypoint;

        #region Money hooks
        public GamblerIntEvent Betted = new GamblerIntEvent();
        public GamblerIntEvent Insured = new GamblerIntEvent();
        public GamblerIntEvent Won = new GamblerIntEvent();
        public GamblerIntEvent Lost = new GamblerIntEvent();

        public GamblerEvent BrokeEven = new GamblerEvent();
        public IntEvent MainBetChanged = new IntEvent();
        public IntEvent InsureBetChanged = new IntEvent();
        #endregion

        #region Action hooks
        public GamblerEvent TurnBegan = new GamblerEvent();
        public GamblerEvent Passed = new GamblerEvent();
        public GamblerEvent SplitHandStarted = new GamblerEvent();
        public GamblerEvent Blackjacked = new GamblerEvent();
        public GamblerCardEvent Hitted = new GamblerCardEvent();
        public GamblerCardEvent Doubled = new GamblerCardEvent();
        public GamblerCardCardEvent Splitted = new GamblerCardCardEvent();
        #endregion

        public GamblerEvent Left = new GamblerEvent();

        ChipPool _availableChips = new ChipPool();
        ChipPool _holdingPool = new ChipPool();
        ChipPool _insurancePool = new ChipPool();
        bool _didSplit;
        Hand _currentHand;

        /// <summary>
        /// Can this player split?
        /// </summary>
        public bool CanSplit { get { return _mainHand.CanSplit; } }
        /// <summary>
        /// The amount of chips this gambler bet on his main bet.
        /// </summary>
        public int MainBet { get { return _mainHand.ChipTotal; } }
        /// <summary>
        /// The card total on this gambler's main hand.
        /// </summary>
        public int MainHandTotal { get { return _mainHand.Total; } }
        /// <summary>
        /// The card total on this gambler's split hand.
        /// </summary>
        public int SplitHandTotal { get { return _splitHand.Total; } }
        /// <summary>
        /// The card total on this gambler's current hand.
        /// </summary>
        public int CurrentHandTotal { get { return _currentHand.Total; } }


        /// <summary>
        /// The amount of chips this gambler bet on insurance.
        /// </summary>
        public int InsuranceBet { get { return _insurancePool.Total; } }
        /// <summary>
        /// The amount of remaining chips this gambler has.
        /// </summary>
        public int ChipTotal { get { return _availableChips.Total; } }

        void Start()
        {
            _mainHand.Stood.AddListener(OnMainHandStoodOrBusted);
            _mainHand.Busted.AddListener(OnMainHandStoodOrBusted);
            _splitHand.Stood.AddListener(OnSplitHandStoodOrBusted);
            _splitHand.Busted.AddListener(OnSplitHandStoodOrBusted);
        }

        public void ResetState()
        {
            _currentHand = _mainHand;
            _holdingPool.Clear();
            _insurancePool.Clear();
            _mainHand.ResetState();
            _splitHand.ResetState();
            _availableChips.Unlock();
            _didSplit = false;
        }

        public void DisableGambler()
        {
            Left.Invoke(this);
            List<Chip> chips = _availableChips.GetChips();
            chips.AddRange(_holdingPool.GetChips());

            int count = chips.Count;

            for (int i = 0; i < count; i++)
            {
                Destroy(chips[i].gameObject);
            }

            _availableChips.Clear();
            ResetState();
            HasBeenGivenInitialChips = false;
        }

        public void UnlockChips()
        {
            _availableChips.Unlock();
        }
        public void BeginTurn()
        {
            if (!_mainHand.HasNatural)
            {
                TurnBegan.Invoke(this);
                IsTakingTurn = true;
            }
            else
            {
                _mainHand.Stand();
                IsTakingTurn = false;
                Blackjacked.Invoke(this);
            }
        }

        /// <summary>
        /// Makes this gambler bet a certain amount.
        /// </summary>
        /// <param name="amount">The amount to bet. Pass in only network data.</param>
        public void Bet(int amount = 0)
        {
            if (IsLocal)
            {
                _mainHand.Bet(_holdingPool.GetChips());
                _holdingPool.Clear();
            }
            else
            {
                List<Chip> toBet = _availableChips.Take(amount);

                for (int i = 0; i < toBet.Count; i++)
                {
                    toBet[i].transform.position = transform.position + new Vector3(0, 0.1f * i);
                    toBet[i].IsMidair = true;
                    toBet[i].GetComponent<Rigidbody>().isKinematic = false;
                }

                _mainHand.Bet(toBet);
            }

            _availableChips.Lock();
            Betted.Invoke(this, _mainHand.ChipTotal);
            
        }
        /// <summary>
        /// Makes this gambler insure for a certain amount.
        /// </summary>
        /// <param name="amount">The amount to bet as insurance. Pass in only network data.</param>
        public void Insure(int amount = 0)
        {
            if (IsLocal)
            {
                _insurancePool.Lock();
                _holdingPool.Clear();
            }
            else
            {
                List<Chip> toBet = _availableChips.Take(amount);

                for (int i = 0; i < toBet.Count; i++)
                {
                    toBet[i].transform.position = transform.position + new Vector3(0, 0.1f * i);
                    toBet[i].IsMidair = true;
                    toBet[i].GetComponent<Rigidbody>().isKinematic = false;
                }

                _insurancePool.AddRange(toBet);
            }

            _availableChips.Lock();
            Insured.Invoke(this, _insurancePool.Total);
        }

        public int CheckVictory(int dealerTotal, bool dealerNatural)
        {
            int mainWinnings = _mainHand.CheckVictory(dealerTotal, dealerNatural);
            int splitWinnings = _splitHand.CheckVictory(dealerTotal, dealerNatural);
            int netProfit = 0;
            int netLoss = 0;

            int insuranceWinnings = new InsuranceEvaluator().CheckVictory(_insurancePool.Total, 0, false, 0, dealerNatural);

            if (insuranceWinnings < 0)
            {
                netLoss += _insurancePool.Total;
                List<Chip> chips = _insurancePool.GetChips();

                int count = chips.Count;

                for (int i = 0; i < count; i++)
                {
                    Destroy(chips[i].gameObject);
                }

                _insurancePool.Clear();
            }
            else
                netProfit += insuranceWinnings;

            if (mainWinnings < 0)
            {
                netLoss += _mainHand.ChipTotal;

                List<Chip> chips = _mainHand.GetChips();

                int count = chips.Count;

                for (int i = 0; i < count; i++)
                {
                    Destroy(chips[i].gameObject);
                }

                _mainHand.ClearChips();
            }
            else
                netProfit += _mainHand.ChipTotal;

            if (_didSplit)
            {
                if (splitWinnings < 0)
                {
                    netLoss += _splitHand.ChipTotal;
                    List<Chip> chips = _splitHand.GetChips();

                    int count = chips.Count;

                    for (int i = 0; i < count; i++)
                    {
                        Destroy(chips[i].gameObject);
                    }

                    _splitHand.ClearChips();
                }
                else
                    netProfit += _splitHand.ChipTotal;
            }

            if (mainWinnings > 0)
                netProfit += mainWinnings;

            if (splitWinnings > 0)
                netProfit += splitWinnings;

            netProfit -= netLoss;

            if (netProfit < 0)
                Lost.Invoke(this, -netProfit);
            else if (netProfit == 0)
                BrokeEven.Invoke(this);
            else if (netProfit > 0)
                Won.Invoke(this, netProfit);

            _mainHand.UnlockChips();
            _splitHand.UnlockChips();
            _insurancePool.Unlock();

            List<Chip> remaining = _mainHand.GetChips();
            remaining.AddRange(_splitHand.GetChips());
            remaining.AddRange(_insurancePool.GetChips());

            foreach (Chip chip in remaining)
            {
                _availableChips.Add(chip);
                Transform spawnpoint = transform.Find("Chip Spawnpoints/" + chip.gameObject.name.Replace("(Clone)", ""));
                chip.transform.position = spawnpoint.position + Vector3.up;
                chip.IsMidair = true;
                chip.GetComponent<Rigidbody>().isKinematic = false;
            }

            return netProfit;
        }

        public void AddChip(Chip chip)
        {
            _availableChips.Add(chip);
        }
        public void RemoveChip(Chip chip)
        {
            _availableChips.Remove(chip);
        }
        public void AcceptStartingCard(Card card)
        {
            _mainHand.AddCard(card);
        }

        public List<Chip> GetChips()
        {
            return _availableChips.GetChips();
        }
        public List<Card> GetCards()
        {
            List<Card> cards = _mainHand.GetCards();
            cards.AddRange(_splitHand.GetCards());
            return cards;
        }

        public void Hit()
        {
            Card card = Dealer.Deal();
            _currentHand.Hit(card);
            Hitted.Invoke(this, card);
        }
        public void Stand()
        {
            _currentHand.Stand();
            IsTakingTurn = false;
        }
        public void Double()
        {
            List<Chip> toBet = _availableChips.Take(_mainHand.ChipTotal);

            for (int i = 0; i < toBet.Count; i++)
            {
                toBet[i].transform.position = _betWaypoint.transform.position + new Vector3(0, 0.1f * i);
                toBet[i].IsMidair = true;
                toBet[i].GetComponent<Rigidbody>().isKinematic = false;
            }

            Card card = Dealer.Deal();
            _currentHand.Double(card, toBet);
            Doubled.Invoke(this, card);
        }
        public void Split()
        {
            _didSplit = true;
            List<Chip> toBet = _availableChips.Take(_mainHand.ChipTotal);

            for (int i = 0; i < toBet.Count; i++)
            {
                toBet[i].transform.position = _betWaypoint.transform.position + new Vector3(0, 0.1f * i);
                toBet[i].IsMidair = true;
                toBet[i].GetComponent<Rigidbody>().isKinematic = false;
            }

            Card card1 = Dealer.Deal();
            Card card2 = Dealer.Deal();

            _currentHand.Split(card1, _splitHand, card2, toBet);
            Splitted.Invoke(this, card1, card2);
        }

        void OnMainHandStoodOrBusted()
        {
            if (!_didSplit)
            {
                IsTakingTurn = false;
                Passed.Invoke(this);
            }
            else
            {
                if (_splitHand.Total == 21)
                    Passed.Invoke(this);

                _currentHand = _splitHand;
                SplitHandStarted.Invoke(this);
            }
        }
        void OnSplitHandStoodOrBusted()
        {
            _currentHand = _mainHand;
            IsTakingTurn = false;
            Passed.Invoke(this);
        }
        void OnChipTriggerChipEntered(Chip chip)
        {
            if (IsLocal)
            {
                switch (Dealer.CurrentAskedBet)
                {
                    case Dealer.BetType.Main:
                        _availableChips.Remove(chip);
                        _holdingPool.Add(chip);
                        MainBetChanged.Invoke(_holdingPool.Total);
                        break;
                    case Dealer.BetType.Insure:
                        if (_holdingPool.Total + chip.Value <= _mainHand.ChipTotal)
                        {
                            _availableChips.Remove(chip);
                            _insurancePool.Add(chip);
                            InsureBetChanged.Invoke(_insurancePool.Total);
                        }
                        break;
                }
            }
        }
        void OnChipTriggerChipExited(Chip chip)
        {
            if (IsLocal)
            {
                switch (Dealer.CurrentAskedBet)
                {
                    case Dealer.BetType.Main:
                        _holdingPool.Remove(chip);
                        _availableChips.Add(chip);
                        MainBetChanged.Invoke(_holdingPool.Total);
                        break;
                    case Dealer.BetType.Insure:
                        _availableChips.Add(chip);
                        _insurancePool.Remove(chip);
                        InsureBetChanged.Invoke(_insurancePool.Total);
                        break;
                }
            }
        }
    }
}