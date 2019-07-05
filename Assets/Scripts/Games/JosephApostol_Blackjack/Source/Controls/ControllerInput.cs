using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace ProjectRenaissance.Controls
{
    public sealed class ControllerInput : MonoBehaviour
    {
        public Vector3 LiftHeight = new Vector3(0, 0.2f);
        public Vector3 Gap = new Vector3(0, 0.05f);
        public Gambler LocalGambler;

        Rigidbody _objectUnderPointer;
        List<Rigidbody> _liftedObjects = new List<Rigidbody>();

        VRTK_DestinationMarker _destinationMarker;
        VRTK_ControllerEvents _controllerEvents;

        void Start()
        {
            _destinationMarker = transform.parent.GetComponent<VRTK_DestinationMarker>();

            if (_destinationMarker != null)
                _destinationMarker.DestinationMarkerHover += DestinationMarker_PointerHover;

            _controllerEvents = transform.parent.GetComponent<VRTK_ControllerEvents>();

            if (_controllerEvents != null)
                _controllerEvents.TriggerPressed += ControllerEvents_TriggerPressed;
        }

        void OnDestroy()
        {
            _destinationMarker = transform.parent.GetComponent<VRTK_DestinationMarker>();

            if (_destinationMarker != null)
                _destinationMarker.DestinationMarkerHover -= DestinationMarker_PointerHover;

            _controllerEvents = GetComponent<VRTK_ControllerEvents>();

            if (_controllerEvents != null)
                _controllerEvents.TriggerPressed -= ControllerEvents_TriggerPressed;
        }

        void Lift(Rigidbody rigidbodyToLift)
        {
            Renderer renderer = rigidbodyToLift.GetComponent<Renderer>();

            for (int i = 0; i < _liftedObjects.Count; i++)
            {
                Rigidbody alreadyLiftedRigidbody = _liftedObjects[i];
                alreadyLiftedRigidbody.position += Gap;
            }

            rigidbodyToLift.isKinematic = true;
            rigidbodyToLift.position += LiftHeight;
            _liftedObjects.Add(rigidbodyToLift);
        }
        void Drop()
        {
            for (int i = 0; i < _liftedObjects.Count; i++)
            {
                Rigidbody rigidbody = _liftedObjects[i];
                rigidbody.isKinematic = false;
            }

            _liftedObjects.Clear();
        }

        void ControllerEvents_TriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (_objectUnderPointer != null)
            {
                Chip chip = _objectUnderPointer.GetComponent<Chip>();

                if (chip != null && !chip.IsLocked && chip.Owner == LocalGambler)
                {
                    Lift(_objectUnderPointer);
                    _objectUnderPointer = null;
                    chip.IsMidair = true;
                }
                else if (chip == null)
                    Drop();
            }
            else
                Drop();
        }

        void DestinationMarker_PointerHover(object sender, DestinationMarkerEventArgs e)
        {
            RaycastHit hit;
            Vector3 direction = (e.raycastHit.point - Camera.main.transform.position);

            if (Physics.Raycast(Camera.main.transform.position, direction.normalized, out hit, 1000f, 1 << 30))
                _objectUnderPointer = hit.collider.attachedRigidbody;
            else
                _objectUnderPointer = null;

            for (int i = 0; i < _liftedObjects.Count; i++)
                _liftedObjects[i].position = e.raycastHit.point + LiftHeight + (Gap * i);
        }
    }
}