using ProjectRenaissance.UI;
using ProjectRenaissance.VictoryEvaluators;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectRenaissance
{
    [RequireComponent(typeof(SortZone))]
    public sealed class Hand : MonoBehaviour
    {
        [Header("State")]
        public int Total;
        public int ChipTotal;
        public bool HasHit;
        public UnityEvent Stood;
        public UnityEvent Busted;
        public CardEvent AddedCard = new CardEvent();

        bool _didStand;
        bool _didBust;
        uint _aceCount;
        uint _softAceCount;
        List<Card> _cards = new List<Card>();
        ChipPool _chips = new ChipPool();
        Card _splitHandHit;

        public bool CanSplit
        {
            get
            {
                if (_cards.Count == 2)
                    return _cards[0].Value == _cards[1].Value;

                return false;
            }
        }
        public bool HasNatural
        {
            get
            {
                return _cards.Count == 2 && Total == 21;
            }
        }

        public void AddCard(Card card)
        {
            _cards.Add(card);

            if (!card.IsRevealed)
            {
                card.Revealed.AddListener(OnCardRevealed);

                bool firstCardIsAce = _cards.Count == 2 && _cards[0].Face == Card.Faces.Ace;

                if (firstCardIsAce && card.Value + 11 == 21)
                    Stand();
            }
            else
            {
                bool wouldBust = Total + card.Value > 21;
                bool hasHardAce = _aceCount > 0 && _softAceCount != _aceCount;

                if (wouldBust)
                {
                    if (hasHardAce)
                    {
                        Total = (Total - 10) + card.Value; // Soften the ace and add the card value
                        _softAceCount++;
                    }
                    else if (card.Face == Card.Faces.Ace)
                    { // else if the new card is an ace, add it softly
                        Total++;
                        _softAceCount++;
                    }
                    else
                        Total += card.Value; // If there's nothing to soften, add it and bust anyway
                }
                else
                    Total += card.Value;

                if (card.Face == Card.Faces.Ace)
                    _aceCount++;
            }

            AddedCard.Invoke(card);
        }

        public void Hit(Card card)
        {
            HasHit = true;

            AddCard(card);

            if (Total > 21)
                Bust();
            else if (Total == 21)
                Stand();
        }
        public void Double(Card card, List<Chip> chips)
        {
            if (!HasHit)
            {
                Bet(chips);
                Hit(card);

                if (!_didStand && !_didBust)
                    Stand();
            }
        }
        public void Stand()
        {
            _didStand = true;
            Stood.Invoke();
        }
        public void Split(Card mainHit, Hand splitHand, Card splitHit, List<Chip> splitChips)
        {
            Card splitCard = _cards[1];

            _cards.Remove(splitCard);

            Total -= splitCard.Value;

            Hit(mainHit);

            splitHand.AddCard(splitCard);
            splitHand._splitHandHit = splitHit;
            splitHand.Bet(splitChips);

            splitCard.Arrived.AddListener(splitHand.OnSplitCardArrived);

            if (splitCard.Face == Card.Faces.Ace)
            {
                _aceCount--;
                Stand();
                splitHand.Stand();
            }
        }
        public void Bet(List<Chip> chips)
        {
            _chips.AddRange(chips);
            ChipTotal = _chips.Total;
            _chips.Lock();
        }
        void Bust()
        {
            _didBust = true;
            Busted.Invoke();
        }

        public int CheckVictory(int dealerTotal, bool dealerNatural)
        {
            int winnings = new StandardHandEvaluator().CheckVictory(_chips.Total, Total, HasNatural, dealerTotal, dealerNatural);
            return winnings;
        }
        public void ResetState()
        {
            Total = 0;
            HasHit = false;
            _didBust = false;
            _didStand = false;
            _aceCount = 0;
            _softAceCount = 0;

            _chips.Clear();
            _cards.Clear();
        }
        public List<Card> GetCards()
        {
            List<Card> deepCopy = new List<Card>();
            deepCopy.AddRange(_cards);
            return deepCopy;
        }
        public List<Chip> GetChips()
        {
            return _chips.GetChips();
        }
        public void UnlockChips()
        {
            _chips.Unlock();
        }
        public void ClearChips()
        {
            _chips.Clear();
        }

        void OnSplitCardArrived(Card card)
        {
            card.Arrived.RemoveListener(OnSplitCardArrived);
            AddCard(_splitHandHit);
            _splitHandHit = null;
        }
        void OnCardRevealed(Card card)
        {
            card.Revealed.RemoveListener(OnCardRevealed);

            if (card.Face == Card.Faces.Ace)
            {
                if (_aceCount > 0)
                {
                    Total++;
                    _softAceCount++;
                }
                else
                    Total += 11;

                _aceCount++;
            }
            else
                Total += card.Value;
        }
    }
}