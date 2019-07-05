using UnityEngine;
using UnityEngine.Events;

namespace ProjectRenaissance.Triggers
{
    public sealed class AvatarTrigger : MonoBehaviour
    {
        public int SeatNumber;

        public AvatarTriggerEvent AvatarEntered = new AvatarTriggerEvent();
        public AvatarTriggerEvent AvatarExited = new AvatarTriggerEvent();

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Avatar" || other.tag == "Actor")
            {
                other.transform.position = transform.position;
                AvatarEntered.Invoke(other.transform.parent.gameObject, SeatNumber);
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Avatar" || other.tag == "Actor")
                AvatarExited.Invoke(other.transform.parent.gameObject, SeatNumber);
        }

        public sealed class AvatarTriggerEvent : UnityEvent<GameObject, int>
        {

        }
    }
}