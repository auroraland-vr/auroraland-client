using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraShmup
{
    /// <summary>
    /// Contains logic for OnTrigger events to ReadyUp the ArcadeMachineManager
    /// </summary>
    public class ReadyUp : MonoBehaviour
    {
        /// <summary>
        /// Reference to the ArcadeMachineManager
        /// </summary>
        private ArcadeMachineManager arcadeManager;

        private void Start()
        {
            arcadeManager = transform.parent.GetComponent<ArcadeMachineManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Avatar"))
            {
                arcadeManager.EnableInteractivity(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Avatar"))
            {
                arcadeManager.DisableInteractivity(other.gameObject);
            }
        }
    }
}