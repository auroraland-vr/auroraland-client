using System;
using UnityEngine;
using VRTK;

/// <summary>
/// Attach this script to each controller game object to automatically turn on the VRTK_Pointer laser when hovering over objects with VRTK_UICanvas / VRTK_InteractableObject.
/// </summary>
/// <remarks>
/// Author: Joseph Apostol
/// </remarks>
namespace Auroraland
{
    [RequireComponent(typeof(VRTK_Pointer))]
    public class AutoRaycaster : MonoBehaviour
    {
        VRTK_Pointer _pointer;

        public event EventHandler<AutoRaycasterEventArgs> Toggled;

        public class AutoRaycasterEventArgs : EventArgs
        {
            /// <summary>
            /// Is the auto-raycaster currently raycasting?
            /// </summary>
            public readonly bool IsOn;

            public AutoRaycasterEventArgs(bool isOn)
            {
                this.IsOn = isOn;
            }
        }

        void Start()
        {
            _pointer = GetComponent<VRTK_Pointer>();
        }

        void Update()
        {
            RaycastHit hit;
            Vector3 direction = transform.forward;

            if (Physics.Raycast(transform.position, direction.normalized, out hit, 1000f))
            {
                VRTK_UICanvas canvas = hit.collider.GetComponent<VRTK_UICanvas>();
                VRTK_InteractableObject hitObject = hit.collider.GetComponent<VRTK_InteractableObject>();

                if (canvas != null || hitObject != null)
                {
                    _pointer.Toggle (true);
                    OnToggled(true);
                }
                else // hit but no script found
                {
                    _pointer.Toggle(false);
                    OnToggled(false);
                }
            }
            else // no hit
            {
                _pointer.Toggle(false);
                OnToggled(false);
            }
        }

        void OnToggled(bool isOn)
        {
            EventHandler<AutoRaycasterEventArgs> handler = Toggled;

            if (handler != null)
                handler(this, new AutoRaycasterEventArgs(isOn));
        }
    }
}
