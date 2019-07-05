using System.Collections.Generic;
using UnityEngine;

namespace ProjectRenaissance.UI
{
    [RequireComponent(typeof(Hand))]
    public sealed class SortZone : MonoBehaviour
    {
        [Header("Configuration")]
        public float XSpacing = 0.15f;

        Hand _hand;
        Vector3 _flyToPosition;

        List<Transform> _sortedObjects = new List<Transform>();
        float _nextLocalX = 0;

        void Start()
        {
            _hand = GetComponent<Hand>();
            _flyToPosition = transform.position + new Vector3(0, 0.1f);
            _hand.AddedCard.AddListener(OnHandAddedCard);
        }

        public void Add(Transform transform)
        {
            AdjustLeft(0, _sortedObjects.Count);

			_sortedObjects.Add(transform);
			transform.SetParent(this.transform);
            transform.localPosition = new Vector3(_nextLocalX, transform.transform.localPosition.y, 0);
			_nextLocalX += XSpacing;
        }
        public void Remove(Transform transform)
        {
            int i = _sortedObjects.IndexOf(transform);

            for (int j = 0; j < i; j++)
                _sortedObjects[j].transform.localPosition += new Vector3(XSpacing, 0);

            AdjustLeft(i + 1, _sortedObjects.Count);
            _sortedObjects.Remove(transform);

            _nextLocalX -= XSpacing;
        }

        void OnHandAddedCard(Card card)
        {
            card.RiseAndFlyTo(_flyToPosition, transform.rotation);
            card.Arrived.AddListener(OnCardArrived);
        }
        void OnCardArrived(Card card)
        {
            Add(card.transform);
            card.Left.AddListener(OnCardLeft);
            card.Arrived.RemoveListener(OnCardArrived);
        }
        void OnCardLeft(Card card)
        {
            Remove(card.transform);
            card.Left.RemoveListener(OnCardLeft);
        }

        /// <summary>
        /// Adjusts sorted items to the left starting from the start index [inclusive] up to the end index [exclusive].
        /// </summary>
        /// <param name="startIndex">Start index [inclusive]</param>
        /// <param name="endIndex">End index [exclusive]</param>
        void AdjustLeft(int startIndex, int endIndex)
        {
            bool startInLimits = startIndex >= 0 && startIndex <= _sortedObjects.Count;
            bool endInLimitsOrGrequal = endIndex <= _sortedObjects.Count && endIndex >= 0 && endIndex > startIndex;

			if (startInLimits && endInLimitsOrGrequal) {
				for (int i = startIndex; i < endIndex; i++)
					_sortedObjects [i].transform.localPosition -= new Vector3 (XSpacing, 0);
			}
		}
        /// <summary>
        /// Adjusts sorted items to the right starting from the start index [inclusive] up to the end index [exclusive].
        /// </summary>
        /// <param name="startIndex">Start index [inclusive]</param>
        /// <param name="endIndex">End index [exclusive]</param>
        void AdjustRight(int startIndex, int endIndex)
        {
            bool startInLimits = startIndex >= 0 && startIndex <= _sortedObjects.Count;
            bool endInLimitsOrGrequal = endIndex <= _sortedObjects.Count && endIndex >= 0 && endIndex > startIndex;

            if (startInLimits && endInLimitsOrGrequal)
                for (int i = startIndex; i < endIndex; i++)
                    _sortedObjects[i].transform.localPosition += new Vector3(XSpacing, 0);
        }
    }
}