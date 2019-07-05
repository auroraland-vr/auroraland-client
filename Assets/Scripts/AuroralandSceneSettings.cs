using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.PostProcessing;

namespace Auroraland
{
    /// <summary>
    /// Author: Joseph Apostol
    /// Purpose: A data object that contains Auroraland-necessary references in the scene.
    /// </summary>
    public class AuroralandSceneSettings : MonoBehaviour
    {
        public Transform Spawnpoint;

        private void Start()
        {
            Assert.IsNotNull(Spawnpoint);
        }
    }
}