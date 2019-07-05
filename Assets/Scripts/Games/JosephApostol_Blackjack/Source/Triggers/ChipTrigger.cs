using UnityEngine;
using System;
using UnityEngine.Events;

namespace ProjectRenaissance.Triggers
{
    public sealed class ChipTrigger : MonoBehaviour
    {
        public ChipTriggerEvent ChipEntered;
		public ChipTriggerEvent ChipExited;

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Chip>() != null)
            {
                Chip chip = other.GetComponent<Chip>();
                ChipEntered.Invoke(chip);
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Chip>())
            {
                Chip chip = other.GetComponent<Chip>();
                ChipExited.Invoke(chip);
            }
        }

        [Serializable]
        public sealed class ChipTriggerEvent : UnityEvent<Chip>
        {

        }
    }
}