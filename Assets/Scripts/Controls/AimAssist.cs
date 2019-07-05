using UnityEngine;

namespace Auroraland
{
    /// <summary>
    /// Author: Joseph Apostol
    /// Purpose: Is attached to the avatar's laser origins so that they point to where the controller laser is pointed at.
    /// </summary>
    public class AimAssist : MonoBehaviour
    {
        public Transform Controller;

        void Update()
        {
            if (Controller != null)
            {
                RaycastHit hit;

                if (Physics.Raycast(Controller.position, Controller.forward, out hit))
                    transform.LookAt(hit.point);
            }
        }
    }
}