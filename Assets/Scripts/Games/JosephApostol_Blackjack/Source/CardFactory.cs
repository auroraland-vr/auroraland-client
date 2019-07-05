using System.Collections.Generic;
using UnityEngine;

namespace ProjectRenaissance
{
    public sealed class CardFactory : MonoBehaviour
    {
        [SerializeField]
        List<Card> _cardPrefabs = new List<Card>();
        [SerializeField]
        GameObject _deckPrefab;

        public Card CreatePipCard(Card.Suits suit, int value)
        {
            if (value >= 2 && value <= 10)
            {
                GameObject prefab = _cardPrefabs.Find(x => x.name == GetKeyFromSuitAndValue(suit, value)).gameObject;
                GameObject go = Instantiate(prefab);
                Card card = go.GetComponent<Card>();
                card.Value = value;
                card.Face = Card.Faces.None;
                card.Suit = suit;
                return card;
            }

            return null;
        }
        public Card CreateFaceCard(Card.Suits suit, Card.Faces face)
        {
            if (face != Card.Faces.None)
            {
                GameObject prefab = _cardPrefabs.Find(x => x.name == GetKeyFromSuitAndFace(suit, face)).gameObject;
                GameObject go = Instantiate(prefab);
                Card card = go.GetComponent<Card>();
                card.Value = face == Card.Faces.Ace ? 11 : 10;
                card.Face = face;
                card.Suit = suit;
                return card;
            }

            return null;
        }

        string GetKeyFromSuitAndValue(Card.Suits suit, int value)
        {
            return value.ToString() + " of " + suit.ToString();
        }
        string GetKeyFromSuitAndFace(Card.Suits suit, Card.Faces face)
        {
            return face.ToString() + " of " + suit.ToString();
        }
    }
}