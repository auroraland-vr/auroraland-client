using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland
{
    public class PropState : MonoBehaviour
    {

        public void PlaySound(AudioClip audioFile)
        {
            GetComponent<AudioSource>().PlayOneShot(audioFile, 1.0f);
        }
    }
}