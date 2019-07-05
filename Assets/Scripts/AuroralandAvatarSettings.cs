using UnityEngine;
using UnityEngine.Assertions;

namespace Auroraland
{
    /// <summary>
    /// Author: Joseph Apostol
    /// Purpose: A data object that contains Auroraland-necessary references from the avatar.
    /// </summary>
    public class AuroralandAvatarSettings : MonoBehaviour
    {
        public Transform EyePosition;
        public Transform RightHandLaserOrigin;
        public Transform LeftHandLaserOrigin;

        private void Start()
        {
            Assert.IsNotNull(EyePosition);
            Assert.IsNotNull(RightHandLaserOrigin);
            Assert.IsNotNull(LeftHandLaserOrigin);
        }
    }
}